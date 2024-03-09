namespace _src.Scripts.Common.Saves.SaveLoadImpl
{
	public class SavableDataLoaderSaver<T> : ILoaderSaverData where T : GenericSavableData<T>, new()
	{
		private readonly IHolderData _holderData;

		private readonly T _savableData;
		
		private readonly bool _enableAutoSave;
		
		private readonly bool _enableAutoCommit;
		
		public SavableDataLoaderSaver(IHolderData holderData, T savableData, bool enableAutoSave = false, bool enableAutoCommit = false)
		{
			_holderData = holderData;
			_savableData = savableData;
			_enableAutoSave = enableAutoSave;
			_enableAutoCommit = enableAutoCommit;

			if(enableAutoSave)
				_savableData.OnDataChanged += AutoSave;
		}
		
		public void Load()
		{
			_savableData.Load(_holderData.Get<T>());
		}

		public void Save()
		{
			if (!_savableData.WasChanged)
				return;

			_holderData.Set(_savableData);
			
			_savableData.UnmarkDataAsChanged();
				
			if(_enableAutoCommit)
				_holderData.Storage.Commit();
		}

		private void AutoSave()
		{
			if(_enableAutoSave)
				Save();
		}
	}
}
