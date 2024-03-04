using UnityEngine;


namespace _src.Scripts.Common.Saves.DataHolderImpl
{
	public class SerializedDataHolder : IHolderData
	{
		private readonly IDataStorage _dataStorage;

		public IDataStorage Storage => _dataStorage;

		public SerializedDataHolder(IDataStorage dataStorage)
		{
			_dataStorage = dataStorage;
		}

		public T Get<T>() where T : new()
		{
			if (_dataStorage.Load(typeof(T).FullName, out var jsonData))
			{
				var deserializeObject = JsonUtility.FromJson<T>(jsonData);
				return deserializeObject;
			}

			var newValue = new T();
			Set(newValue);
			return newValue;
		}

		public void Set<T>(T value)
		{
			var serializedData = JsonUtility.ToJson(value);
			_dataStorage.Save(typeof(T).FullName, serializedData);
		}
	}
}