using System.Collections.Generic;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public readonly struct RetrieveLeaderboardResponse
    {
        public readonly bool IsSuccess;
        public readonly List<LeaderboardEntry> Entries;

        public RetrieveLeaderboardResponse(
            bool isSuccess, 
            List<LeaderboardEntry> entries)
        {
            IsSuccess = isSuccess;
            Entries = entries;
        }
    }
}