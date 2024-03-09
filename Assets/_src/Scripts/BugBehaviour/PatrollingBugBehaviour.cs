﻿using _src.Scripts.BugBehaviour.Actions;
using _src.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _src.Scripts.BugBehaviour
{
    public class PatrollingBugBehaviour : BaseBugBehaviour
    {
        [SerializeField] private BugMeleeAttackAction _attackAction;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _maxPatrolDistance = 8f;
        [SerializeField] private float _minWaitAfterPatrolTime = 1f;
        [SerializeField] private float _maxWaitAfterPatrolTime = 1.5f;
        [SerializeField] private float _agroDistance = 5f;

        private Vector3 _patrolPositionA;
        private Vector3 _patrolPositionB;
        private bool _isGoingB;
        private Timer _idleWaitTimer;

        protected override void OnDeactivated()
        {
            _navMeshAgent.enabled = false;
            _attackAction.Deactivate();
        }
        
        private void Start()
        {
            SnapToNavMesh(_navMeshAgent);
            _patrolPositionA = transform.position;
            _patrolPositionB = FindPatrolPosition();
            _navMeshAgent.SetDestination(_patrolPositionB);
            _isGoingB = true;
        }

        private void Update()
        {
            if (!_navMeshAgent.isOnNavMesh || _navMeshAgent.pathPending || _attackAction.IsAttacking)
                return;

            var attackDistance = CalculateAttackDistance(_boxCollider);
            if (IsPlayerBallWithinDistance(attackDistance))
            {
                _navMeshAgent.speed = 0f;
                _attackAction.PerformAttack(MeleeAttackState.CreateDefault(attackDistance));
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

                _idleWaitTimer.Stop();
                SwitchDirection();
                return;
            }

            if (_navMeshAgent.remainingDistance < 0.15f)
            {
                _navMeshAgent.speed = 0f;
                _idleWaitTimer.Start(Random.Range(_minWaitAfterPatrolTime, _maxWaitAfterPatrolTime));
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
            var patrolPosition = transform.position + RandomUtils.GetRandomHorizontalOffset() * _maxPatrolDistance;
            if (NavMesh.SamplePosition(patrolPosition, out var hit, _maxPatrolDistance, NavMesh.AllAreas))
                return hit.position;

            Debug.LogError("Patrol position not found");
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