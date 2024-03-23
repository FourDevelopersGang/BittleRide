using UnityEngine;
using UnityEngine.UI;


namespace _src.Scripts.Utils
{
	public static class LayoutGroupsExtensions
	{
		public static float CalculateHeightContainer(this VerticalLayoutGroup layout)
		{
			var sumElementsHeight = 0f;

			var countElements = 0;
			
			foreach (RectTransform element in layout.transform)
			{
				if(!element || !element.gameObject.activeSelf) continue;

				sumElementsHeight += element.rect.height;

				++countElements;
			}

			return countElements * layout.spacing + sumElementsHeight + layout.padding.top + layout.padding.bottom;
		}
	}
}
