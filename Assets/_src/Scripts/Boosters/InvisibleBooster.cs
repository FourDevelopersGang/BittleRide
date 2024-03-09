using System;
using System.Collections;
using System.Linq;
using _src.Scripts.Common.Timer;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts.Boosters
{
	public class InvisibleBooster : Booster
	{
		[SerializeField]
		[Range(0,
			1
		)]
		private float _transparency = 0.5f;


		[ValueDropdown("GetAllLayers")]
		[SerializeField]
		private string _ignoreCollisionWithEnemyLayer;


		private string _cachedLayer;


		private async UniTask Behaviour()
		{
			if (_player)
			{
				if (_player.TryGetComponent(out PlayerBugSmasher playerBugSmasher))
				{
					await HandlePlayerInvisibility();
				}
			}
			else
			{
				Debug.LogError("No have player on",
					gameObject
				);
			}
		}


		private async UniTask HandlePlayerInvisibility()
		{
			// Attempt to get the PlayerBugSmasher component.
			if (_player.TryGetComponent(out PlayerBugSmasher playerBugSmasher))
			{
				SetPlayerAndBugsTransparency(playerBugSmasher,
					_transparency
				);

				await UniTask.Delay(TimeSpan.FromSeconds(_duration));
				ResetPlayerAndBugsVisibility(playerBugSmasher);
				_boosterTimerUI.StopUITimer();
				playerBugSmasher.Invisible = false;
			}
		}


		// Sets the player and bugs' transparency.
		private void SetPlayerAndBugsTransparency(PlayerBugSmasher playerBugSmasher, float transparency)
		{
			playerBugSmasher.Invisible = true;

			// Extract common operations into separate methods to improve readability.
			var references = playerBugSmasher.GetComponent<PlayerReferences>();

			SetGameObjectLayer(playerBugSmasher.gameObject,
				_ignoreCollisionWithEnemyLayer,
				out _cachedLayer
			);

			SetMaterialTransparency(references.MainRenderer.material,
				transparency
			);

			foreach (var bugRenderer in references.BugsRenderers)
			{
				SetMaterialTransparency(bugRenderer.material,
					transparency
				);
			}
		}


		// Resets the visibility of the player and bugs to opaque.
		private void ResetPlayerAndBugsVisibility(PlayerBugSmasher playerBugSmasher)
		{
			var references = playerBugSmasher.GetComponent<PlayerReferences>();

			SetGameObjectLayer(playerBugSmasher.gameObject,
				_cachedLayer
			);

			ResetMaterialTransparency(references.MainRenderer.material);

			foreach (var bugRenderer in references.BugsRenderers)
			{
				ResetMaterialTransparency(bugRenderer.material);
			}
		}


		// Sets a GameObject's layer with optional caching of the previous layer name.
		private void SetGameObjectLayer(GameObject gameObject, string layerName, out string cachedLayerName)
		{
			cachedLayerName = LayerMask.LayerToName(gameObject.layer);
			gameObject.layer = LayerMask.NameToLayer(layerName);
		}


		// Overload without the need to cache the previous layer.
		private void SetGameObjectLayer(GameObject gameObject, string layerName)
		{
			gameObject.layer = LayerMask.NameToLayer(layerName);
		}


		// Sets material transparency.
		private void SetMaterialTransparency(Material material, float transparency)
		{
			var color = material.color;
			color.a = transparency;
			material.color = color;
		}


		// Resets material transparency to fully opaque.
		private void ResetMaterialTransparency(Material material)
		{
			SetMaterialTransparency(material,
				1.0f
			);
		}


		protected override void OnPickUp()
		{
			base.OnPickUp();
			Behaviour().Forget();
		}


		private static IEnumerable GetAllLayers()
		{
			return Enumerable.Range(0,
				32
			).Select(LayerMask.LayerToName).Where(l => !string.IsNullOrEmpty(l)).ToArray();
		}
	}
}
