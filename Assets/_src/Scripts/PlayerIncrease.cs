using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.Signals;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


namespace _src.Scripts
{
	[RequireComponent(typeof(SphereCollider),
		typeof(Rigidbody)
	)]
	public class PlayerIncrease : MonoBehaviour
	{
		
		[SerializeField]
		private float _level = 0f;
		
		public float CurrentLevel
		{
			get => _level;

			set => _level = value;
		}


		// Фактический физический размер игрока в Unity
		private float _physicalSize = 0.01f;


		[SerializeField]
		private float _minSize = 0.01f; // Минимальный физический размер


		[SerializeField]
		private float _maxSize = 1.0f; // Максимальный физический размер


		[SerializeField, Required]
		private CinemachineVirtualCamera _cinemachineVirtualCamera;


		[SerializeField]
		private float _centerOfSphereDelta = 2.5f;


		[SerializeField]
		private float _increaseSizeValue = 0.01f; // Значение изменения физического размера


		public UnityEvent OnIncreaseSize = new();

		private List<Bug> _smashedBugs = new();


		private Rigidbody _rb;


		[SerializeField, Required,ChildGameObjectsOnly]
		private SignalSender _defeatSignal;


		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
		}


		private void OnCollisionEnter(Collision other)
		{
			if (other.gameObject.CompareTag("Bug") && other.transform.TryGetComponent(out Bug bug))
			{
				if (IsBugLowerThenPlayer(bug))
				{
					IncreaseSize();
					InsertBug(bug);
				}
				else
				{
					_defeatSignal.SendSignal();
					Destroy(other.gameObject);
				}
			}
		}


		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Bug") && other.transform.TryGetComponent(out Bug bug))
			{
				if (IsBugLowerThenPlayer(bug))
				{
					IncreaseSize();
					InsertBug(bug);
				}
				else
				{
					_defeatSignal.SendSignal();
					Destroy(other.gameObject);
				}
			}
		}


		private void IncreaseSize()
		{
			if (_physicalSize + _increaseSizeValue <= _maxSize) // Увеличиваем размер, если не превышен максимум
			{
				_rb.mass += 3;
				OnIncreaseSize.Invoke();
				_physicalSize += _increaseSizeValue; // Точное увеличение физического размера

				UpdateScaleAndCameraOffset();
				AdjustBugsScale();
			}
		}


		private void DecreaseSize()
		{
			if (_physicalSize - _increaseSizeValue >= _minSize) // Уменьшаем размер, если не меньше минимума
			{
				_level = Mathf.Max(_level - 1,
					1
				); // Уменьшаем "уровень" игрока, не опускаясь ниже 1

				_physicalSize -= _increaseSizeValue; // Точное уменьшение физического размера
				UpdateScaleAndCameraOffset();
				AdjustBugsScale();
			}
		}


		private void UpdateScaleAndCameraOffset()
		{
			transform.DOScale(Vector3.one * _physicalSize,
				1f
			);

			var transposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();

			if (transposer != null)
			{
				Vector3 baseOffset = new Vector3(transposer.m_FollowOffset.x,
					transposer.m_FollowOffset.y + _increaseSizeValue,
					transposer.m_FollowOffset.z - _increaseSizeValue
				);

				transposer.m_FollowOffset = baseOffset;
			}
		}


		private void InsertBug(Bug bug)
		{
			bug.Deactivate();

			if (!_smashedBugs.Contains(bug))
				_smashedBugs.Add(bug);
			else
				return;

			// Определяем точку для размещения на сфере, учитывая текущий масштаб игрока

			// Делаем объект дочерним и устанавливаем начальную позицию
			bug.transform.parent = transform;
			bug.transform.localPosition = Vector3.zero;

			// Установим фиксированный масштаб для жука, предположим, что исходный масштаб жука подходит
			bug.transform.localScale = new Vector3(0.1f,
				0.1f,
				0.1f
			); // Можно настроить этот масштаб

			// Ориентируем объект и перемещаем его к заданной точке
			bug.transform.LookAt(transform.position);

			bug.transform.Rotate(-90,
				0f,
				0f
			);

			bug.transform.localPosition = Random.onUnitSphere / _centerOfSphereDelta;
		}


		private void AdjustBugsScale()
		{
			// Рассчитываем обратный масштаб, основываясь на текущем масштабе игрока
			Vector3 inverseScale = Vector3.one / transform.localScale.x; // Предполагая, что масштаб игрока одинаков по всем осям

			foreach (var bug in _smashedBugs)
			{
				bug.transform.localScale = Vector3.zero;
				// Применяем обратный масштаб к каждому жуку, чтобы их размер оставался постоянным в мировом пространстве
				bug.transform.localScale = inverseScale * 0.01f; // 0.1f - базовый размер жука
				bug.transform.LookAt(transform.position);
				bug.transform.Rotate(-90,
					0f,
					0f
				);
			}
		}


		private bool IsBugLowerThenPlayer(Bug bug) => bug.Level <= _level; 
	}
}
