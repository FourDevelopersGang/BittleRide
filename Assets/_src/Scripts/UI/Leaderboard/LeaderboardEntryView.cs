using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _src.Scripts.UI.Leaderboard
{
    public class LeaderboardEntryView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _userNameText;
        [SerializeField] private TextMeshProUGUI _scoreValueText;
        [SerializeField] private GameObject _showIfHighlighted;

        private CancellationTokenSource _ctSource;

        public bool IsActive
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        private void OnDisable()
        {
            _ctSource?.Cancel();
        }

        private void OnDestroy()
        {
            _ctSource?.Cancel();
        }

        public void Init(int rank, string userName, long scoreValueText, bool isHighlighted)
        {
            _rankText.SetText("{0}.", rank);
            _userNameText.SetText(userName);
            _scoreValueText.SetText("{0}", scoreValueText);
            _showIfHighlighted.SetActive(isHighlighted);

            _ctSource?.Cancel();
            _ctSource = new CancellationTokenSource();
            var token = _ctSource.Token;
            // Load avatar
        }
    }
}