using System;
using _src.Scripts.Common;
using _src.Scripts.Common.Timer;
using _src.Scripts.UI;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts.Boosters
{
	[RequireComponent(typeof(SphereCollider))]
	public abstract class Booster : Pickupable, IHasTimer
	{
		private Collider _collider;
		
		[SerializeField]
		protected float _duration;


		[SerializeField, Required]
		protected BoosterTimerUI _boosterTimerUI;
		
		protected Timer _timer;

		protected virtual void Awake()
		{
			_collider = GetComponent<Collider>();
			_collider.isTrigger = true;
		}

		protected override void OnPickUp()
		{
			_timer = new Timer(_duration);
			_boosterTimerUI.StartUITimer(_timer);
			PlayTimerTickInUpdate().Forget();
			Behaviour();
		}
		public Timer Timer => _timer;


		protected async UniTask PlayTimerTickInUpdate()
		{
			while (!_timer.IsCanceled)
			{
				await UniTask.Yield(PlayerLoopTiming.Update, gameObject.GetCancellationTokenOnDestroy());

				_timer.Tick();
			}
		}


		protected abstract UniTask Behaviour();
	

		protected void StopTimer()
		{
			_timer.ForceCancel();
		}
	}
}
