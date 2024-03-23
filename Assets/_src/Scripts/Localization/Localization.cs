using System;
using _src.Scripts.Data;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


namespace _src.Scripts.Localization
{
	public class Localization : MonoBehaviour
	{
		[SerializeField]
		private SceneSavableDataProvider _sceneSavableDataProvider;

		private void Awake()
		{
			if (_sceneSavableDataProvider.LocalApp.LocalId.IsNullOrEmpty())
				return;
			
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(_sceneSavableDataProvider.LocalApp.LocalId);
		}

		private void OnEnable()
		{
			LocalizationSettings.SelectedLocaleChanged += LocalizationSettingsOnSelectedLocaleChanged;
		}

		private void OnDestroy()
		{
			LocalizationSettings.SelectedLocaleChanged -= LocalizationSettingsOnSelectedLocaleChanged;
		}

		private void LocalizationSettingsOnSelectedLocaleChanged(Locale obj)
		{
			_sceneSavableDataProvider.LocalApp.LocalId = obj.Identifier.Code;
		}
	}
}
