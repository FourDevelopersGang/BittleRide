using System;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace _src.Scripts.UI
{
	public class LevelUI : MonoBehaviour
	{
		[SerializeField]
		private GameObject _levelCompleteMark;

		[SerializeField]
		private UIButton _loadLevelButton;

		private void Awake()
		{
			_levelCompleteMark.gameObject.SetActive(false);
			_loadLevelButton.gameObject.SetActive(false);
		}

		public void Unlock()
		{
			_loadLevelButton.gameObject.SetActive(true);
		}

		public void MarkAsCompleted()
		{
			_levelCompleteMark.SetActive(true);
		}
	}
}
