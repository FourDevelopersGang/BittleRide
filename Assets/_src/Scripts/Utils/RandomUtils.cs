using UnityEngine;

namespace _src.Scripts.Utils
{
    public static class RandomUtils
    {
        public static Vector3 GetRandomHorizontalOffset()
        {
            var result = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            if (result == Vector3.zero)
                return Vector3.forward;

            return result.normalized;
        }
    }
}