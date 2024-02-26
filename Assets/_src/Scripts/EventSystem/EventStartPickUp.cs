using _src.Scripts.Common;


namespace _src.Scripts.EventSystem
{
	public class EventStartPickUp : Pickupable
	{
		protected override void OnPickUp()
		{
			Destroy(gameObject, 3);
		}
	}
}
