using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _src.Scripts
{
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public class PlayerIncrease : MonoBehaviour
    {
        [SerializeField]
        private float _size = 0.01f;

        [SerializeField]
        private float _minSize;

        [SerializeField]
        private float _maxSize;

        [SerializeField, Required]
        private CinemachineVirtualCamera _cinemachineVirtualCamera;

        [SerializeField]
        private float _increaseSizeValue = 0.01f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bug") && other.TryGetComponent(out Bug bug))
            {
                if (IsBugLowerThenPlayer(bug))
                {
                    IncreaseSize();
                    Destroy(other.gameObject); // Исправлено с Destroy(other) на Destroy(other.gameObject)
                }
                else
                {
                    DecreaseSize();
                    Destroy(other.gameObject); // Также исправлено здесь
                }
            }
        }

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
            if (CanChangeSize())
            {
             
                float scaleFactor = 1 + _increaseSizeValue; // Пример увеличения на процент
                transform.localScale *= scaleFactor;

                UpdateCameraOffset(scaleFactor); // Обновляем смещение камеры
                _size += 1;
            }
        }

        private void DecreaseSize()
        {
            if (CanChangeSize())
            {
             
                float scaleFactor = 1 - _increaseSizeValue; // Пример уменьшения на процент
                transform.localScale *= scaleFactor;

                UpdateCameraOffset(scaleFactor); // Обновляем смещение камеры
                _size -= 1;
            }
        }

        private void UpdateCameraOffset(float scaleFactor)
        {
            var transposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                // Обновляем только компоненты y и z смещения камеры, оставляя x без изменений
                Vector3 currentOffset = transposer.m_FollowOffset;
                transposer.m_FollowOffset = new Vector3(
                    currentOffset.x, // оставляем x без изменений
                    currentOffset.y * scaleFactor, // умножаем y на scaleFactor
                    currentOffset.z * scaleFactor  // умножаем z на scaleFactor
                );
            }
        }


        private bool CanChangeSize()
        {
      
            return transform.localScale.x >= _minSize && transform.localScale.y >= _minSize &&
                   transform.localScale.z >= _minSize && transform.localScale.x <= _maxSize &&
                   transform.localScale.y <= _maxSize && transform.localScale.z <= _maxSize;
        }

        private bool IsBugLowerThenPlayer(Bug bug) => bug.Size < _size;
    }
}
