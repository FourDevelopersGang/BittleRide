using _src.Scripts.GameEventsSystem;


namespace _src.Scripts.SignalsData
{
	public class StartGameEventData
	{
		public readonly GameEventType EventType;

		public readonly BaseGameEvent Event;

		public StartGameEventData(GameEventType eventType, BaseGameEvent @event)
		{
			EventType = eventType;
			Event = @event;
		}
	}
}
