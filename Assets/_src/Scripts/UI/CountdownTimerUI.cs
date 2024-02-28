using System;
using _src.Scripts.Common.Timer;
using DG.Tweening;
using Doozy.Runtime.Signals;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;


namespace _src.Scripts.UI
{
	public class CountdownTimerUI : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _timer;

		[SerializeField, Min(0)]
		private int _lastTickStartFrom;

		[SerializeField]
		private MMF_Player _endTimeSecondTickFeedback;

		private Timer.ITimerInfoProvider _timerInfoProvider;

		private int _tick;

		private Tween _lastTickAnimation;

		private void Awake()
		{
			_lastTickAnimation = DOTween.Sequence()
				.Append(_timer.transform.DOShakePosition(.20f, strength: 10f))
				.Join(_timer.DOColor(Color.red, 0.25f)
					.SetEase(Ease.InOutFlash, 10, 0)
				)
				.SetAutoKill(false);

			_lastTickAnimation.Rewind();
		}

		public void SetTimerInfoProvider(Timer timer)
		{
			_timerInfoProvider = timer.GetInfoProvider(true);
		}

		private void Update()
		{
			UpdateView();
		}

		private void OnDestroy()
		{
			_lastTickAnimation.Kill();
		}

		private void UpdateView()
		{
			if (_timerInfoProvider == null)
				return;

			var ceiledSeconds = Mathf.CeilToInt(_timerInfoProvider.LeftTime);

			_timer.text = ceiledSeconds + " .sec";

			if (ceiledSeconds <= _lastTickStartFrom && ceiledSeconds != _tick)
			{
				if (_endTimeSecondTickFeedback)
					_endTimeSecondTickFeedback.PlayFeedbacks();

				_lastTickAnimation.Restart();
			}

			_tick = ceiledSeconds;
		}
	}
}
