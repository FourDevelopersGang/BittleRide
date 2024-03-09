using _src.Scripts.Common.Counter;
using DG.Tweening;
using TMPro;
using UnityEngine;


namespace _src.Scripts.UI
{
	public class CounterUI : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _counterInfo;

		private Counter.CounterInfoProvider _counterInfoProvider;

		private Tween _counterChangeValueAnimation;

		private void Awake()
		{
			_counterChangeValueAnimation = DOTween.Sequence()
				.Append(_counterInfo.transform.DOShakePosition(.20f, strength: 10f))
				.Join(_counterInfo.DOColor(Color.green, .25f)
					.SetEase(Ease.InOutFlash, 10, 0)
				)
				.SetAutoKill(false);

			_counterChangeValueAnimation.Rewind();
		}

		private void OnDestroy()
		{
			_counterChangeValueAnimation.Kill();
		}

		public void SetCounterInfoProvider(Counter counter)
		{
			_counterInfoProvider = counter.GetInfoProvider();
			_counterInfoProvider.AddListenerReachedTarget(OnReachTarget);
			_counterInfoProvider.AddListenerChangedCurrentValue(OnChangedCurrentValue);

			OnChangedCurrentValue(0);
		}

		private void OnChangedCurrentValue(int value)
		{
			_counterInfo.text = $"{_counterInfoProvider.CurrentValue} / {_counterInfoProvider.Target}";

			_counterChangeValueAnimation.Restart();
		}

		private void OnReachTarget()
		{
			_counterInfoProvider.RemoveListenerChangedCurrentValue(OnChangedCurrentValue);
			_counterInfoProvider.RemoveListenerReachedTarget(OnReachTarget);
		}
	}
}
