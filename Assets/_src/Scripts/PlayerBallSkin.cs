using System;
using System.Collections.Generic;
using UnityEngine;


namespace _src.Scripts
{
	public class PlayerBallSkin : MonoBehaviour
	{
		private List<BallVariant> _ballVariants;

		private BallVariant _activeSkin;

		public Renderer Renderer => _activeSkin.Renderer;

		private void Awake()
		{
			_ballVariants = new List<BallVariant>(GetComponentsInChildren<BallVariant>(true));

			for (var index = 0; index < _ballVariants.Count; index++)
			{
				var ballVariant = _ballVariants[index];

				if (index == 0)
				{
					ballVariant.gameObject.SetActive(true);
					_activeSkin = ballVariant;
				}
				else
				{
					ballVariant.gameObject.SetActive(false);
				}
			}
		}

		public void Select(int skinId)
		{
			_activeSkin.gameObject.SetActive(false);
			
			if(skinId >= _ballVariants.Count)
				Debug.LogWarning("Haven't chosen skin");
			
			_activeSkin = _ballVariants[skinId % _ballVariants.Count];
			_activeSkin.gameObject.SetActive(true);
		}

	}
}
