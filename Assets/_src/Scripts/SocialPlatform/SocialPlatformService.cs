using System;
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
        
        private AuthState _authState = AuthState.None;
        private ILeaderboard _leaderboard;
        private static SocialPlatformService _instance;

        public const int LeaderboardCapacity = 10;

        public static SocialPlatformService Instance => _instance ??= _instance;
        
        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitPlatform()
        {
            _instance = new SocialPlatformService();
        }

        private SocialPlatformService()
        {
#if UNITY_EDITOR
            Social.Active = new Local();
#elif UNITY_ANDROID
            GooglePlayGames.PlayGamesPlatform.Activate();
#elif UNITY_IOS
            Social.Active = new UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform();
#endif
            
            Debug.Log($"[Social] Provider: {Social.Active.GetType().Name}");
            EnsureAuth().Forget();
        }

        private ILeaderboard CreateLeaderboard()
        {
            var leaderboard = Social.CreateLeaderboard();
            leaderboard.id = GetLeaderboardId();
            leaderboard.range = new Range(0, LeaderboardCapacity);
            leaderboard.timeScope = TimeScope.Today;
            leaderboard.userScope = UserScope.Global;
            return leaderboard;
        }

        public async UniTask EnsureAuth()
        {
            switch (_authState)
            {
                case AuthState.Done:
                    return;
                case AuthState.None:
                    _authState = AuthState.InProcess;
                    Social.localUser.Authenticate((isOk, message) =>
                    {
                        Debug.Log($"[Social] SocialPlatform auth: msg={message}, status={isOk}");
                        var lu = Social.localUser;
                        Debug.Log($"[Social] Local user: id={lu.id}, userName={lu.userName}, state={lu.state}");
                        _authState = AuthState.Done;
                    });
                    await UniTask.WaitUntil(() => _authState == AuthState.Done);
                    break;
                case AuthState.InProcess:
                    await UniTask.WaitUntil(() => _authState == AuthState.Done);
                    break;
            }
        }

        public string GetUserName()
        {
            return Social.localUser.userName;
        }

        public void LoadScores(Action<IScore, IScore[]> onSuccess)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            GooglePlayGames.PlayGamesPlatform.Instance.LoadScores(
                GetLeaderboardId(), 
                GooglePlayGames.BasicApi.LeaderboardStart.TopScores, 
                LeaderboardCapacity, 
                GooglePlayGames.BasicApi.LeaderboardCollection.Public, 
                GooglePlayGames.BasicApi.LeaderboardTimeSpan.Daily,
                data => onSuccess?.Invoke(data.PlayerScore, data.Scores));
#else
            _leaderboard ??= CreateLeaderboard();
            Social.Active.LoadScores(_leaderboard, isOk =>
            {
                if (isOk)
                {
                    Debug.Log("[Social] Scores loaded successfully");
                    onSuccess?.Invoke(_leaderboard.localUserScore, _leaderboard.scores);
                }
                else
                {
                    Debug.Log("[Social] Scores load failed");
                }
            });
#endif
        }

        public string GetLeaderboardId()
        {
#if UNITY_EDITOR
            return "Leaderboard01";
#else
            return GPGSIds.leaderboard_leaderboard;
#endif
        }

        public async UniTask<Texture2D> LoadImage(IUserProfile user)
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
    }
}