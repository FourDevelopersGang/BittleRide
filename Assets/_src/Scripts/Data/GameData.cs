using System;
using _src.Scripts.Common.Saves;
using UnityEngine;


namespace _src.Scripts.Data
{
	[Serializable]
	public class GameData : GenericSavableData<GameData>
	{
		[SerializeField]
		private int _skin = 0;

		public int Skin
		{
			set => ChangeField(ref _skin, value);
			get => _skin;
		}
		
		public override void Load(GameData savableData)
		{
			_skin = savableData._skin;
		}
	}
}
