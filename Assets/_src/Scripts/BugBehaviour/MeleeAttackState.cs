using UnityEngine;

namespace _src.Scripts.BugBehaviour
{
    public struct MeleeAttackState
    {
        public float AttackArcAngle;
        public float AttackDistance;
        public float Damage;
        public float AngularSpeed;
        public BallController Target;

        public static MeleeAttackState CreateDefault(float attackDistance)
        {
            return new MeleeAttackState
            {
                AttackArcAngle = 50f,
                AttackDistance = attackDistance,
                Damage = 1,
                Target = BallController.Instance,
                AngularSpeed = 140f,
            };
        }
    }
}