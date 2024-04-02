using System;
using UnityEngine;
using UnityEngine.Events;


namespace _src.Scripts.Ads
{
	public class InterstitialCallback : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent _onClosedAd;

		public void RaiseOnClosed()
		{
			_onClosedAd.Invoke();
		}
	}
}
