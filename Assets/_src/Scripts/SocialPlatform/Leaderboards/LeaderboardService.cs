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
        private static long? _cachedTotalScore;
        private static readonly LeaderboardService _instance = new();

        private LeaderboardService()
        {
        }


        public static LeaderboardService Instance => _instance;

        public void AddScore(int score)
        {
            AddScoreAsync(score).Forget();
        }

        public void RetrieveData(Action<RetrieveLeaderboardResponse> onResponse)
        {
            RetrieveDataAsync(onResponse).Forget();
        }

        private async UniTaskVoid AddScoreAsync(int scoreDelta)
        {
            if (_cachedTotalScore == null)
            {
                await SocialPlatformService.EnsureAuth();
                var score = await SocialPlatformService.GetLocalUserScore();
                if (score == null)
                {
                    Debug.LogWarning("[Leaderboard] Can't report score, local score is null");
                    return;
                }
                
                _cachedTotalScore = score;
            }
            
            _cachedTotalScore += scoreDelta;
            var newTotal = _cachedTotalScore.Value;
            Social.ReportScore(newTotal, SocialPlatformService.GetLeaderboardId(), isOk =>
            {
                Debug.Log($"[Leaderboard] Report score: delta={scoreDelta}, newTotal={newTotal}, status={isOk}");
            });
        }

        private async UniTaskVoid RetrieveDataAsync(Action<RetrieveLeaderboardResponse> onResponse)
        {
            await SocialPlatformService.EnsureAuth();
            var leaderboardId = SocialPlatformService.GetLeaderboardId();
            Debug.Log($"[Leaderboard] Loading scores: {leaderboardId}");
            SocialPlatformService.LoadScores((localScore, scores) =>
            {
                DebugLogAllLeaderboards();
                Debug.Log($"[Leaderboard] Scores loaded successfully: {scores.Length}");
                LoadEntries(scores, entries =>
                {
                    if (localScore == null)
                    {
                        Debug.LogWarning("[Leaderboard] Local score is null");
                    }
                    else
                    {
                        Debug.Log($"[Leaderboard] Local score: {ScoreToString(localScore)}");
                    }
                    
                    onResponse?.Invoke(new RetrieveLeaderboardResponse(
                        true,
                        CreateEntry(Social.localUser, localScore),
                        entries));
                });
            });
        }

        private static void LoadEntries(IScore[] scores, Action<List<LeaderboardEntry>> onSuccess)
        {
            if (scores.Length == 0)
            {
                Debug.Log("[Leaderboard] Ignoring users loading because scores empty, success");
                onSuccess?.Invoke(new List<LeaderboardEntry>());
                return;
            }

            var scoresStr = string.Join("\n", scores.Select(ScoreToString));
            
            Debug.Log($"[Leaderboard] Loaded scores: {scoresStr}");
            
            var userIds = new List<string>();
            foreach (var score in scores.OrderBy(s => s.value))
            {
                if (!userIds.Contains(score.userID))
                    userIds.Add(score.userID);
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

                    var entry = CreateEntry(user, score);
                    if (entry.HasValue)
                        entries.Add(entry.Value);
                }

                onSuccess?.Invoke(entries);
            });
        }

        private static string ScoreToString(IScore s)
        {
            return $"userId={s.userID} rank={s.rank} value={s.value}";
        }

        private static LeaderboardEntry? CreateEntry(IUserProfile user, IScore score)
        {
            if (user == null && score == null)
            {
                Debug.LogWarning("[Leaderboard] CreateEntry failed, user and score is null");
                return null;
            }

            if (score == null)
            {
                Debug.LogWarning("[Leaderboard] CreateEntry partially failed, score is null");
                return new LeaderboardEntry(user.id, user.userName, -1, -1, () => SocialPlatformService.LoadImage(user));
            }
            
            var userId = user != null ? user.id : score.userID;
            var userName = user != null ? user.userName : $"**unknown_{score.userID}**";
            return new LeaderboardEntry(userId, userName, score.value, score.rank, () => SocialPlatformService.LoadImage(user));
        }

        private void DebugLogAllLeaderboards()
        {
            if (Social.Active is Local loc)
            {
                var leadField = loc.GetType().GetField("m_Leaderboards", BindingFlags.Instance | BindingFlags.NonPublic);
                var leaderboards = (List<UnityEngine.SocialPlatforms.Impl.Leaderboard>) leadField.GetValue(loc);
                foreach (var lb in leaderboards)
                {
                    Debug.Log($"Debug leaderboard: id={lb.id}, title={lb.title}");
                }
            }
        }
    }
}