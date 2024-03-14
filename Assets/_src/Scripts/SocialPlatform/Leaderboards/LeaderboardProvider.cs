namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public static class LeaderboardProvider
    {
        private static ILeaderboardService _instance = new FakeLeaderboardService();
        
        public static ILeaderboardService Instance => _instance;
    }
}