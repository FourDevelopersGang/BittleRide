using System;
using UnityEngine;

namespace _src.Scripts.Utils
{
    public class DisableOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}