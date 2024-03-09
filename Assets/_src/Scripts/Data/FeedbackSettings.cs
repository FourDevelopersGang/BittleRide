using System;
using _src.Scripts.Common.Saves;
using UnityEngine;


namespace _src.Scripts.Data
{
	[Serializable]
	public class FeedbackSettings : GenericSavableData<FeedbackSettings>
	{
		[SerializeField]
		private float _musicVolume;

		[SerializeField]
		private float _sfxVolume;

		[SerializeField]
		private bool _vibrate;
		
		public float MusicVolume
		{
			set => ChangeField(ref _musicVolume, value);
			get => _musicVolume;
		}

		public float SfxVolume
		{
			set => ChangeField(ref _sfxVolume, value);
			get => _sfxVolume;
		}

		public bool Vibrate
		{
			set => ChangeField(ref _vibrate, value);
			get => _vibrate;
		}

		public override void Load(FeedbackSettings savableData)
		{
			_musicVolume = savableData._musicVolume;
			_sfxVolume = savableData._sfxVolume;
			_vibrate = savableData._vibrate;
		}
	}
}
