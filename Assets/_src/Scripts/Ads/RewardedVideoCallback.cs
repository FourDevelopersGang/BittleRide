using UnityEngine;
using UnityEngine.Events;


namespace _src.Scripts.Ads
{
	public class RewardedVideoCallback : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent _onCompleteWatchAd;
		
		[SerializeField]
		private UnityEvent _onLoadRewardAd;
		
		public void RaiseOnCompleteWatch()
		{
			_onCompleteWatchAd.Invoke();
		}

		public void RaiseOnLoadRewardVideo()
		{
			_onLoadRewardAd.Invoke();
		}
	}
}
