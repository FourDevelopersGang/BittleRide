using System;
using _src.Scripts.Common.Timer;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace _src.Scripts.Boosters
{
	public class InvisibleBooster : Booster
	{
		[SerializeField]
		[Range(0,1)]
		private float _transparency = 0.5f;
		
		private async UniTask Behaviour()
		{
			if (_player)
			{
				if (_player.TryGetComponent(out PlayerBugSmasher playerBugSmasher))
				{
		
					playerBugSmasher.Invisible = true;
					await UniTask.Delay(TimeSpan.FromSeconds(_duration));
			
					_boosterTimerUI.StopUITimer();
					playerBugSmasher.Invisible = false;
				}
			}
			else
			{
				Debug.LogError("No have player on", gameObject);
			}
		}


		protected override void OnPickUp()
		{
			base.OnPickUp();
			Behaviour().Forget();
		}
	}
}
