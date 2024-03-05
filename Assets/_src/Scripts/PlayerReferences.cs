using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts
{
	public class PlayerReferences : MonoBehaviour
	{
		[Required, ChildGameObjectsOnly]
		public Renderer MainRenderer;


		[Required, ChildGameObjectsOnly]
		public Transform BugsContainer;


		public Renderer[] BugsRenderers => BugsContainer.GetComponentsInChildren<Renderer>();
	}
}
