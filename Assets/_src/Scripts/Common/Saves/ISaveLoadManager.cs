namespace _src.Scripts.Common.Saves
{
	public interface ISaveLoadManager
	{
		void LoadData();
		void SaveData();
		void SubmitChanges();
	}
}