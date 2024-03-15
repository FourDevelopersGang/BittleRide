﻿using System;
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
            var selfEntry = new LeaderboardEntry("Player", localScore, isSelf: true);
            var fakeEntries = GenerateFakeEntries();
            fakeEntries.Add(selfEntry);
            fakeEntries = fakeEntries.OrderByDescending(e => e.ScoreValue).ToList();
            onResponse?.Invoke(new RetrieveLeaderboardResponse(true, fakeEntries));
        }

        private List<LeaderboardEntry> GenerateFakeEntries()
        {
            return new List<LeaderboardEntry>
            {
                new ("Bob23", 40),
                new ("x_Mike_x", 89),
                new ("99_Mar", 120),
                new ("CoolLucas13", 190),
                new ("Lisa1337", 245),
                new ("Master_JohhhN", 321),
                new ("eEmmaStrikeR", 445),
                new ("PatrickBateman", 555),
                new ("77_Lily_77", 623),
            };
        }
    }
}