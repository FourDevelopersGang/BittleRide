using System;
using System.Collections.Generic;
using _src.Scripts.Common.Saves;
using _src.Scripts.Common.Saves.DataHolderImpl;
using _src.Scripts.Common.Saves.DataStorageImpl;
using _src.Scripts.Common.Saves.SaveLoadImpl;
using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts.Data
{
	[DefaultExecutionOrder(-1)]
	public class SceneSavableDataProvider : MonoBehaviour
	{
		[TitleGroup("Savable Data Requirement")]

		[BoxGroup("Savable Data Requirement/Game Data")]
		[SerializeField]
		public bool _gameDataRequiredForScene;
		
		[BoxGroup("Savable Data Requirement/Game Data")]
		[SerializeField, ShowIf(nameof(_gameDataRequiredForScene)), ReadOnly]
		private GameData _gameData;
	    
	    [BoxGroup("Savable Data Requirement/Complete Level Info")]
		[SerializeField]
		private bool _completeLevelInfoRequiredForScene;

		[BoxGroup("Savable Data Requirement/Complete Level Info")]
		[SerializeField, ShowIf(nameof(_completeLevelInfoRequiredForScene)), ReadOnly]
		private CompletedLevelInfo _completedLevelInfo;
		
		[BoxGroup("Savable Data Requirement/Feedback Settings")]
		[SerializeField]
		private bool _feedbackSettingsRequiredForScene;

		[BoxGroup("Savable Data Requirement/Feedback Settings")]
		[SerializeField, ShowIf(nameof(_feedbackSettingsRequiredForScene)), ReadOnly]
		private FeedbackSettings _feedbackSettings;

		[BoxGroup("Savable Data Requirement/Local")]
		[SerializeField]
		private bool _localAppRequiredForScene;

		[BoxGroup("Savable Data Requirement/Local")]
		[SerializeField, ShowIf(nameof(_localAppRequiredForScene)), ReadOnly]
		private LocalApp _localApp;
		
		public SaveLoadManager SaveLoadManager { get; private set; }

		public GameData GameData
		{
			get
			{
				if (!_gameDataRequiredForScene)
				{
					Debug.LogError($"{nameof(GameData)} not init in this scene", this);
				}
				return _gameData;
			}
		}
		
		public CompletedLevelInfo CompletedLevelInfo
		{
			get
			{
				if (!_completeLevelInfoRequiredForScene)
				{
					Debug.LogError($"{nameof(CompletedLevelInfo)} not init in this scene", this);
				}
				return _completedLevelInfo;
			}
		}

		public FeedbackSettings FeedbackSettings
		{
			get
			{
				if (!_feedbackSettingsRequiredForScene)
				{
					Debug.LogError($"{nameof(FeedbackSettings)} not init in this scene", this);
				}
				return _feedbackSettings;
			}
		}

		public LocalApp LocalApp
		{
			get
			{
				if (!_localAppRequiredForScene)
				{
					Debug.LogError($"{nameof(LocalApp)} not init in this scene", this);
				}
				return _localApp;
			}
		}

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			var serializedDataHolder = new SerializedDataHolder(GetDataStorageByPlatform());

			var loaderAndSavers = new List<ILoaderSaverData>();

			if (_gameDataRequiredForScene)
			{
				_gameData = new GameData();
				loaderAndSavers.Add(new SavableDataLoaderSaver<GameData>(serializedDataHolder, _gameData));
			}
			
			if (_feedbackSettingsRequiredForScene)
			{
				_feedbackSettings = new FeedbackSettings();
				loaderAndSavers.Add(new SavableDataLoaderSaver<FeedbackSettings>(serializedDataHolder, _feedbackSettings));
			}

			if (_completeLevelInfoRequiredForScene)
			{
				_completedLevelInfo = new CompletedLevelInfo();
				loaderAndSavers.Add(new SavableDataLoaderSaver<CompletedLevelInfo>(serializedDataHolder, _completedLevelInfo));
			}
			
			if (_localAppRequiredForScene)
			{
				_localApp = new LocalApp();
				loaderAndSavers.Add(new SavableDataLoaderSaver<LocalApp>(serializedDataHolder, _localApp));
			}

			SaveLoadManager = new SaveLoadManager(serializedDataHolder, loaderAndSavers);
			
			SaveLoadManager.LoadData();
		}

		[Button]
		private void ShowCurrentDataInInspector()
		{
			Init();
		}

		private static IDataStorage GetDataStorageByPlatform()
		{
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
			return new PlayerPrefDataStorage();
			#elif UNITY_WEBGL
			// implement for webgl
			#endif
		}
	}
}
