using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public readonly struct LeaderboardEntry
    {
        public readonly string UserId;
        public readonly string UserName;
        public readonly long ScoreValue;
        public readonly int Rank;
        public readonly Func<UniTask<Texture2D>> LoadAvatar;

        public LeaderboardEntry(string userId, string userName, long scoreValue, int rank, Func<UniTask<Texture2D>> loadAvatar)
        {
            UserId = userId;
            UserName = userName;
            ScoreValue = scoreValue;
            Rank = rank;
            LoadAvatar = loadAvatar;
        }
    }
}