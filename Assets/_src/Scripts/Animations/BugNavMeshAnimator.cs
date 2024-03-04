using UnityEngine;
using UnityEngine.AI;

namespace _src.Scripts.Animations
{
    public class BugNavMeshAnimator : MonoBehaviour
    {
        [SerializeField] private Bug _bug;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;
        
        private float _lastNormalizedSpeed;

        private void OnEnable()
        {
            _lastNormalizedSpeed = -1f;
            _bug.Deactivated += OnDeactivated;
        }

        private void OnDisable()
        {
            _bug.Deactivated -= OnDeactivated;
        }

        private void OnDeactivated()
        {
            enabled = false;
            _animator.SetFloat(AnimParams.Speed, 0f);
        }

        private void Update()
        {
            var maxSpeed = _navMeshAgent.speed;
            var speed = _navMeshAgent.velocity.magnitude;
            var normalizedSpeed = maxSpeed != 0f ? speed / maxSpeed : 0f;
            if (Mathf.Approximately(normalizedSpeed, _lastNormalizedSpeed))
                return;

            _lastNormalizedSpeed = normalizedSpeed;
            _animator.SetFloat(AnimParams.Speed, normalizedSpeed);
        }
    }
}