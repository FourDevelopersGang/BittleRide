using UnityEngine;


namespace _src.Scripts.Common.Saves.DataStorageImpl
{
	public class PlayerPrefDataStorage : IDataStorage
	{
		public bool Load(string key, out string data)
		{
			if (PlayerPrefs.HasKey(key))
			{
				data = PlayerPrefs.GetString(key);
				return true;
			}

			data = default;
			
			return false;
		}

		public void Save(string key, string saveData)
		{
			PlayerPrefs.SetString(key, saveData);
		}

		public void Commit()
		{
			PlayerPrefs.Save();
		}
	}
}