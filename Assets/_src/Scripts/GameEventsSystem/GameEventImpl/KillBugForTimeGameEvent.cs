using System;
using System.Threading;
using _src.Scripts.Common.Counter;
using _src.Scripts.Common.Timer;
using Cysharp.Threading.Tasks;
using Timer = _src.Scripts.Common.Timer.Timer;


namespace _src.Scripts.GameEventsSystem.GameEventImpl
{
	public class KillBugForTimeGameEvent : BaseGameEvent, IHasCounter, IHasTimer
	{
		private readonly float _durationEvent;
		private readonly int _countKillForComplete;
		private readonly PlayerIncrease _playerIncrease;

		private CancellationTokenSource _stopEventCancellationTokenSource;

		private Timer _timer;
		private Counter _counter;

		public Counter Counter => _counter;

		public Timer Timer => _timer;

		public KillBugForTimeGameEvent(float durationEvent, int countKillForComplete, PlayerIncrease playerIncrease)
		{
			_durationEvent = durationEvent;
			_countKillForComplete = countKillForComplete;
			_playerIncrease = playerIncrease;
		}

		public override async UniTaskVoid Start(CancellationTokenSource stopCancellationTokenSource)
		{
			InitStart();

			while (!_counter.IsCancelled && !_timer.IsCanceled && !stopCancellationTokenSource.IsCancellationRequested)
			{
				await UniTask.Yield(PlayerLoopTiming.Update, stopCancellationTokenSource.Token);

				_timer.Tick();
			}

			RemoveListeners();
		}

		private void InitStart()
		{
			_timer = new Timer(_durationEvent);
			_timer.OnCancelling += OnFailedEvent;

			_counter = new Counter(_countKillForComplete);
			_counter.ReachedTarget += OnKillRequiredBugs;
			
			_playerIncrease.OnKillBug.AddListener(OnKillBug);
		}

		private void OnKillBug()
		{
			_counter.Add();
		}

		private void RemoveListeners()
		{
			_timer.OnCancelling -= OnFailedEvent;
			_counter.ReachedTarget -= OnKillRequiredBugs;
			
			_playerIncrease.OnKillBug.RemoveListener(OnKillBug);
		}

		private void OnKillRequiredBugs()
		{
			RaiseOnComplete();
		}

		private void OnFailedEvent(bool manualCancel)
		{
			if (!manualCancel)
				RaiseOnFail();
		}
	}
}
