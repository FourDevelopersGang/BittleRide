using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


namespace _src.Scripts.UI
{
	public class LevelProgressSelector : MonoBehaviour
	{
		[SerializeField, SceneObjectsOnly, Required]
		private LevelProgress _levelProgress;

		public UnityDictionary<int, Slider> _sliders = new();

		public int _currentLevelSliderId = 0;



		private void Start()
		{
			foreach (var slider in _sliders)
			{
				if (_currentLevelSliderId == slider.Key)
				{
					slider.Value.gameObject.SetActive(true);
					_levelProgress.SetupSlider(slider.Value);
					return;
				}
			}
		}
	}
}
