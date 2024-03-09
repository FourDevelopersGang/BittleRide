using System.Collections.Generic;
using _src.Scripts.Common.Saves.DataStorageImpl;
using _src.Scripts.Common.Saves.SaveLoadImpl;


namespace _src.Scripts.Common.Saves
{
	public class SaveLoadManager : ISaveLoadManager
	{
		private readonly IHolderData _holderData;
		private readonly List<ILoaderSaverData> _loaderAndSaver;
		
		public SaveLoadManager(IHolderData holderData, IEnumerable<ILoaderSaverData> loaderSavers)
		{
			_holderData = holderData;
			_loaderAndSaver = new List<ILoaderSaverData>(loaderSavers);
		}

		public void LoadData()
		{
			foreach(var saverLoader in _loaderAndSaver)
			{
				saverLoader.Load();
			}
		}

		public void SaveData()
		{
			foreach(var saverLoader in _loaderAndSaver)
			{
				saverLoader.Save();
			}
		}

		public void SubmitChanges()
		{
			_holderData.Storage.Commit();
		}
	}
}