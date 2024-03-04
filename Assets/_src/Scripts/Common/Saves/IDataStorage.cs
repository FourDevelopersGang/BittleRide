namespace _src.Scripts.Common.Saves
{
	public interface IDataStorage
	{
		bool Load(string key, out string data);
		void Save(string key, string saveData);
		void Commit();
	}
}