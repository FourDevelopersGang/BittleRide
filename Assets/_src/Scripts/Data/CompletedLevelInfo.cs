using System;
using System.Collections.Generic;
using _src.Scripts.Common.Saves;
using UnityEngine;


namespace _src.Scripts.Data
{
	[Serializable]
	public class CompletedLevelInfo : GenericSavableData<CompletedLevelInfo>
	{
		[SerializeField]
		private List<int> _completedLevels;

		public IReadOnlyList<int> CompletedLevels => _completedLevels;

		public override void Load(CompletedLevelInfo savableData)
		{
			_completedLevels = savableData._completedLevels;
		}

		public void AddCompletedLevel(int levelId)
		{
			_completedLevels ??= new List<int>();

			if (_completedLevels.Contains(levelId))
				return;

			_completedLevels.Add(levelId);
			MarkDataAsChanged();
		}
	}
}
