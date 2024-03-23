using System;
using System.Linq;
using _src.Scripts.Data;
using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts.UI
{
	public class LevelsMenuUI : MonoBehaviour
	{
		[SerializeField, SceneObjectsOnly]
		private SceneSavableDataProvider _sceneSavableDataProvider;

		private LevelUI[] _levelUis;

		private void Awake()
		{
			_levelUis = GetComponentsInChildren<LevelUI>(true);
		}

		private void Start()
		{
			InitLevelsUi();
		}

		private void InitLevelsUi()
		{
			var lastCompletedLevelIndex = -1;

			for (var index = 0; index < _levelUis.Length; ++index)
			{
				if (!CheckLevelCompleted(index + 1))
					continue;

				_levelUis[index].Unlock();
				_levelUis[index].MarkAsCompleted();
				lastCompletedLevelIndex = index;
			}
			
			// unlock next level after last completed
			if (lastCompletedLevelIndex + 1 < _levelUis.Length)
			{
				_levelUis[lastCompletedLevelIndex + 1].Unlock();
			}
		}

		private bool CheckLevelCompleted(int levelId) => 
			_sceneSavableDataProvider.CompletedLevelInfo.CompletedLevels is {Count: > 0} && 
			_sceneSavableDataProvider.CompletedLevelInfo.CompletedLevels.Contains(levelId);
	}
}
