using _src.Scripts.BugBehaviour.Actions;
using _src.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace _src.Scripts.BugBehaviour
{
    public class IdlingBugBehaviour : BaseBugBehaviour
    {
        [SerializeField] private BugMeleeAttackAction _attackAction;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _agroDistance = 5f;
        [SerializeField] private float _attackCooldown = 1f;
        
        private void Start()
        {
            SnapToNavMesh(_navMeshAgent);
        }

        protected override void OnDeactivated()
        {
            _navMeshAgent.enabled = false;
            _attackAction.Deactivate();
        }

        private void Update()
        {
            if (!_navMeshAgent.isOnNavMesh || _navMeshAgent.pathPending || _attackAction.IsAttacking)
                return;

            var attackDistance = CalculateAttackDistance(_boxCollider);
            if (IsPlayerBallWithinDistance(attackDistance))
            {
                _navMeshAgent.speed = 0f;
                if (!_attackAction.IsCoolingDown)
                    _attackAction.PerformAttack(MeleeAttackState.CreateDefault(attackDistance, _attackCooldown));
                return;
            }

            if (IsPlayerBallWithinDistance(_agroDistance))
            {
                _navMeshAgent.speed = _moveSpeed;
                _navMeshAgent.SetDestination(PlayerBallPosition);
                return;
            }

            _navMeshAgent.speed = 0f;
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            
            if (IsPlayerBallWithinDistance(CalculateAttackDistance(_boxCollider)))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, PlayerBallPosition);
            }
        }
    }
}