using UnityEngine;

namespace _src.Scripts.Animations
{
    public static class AnimatorUtils
    {
        public static bool IsNullState(this AnimatorStateInfo animatorStateInfo)
        {
            return animatorStateInfo.fullPathHash == 0;
        }
    }
}