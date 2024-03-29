using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace _src.Scripts.UI
{
	public class PlayerProgressionUi : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _currentLevel;


		[SerializeField]
		private MMF_Player _levelUpFeedbacks;


		[SerializeField]
		private Slider _slider;
		
		public void UpdateSliderValue(float value)
		{
			_slider.value = value;
		}


		public void LevelUp(int level)
		{
			_levelUpFeedbacks.PlayFeedbacks();
			_currentLevel.text = level.ToString();
			_slider.value = 0;
		}

	}
}
