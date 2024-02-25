using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts.Common
{
	[RequireComponent(typeof(Collider))]
	public abstract class Pickupable : MonoBehaviour
	{
		[SerializeField]
		[Required]
		[ChildGameObjectsOnly]
		private MMF_Player _pickUpFeedbacks;
		
		private bool _used;

		public void PickUp()
		{
			if (_used)
				return;

			_used = true;
			_pickUpFeedbacks.PlayFeedbacks();
			OnPickUp();
		}

		protected abstract void OnPickUp();
	}
}
