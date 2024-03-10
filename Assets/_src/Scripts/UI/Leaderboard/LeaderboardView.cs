using System;
using System.Collections.Generic;
using System.Threading;
using _src.Scripts.SocialPlatform;
using _src.Scripts.SocialPlatform.Leaderboards;
using Core.Ui.Leaderboard;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.UI;

namespace _src.Scripts.UI.Leaderboard
{
    public class LeaderboardView : MonoBehaviour
    {
        [SerializeField] private UIView _uiView;
        [SerializeField] private GameObject _showIfLoading;
        [SerializeField] private LeaderboardEntryView _entryViewPrefabPrefab;
        [SerializeField] private RectTransform _contentParent;

        private List<LeaderboardEntryView> _entryViews;
        private readonly LeaderboardService _leaderboardService = LeaderboardService.Instance;
        private CancellationTokenSource _ctSource;

        private static bool IsCheatRegistered;
        
        private void Start()
        {
            _entryViews = new List<LeaderboardEntryView>(
                _contentParent.GetComponentsInChildren<LeaderboardEntryView>(true));
            SetAllEntryViewActive(false);
            SetLoading(false);
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
            var indexOfSelf = response.Entries.FindIndex(e => e.UserId == response.SelfEntry.UserId);
            var shouldDisplaySelf = indexOfSelf == -1;
            var displayCount = SocialPlatformService.LeaderboardCapacity;
            if (shouldDisplaySelf)
                displayCount--;
            
            for (var i = 0; i < response.Entries.Count && i < displayCount; i++)
            {
                var entry = response.Entries[i];
                var entryView = GetEntryView();
                var isSelf = i == indexOfSelf;
                entryView.Init(
                    entry.LoadAvatar,
                    entry.Rank,
                    entry.UserName, 
                    entry.ScoreValue, 
                    isSelf);
            }
            
            if (shouldDisplaySelf)
            {
                var selfEntryView = GetEntryView();
                selfEntryView.Init(
                    response.SelfEntry.LoadAvatar,
                    response.SelfEntry.Rank,
                    response.SelfEntry.UserName,
                    response.SelfEntry.ScoreValue,
                    true);
            }
        }

        private LeaderboardEntryView GetEntryView()
        {
            foreach (var entryView in _entryViews)
            {
                if (entryView.IsActive) 
                    continue;
                
                entryView.IsActive = true;
                return entryView;
            }

            var newEntryView = Instantiate(_entryViewPrefabPrefab, _contentParent);
            _entryViews.Add(newEntryView);
            return newEntryView;
        }

        private void SetLoading(bool isLoading)
        {
            _showIfLoading.SetActive(isLoading);
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