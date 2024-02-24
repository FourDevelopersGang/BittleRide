using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _src.Scripts
{
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public class PlayerIncrease : MonoBehaviour
    {
        // Используется как "уровень" или "сила" игрока, а не как физический размер
        [SerializeField]
        private float _size = 1f;

        // Фактический физический размер игрока в Unity
        private float _physicalSize = 0.01f;

        [SerializeField]
        private float _minSize = 0.01f; // Минимальный физический размер

        [SerializeField]
        private float _maxSize = 1.0f; // Максимальный физический размер

        [SerializeField, Required]
        private CinemachineVirtualCamera _cinemachineVirtualCamera;

        [SerializeField]
        private float _increaseSizeValue = 0.01f; // Значение изменения физического размера

        public UnityEvent OnIncreaseSize = new();
        
        public UnityEvent OnDecreaseSize = new();
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bug") && other.transform.TryGetComponent(out Bug bug))
            {
                if (IsBugLowerThenPlayer(bug))
                {
                    IncreaseSize();
                    Destroy(other.gameObject);
                }
                else
                {
                    DecreaseSize();
                    Destroy(other.gameObject);
                }
            }
        }

        private void IncreaseSize()
        {
            if (_physicalSize + _increaseSizeValue <= _maxSize) // Увеличиваем размер, если не превышен максимум
            {
                OnIncreaseSize.Invoke();
                _size += 1; // Увеличиваем "уровень" игрока
                _physicalSize += _increaseSizeValue; // Точное увеличение физического размера
                UpdateScaleAndCameraOffset();
            }
        }

        private void DecreaseSize()
        {
            if (_physicalSize - _increaseSizeValue >= _minSize) // Уменьшаем размер, если не меньше минимума
            {
                OnDecreaseSize.Invoke();
                _size = Mathf.Max(_size - 1, 1); // Уменьшаем "уровень" игрока, не опускаясь ниже 1
                _physicalSize -= _increaseSizeValue; // Точное уменьшение физического размера
                UpdateScaleAndCameraOffset();
            }
        }

        private void UpdateScaleAndCameraOffset()
        {
            transform.localScale = Vector3.one * _physicalSize; // Применяем физический размер
    
            var transposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                Vector3 baseOffset = new Vector3(transposer.m_FollowOffset.x, transposer.m_FollowOffset.y +_increaseSizeValue, transposer.m_FollowOffset.z - _increaseSizeValue);
                transposer.m_FollowOffset = baseOffset;
            }
        }


        private bool IsBugLowerThenPlayer(Bug bug) => bug.Size < _size; // Сравнение уровня игрока и жука
    }
}
