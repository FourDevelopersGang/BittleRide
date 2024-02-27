using System.Collections.Generic;

namespace _src.Scripts.OpenUnitySolutions
{
    public static class IDictionaryExtensions
    {
        public static UnityDictionary<TKey, TElement> ToUnityDictionary<TKey, TElement>(this IDictionary<TKey, TElement> source)
            => new (source);
    }
}