using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public readonly struct LeaderboardEntry
    {
        public readonly string UserName;
        public readonly long ScoreValue;
        public readonly bool IsSelf;

        public LeaderboardEntry(string userName, long scoreValue, bool isSelf = false)
        {
            UserName = userName;
            ScoreValue = scoreValue;
            IsSelf = isSelf;
        }
    }
}