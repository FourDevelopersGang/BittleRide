using System;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;


namespace _src.Scripts.Ads
{
	public class AdsManager : MonoBehaviour
	{
		[SerializeField]
		private bool _loadInterstitialOnEnable;

		[SerializeField]
		private bool _loadRewardedOnEnable;

		[SerializeField]
		private bool _loadNextInterstitialAfterClose;
		
		[SerializeField]
		private bool _loadNextRewardAfterClose;
		
		private InterstitialCallback _interstitialCallback;

		private RewardedVideoCallback _rewardedVideoCallback;

		private void OnEnable()
		{
			IronSourceInterstitialEvents.onAdClosedEvent += OnInterstitialAdClosedEvent;
			
			IronSourceRewardedVideoEvents.onAdRewardedEvent += OnCompleteWatchRewardedVideo;
			IronSourceRewardedVideoEvents.onAdAvailableEvent += OnLoadRewardedVideo;
			IronSourceRewardedVideoEvents.onAdClosedEvent += OnRewardedVideoClosed;
			
			if(_loadInterstitialOnEnable)
				RequestInterstitial();
			
			if(_loadRewardedOnEnable)
				RequestRewardVideo();
		}

		private void OnDisable()
		{
			IronSourceInterstitialEvents.onAdClosedEvent -= OnInterstitialAdClosedEvent;
			
			IronSourceRewardedVideoEvents.onAdRewardedEvent -= OnCompleteWatchRewardedVideo;
			IronSourceRewardedVideoEvents.onAdAvailableEvent -= OnLoadRewardedVideo;
			IronSourceRewardedVideoEvents.onAdClosedEvent -= OnRewardedVideoClosed;
		}
		
		private void OnRewardedVideoClosed(IronSourceAdInfo obj)
		{
			if(_loadNextRewardAfterClose)
				RequestRewardVideo();
		}
		
		private void OnLoadRewardedVideo(IronSourceAdInfo obj)
		{
			if (_rewardedVideoCallback != null)
				_rewardedVideoCallback.RaiseOnLoadRewardVideo();
		}

		private void OnCompleteWatchRewardedVideo(IronSourcePlacement arg1, IronSourceAdInfo arg2)
		{
			if (_rewardedVideoCallback != null)
				_rewardedVideoCallback.RaiseOnCompleteWatch();
		}

		

		private void OnInterstitialAdClosedEvent(IronSourceAdInfo obj)
		{
			if(_loadNextInterstitialAfterClose)
				RequestInterstitial();
			
			if(_interstitialCallback != null)
				_interstitialCallback.RaiseOnClosed();
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			IronSource.Agent.onApplicationPause(pauseStatus);
		}

		public void RequestInterstitial()
		{
			IronSource.Agent.loadInterstitial();
		}

		public void ShowInterstitial(string adName, InterstitialCallback callback)
		{
			if (!IronSource.Agent.isInterstitialReady())
				return;

			_interstitialCallback = callback;
			
			if (adName.IsNullOrEmpty())
			{
				IronSource.Agent.showInterstitial();	
			}
			else
			{
				IronSource.Agent.showInterstitial(adName);
			}
		}
		
		public void RequestRewardVideo()
		{
			IronSource.Agent.loadRewardedVideo();
		}

		public void ShowRewardVideo(string adName, RewardedVideoCallback callback)
		{
			if (!IronSource.Agent.isRewardedVideoAvailable())
				return;

			_rewardedVideoCallback = callback;
			
			if (adName.IsNullOrEmpty())
			{
				IronSource.Agent.showRewardedVideo();
			}
			else
			{
				IronSource.Agent.showRewardedVideo(adName);
			}
		}

	}
}
