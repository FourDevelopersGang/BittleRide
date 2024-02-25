using System;
using UnityEngine;


namespace _src.Scripts.Common
{
	[RequireComponent(typeof(Collider))]
	public class Picker : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (!other.TryGetComponent(out Pickupable pickupable))
				return;
			
			pickupable.PickUp();
		}
	}
}
