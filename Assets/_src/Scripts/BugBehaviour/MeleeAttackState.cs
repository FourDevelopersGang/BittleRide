using UnityEngine;

namespace _src.Scripts.BugBehaviour
{
    public struct MeleeAttackState
    {
        public float AttackArcAngle;
        public float AttackDistance;
        public float Damage;
        public BallController Target;

        public static MeleeAttackState CalculateDefault(float attackDistance, BoxCollider selfCollider)
        {
            var target = BallController.Instance;
            var calculatedAttackDistance = selfCollider.bounds.extents.z + attackDistance + target.Radius + 0.1f;

            return new MeleeAttackState
            {
                AttackArcAngle = 50f,
                AttackDistance = calculatedAttackDistance,
                Damage = 1,
                Target = target
            };
        }
    }
}