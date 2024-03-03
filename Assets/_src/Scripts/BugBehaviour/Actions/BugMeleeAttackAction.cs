using _src.Scripts.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _src.Scripts.BugBehaviour.Actions
{
    public class BugMeleeAttackAction : MonoBehaviour
    {
        [SerializeField] private float _damage;
        [SerializeField] private Animator _animator;

        public bool IsAttacking { get; private set; }
        
        public void PerformAttack()
        {
            if (IsAttacking)
            {
                Debug.LogError("Bug is already attacking");
                return;
            }

            Attack().Forget();
        }

        private async UniTask Attack()
        {
            _animator.SetTrigger(AnimParams.Attack);
            await UniTask.WaitForSeconds(2f);
        }

        public void Deactivate()
        {
            _animator.ResetTrigger(AnimParams.Attack);
            IsAttacking = false;
        }
    }
}