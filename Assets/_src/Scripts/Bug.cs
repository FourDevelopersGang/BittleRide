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

		private Animator _animator;


		private void Start()
		{
			_collider = GetComponent<Collider>();
			_rb = GetComponent<Rigidbody>();
			_animator = GetComponentInChildren<Animator>();
		}


		public void Deactivate()
		{
			_collider.enabled = false;
			Destroy(_rb);
			_animator.enabled = false;
		}
	}
}
