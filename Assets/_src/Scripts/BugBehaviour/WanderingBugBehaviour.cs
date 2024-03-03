using System;
using _src.Scripts.BugBehaviour.Actions;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _src.Scripts.BugBehaviour
{
    public class WanderingBugBehaviour : BaseBugBehaviour
    {
        [SerializeField] private BugAttackAction _bugAttackAction;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _maxWanderDistance = 8f;
        [SerializeField] private float _minWaitAfterWanderTime = 1f;
        [SerializeField] private float _maxWaitAfterWanderTime = 1.5f;
        [SerializeField] private float _agroDistance = 5f;
        [SerializeField] private float _attackDistance = 2f;

        private Vector3 _wanderOrigin;
        private float _idleWaitTimer;

        protected override void OnDeactivated()
        {
            _navMeshAgent.enabled = false;
            _bugAttackAction.Deactivate();
        }

        private void Start()
        {
            _wanderOrigin = transform.position;
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

            if (_idleWaitTimer > 0)
            {
                _idleWaitTimer -= Time.deltaTime;
                if (_idleWaitTimer > 0f)
                    return;
                
                _navMeshAgent.speed = _moveSpeed;
                var nextDestination = FindWanderDestination();
                _navMeshAgent.SetDestination(nextDestination);
                return;
            }

            if (_navMeshAgent.remainingDistance < 0.15f)
            {
                _navMeshAgent.speed = 0f;
                _idleWaitTimer = Random.Range(_minWaitAfterWanderTime, _maxWaitAfterWanderTime);
            }
        }
        
        private Vector3 FindWanderDestination()
        {
            var wanderPosition = _wanderOrigin + GetRandomHorizontalOffset() * _maxWanderDistance;
            if (NavMesh.SamplePosition(wanderPosition, out var hit, _maxWanderDistance, NavMesh.AllAreas))
                return hit.position;

            Debug.LogError("Wander destination not found");
            return transform.position;
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