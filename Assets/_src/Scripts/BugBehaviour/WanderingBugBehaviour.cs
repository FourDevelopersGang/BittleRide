using _src.Scripts.BugBehaviour.Actions;
using _src.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _src.Scripts.BugBehaviour
{
    public class WanderingBugBehaviour : BaseBugBehaviour
    {
        [SerializeField] private BugMeleeAttackAction _bugMeleeAttackAction;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _maxWanderDistance = 8f;
        [SerializeField] private float _minWaitAfterWanderTime = 1f;
        [SerializeField] private float _maxWaitAfterWanderTime = 1.5f;
        [SerializeField] private float _agroDistance = 5f;

        private Vector3 _wanderOrigin;
        private Timer _idleWaitTimer;

        protected override void OnDeactivated()
        {
            _navMeshAgent.enabled = false;
            _bugMeleeAttackAction.Deactivate();
        }

        private void Start()
        {
            SnapToNavMesh(_navMeshAgent);
            _wanderOrigin = transform.position;
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

            if (_idleWaitTimer.IsActive)
            {
                if (!_idleWaitTimer.StopIfExpired())
                    return;
                
                _navMeshAgent.speed = _moveSpeed;
                var nextDestination = FindWanderDestination();
                _navMeshAgent.SetDestination(nextDestination);
                return;
            }

            if (_navMeshAgent.remainingDistance < 0.15f)
            {
                _navMeshAgent.speed = 0f;
                _idleWaitTimer.Start(Random.Range(_minWaitAfterWanderTime, _maxWaitAfterWanderTime));
            }
        }
        
        private Vector3 FindWanderDestination()
        {
            var wanderPosition = _wanderOrigin + RandomUtils.GetRandomHorizontalOffset() * _maxWanderDistance;
            if (NavMesh.SamplePosition(wanderPosition, out var hit, Mathf.Infinity, NavMesh.AllAreas))
                return hit.position;
            
            Debug.LogError("Wander destination not found");
            return transform.position;
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