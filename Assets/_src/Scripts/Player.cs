using _src.Scripts.Data;
using UnityEngine;


namespace _src.Scripts
{
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private SceneSavableDataProvider _sceneSavableDataProvider;

		[SerializeField]
		private PlayerReferences _playerReferences;

		[SerializeField]
		private PlayerBallSkin _ballSkin;

		private void Start()
		{
			InitPlayerSkin();
		}

		private void InitPlayerSkin()
		{
			_ballSkin.Select(_sceneSavableDataProvider.GameData.Skin);
			_playerReferences.MainRenderer = _ballSkin.Renderer;
		}
	}
}
