using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts
{
	public class Bug : MonoBehaviour
	{
		[SerializeField]
		private float _level;


		[SerializeField]
		[Required]
		[ChildGameObjectsOnly]
		private MMF_Player _destroyFeedbacks;


		public float Level => _level;


		private Collider _collider;

		private Rigidbody _rb;

		private Animator _animator;

		public event Action Deactivated;

		private void Start()
		{
			_collider = GetComponent<Collider>();
			_rb = GetComponent<Rigidbody>();
			_animator = GetComponentInChildren<Animator>();
		}


		public void Deactivate()
		{
			_collider.enabled = false;
			_destroyFeedbacks.PlayFeedbacks();
			Destroy(_rb);
			_animator.enabled = false;
			Deactivated?.Invoke();
		}
	}
}
