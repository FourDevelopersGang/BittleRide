namespace _src.Scripts.BugBehaviour
{
    public struct MeleeAttackState
    {
        public float AttackArcAngle;
        public float AttackDistance;
        public float Damage;
        public BallController Target;

        public static MeleeAttackState CreateDefault(float behaviourAttackDistance)
        {
            var target = BallController.Instance;
            var attackDistance = target.Radius + behaviourAttackDistance + 0.1f;

            return new MeleeAttackState
            {
                AttackArcAngle = 50f,
                AttackDistance = attackDistance,
                Damage = 1,
                Target = target
            };
        }
    }
}