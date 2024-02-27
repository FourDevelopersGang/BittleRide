using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;


namespace _src.Scripts.LevelManagment
{
	public class StageTimer : MonoBehaviour
	{
		[Required]
		public TextMeshProUGUI timerText; // Переменная для хранения ссылки на текстовый компонент
		private float timer; // Переменная для отслеживания времени

		private void Start()
		{
			if (timerText == null)
			{
				Debug.LogError("TimerDisplay: Не задан компонент TextMeshProUGUI.");
				this.enabled = false; // Отключаем скрипт, если не настроен компонент TextMeshProUGUI
			}

			timer = 0f; // Инициализация таймера значением 0
		}

		private void Update()
		{
			timer += Time.deltaTime; // Увеличиваем таймер с учетом Time.timeScale

			// Преобразование таймера в минуты и секунды
			TimeSpan timeSpan = TimeSpan.FromSeconds(timer);
			string timeText = string.Format("{0:D2} : {1:D2}", timeSpan.Minutes, timeSpan.Seconds);

			// Обновление текста компонента TextMeshProUGUI
			timerText.text = timeText;
		}
	}
}
