using System;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public interface ILeaderboardService
    {
        void TrySetNewHighScore(int newScore);
        void RetrieveData(Action<RetrieveLeaderboardResponse> onResponse);
    }
}