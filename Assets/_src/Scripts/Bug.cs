using System;
using UnityEngine;


namespace _src.Scripts
{
	public class Bug : MonoBehaviour
	{
		[SerializeField]
		private float _size;


		public float Size => _size;


		private Collider _collider;

		private Rigidbody _rb;


		private void Start()
		{
			_collider = GetComponent<Collider>();
			_rb = GetComponent<Rigidbody>();
		}


		public void Deactivate()
		{
			_collider.enabled = false;
			Destroy(_rb);
		}
	}
}
