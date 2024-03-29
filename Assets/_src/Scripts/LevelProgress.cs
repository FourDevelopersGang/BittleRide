using _src.Scripts.Data;
using _src.Scripts.SocialPlatform.Leaderboards;
using _src.Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


namespace _src.Scripts
{
	public class LevelProgress : MonoBehaviour
	{
		[SerializeField, SceneObjectsOnly]
		private SceneSavableDataProvider _sceneSavableDataProvider;


		[SerializeField, Required]
		private PlayerBugSmasher _playerBugSmasher;


		[SerializeField, Required]
		private PlayerProgression _playerProgression;


		[SerializeField]
		private int _levelForWin = 1;


		private void Start()
		{
			_playerProgression.OnLevelUp.AddListener(CheckLevelCompleteConditionals);
		}


		


		private void OnCompleteLevel()
		{
			LeaderboardProvider.Instance.TrySetNewHighScore(BallController.Instance.GetTotalScore());
			_sceneSavableDataProvider.CompletedLevelInfo.AddCompletedLevel(_playerProgression.CurrentLevel); // todo change _currenLevel on level id
			_sceneSavableDataProvider.SaveLoadManager.SaveData();
			_sceneSavableDataProvider.SaveLoadManager.SubmitChanges();
		}


		private void CheckLevelCompleteConditionals(int currentPlayerLevel)
		{
			if (currentPlayerLevel == _levelForWin)
			{
				OnCompleteLevel(); // Обработка завершения уровня
				_playerBugSmasher.WinSignal.SendSignal();
			}
		}


		private void OnDestroy()
		{
			if (_playerProgression != null)
			{
				_playerProgression.OnLevelUp.RemoveListener(CheckLevelCompleteConditionals);
			}
		}
	}
}
