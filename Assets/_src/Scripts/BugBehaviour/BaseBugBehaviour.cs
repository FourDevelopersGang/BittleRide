using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _src.Scripts.BugBehaviour
{
    public abstract class BaseBugBehaviour : MonoBehaviour
    {
        [SerializeField] private Bug _bug;

        protected Vector3 PlayerBallPosition => BallController.Instance.transform.position;

        private void OnEnable()
        {
            _bug.Deactivated += DeactivateBehaviour;
        }

        private void OnDisable()
        {
            _bug.Deactivated -= DeactivateBehaviour;
        }

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
        

    }
}