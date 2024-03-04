using System;
using _src.Scripts.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _src.Scripts.BugBehaviour.Actions
{
    public class BugMeleeAttackAction : MonoBehaviour
    {
        [SerializeField] private float _damage;
        [SerializeField] private Animator _animator;

        private BugAttackEventListener _attackEventListener;
        
        public bool IsAttacking { get; private set; }

        private void Awake()
        {
            if (_animator.TryGetComponent(out BugAttackEventListener listener))
            {
                _attackEventListener = listener;
            }
            else
            {
                _attackEventListener = _animator.gameObject.AddComponent<BugAttackEventListener>();
            }
        }

        private void OnEnable()
        {
            _attackEventListener.Triggered += OnAttackTriggered;
        }

        private void OnDisable()
        {
            _attackEventListener.Triggered -= OnAttackTriggered;
        }

        public void PerformAttack()
        {
            if (IsAttacking)
            {
                Debug.LogError("Bug is already attacking");
                return;
            }

            Attack().Forget();
        }

        private void OnAttackTriggered()
        {
            var nextStateInfo = _animator.GetNextAnimatorStateInfo(0);
            var isTransitioning = nextStateInfo.fullPathHash != 0;
            if (isTransitioning)
                return;
            
            Debug.Log("Attack triggered");
        }

        private async UniTask Attack()
        {
            IsAttacking = true;
            _animator.SetTrigger(AnimParams.Attack);
            
            while (_animator && IsAttacking)
            {
                var stateInfo = _animator.GetAnimatorTransitionInfo(0);
                if (stateInfo.fullPathHash != 0)
                    break;

                await UniTask.Yield();
            }
            
            IsAttacking = false;
        }

        public void Deactivate()
        {
            _animator.ResetTrigger(AnimParams.Attack);
            IsAttacking = false;
        }
    }
}