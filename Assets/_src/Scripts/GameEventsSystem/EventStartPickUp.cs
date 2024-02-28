using _src.Scripts.Common;
using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts.GameEventsSystem
{
	public class EventStartPickUp : Pickupable
	{
		[SerializeField, Required]
		private GameEventsSystem _gameEventsSystem;

		[SerializeField]
		private GameEventType _gameEventType;
		
		protected override void OnPickUp()
		{
			_gameEventsSystem.TryStartEvent(_gameEventType);
			Destroy(gameObject, 3);
		}
	}
}
