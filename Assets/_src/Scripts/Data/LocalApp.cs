using System;
using _src.Scripts.Common.Saves;
using UnityEngine;


namespace _src.Scripts.Data
{
	[Serializable]
	public class LocalApp : GenericSavableData<LocalApp>
	{
		[SerializeField]
		private string _localId = "";

		public string LocalId
		{
			set => ChangeField(ref _localId, value);
			get => _localId;
		}
		
		public override void Load(LocalApp savableData)
		{
			_localId = savableData._localId;
		}
	}
}
