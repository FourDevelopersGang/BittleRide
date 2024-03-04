using System;


namespace _src.Scripts.Common.Saves
{
	public abstract class GenericSavableData<T> : BaseSavableData where T : BaseSavableData, new()
	{
		public abstract void Load(T savableData);
	}
}
