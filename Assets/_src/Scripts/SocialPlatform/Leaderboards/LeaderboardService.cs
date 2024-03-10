using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;

namespace _src.Scripts.SocialPlatform.Leaderboards
{
    public class LeaderboardService
    {
        private static LeaderboardService _instance;

        public static LeaderboardService Instance => _instance ??= new LeaderboardService();
        
        public void ReportScore(long score)
        {
            ReportScoreAsync(score).Forget();
        }

        public void RetrieveData(Action<RetrieveLeaderboardResponse> onResponse)
        {
            RetrieveDataAsync(onResponse).Forget();
        }

        private async UniTaskVoid ReportScoreAsync(long score)
        {
            await SocialPlatformService.Instance.EnsureAuth();
            Social.ReportScore(
                score,
                SocialPlatformService.Instance.GetLeaderboardId(),
                isOk => Debug.Log($"[Leaderboard] Report score: val={score}, status={isOk}"));
        }

        private async UniTaskVoid RetrieveDataAsync(Action<RetrieveLeaderboardResponse> onResponse)
        {
            await SocialPlatformService.Instance.EnsureAuth();
            var leaderboardId = SocialPlatformService.Instance.GetLeaderboardId();
            Debug.Log($"[Leaderboard] Loading scores: {leaderboardId}");
            SocialPlatformService.Instance.LoadScores((localScore, scores) =>
            {
                DebugLogAllLeaderboards();
                Debug.Log($"[Leaderboard] Scores loaded successfully: {scores.Length}");
                LoadEntries(scores, entries =>
                {
                    onResponse?.Invoke(new RetrieveLeaderboardResponse(
                        true,
                        CreateEntry(Social.localUser, localScore),
                        entries));
                });
            });
        }

        private static void LoadEntries(IScore[] scores, Action<List<LeaderboardEntry>> onSuccess)
        {
            var userIds = new List<string>();
            foreach (var score in scores)
            {
                userIds.Add(score.userID);
            }

            if (scores.Length == 0)
            {
                Debug.Log("[Leaderboard] Ignoring users loading because scores empty, success");
                onSuccess?.Invoke(new List<LeaderboardEntry>());
                return;
            }
            
            Debug.Log($"[Leaderboard] Loading users: {userIds.Count}");
            Social.LoadUsers(userIds.ToArray(), users =>
            {
                Debug.Log($"[Leaderboard] Users loaded successfully: {users.Length}");
                var entries = new List<LeaderboardEntry>(scores.Length);

                for (var i = 0; i < scores.Length; i++)
                {
                    var score = scores[i];
                    var user = users.FirstOrDefault(u => u.id == score.userID);
                    if (user == null)
                    {
                        Debug.LogWarning($"[Leaderboard] Loaded user({score.userID}) is null, skipping");
                        continue;
                    }

                    entries.Add(CreateEntry(user, score));
                }

                onSuccess?.Invoke(entries);
            });
        }

        private static LeaderboardEntry CreateEntry(IUserProfile user, IScore score)
        {
            var userId = user != null ? user.id : score.userID;
            var userName = user != null ? user.userName : $"unknown_user_{score.userID}";
            return new LeaderboardEntry(userId, userName, score.value, score.rank, () => SocialPlatformService.Instance.LoadImage(user));
        }

        private void DebugLogAllLeaderboards()
        {
            if (Social.Active is Local loc)
            {
                var leadField = loc.GetType().GetField("m_Leaderboards", BindingFlags.Instance | BindingFlags.NonPublic);
                var leaderboards = (List<Leaderboard>) leadField.GetValue(loc);
                foreach (var lb in leaderboards)
                {
                    Debug.Log($"Existing leaderboards: id={lb.id}, title={lb.title}");
                }
            }
        }
    }
}