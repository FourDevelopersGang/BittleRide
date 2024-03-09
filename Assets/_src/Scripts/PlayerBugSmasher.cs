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
	public class PlayerBugSmasher : MonoBehaviour
	{
		// Фактический физический размер игрока в Unity
		private float _physicalSize = 0.01f;


		[SerializeField]
		private float _minSize = 0.01f; // Минимальный физический размер


		[SerializeField]
		private float _maxSize = 1.0f; // Максимальный физический размер


		[SerializeField, Required]
		private CinemachineVirtualCamera _cinemachineVirtualCamera;


		[SerializeField]
		[OnValueChanged("GenerateRandomPositions")]
		private float _centerOfSphereDelta = 2.5f;


		[SerializeField]
		private float _increaseSizeValue = 0.01f; // Значение изменения физического размера


		public UnityEvent OnIncreaseSize = new();

		public UnityEvent OnKillBug = new();


		[SerializeField, ReadOnly]
		private List<Bug> _smashedBugs = new();


		private Rigidbody _rb;


		[SerializeField, Required, ChildGameObjectsOnly]
		private SignalSender _defeatSignal;


		[SerializeField, Required]
		private PlayerProgression _playerProgression;


		[FoldoutGroup("Gizmos settings")]
		[SerializeField]
		private float _spheresRadius = 1;


		[FoldoutGroup("Gizmos settings")]
		[SerializeField]
		private float _spheresCount = 30;


		[SerializeField, ReadOnly]
		private List<Vector3> _randomPositions = new();


		private PlayerReferences _playerReferences;


		public bool Invisible
		{
			get => _invisible;
			set => _invisible = value;
		}


		private bool _invisible;


		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_playerReferences = GetComponent<PlayerReferences>();
		}


		private void OnCollisionEnter(Collision other)
		{
			if (_invisible)
				return;

			if (other.gameObject.CompareTag("Bug") && other.transform.TryGetComponent(out Bug bug))
			{
				if (bug.Level == _playerProgression.CurrentLevel)
				{
					InsertBug(bug);
					IncreaseSize();
					OnIncreaseSize.Invoke();
				}
				else if (bug.Level < _playerProgression.CurrentLevel)
				{
					InsertBug(bug);
					IncreaseSize();
				}
				else if (bug.Level > _playerProgression.CurrentLevel)
				{
					_defeatSignal.SendSignal();
					Destroy(other.gameObject);
				}
			}
		}


		private void OnTriggerEnter(Collider other)
		{
			if (_invisible)
				return;

			if (other.gameObject.CompareTag("Bug") && other.transform.TryGetComponent(out Bug bug))
			{
				if (bug.Level == _playerProgression.CurrentLevel)
				{
					InsertBug(bug);
					IncreaseSize();
					OnIncreaseSize.Invoke();
				}
				else if (bug.Level < _playerProgression.CurrentLevel)
				{
					InsertBug(bug);
					IncreaseSize();
				}
				else if (bug.Level > _playerProgression.CurrentLevel)
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
				_physicalSize += _increaseSizeValue; // Точное увеличение физического размера

				UpdateScaleAndCameraOffset();
				AdjustBugsScaleAndUpdatePositions();
			}
		}


		public void DecreaseSize()
		{
			if (_physicalSize - _increaseSizeValue >= _minSize) // Уменьшаем размер, если не меньше минимума
			{
				_physicalSize -= _increaseSizeValue; // Точное уменьшение физического размера
				UpdateScaleAndCameraOffset();
				AdjustBugsScaleAndUpdatePositions();
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
			if (!_smashedBugs.Contains(bug))
				_smashedBugs.Add(bug);
			else
				return;

			bug.Deactivate();
			OnKillBug.Invoke();

			// Определяем точку для размещения на сфере, учитывая текущий масштаб игрока

			// Делаем объект дочерним и устанавливаем начальную позицию
			bug.transform.parent = _playerReferences.BugsContainer;
			Vector3 inverseScale = Vector3.one / transform.localScale.x;
			bug.transform.localScale = Vector3.zero;

			// Установим фиксированный масштаб для жука, предположим, что исходный масштаб жука подходит
			bug.transform.localScale = inverseScale * 0.01f; // 0.1f - базовый размер жука
			bug.transform.LookAt(transform.position);

			bug.transform.Rotate(-90,
				0f,
				0f
			);

			bug.transform.position = GetRandomPositionWithCurrentScale();
		}


		private void AdjustBugsScaleAndUpdatePositions()
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

				bug.transform.position = GetRandomPositionWithCurrentScale();
			}
		}


		[Button]
		private void GenerateRandomPositions()
		{
			_randomPositions.Clear();

			for (int i = 0; i < _spheresCount; i++)
			{
				_randomPositions.Add(Random.onUnitSphere * _centerOfSphereDelta * transform.localScale.x);
			}
		}


		private Vector3 GetRandomPositionWithCurrentScale() => transform.position + (Random.onUnitSphere * _centerOfSphereDelta * transform.localScale.x);


		[Button]
		private void ClearGizmosPositions()
		{
			_randomPositions.Clear();
		}


		private void OnDrawGizmosSelected()
		{
			if (_randomPositions.Count > 0)
			{
				Gizmos.color = Color.green;

				foreach (var position in _randomPositions)
				{
					Gizmos.DrawSphere(transform.position + position,
						_spheresRadius * transform.localScale.x
					);
				}
			}
		}
	}
}
