using System;
using System.Collections.Generic;
using _src.Scripts.Data;
using DG.Tweening;
using UnityEngine;


namespace _src.Scripts
{
	public class BallSelector : MonoBehaviour
	{
		[SerializeField]
		private SceneSavableDataProvider _sceneSavableDataProvider;
		
		private List<BallVariant> _ballVariants;

		private int _chosenSkinId;

		public void ChooseNext()
		{
			ChooseBallVariant(_chosenSkinId + 1);
		}

		public void ChoosePrevious()
		{
			ChooseBallVariant(_chosenSkinId - 1);
		}
		
		private void Awake()
		{
			_ballVariants = new List<BallVariant>(GetComponentsInChildren<BallVariant>(true));

			for (var index = 0; index < _ballVariants.Count; index++)
			{
				var ballVariant = _ballVariants[index];

				if (index == _sceneSavableDataProvider.GameData.Skin)
				{
					ballVariant.gameObject.SetActive(true);
					_chosenSkinId = index;
				}
				else
				{
					ballVariant.gameObject.SetActive(false);
				}
				
				ApplyRotationToPreview(ballVariant, Vector3.zero);
			}
		}

		private void ChooseBallVariant(int index)
		{
			var lastVariant = _chosenSkinId;
			_chosenSkinId = index % _ballVariants.Count;
			_sceneSavableDataProvider.GameData.Skin = _chosenSkinId;
			
			_ballVariants[lastVariant].gameObject.SetActive(false);
			
			_ballVariants[_chosenSkinId].gameObject.SetActive(true);
		}

		private void ApplyRotationToPreview(BallVariant ballVariant, Vector3 startStartRotation)
		{
			ballVariant.transform.DORotate(new Vector3(0, 360, 0), 7.5f, RotateMode.FastBeyond360)
				.From(startStartRotation)
				.SetRelative(true)
				.SetLoops(-1)
				.SetEase(Ease.Linear);
		}
	}
}
