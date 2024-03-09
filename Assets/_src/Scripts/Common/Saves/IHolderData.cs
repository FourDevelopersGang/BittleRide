namespace _src.Scripts.Common.Saves
{
	public interface IHolderData
	{
		IDataStorage Storage { get; }
		
		T Get<T>() where T : new();
		
		void Set<T>(T value);
	}
}