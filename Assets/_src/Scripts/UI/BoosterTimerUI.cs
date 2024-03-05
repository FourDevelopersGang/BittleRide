using _src.Scripts.Boosters;
using _src.Scripts.Common.Timer;
using Doozy.Runtime.Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;


namespace _src.Scripts.UI
{
	public class BoosterTimerUI : MonoBehaviour
	{
		[Title("Signal Start Booster Timer")]
		[SerializeField]
		private StreamId _signalStarBoosterTimer;
		
		[Title("Signal Stop Booster Timer")]
		[SerializeField]
		private StreamId _signalStopBoosterTimer;
		[SerializeField, ChildGameObjectsOnly, Required]
		private TextMeshProUGUI _nameEvent;
		
		[SerializeField, ChildGameObjectsOnly, Required]
		private CountdownTimerUI _countdownTimerUI;


		[SerializeField]
		private BoosterType _boosterType;

		private SignalReceiver _signalStartReceiver;
		
		private SignalReceiver _signalStopReceiver;
		
		public void StartUITimer(Timer timer)
		{
			Signal.Send(_signalStarBoosterTimer.Category, _signalStarBoosterTimer.Name);
			_nameEvent.text = _boosterType.ToString();
			_countdownTimerUI.gameObject.SetActive(true);
			_countdownTimerUI.SetTimerInfoProvider(timer);

		}


		public void StopUITimer()
		{
			Signal.Send(_signalStopBoosterTimer.Category, _signalStopBoosterTimer.Name);
		}
	}
}
