using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _src.Scripts;
using _src.Scripts.UI;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;


public class PlayerProgression : MonoBehaviour
{
	[SerializeField, Required]
	private PlayerBugSmasher _playerBugSmasher; // Ссылка на скрипт PlayerIncrease


	[SerializeField, ReadOnly]
	private float bugsSmashed = 0; // Количество уничтоженных жуков


	private float bugsToLevelUp = 100; // Необходимое количество жуков для повышения уровня


	[SerializeField, ReadOnly]
	private int currentLevel = 0; // Текущий уровень игрока


	[SerializeField, SceneObjectsOnly, Required]
	private PlayerProgressionUi _playerProgressionUI;


	public int CurrentLevel => currentLevel;


	public float BugsToLevelUp => bugsToLevelUp;


	[SerializeField]
	private UnityDictionary<int, int> levelUpRequirements = new();


	public UnityDictionary<int, int> LevelUpRequirements => levelUpRequirements;


	public UnityEvent<int> OnLevelUp = new();


	private void Start()
	{
		if (_playerBugSmasher != null)
		{
			// Подписываемся на событие OnIncreaseSize
			_playerBugSmasher.OnIncreaseSize.AddListener(HandleIncreaseSize);
		}

		bugsToLevelUp = levelUpRequirements.FirstOrDefault().Value;
	}


	private void HandleIncreaseSize()
	{
		bugsSmashed++; // Увеличиваем счётчик уничтоженных жуков
		UpdateSliderUI();
		CheckLevelProgression(); // Проверяем, достигнут ли порог для повышения уровня
	}


	private void UpdateSliderUI()
	{
		Debug.Log(bugsToLevelUp);
		Debug.Log(bugsSmashed);
		_playerProgressionUI.UpdateSliderValue(bugsSmashed / bugsToLevelUp);
	}


	private void CheckLevelProgression()
	{
		if (bugsSmashed >= bugsToLevelUp && levelUpRequirements.TryGetValue(currentLevel,
			    out int nextLevelRequirement
		    ))
		{
			LevelUp();
			_playerProgressionUI.LevelUp(currentLevel);
		}
	}


	private void LevelUp()
	{
		bugsSmashed = 0; // Сбрасываем счётчик жуков
		currentLevel += 1; // Повышаем уровень
		_playerProgressionUI.LevelUp(currentLevel);
		bugsToLevelUp = levelUpRequirements[currentLevel]; // Устанавливаем новый порог жуков для следующего уровня
		OnLevelUp.Invoke(currentLevel);
		Debug.Log($"Player level increased to {currentLevel}. Next level requires {levelUpRequirements[currentLevel]} bugs.");
	}


	private void OnDestroy()
	{
		if (_playerBugSmasher != null)
		{
			// Отписываемся от события OnIncreaseSize
			_playerBugSmasher.OnIncreaseSize.RemoveListener(HandleIncreaseSize);
		}
	}
}
