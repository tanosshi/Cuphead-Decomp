using System.Collections.Generic;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	public static class UISelectionUtility
	{
		public static Selectable FindNextSelectable(Selectable selectable, Transform transform, List<Selectable> allSelectables, Vector3 direction)
		{
			RectTransform rectTransform = transform as RectTransform;
			if (rectTransform == null)
			{
				return null;
			}
			direction = direction.normalized;
			Vector2 vector = Quaternion.Inverse(transform.rotation) * direction;
			Vector2 vector2 = transform.TransformPoint(UITools.GetPointOnRectEdge(rectTransform, vector));
			bool flag = direction == Vector3.left || direction == Vector3.right;
			float num = float.PositiveInfinity;
			float num2 = float.PositiveInfinity;
			Selectable selectable2 = null;
			Selectable selectable3 = null;
			Vector2 point = vector2 + vector * 999999f;
			for (int i = 0; i < allSelectables.Count; i++)
			{
				Selectable selectable4 = allSelectables[i];
				if (selectable4 == selectable || selectable4 == null || selectable4.navigation.mode == Navigation.Mode.None || (!selectable4.IsInteractable() && !ReflectionTools.GetPrivateField<Selectable, bool>(selectable4, "m_GroupsAllowInteraction")))
				{
					continue;
				}
				RectTransform rectTransform2 = selectable4.transform as RectTransform;
				if (rectTransform2 == null)
				{
					continue;
				}
				Rect worldSpaceRect = UITools.GetWorldSpaceRect(rectTransform2);
				float sqrMagnitude;
				if (MathTools.LineIntersectsRect(vector2, point, worldSpaceRect, out sqrMagnitude))
				{
					if (flag)
					{
						sqrMagnitude *= 0.25f;
					}
					if (sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
						selectable3 = selectable4;
					}
				}
				Vector2 vector3 = (Vector3)rectTransform2.rect.center;
				Vector2 to = (Vector2)selectable4.transform.TransformPoint(vector3) - vector2;
				float num3 = Mathf.Abs(Vector2.Angle(vector, to));
				if (!(num3 > 75f))
				{
					float sqrMagnitude2 = to.sqrMagnitude;
					if (sqrMagnitude2 < num)
					{
						num = sqrMagnitude2;
						selectable2 = selectable4;
					}
				}
			}
			if (selectable3 != null && selectable2 != null)
			{
				if (num2 > num)
				{
					return selectable2;
				}
				return selectable3;
			}
			return selectable3 ?? selectable2;
		}
	}
}
