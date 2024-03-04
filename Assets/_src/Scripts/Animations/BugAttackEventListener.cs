using System;
using UnityEngine;

namespace _src.Scripts.Animations
{
    public class BugAttackEventListener : MonoBehaviour
    {
        public event Action Triggered;
        
        public void AttackHit()
        {
            Triggered?.Invoke();
        }
    }
}