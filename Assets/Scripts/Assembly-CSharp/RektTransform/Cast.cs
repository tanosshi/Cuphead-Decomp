using UnityEngine;
using UnityEngine.EventSystems;

namespace RektTransform
{
	public static class Cast
	{
		public static RectTransform RT(this GameObject go)
		{
			if (go == null || go.transform == null)
			{
				return null;
			}
			return go.GetComponent<RectTransform>();
		}

		public static RectTransform RT(this Transform t)
		{
			if (!(t is RectTransform))
			{
				return null;
			}
			return t as RectTransform;
		}

		public static RectTransform RT(this Component c)
		{
			return c.transform.RT();
		}

		public static RectTransform RT(this UIBehaviour ui)
		{
			if (ui == null)
			{
				return null;
			}
			return ui.transform as RectTransform;
		}
	}
}
