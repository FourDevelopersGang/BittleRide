using UnityEngine;

namespace _src.Scripts.BugBehaviour
{
    public struct MeleeAttackState
    {
        public float AttackArcAngle;
        public float AttackDistance;
        public float Damage;
        public float AngularSpeed;
        public float StartDelay;
        public float Cooldown;
        public BallController Target;

        public static MeleeAttackState CreateDefault(float attackDistance, float cooldown)
        {
            return new MeleeAttackState
            {
                AttackArcAngle = 50f,
                AttackDistance = attackDistance,
                StartDelay = 0.5f,
                Cooldown = cooldown,
                Damage = 1,
                Target = BallController.Instance,
                AngularSpeed = 140f,
            };
        }
    }
}