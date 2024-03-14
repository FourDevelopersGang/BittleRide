using System.Collections.Generic;
using System.Threading;
using _src.Scripts.SocialPlatform.Leaderboards;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _src.Scripts.UI.Leaderboard
{
    public class LeaderboardView : MonoBehaviour
    {
        [SerializeField] private UIView _uiView;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private LeaderboardEntryView _entryViewPrefabPrefab;
        [SerializeField] private ScrollRect _scrollRect;

        private List<LeaderboardEntryView> _entryViews;
        private ILeaderboardService _leaderboardService;
        private CancellationTokenSource _ctSource;

        private void Start()
        {
            _entryViews = new List<LeaderboardEntryView>(
                _scrollRect.content.GetComponentsInChildren<LeaderboardEntryView>(true));
            SetAllEntryViewActive(false);
            SetLoading(false);
            _leaderboardService = LeaderboardProvider.Instance;
            _uiView.OnShowCallback.Event.AddListener(OnShown);
            _uiView.OnHiddenCallback.Event.AddListener(OnHidden);
        }

        private void OnDestroy()
        {
            if (_uiView)
            {
                _uiView.OnShowCallback.Event.RemoveListener(OnShown);
                _uiView.OnHiddenCallback.Event.RemoveListener(OnHidden);
            }
        }
        
        private void OnShown()
        {
            SetAllEntryViewActive(false);
            SetLoading(true);
            _ctSource?.Cancel();
            _ctSource = new CancellationTokenSource();
            var token = _ctSource.Token;

            _leaderboardService.RetrieveData(response =>
            {
                if (this == null || token.IsCancellationRequested)
                    return;
                
                SetLoading(false);
                if (response.IsSuccess)
                {
                    DisplayEntries(response);
                }
                else
                {
                    Debug.LogWarning("Leaderboard can't be retrieved");
                }
            });
        }

        private void OnHidden()
        {
            _ctSource?.Cancel();
            SetLoading(false);
            SetAllEntryViewActive(false);
        }

        private void DisplayEntries(RetrieveLeaderboardResponse response)
        {
            var rank = 1;
            
            for (var i = 0; i < response.Entries.Count; i++)
            {
                var entry = response.Entries[i];
                var entryView = SpawnEntryView();
                entryView.Init(
                    rank++,
                    entry.UserName, 
                    entry.ScoreValue, 
                    entry.IsSelf);
            }

            _scrollRect.verticalNormalizedPosition = 0f;
        }

        private LeaderboardEntryView SpawnEntryView()
        {
            foreach (var entryView in _entryViews)
            {
                if (entryView.IsActive) 
                    continue;
                
                entryView.IsActive = true;
                return entryView;
            }

            var newEntryView = Instantiate(_entryViewPrefabPrefab, _scrollRect.content);
            _entryViews.Add(newEntryView);
            return newEntryView;
        }

        private void SetLoading(bool isLoading)
        {
            if (isLoading)
            {
                SetStatus("Loading...");
            }
            else
            {
                SetStatus(string.Empty);
            }
        }

        private void SetStatus(string message)
        {
            _statusText.text = message;
        }

        private void SetAllEntryViewActive(bool isActive)
        {
            foreach (var entry in _entryViews)
            {
                entry.IsActive = isActive;
            }
        }
    }
}