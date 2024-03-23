using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;


namespace _src.Scripts.UI.Localization
{
	[RequireComponent(typeof(Button), typeof(Image))]
	public class LocalizationSetterButton : MonoBehaviour
	{
		[field: SerializeField]
		public Locale AssignLocal { get; private set; }

		[SerializeField]
		private LocalizedSprite _assignSpriteKeyReference = new();

		private Button _button;

		private Image _image;

		private void Awake()
		{
			_button = GetComponent<Button>();

			_image = GetComponent<Image>();

			_button.onClick.AddListener(SetLocal);
		}

		private void SetLocal()
		{
			Debug.Log("Set local " + AssignLocal);
			LocalizationSettings.SelectedLocale = AssignLocal;
			LocalizationSettings.ProjectLocale = AssignLocal;
		}

		private void OnValidate()
		{
			if (_assignSpriteKeyReference is not {IsEmpty: true})
				return;
			
			_image.sprite = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>(_assignSpriteKeyReference.TableReference, 
				_assignSpriteKeyReference.TableEntryReference, AssignLocal);
		}
	}
}
