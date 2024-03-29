using _src.Scripts.Data;
using _src.Scripts.SocialPlatform.Leaderboards;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _src.Scripts
{
	public class LevelProgress : MonoBehaviour
	{
		[SerializeField, SceneObjectsOnly]
		private SceneSavableDataProvider _sceneSavableDataProvider;
		
		[SerializeField]
		private Slider _progressFill;

		[SerializeField, Required]
		private PlayerBugSmasher _playerBugSmasher;

		[SerializeField, Required]
		private PlayerProgression _playerProgression;

		private float _currentProgress;
		[SerializeField, ReadOnly]
		private int _currentLevel = 1;

		private void Start()
		{
			_playerBugSmasher.OnIncreaseSize.AddListener(IncrementProgress);
			_currentProgress = 0f;
		}


		public void SetupSlider(Slider slider)
		{
			_progressFill = slider;
			_progressFill.value = CalculateFillAmount();
		}

		private void IncrementProgress()
		{
			_currentProgress++;
			// Обновление уровня, если текущий уровень в _playerProgression изменился
			if (_playerProgression.CurrentLevel != _currentLevel)
			{
				// Обновление текущего уровня и сброс прогресса
				_currentLevel = _playerProgression.CurrentLevel;
				OnCompleteLevel(); // todo move to level complete when will be ready
				_currentProgress = 1; // Поскольку событие увеличения уже произошло, начинаем отсчет с 1
			}
			_progressFill.value = CalculateFillAmount();
		}

		private float CalculateFillAmount()
		{
			// Определяем количество жуков, необходимых для следующего уровня
			int bugsRequiredForCurrentLevel = _playerProgression.LevelUpRequirements.ContainsKey(_currentLevel + 1) ?
				_playerProgression.LevelUpRequirements[_currentLevel + 1] : 
				0; // Если ключа нет, используем 0 для безопасности

			float progressFraction = bugsRequiredForCurrentLevel > 0 ? 
				(float)(_currentProgress - 1) / (bugsRequiredForCurrentLevel - 1) : 
				0; // Вычитаем 1, так как счет начинается с 1

			// Рассчитываем положение на слайдере в зависимости от текущего уровня и прогресса
			float baseValue = _currentLevel * 0.25f;
			float scaledProgress = baseValue + (progressFraction * 0.25f); // Добавляем долю прогресса к базовому значению

			return Mathf.Clamp(scaledProgress, 0, 1); // Ограничиваем значение между 0 и 1 для безопасности
		}


		private void OnCompleteLevel()
		{
			LeaderboardProvider.Instance.TrySetNewHighScore(BallController.Instance.GetTotalScore());
			_sceneSavableDataProvider.CompletedLevelInfo.AddCompletedLevel(_currentLevel); // todo change _currenLevel on level id
			_sceneSavableDataProvider.SaveLoadManager.SaveData();	
			_sceneSavableDataProvider.SaveLoadManager.SubmitChanges();	
		}


		private void OnDestroy()
		{
			if (_playerBugSmasher != null)
			{
				_playerBugSmasher.OnIncreaseSize.RemoveListener(IncrementProgress);
			}
		}
	}
}
