using System;
using System.Collections.Generic;
using System.Threading;
using _src.Scripts.GameEventsSystem.GameEventImpl;
using _src.Scripts.SignalsData;
using Doozy.Runtime.Signals;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;


namespace _src.Scripts.GameEventsSystem
{
	public class GameEventsSystem : MonoBehaviour
	{
		[Title("Signal Start Game Event")]
		[SerializeField]
		private StreamId _signalStartGameEventId;

		[Title("Signal End Game Event")]
		[SerializeField]
		private StreamId _signalEndGameEventId;

		[TitleGroup("Event settings")]
		[BoxGroup("Event settings/Kill Bug For Time event"), SerializeField, Min(10)]
		private float _durationEventKillBugForTime;

		[BoxGroup("Event settings/Kill Bug For Time event"), SerializeField, Min(1)]
		private int _countKillBugForComplete;

		[BoxGroup("Event settings/Kill Bug For Time event"), SerializeField, Required]
		private PlayerBugSmasher _playerBugSmasher;

		[Title("Feedbacks")]
		[SerializeField]
		private MMF_Player _startEventFeedback;

		[SerializeField]
		private MMF_Player _endEventFeedback;

		[SerializeField]
		private MMF_Player _completeEventFeedback;

		[SerializeField]
		private MMF_Player _failEventFeedback;

		private readonly Dictionary<GameEventType, BaseGameEvent> _gameEvents = new();

		private CancellationTokenSource _currentEventCancellationTokenSource;

		private BaseGameEvent _currentGameEvent;

		private bool EventInRunning => _currentEventCancellationTokenSource is {IsCancellationRequested: true};

		private void Awake()
		{
			InitialEvents();
		}

		private void InitialEvents()
		{
			_gameEvents.Add(GameEventType.KillBugForTime,
				new KillBugForTimeGameEvent(_durationEventKillBugForTime, _countKillBugForComplete, _playerBugSmasher)
			);
		}

		private void OnEndEvent()
		{
			if (_endEventFeedback)
				_endEventFeedback.PlayFeedbacks();

			SignalsService.SendSignal(_signalEndGameEventId.Category, _signalEndGameEventId.Name);
		}

		private void OnCompleteEvent()
		{
			ApplyReward();

			StopCurrentEvent();

			RemoveListeners(_currentGameEvent);

			if (_completeEventFeedback)
				_completeEventFeedback.PlayFeedbacks();

			OnEndEvent();
		}

		private void ApplyReward()
		{
			Debug.Log("Apply reward for complete event");
			//todo: Make reward when booster will be ready
		}

		private void OnFailEvent()
		{
			Debug.Log("Fail event");
			
			StopCurrentEvent();

			RemoveListeners(_currentGameEvent);

			if (_failEventFeedback)
				_failEventFeedback.PlayFeedbacks();

			OnEndEvent();
		}

		private void OnDestroy()
		{
			StopCurrentEvent();
		}

		private void StopCurrentEvent()
		{
			if (_currentEventCancellationTokenSource is {IsCancellationRequested: false})
			{
				_currentEventCancellationTokenSource.Cancel();
				_currentEventCancellationTokenSource.Dispose();
			}
		}

		public bool TryStartEvent(GameEventType gameEventType)
		{
			if (!_gameEvents.ContainsKey(gameEventType))
			{
				Debug.LogWarning($"Try start event ({gameEventType.ToString()}) that not init in system");

				return false;
			}

			if (EventInRunning)
				return false;

			StartGameEvent(gameEventType);

			return true;
		}

		private void StartGameEvent(GameEventType gameEventType)
		{
			_currentEventCancellationTokenSource = new CancellationTokenSource();

			_currentGameEvent = _gameEvents[gameEventType];

			InitListeners(_currentGameEvent);

			_currentGameEvent.Start(_currentEventCancellationTokenSource).Forget();
			
			if (_startEventFeedback)
				_startEventFeedback.PlayFeedbacks();

			var startGameEventData = new StartGameEventData(gameEventType, _currentGameEvent);
			Signal.Send(_signalStartGameEventId.Category, _signalStartGameEventId.Name, startGameEventData);
		}

		private void InitListeners(BaseGameEvent gameEvent)
		{
			gameEvent.OnComplete += OnCompleteEvent;
			gameEvent.OnFail += OnFailEvent;
		}

		private void RemoveListeners(BaseGameEvent gameEvent)
		{
			gameEvent.OnComplete -= OnCompleteEvent;
			gameEvent.OnFail -= OnFailEvent;
		}
	}
}
