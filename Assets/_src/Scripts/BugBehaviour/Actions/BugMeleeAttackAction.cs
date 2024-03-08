using System;
using _src.Scripts.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _src.Scripts.BugBehaviour.Actions
{
    public class BugMeleeAttackAction : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private BugAttackEventListener _attackEventListener;
        private MeleeAttackState _state;
        private bool _isWaitingForEvent;
        private bool _isAttacking;

        public bool IsAttacking => _isAttacking;

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
            _attackEventListener.Triggered += ExecuteAttackHit;
        }

        private void OnDisable()
        {
            _attackEventListener.Triggered -= ExecuteAttackHit;
            _isAttacking = false;
            _isWaitingForEvent = false;
        }

        public void PerformAttack(MeleeAttackState state)
        {
            if (_isAttacking)
            {
                Debug.LogError("Bug is already attacking");
                return;
            }

            _state = state;
            Attack().Forget();
        }

        public void Deactivate()
        {
            _animator.ResetTrigger(AnimParams.Attack);
            _isAttacking = false;
            _isWaitingForEvent = false;
        }

        private async UniTask Attack()
        {
            _isAttacking = true;
            _isWaitingForEvent = true;
            _animator.SetTrigger(AnimParams.Attack);
            
            while (_animator && IsAttacking)
            {
                if (_isWaitingForEvent)
                {
                    RotateTowardsTarget();
                }
                else
                {
                    var stateInfo = _animator.GetAnimatorTransitionInfo(0);
                    if (stateInfo.fullPathHash != 0)
                        break;
                }

                await UniTask.Yield();
            }

            _isAttacking = false;
            _isWaitingForEvent = false;
        }

        private void RotateTowardsTarget()
        {
            var dirToTarget = _state.Target.transform.position - transform.position;
            dirToTarget.y = 0;
            var rotationToTarget = Quaternion.LookRotation(dirToTarget, Vector3.up);
            var newRotation = Quaternion.RotateTowards(
                    transform.rotation,
                    rotationToTarget, 
                    _state.AngularSpeed * Time.deltaTime);
            transform.rotation = newRotation;
        }

        private void ExecuteAttackHit()
        {
            _isWaitingForEvent = false;
            var nextStateInfo = _animator.GetNextAnimatorStateInfo(0);
            var isTransitioning = nextStateInfo.fullPathHash != 0;
            if (isTransitioning)
                return;
            
            var dirToTarget = _state.Target.transform.position - transform.position;
            if (dirToTarget.sqrMagnitude > _state.AttackDistance * _state.AttackDistance)
                return;

            dirToTarget.y = 0;
            var angleToTarget = Vector3.Angle(dirToTarget, transform.forward);
            var maxAngleOffset = _state.AttackArcAngle / 2f;
            if (angleToTarget > maxAngleOffset)
                return;

            _state.Target.ApplyDamage(_state.Damage);
        }
    }
}