using System;
using System.Collections.Generic;
using _src.Scripts.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


namespace _src.Scripts.UI.Localization
{
	public class SelectorLocalizationUI : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField]
		private RectTransform _panelSelectable;

		private VerticalLayoutGroup _localsSelectorButtonsContainer;

		private List<LocalizationSetterButton> _localSelectorButtons;

		private bool _expanded;

		private float _defaultSize;

		private void Awake()
		{
			_localsSelectorButtonsContainer = GetComponentInChildren<VerticalLayoutGroup>();
			_localSelectorButtons = new List<LocalizationSetterButton>(GetComponentsInChildren<LocalizationSetterButton>(true));
			_defaultSize = _panelSelectable.rect.height;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_expanded)
				return;

			Expand();
		}

		private void Update()
		{
			if (_expanded && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
				Close();
		}

		private void Expand()
		{
			InitLocalButtons();
			
			var containerLocalHeight = _localsSelectorButtonsContainer.CalculateHeightContainer();

			_panelSelectable
				.DOSizeDelta(new Vector2(_panelSelectable.rect.width, _defaultSize + containerLocalHeight), .75f)
				.OnComplete(() => { _expanded = true; });
		}

		private void InitLocalButtons()
		{
			foreach (var localizationSetterButton in _localSelectorButtons)
			{
				localizationSetterButton.gameObject.SetActive(localizationSetterButton.AssignLocal != LocalizationSettings.SelectedLocale);
			}
		}

		private void Close()
		{
			_panelSelectable
				.DOSizeDelta(new Vector2(_panelSelectable.rect.width, _defaultSize), .75f)
				.OnComplete(() => { _expanded = false; });
		}
	}
}
