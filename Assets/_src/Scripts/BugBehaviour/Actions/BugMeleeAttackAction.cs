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
            Debug.Log("Attack");
            IsAttacking = true;
            await UniTask.WaitForSeconds(2f);
            IsAttacking = false;
        }

        public void Deactivate()
        {
            // TODO Disable animator
        }
    }
}