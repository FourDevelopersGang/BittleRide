using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BallSoundController : MonoBehaviour
{
	private Rigidbody rb;
	private AudioSource audioSource;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		audioSource.loop = true; // Зацикливаем звук
	}

	void Update()
	{
		// Рассчитываем скорость шара
		float speed = rb.velocity.magnitude;

		// Регулируем громкость и скорость воспроизведения в зависимости от скорости шара
		// Значения для громкости и Pitch можно настроить в соответствии с вашими предпочтениями
		audioSource.volume = Mathf.Clamp(speed / 10, 0, 1); // Примерное масштабирование
		audioSource.pitch = 1 + Mathf.Clamp(speed / 10, 0, 1); // Увеличиваем Pitch при увеличении скорости

		// Воспроизводим звук, если шар движется и звук еще не играет
		if (!audioSource.isPlaying && speed > 0.1f)
		{
			audioSource.Play();
		}
		else if (audioSource.isPlaying && speed <= 0.1f)
		{
			// Останавливаем воспроизведение, если шар почти не движется
			audioSource.Stop();
		}
	}
}
