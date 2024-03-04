using System;
using _src.Scripts.BugBehaviour.Actions;
using UnityEngine;
using UnityEngine.AI;

namespace _src.Scripts.BugBehaviour
{
    public class IdlingBugBehaviour : BaseBugBehaviour
    {
        [SerializeField] private BugMeleeAttackAction _bugMeleeAttackAction;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _agroDistance = 5f;

        private void Start()
        {
            SnapToNavMesh(_navMeshAgent);
        }

        protected override void OnDeactivated()
        {
            _navMeshAgent.enabled = false;
            _bugMeleeAttackAction.Deactivate();
        }

        private void Update()
        {
            if (!_navMeshAgent.isOnNavMesh || _navMeshAgent.pathPending || _bugMeleeAttackAction.IsAttacking)
                return;

            var attackDistance = CalculateAttackDistance(_boxCollider);
            if (IsPlayerBallWithinDistance(attackDistance))
            {
                _navMeshAgent.speed = 0f;
                _bugMeleeAttackAction.PerformAttack(MeleeAttackState.CreateDefault(attackDistance));
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