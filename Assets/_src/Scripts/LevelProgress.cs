using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _src.Scripts
{
	public class LevelProgress : MonoBehaviour
	{
		[SerializeField]
		private Slider _progressFill;

		[SerializeField, Required]
		private PlayerIncrease _playerIncrease;

		[SerializeField, Required]
		private PlayerProgression _playerProgression;

		private float _currentProgress;
		[SerializeField, ReadOnly]
		private int _currentLevel;
		private int _bugsToLevelUp;

		private void Start()
		{
			_currentLevel = _playerProgression.CurrentLevel;
			_bugsToLevelUp = _playerProgression.BugsToLevelUp;
			_playerIncrease.OnIncreaseSize.AddListener(IncrementProgress);
			_currentProgress = 0f;
			_progressFill.value = CalculateFillAmount();
		}

		private void IncrementProgress()
		{
			_currentProgress++;
			// Проверка на изменение уровня для корректировки прогресса
			if (_playerProgression.CurrentLevel != _currentLevel)
			{
				_currentLevel = _playerProgression.CurrentLevel;
				_bugsToLevelUp = _playerProgression.BugsToLevelUp;
				_currentProgress = 1; // Сброс прогресса с учетом нового уровня
			}
			_progressFill.value = CalculateFillAmount();
		}

		private float CalculateFillAmount()
		{
			// Рассчитываем процент заполнения для текущего уровня
			return _currentProgress / _bugsToLevelUp;
		}

		private void OnDestroy()
		{
			if (_playerIncrease != null)
			{
				_playerIncrease.OnIncreaseSize.RemoveListener(IncrementProgress);
			}
		}
	}
}
