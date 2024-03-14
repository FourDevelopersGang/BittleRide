using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SocialPlatforms;
using Range = UnityEngine.SocialPlatforms.Range;

namespace _src.Scripts.SocialPlatform
{
    public class SocialPlatformService
    {
        private enum AuthState
        {
            None,
            InProcess,
            Done
        }

        private static AuthState _authState = AuthState.None;

        public const int LeaderboardCapacity = 10;

        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitPlatform()
        {
#if UNITY_EDITOR
            Social.Active = new UnityEngine.SocialPlatforms.Local();
#elif UNITY_ANDROID
            GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = true;
            GooglePlayGames.PlayGamesPlatform.Activate();
#elif UNITY_IOS
            Social.Active = new UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform();
#endif
            Debug.Log($"[Social] Provider: {Social.Active.GetType().Name}");

            EnsureAuth().Forget();
        }

        public static string GetUserName()
        {
            EnsureAuth().Forget();
            return Social.localUser?.userName ?? "stranger";
        }

        public static async UniTask<long?> GetLocalUserScore()
        {
            IScore localScore = null;
            var scores = Array.Empty<IScore>();
            var anyResponse = false;

            LoadScores((loadedLocalScore, loadedScores) =>
            {
                localScore = loadedLocalScore;
                scores = loadedScores;
                anyResponse = true;
            });

            await UniTask.WaitUntil(() => anyResponse);
            var scoreInTable = scores.FirstOrDefault(s => s.userID == localScore.userID);
            if (scoreInTable != null && scoreInTable.value > localScore.value)
                return scoreInTable.value;
            
            return localScore.value;
        }

        public static void LoadScores(Action<IScore, IScore[]> onResult)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            void LoadGoogleLeaderboard()
            {
                GooglePlayGames.PlayGamesPlatform.Instance.LoadScores(
                    GetLeaderboardId(),
                    GooglePlayGames.BasicApi.LeaderboardStart.TopScores,
                    LeaderboardCapacity,
                    GooglePlayGames.BasicApi.LeaderboardCollection.Public,
                    GooglePlayGames.BasicApi.LeaderboardTimeSpan.Daily,
                    data => onResult?.Invoke(data.PlayerScore, data.Scores));
            }
            
            var eventsClient = GooglePlayGames.PlayGamesPlatform.Instance.Events;

            if (eventsClient == null)
            {
                Debug.LogWarning("[Social] Google IEventsClient is null, probably not authenticated");
                onResult?.Invoke(null, Array.Empty<IScore>());
                return;
            }
            
            eventsClient.FetchAllEvents(
                GooglePlayGames.BasicApi.DataSource.ReadNetworkOnly,
                (status, events) =>
                {
                    Debug.Log($"[Social] Fetch all events: {status}");
                    if (status == GooglePlayGames.BasicApi.ResponseStatus.Success || 
                        status == GooglePlayGames.BasicApi.ResponseStatus.SuccessWithStale)
                        LoadGoogleLeaderboard();
                    else
                        onResult?.Invoke(null, Array.Empty<IScore>());
                });
            return;
#endif

            var leaderboard = Social.CreateLeaderboard();
            leaderboard.id = GetLeaderboardId();
            leaderboard.range = new Range(0, LeaderboardCapacity);
            leaderboard.timeScope = TimeScope.Today;
            leaderboard.userScope = UserScope.Global;
            leaderboard.LoadScores(isOk =>
            {
                if (isOk)
                {
                    Debug.Log("[Social] Scores loaded successfully");
                    onResult?.Invoke(leaderboard.localUserScore, leaderboard.scores);
                }
                else
                {
                    Debug.Log("[Social] Scores load failed");
                    onResult?.Invoke(null, Array.Empty<IScore>());
                }
            });
        }

        public static string GetLeaderboardId()
        {
#if UNITY_EDITOR
            return "Leaderboard01";
#endif
            // TODO Google console
            //return GPGSIds.leaderboard_leaderboard;
        }

        public static async UniTask<Texture2D> LoadImage(IUserProfile user)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (user is GooglePlayGames.PlayGamesUserProfile googleUser)
            {
                if (string.IsNullOrEmpty(googleUser.AvatarURL))
                {
                    Debug.LogWarning($"[User={googleUser.id}] No URL found");
                    return Texture2D.blackTexture;
                }

                try
                {
                    var www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(googleUser.AvatarURL);
                    await www.SendWebRequest();
                    
                    if (www.error != null)
                    {
                        Debug.LogWarning($"[User={googleUser.id}] Error downloading image: {www.error}");
                        return Texture2D.blackTexture;
                    }

                    return UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[User={googleUser.id}] Error downloading image: {ex}");
                    return Texture2D.blackTexture;
                }
            }
#endif

            return user?.image;
        }

        public static async UniTask EnsureAuth()
        {
            if (_authState == AuthState.InProcess)
                await UniTask.WaitWhile(() => _authState == AuthState.InProcess);

            switch (_authState)
            {
                case AuthState.Done:
                    return;
                case AuthState.None:
                    _authState = AuthState.InProcess;
                    var anyResponse = false;
                    Social.localUser.Authenticate((isOk, message) =>
                    {
                        Debug.Log($"[Social] SocialPlatform auth: msg={message}, status={isOk}");
                        if (isOk)
                        {
                            _authState = AuthState.Done;
                            var lu = Social.localUser;
                            Debug.Log($"[Social] Local user: id={lu.id}, userName={lu.userName}, state={lu.state}");
                        }
                        else
                        {
                            _authState = AuthState.None;
                        }

                        anyResponse = true;
                    });
                    await UniTask.WaitUntil(() => anyResponse);
                    break;
            }
        }
    }
}