using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


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


        private Rigidbody _rb;


        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }


        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bug") && other.transform.TryGetComponent(out Bug bug))
            {
                if (IsBugLowerThenPlayer(bug))
                {
                    IncreaseSize();
                    InsertBug(bug);
                }
                else
                {
                    DecreaseSize();
                    Destroy(other.gameObject);
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Bug") && other.transform.TryGetComponent(out Bug bug))
            {
                if (IsBugLowerThenPlayer(bug))
                {
                    IncreaseSize();
                    InsertBug(bug);
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
                _rb.mass += 3;
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
            transform.DOScale(Vector3.one * _physicalSize,
                1f
            );

            var transposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                Vector3 baseOffset = new Vector3(transposer.m_FollowOffset.x, transposer.m_FollowOffset.y +_increaseSizeValue, transposer.m_FollowOffset.z - _increaseSizeValue);
                transposer.m_FollowOffset = baseOffset;
            }
        }


        private void InsertBug(Bug bug)
        {
            bug.Deactivate();

            // Определяем точку для размещения на сфере, учитывая текущий масштаб игрока

            // Делаем объект дочерним и устанавливаем начальную позицию
            bug.transform.parent = transform;
            bug.transform.localPosition = Vector3.zero;

            // Установим фиксированный масштаб для жука, предположим, что исходный масштаб жука подходит
            bug.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Можно настроить этот масштаб

            // Ориентируем объект и перемещаем его к заданной точке
            bug.transform.LookAt(transform.position);
            bug.transform.Rotate(0, 0f, 0f);

            bug.transform.DOLocalMove(Random.onUnitSphere / 2.1f, 1);
        }




        private bool IsBugLowerThenPlayer(Bug bug) => bug.Size < _size; // Сравнение уровня игрока и жука
    }
}
