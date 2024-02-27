using System.Collections;
using System.Collections.Generic;
using _src.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;


public class PlayerProgression : MonoBehaviour
{
	[SerializeField]
	private PlayerIncrease _playerIncrease; // Ссылка на скрипт PlayerIncrease


	[SerializeField, ReadOnly]
	private int bugsSmashed = 0; // Количество уничтоженных жуков


	private int bugsToLevelUp = 100; // Необходимое количество жуков для повышения уровня
	[SerializeField, ReadOnly]
	private int currentLevel = 0; // Текущий уровень игрока


	public int CurrentLevel => currentLevel;


	public int BugsToLevelUp => bugsToLevelUp;


	[SerializeField]
	private UnityDictionary<int, int> levelUpRequirements = new();


	public UnityDictionary<int, int> LevelUpRequirements => levelUpRequirements;


	private void Start()
	{
		if (_playerIncrease != null)
		{
			// Подписываемся на событие OnIncreaseSize
			_playerIncrease.OnIncreaseSize.AddListener(HandleIncreaseSize);
		}

		bugsToLevelUp = levelUpRequirements[1];
	}


	private void HandleIncreaseSize()
	{
		bugsSmashed++; // Увеличиваем счётчик уничтоженных жуков
		CheckLevelProgression(); // Проверяем, достигнут ли порог для повышения уровня
	}


	private void CheckLevelProgression()
	{
		if (bugsSmashed >= bugsToLevelUp && levelUpRequirements.ContainsKey(currentLevel + 1))
		{
			bugsSmashed = 0; // Сбрасываем счётчик жуков
			currentLevel++; // Повышаем уровень

			if (levelUpRequirements.TryGetValue(currentLevel,
				    out int nextLevelRequirement
			    ))
			{
				bugsToLevelUp = nextLevelRequirement; // Устанавливаем новый порог жуков для следующего уровня
			}

			Debug.Log($"Player level increased to {currentLevel}. Next level requires {bugsToLevelUp} bugs.");
		}
	}


	private void OnDestroy()
	{
		if (_playerIncrease != null)
		{
			// Отписываемся от события OnIncreaseSize
			_playerIncrease.OnIncreaseSize.RemoveListener(HandleIncreaseSize);
		}
	}
}
