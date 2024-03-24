using Sirenix.OdinInspector;
using UnityEngine;


namespace _src.Scripts
{
	public class BallVariant : MonoBehaviour
	{
		[field: SerializeField, Required]
		public Renderer Renderer { get; private set; }
	}
}
