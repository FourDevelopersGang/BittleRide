using UnityEngine;

namespace _src.Scripts.Utils
{
    public struct Timer
    {
        private bool _isActive;
        private bool _useTimeScale;
        private float _expirationTime;

        public bool IsActive => _isActive;
        
        public void Start(float expireAfter, bool useTimeScale = true)
        {
            _useTimeScale = useTimeScale;
            _expirationTime = GetTime(useTimeScale) + expireAfter;
            _isActive = true;
        }

        public void Stop()
        {
            _isActive = false;
        }
        
        public bool IsExpired()
        {
            if (!_isActive)
                return false;
            
            var time = GetTime(_useTimeScale);
            return time >= _expirationTime;
        }

        public bool StopIfExpired()
        {
            var isExpired = IsExpired();
            if (isExpired)
                Stop();
            return isExpired;
        }
        
        private static float GetTime(bool useTimeScale)
        {
            return useTimeScale ? Time.time : Time.unscaledTime;
        }
    }
}