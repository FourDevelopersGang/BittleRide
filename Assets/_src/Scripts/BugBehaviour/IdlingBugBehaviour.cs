using _src.Scripts.BugBehaviour.Actions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace _src.Scripts.BugBehaviour
{
    public class IdlingBugBehaviour : BaseBugBehaviour
    {
        [SerializeField] private BugAttackAction _bugAttackAction;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _agroDistance = 5f;
        [SerializeField] private float _attackDistance = 2f;

        protected override void OnDeactivated()
        {
            _navMeshAgent.enabled = false;
            _bugAttackAction.Deactivate();
        }

        private void Update()
        {
            if (!_navMeshAgent.isOnNavMesh || _navMeshAgent.pathPending || _bugAttackAction.IsAttacking)
                return;

            if (IsPlayerBallWithinDistance(_attackDistance))
            {
                _navMeshAgent.speed = 0f;
                _bugAttackAction.PerformAttack();
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
            if (IsPlayerBallWithinDistance(_attackDistance))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, PlayerBallPosition);
            }
        }
    }
}