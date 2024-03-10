using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Leaderboard
{
    public class LeaderboardEntryView : MonoBehaviour
    {
        [SerializeField] private RawImage _avatarImage;
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _userNameText;
        [SerializeField] private TextMeshProUGUI _scoreValueText;
        [SerializeField] private Texture2D _defaultAvatar;
        //[SerializeField] private GameObject _showIfHighlighted; TODO

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

        public void Init(Func<UniTask<Texture2D>> loadAvatar, int rank, string userName, long scoreValueText, bool isHighlighted)
        {
            _avatarImage.texture = _defaultAvatar;
            _rankText.SetText("{0}.", rank);
            _userNameText.SetText(userName);
            _scoreValueText.SetText("{0}", scoreValueText);
            //_showIfHighlighted.SetActive(isHighlighted);

            _ctSource?.Cancel();
            _ctSource = new CancellationTokenSource();
            var token = _ctSource.Token;
            loadAvatar()
                .ContinueWith(avatar =>
                {
                    if (token.IsCancellationRequested || _avatarImage == null)
                        return;

                    _avatarImage.texture = avatar ? avatar : _defaultAvatar;
                })
                .Forget();
        }
    }
}