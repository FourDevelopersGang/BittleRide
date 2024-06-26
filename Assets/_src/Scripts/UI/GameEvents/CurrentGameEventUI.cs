﻿using System;
using _src.Scripts.Common.Counter;
using _src.Scripts.Common.Timer;
using _src.Scripts.GameEventsSystem;
using _src.Scripts.SignalsData;
using Doozy.Runtime.Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Serialization;


namespace _src.Scripts.UI.GameEvents
{
	public class CurrentGameEventUI : MonoBehaviour
	{
		private const string GameEventLocalizationKeyPrefix = "name_";
		
		[Title("Signal Start Game Event")]
		[SerializeField]
		private StreamId _signalStartGameEventId;

		[SerializeField, ChildGameObjectsOnly, Required]
		private TextMeshProUGUI _nameEvent;
		
		[SerializeField, ChildGameObjectsOnly, Required]
		private CounterUI _counterUI;
		
		[SerializeField, ChildGameObjectsOnly, Required]
		private CountdownTimerUI _countdownTimerUI;

		[Title("Localization")]
		[SerializeField]
		private LocalizedStringTable  _eventLocalizationTable;

		private SignalReceiver _signalStartGameEventsReceiver;

		private void Awake()
		{
			_signalStartGameEventsReceiver = new SignalReceiver().SetOnSignalCallback(OnStartGameEvent);
		}

		private void OnEnable()
		{
			SignalStream.Get(_signalStartGameEventId.Category, _signalStartGameEventId.Name)
				.ConnectReceiver(_signalStartGameEventsReceiver);
		}

		private void OnDisable()
		{
			SignalStream.Get(_signalStartGameEventId.Category, _signalStartGameEventId.Name)
				.DisconnectReceiver(_signalStartGameEventsReceiver);
		}

		private void OnStartGameEvent(Signal signal)
		{
			if (signal.hasValue && signal.TryGetValue(out StartGameEventData startGameEventData))
			{
				InitGameEventUI(startGameEventData);
			}
		}

		private void InitGameEventUI(StartGameEventData startGameEventData)
		{
			_nameEvent.text = GetNameEventByType(startGameEventData.EventType);

			_counterUI.gameObject.SetActive(startGameEventData.Event is IHasCounter);
			if (startGameEventData.Event is IHasCounter hasCounter)
			{
				_counterUI.SetCounterInfoProvider(hasCounter.Counter);
			}
			
			
			_countdownTimerUI.gameObject.SetActive(startGameEventData.Event is IHasTimer);
			if (startGameEventData.Event is IHasTimer hasTimer)
			{
				_countdownTimerUI.SetTimerInfoProvider(hasTimer.Timer);	
			}
		}

		private string GetNameEventByType(GameEventType gameEventType) => 
			LocalizationSettings.StringDatabase.GetLocalizedString(_eventLocalizationTable.TableReference, 
				GameEventLocalizationKeyPrefix + gameEventType);
	}
}
