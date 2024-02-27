using System;
using _src.Scripts.BugBehaviour.Actions;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _src.Scripts.BugBehaviour
{
    public class PatrollingBugBehaviour : BaseBugBehaviour
    {
        [SerializeField] private BugAttackAction _bugAttackAction;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _maxPatrolDistance = 8f;
        [SerializeField] private float _minWaitAfterPatrolTime = 1f;
        [SerializeField] private float _maxWaitAfterPatrolTime = 1.5f;
        [SerializeField] private float _agroDistance = 5f;
        [SerializeField] private float _attackDistance = 2f;

        private Vector3 _patrolPositionA;
        private Vector3 _patrolPositionB;
        private bool _isGoingB;
        private float _idleWaitTimer;

        protected override void OnDeactivated()
        {
            _navMeshAgent.enabled = false;
            _bugAttackAction.Deactivate();
        }
        
        private void Start()
        {
            _patrolPositionA = transform.position;
            _patrolPositionB = FindPatrolPosition();
            _navMeshAgent.SetDestination(_patrolPositionB);
            _isGoingB = true;
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
                
                SwitchDirection();
                return;
            }

            if (_navMeshAgent.remainingDistance < 0.15f)
            {
                _navMeshAgent.speed = 0f;
                _idleWaitTimer = Random.Range(_minWaitAfterPatrolTime, _maxWaitAfterPatrolTime);
            }
        }

        private void SwitchDirection()
        {
            _isGoingB = !_isGoingB;
            var nextDestination = _isGoingB ? _patrolPositionB : _patrolPositionA;
            _navMeshAgent.speed = _moveSpeed;
            _navMeshAgent.SetDestination(nextDestination);
        }

        private Vector3 FindPatrolPosition()
        {
            var patrolPosition = transform.position + GetRandomHorizontalOffset() * _maxPatrolDistance;
            if (NavMesh.SamplePosition(patrolPosition, out var hit, _maxPatrolDistance, NavMesh.AllAreas))
                return hit.position;

            Debug.LogError("Patrol position not found");
            return transform.position;
        }
    }
}