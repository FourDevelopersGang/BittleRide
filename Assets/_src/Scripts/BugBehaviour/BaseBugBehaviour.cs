using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _src.Scripts.BugBehaviour
{
    public abstract class BaseBugBehaviour : MonoBehaviour
    {
        [SerializeField] private Bug _bug;

        private void OnEnable()
        {
            _bug.Deactivated += DeactivateBehaviour;
        }

        private void OnDisable()
        {
            _bug.Deactivated -= DeactivateBehaviour;
        }

        protected Vector3 PlayerBallPosition => BallController.Instance.transform.position;

        private void DeactivateBehaviour()
        {
            enabled = false;
            OnDeactivated();
        }
        
        protected virtual void OnDeactivated()
        {
        }

        protected bool IsPlayerBallWithinDistance(float attackDistance)
        {
            var target = BallController.Instance;
            var sqrDistToTarget = (target.transform.position - transform.position).sqrMagnitude;
            return sqrDistToTarget <= attackDistance * attackDistance;
        }
        
        protected static Vector3 GetRandomHorizontalOffset()
        {
            var result = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            if (result == Vector3.zero)
                return Vector3.forward;

            return result.normalized;
        }
    }
}