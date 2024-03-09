using System;
using _src.Scripts.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace _src.Scripts.UI
{
	public class SettingsUI : MonoBehaviour
	{
		[SerializeField, SceneObjectsOnly]
		private SceneSavableDataProvider _sceneSavableDataProvider;

		[SerializeField]
		private Slider _musicVolume;

		[SerializeField]
		private AudioMixerGroup _musicMixerGroup;

		[SerializeField]
		private Slider _sfxVolume;

		[SerializeField]
		private AudioMixerGroup _sfxMixerGroup;

		[SerializeField]
		private Toggle _vibrate;

		private void Awake()
		{
			Init();

			_musicVolume.onValueChanged.AddListener(OnChangeMusicVolume);
			_sfxVolume.onValueChanged.AddListener(OnChangeSfxVolume);
			_vibrate.onValueChanged.AddListener(OnToggleVibrate);
		}

		private void Init()
		{
			_musicVolume.value = _sceneSavableDataProvider.FeedbackSettings.MusicVolume;
			_musicMixerGroup.audioMixer.SetFloat("MusicVolume", _musicVolume.value);
			_sfxVolume.value = _sceneSavableDataProvider.FeedbackSettings.SfxVolume;
			_sfxMixerGroup.audioMixer.SetFloat("SfxVolume", _sfxVolume.value);
			_vibrate.isOn = _sceneSavableDataProvider.FeedbackSettings.Vibrate;
		}

		private void OnToggleVibrate(bool newValue)
		{
			_sceneSavableDataProvider.FeedbackSettings.Vibrate = newValue;
		}

		private void OnChangeSfxVolume(float newValue)
		{
			_sceneSavableDataProvider.FeedbackSettings.SfxVolume = newValue;
			_sfxMixerGroup.audioMixer.SetFloat("SfxVolume", newValue);
		}

		private void OnChangeMusicVolume(float newValue)
		{
			_sceneSavableDataProvider.FeedbackSettings.MusicVolume = newValue;
			_musicMixerGroup.audioMixer.SetFloat("MusicVolume", newValue);
		}

		private void OnDisable()
		{
			_sceneSavableDataProvider.SaveLoadManager.SaveData();
			_sceneSavableDataProvider.SaveLoadManager.SubmitChanges();
		}
	}
}
