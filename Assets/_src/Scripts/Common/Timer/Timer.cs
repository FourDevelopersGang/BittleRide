using System;
using UnityEngine;


namespace _src.Scripts.Common.Timer
{
	public class Timer
	{
		private float _lapsedTime;

		private float _timerTime;

		public event Action<bool> OnCancelling; 

		public bool IsCanceled => _lapsedTime >= _timerTime;

		public Timer(float timerTime)
		{
			_timerTime = timerTime;
			_lapsedTime = 0;
		}

		public void Tick()
		{
			if (IsCanceled)
				return;
			
			_lapsedTime += Time.deltaTime;

			if (IsCanceled)
				OnCancelling?.Invoke(false);
		}

		public void ForceCancel()
		{
			_lapsedTime = _timerTime;
			OnCancelling?.Invoke(true);
		}

		public ITimerInfoProvider GetInfoProvider(bool isCountdown = false)
		{
			if (isCountdown)
				return new TimerInfoCountdownProvider(this);

			return new TimerInfoProvider(this);
		}
		
		public interface ITimerInfoProvider
		{
			public float LeftTime { get; }
			
			public float TimerTime { get; }
			
			public bool IsCancelled { get; }
		}

		public class TimerInfoCountdownProvider : ITimerInfoProvider
		{
			private Timer _timer;

			public TimerInfoCountdownProvider(Timer timer)
			{
				_timer = timer;
			}

			public float LeftTime => Mathf.Max(_timer._timerTime - _timer._lapsedTime, 0);

			public float TimerTime => _timer._timerTime;

			public bool IsCancelled => _timer.IsCanceled;
		}
		
		public class TimerInfoProvider : ITimerInfoProvider
		{
			private Timer _timer;

			public TimerInfoProvider(Timer timer)
			{
				_timer = timer;
			}

			public float LeftTime => Mathf.Max(_timer._lapsedTime, _timer._timerTime);

			public float TimerTime => _timer._timerTime;

			public bool IsCancelled => _timer.IsCanceled;
		}
	}
}
