using System.Collections.Generic;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public readonly struct RetrieveLeaderboardResponse
    {
        public readonly bool IsSuccess;
        public readonly LeaderboardEntry? SelfEntry;
        public readonly List<LeaderboardEntry> Entries;

        public RetrieveLeaderboardResponse(
            bool isSuccess, 
            LeaderboardEntry? selfEntry, 
            List<LeaderboardEntry> entries)
        {
            IsSuccess = isSuccess;
            SelfEntry = selfEntry;
            Entries = entries;
        }
    }
}