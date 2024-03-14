using System;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public interface ILeaderboardService
    {
        void AddScore(int scoreDelta);
        void RetrieveData(Action<RetrieveLeaderboardResponse> onResponse);
    }
}