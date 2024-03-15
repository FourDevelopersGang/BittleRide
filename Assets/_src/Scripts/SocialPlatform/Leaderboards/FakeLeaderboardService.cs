using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public class FakeLeaderboardService : ILeaderboardService
    {
        private const string ScorePrefsKey = "local_high_score";

        public void TrySetNewHighScore(int newScore)
        {
            var oldHighScore = PlayerPrefs.GetInt(ScorePrefsKey, 0);
            if (newScore > oldHighScore)
            {
                PlayerPrefs.SetInt(ScorePrefsKey, newScore);
                PlayerPrefs.Save();
            }
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
                new ("Bob23", 25),
                new ("x_Mike_x", 57),
                new ("99_Mar", 89),
                new ("Lucas_Cool_Dude", 136),
                new ("Lisa1337", 197),
                new ("Master_JohhhN", 276),
                new ("Emma_What_Son?", 390),
                new ("PatrickBatemanOnTheLoose", 456),
                new ("777_Lily_777", 555),
            };
        }
    }
}