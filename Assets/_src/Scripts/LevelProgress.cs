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


		[SerializeField]
		private int _levelForWin = 1;


		private void Start()
		{
			_playerBugSmasher.OnIncreaseSize.AddListener(IncrementProgress);
			_playerProgression.OnLevelUp.AddListener(CheckLevelCompleteConditionals);
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
			UpdateLevelAndProgress(); // Обновляем уровень и прогресс
		}


		private void UpdateLevelAndProgress()
		{
			_progressFill.value = CalculateFillAmount();
		}


		private float CalculateFillAmount()
		{
			int bugsRequiredForCurrentLevel = _playerProgression.LevelUpRequirements.ContainsKey(_currentLevel + 1) ? _playerProgression.LevelUpRequirements[_currentLevel + 1] : 0;

			// Изменение логики расчета прогресса в соответствии с текущим и следующим уровнем
			float progressFraction = bugsRequiredForCurrentLevel > 0 ? (float)_currentProgress / bugsRequiredForCurrentLevel : 0;

			// Учитываем количество флагов на слайдере в зависимости от уровня
			float numberOfFlags = _currentLevel + 1; // Количество флагов равно номеру текущего уровня + 1
			float sliderStep = 1f / numberOfFlags; // Шаг слайдера между флагами

			// Рассчитываем значение для слайдера
			float scaledProgress = sliderStep * _currentLevel + (progressFraction * sliderStep);

			return Mathf.Clamp(scaledProgress,
				0,
				1
			);
		}


		private void OnCompleteLevel()
		{
			LeaderboardProvider.Instance.TrySetNewHighScore(BallController.Instance.GetTotalScore());
			_sceneSavableDataProvider.CompletedLevelInfo.AddCompletedLevel(_currentLevel); // todo change _currenLevel on level id
			_sceneSavableDataProvider.SaveLoadManager.SaveData();
			_sceneSavableDataProvider.SaveLoadManager.SubmitChanges();
		}


		private void CheckLevelCompleteConditionals(int currentPlayerLeve)
		{
			if (currentPlayerLeve == _levelForWin)
			{
				_currentLevel = _playerProgression.CurrentLevel;
				OnCompleteLevel(); // Обработка завершения уровня
				_currentProgress = 1; // Начало отсчета с 1 для следующего уровня
				_playerBugSmasher.WinSignal.SendSignal();
			}
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
