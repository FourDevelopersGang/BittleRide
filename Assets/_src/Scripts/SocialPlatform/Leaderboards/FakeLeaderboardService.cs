using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public class FakeLeaderboardService : ILeaderboardService
    {
        private const string ScorePrefsKey = "local_high_score";
        
        public void AddScore(int scoreDelta)
        {
            var localScore = PlayerPrefs.GetInt(ScorePrefsKey, 0);
            PlayerPrefs.SetInt(ScorePrefsKey, localScore + scoreDelta);
            PlayerPrefs.Save();
        }

        public void RetrieveData(Action<RetrieveLeaderboardResponse> onResponse)
        {
            var localScore = PlayerPrefs.GetInt(ScorePrefsKey, 0);
            var selfEntry = new LeaderboardEntry("User", localScore, isSelf: true);
            var fakeEntries = GenerateFakeEntries();
            fakeEntries.Add(selfEntry);
            fakeEntries = fakeEntries.OrderByDescending(e => e.ScoreValue).ToList();
            onResponse?.Invoke(new RetrieveLeaderboardResponse(true, fakeEntries));
        }

        private List<LeaderboardEntry> GenerateFakeEntries()
        {
            return new List<LeaderboardEntry>
            {
                new ("Bob", 25),
                new ("Mike", 57),
                new ("Maria", 89),
                new ("Lucas", 136),
                new ("Lisa", 197),
                new ("John", 276),
                new ("Emma", 390),
                new ("Christian", 456),
                new ("Lily", 555),
            };
        }
    }
}