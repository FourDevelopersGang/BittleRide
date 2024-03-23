using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts.Boosters
{
	public class MagneticBooster : Booster
	{
		[SerializeField]
		[Range(0,
			3
		)]
		private float _magneticRadius = 30;


		[SerializeField]
		[Range(0,
			10
		)]
		private float _magneticSpeed = 3;



		[ValueDropdown("GetAllLayers")]
		[SerializeField]
		private string _bugsLayer;


		private float _localTimer;


		protected override async UniTask Behaviour()
		{
			_localTimer = 0;
			_player.TryGetComponent(out PlayerProgression playerProgression);

			while (_localTimer < _duration)
			{
				_localTimer += Time.deltaTime;

				// Преобразуем имя слоя в маску слоя
				int layerMask = 1 << LayerMask.NameToLayer(_bugsLayer);

				// Используем OverlapSphere вместо SphereCastAll
				var bugs = Physics.OverlapSphere(_player.transform.position,
					_magneticRadius,
					layerMask
				);

				foreach (var collider in bugs)
				{
					if (collider.TryGetComponent(out Bug bugEntity) && !bugEntity.isSmashed)
					{
						// Рассчитываем продолжительность движения на основе расстояния и скорости
						float distance = Vector3.Distance(bugEntity.transform.position,
							_player.transform.position
						);

						float moveDuration = distance / _magneticSpeed;

						// Используем DOMove для анимации движения жука к игроку
						if (playerProgression.CurrentLevel >= bugEntity.Level)
							bugEntity.transform.DOMove(_player.transform.position,
								moveDuration
							).SetEase(Ease.Linear);
					}
				}

				await UniTask.Yield(PlayerLoopTiming.Update);
			}

			_localTimer = 0;
			_boosterTimerUI.StopUITimer();
		}


		private static IEnumerable GetAllLayers()
		{
			return Enumerable.Range(0,
				32
			).Select(LayerMask.LayerToName).Where(l => !string.IsNullOrEmpty(l)).ToArray();
		}


		private void OnDrawGizmos()
		{
			if (_player)
			{
				Gizmos.color = Color.green;

				Gizmos.DrawSphere(_player.transform.position,
					_magneticRadius
				);
			}
		}
	}
}
