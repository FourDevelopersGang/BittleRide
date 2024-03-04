using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class BallController : MonoBehaviour
{    
    public float forceMagnitude = 500f;
    private Vector2 startTouchPosition;
    private Rigidbody rb;
    private SphereCollider coll;
    private bool isDragging = false;
    [SerializeField, Required]
    private Camera _camera;

    public static BallController Instance { get; private set; }
    public float Radius => coll.radius * transform.localScale.x;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<SphereCollider>();
        _camera = Camera.main;
    }

    private void Update()
    {
        HandleSwipe();
    }

    private void HandleSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Начало касания: запоминаем позицию и начинаем отслеживание
            startTouchPosition = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            // Касание продолжается: вычисляем вектор движения от начальной точки
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 swipeDirection = currentTouchPosition - startTouchPosition;

            if (swipeDirection.magnitude > 20) // Минимальная дистанция для реакции
            {
                swipeDirection.Normalize();

                // Преобразуем направление свайпа в соответствии с ориентацией камеры
                Vector3 cameraRight = _camera.transform.right;
                Vector3 cameraForward = _camera.transform.forward;
                cameraForward.y = 0; // Игнорируем изменение по вертикали, чтобы движение было только в горизонтальной плоскости

                Vector3 moveDirection = (cameraRight * swipeDirection.x + cameraForward * swipeDirection.y).normalized;

                // Применяем силу для вращения в соответствии с направлением движения
                rb.AddTorque(new Vector3(moveDirection.z, 0, -moveDirection.x) * forceMagnitude);

                // Сброс начальной позиции для поддержки непрерывного ввода
                startTouchPosition = currentTouchPosition;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Конец касания: останавливаем отслеживание
            isDragging = false;
        }
    }
}
