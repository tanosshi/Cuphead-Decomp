using System.Diagnostics;
using UnityEngine;

namespace RektTransform
{
	public static class RectTransformExtension
	{
		[Conditional("REKT_LOG_ACTIVE")]
		private static void Log(object message)
		{
			UnityEngine.Debug.Log(message);
		}

		public static void DebugOutput(this RectTransform RT)
		{
		}

		public static Rect GetWorldRect(this RectTransform RT)
		{
			Vector3[] array = new Vector3[4];
			RT.GetWorldCorners(array);
			return new Rect(size: new Vector2(array[2].x - array[1].x, array[1].y - array[0].y), position: new Vector2(array[1].x, 0f - array[1].y));
		}

		public static MinMax GetAnchors(this RectTransform RT)
		{
			return new MinMax(RT.anchorMin, RT.anchorMax);
		}

		public static void SetAnchors(this RectTransform RT, MinMax anchors)
		{
			RT.anchorMin = anchors.min;
			RT.anchorMax = anchors.max;
		}

		public static RectTransform GetParent(this RectTransform RT)
		{
			return RT.parent as RectTransform;
		}

		public static float GetWidth(this RectTransform RT)
		{
			return RT.rect.width;
		}

		public static float GetHeight(this RectTransform RT)
		{
			return RT.rect.height;
		}

		public static Vector2 GetSize(this RectTransform RT)
		{
			return new Vector2(RT.GetWidth(), RT.GetHeight());
		}

		public static void SetWidth(this RectTransform RT, float width)
		{
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}

		public static void SetHeight(this RectTransform RT, float height)
		{
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		public static void SetSize(this RectTransform RT, float width, float height)
		{
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		public static void SetSize(this RectTransform RT, Vector2 size)
		{
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
		}

		public static Vector2 GetLeft(this RectTransform RT)
		{
			return new Vector2(RT.offsetMin.x, RT.anchoredPosition.y);
		}

		public static Vector2 GetRight(this RectTransform RT)
		{
			return new Vector2(RT.offsetMax.x, RT.anchoredPosition.y);
		}

		public static Vector2 GetTop(this RectTransform RT)
		{
			return new Vector2(RT.anchoredPosition.x, RT.offsetMax.y);
		}

		public static Vector2 GetBottom(this RectTransform RT)
		{
			return new Vector2(RT.anchoredPosition.x, RT.offsetMin.y);
		}

		public static void SetLeft(this RectTransform RT, float left)
		{
			float xMin = RT.GetParent().rect.xMin;
			float num = RT.anchorMin.x * 2f - 1f;
			RT.offsetMin = new Vector2(xMin + xMin * num + left, RT.offsetMin.y);
		}

		public static void SetRight(this RectTransform RT, float right)
		{
			float xMax = RT.GetParent().rect.xMax;
			float num = RT.anchorMax.x * 2f - 1f;
			RT.offsetMax = new Vector2(xMax - xMax * num + right, RT.offsetMax.y);
		}

		public static void SetTop(this RectTransform RT, float top)
		{
			float yMax = RT.GetParent().rect.yMax;
			float num = RT.anchorMax.y * 2f - 1f;
			RT.offsetMax = new Vector2(RT.offsetMax.x, yMax - yMax * num + top);
		}

		public static void SetBottom(this RectTransform RT, float bottom)
		{
			float yMin = RT.GetParent().rect.yMin;
			float num = RT.anchorMin.y * 2f - 1f;
			RT.offsetMin = new Vector2(RT.offsetMin.x, yMin + yMin * num + bottom);
		}

		public static void Left(this RectTransform RT, float left)
		{
			RT.SetLeft(left);
		}

		public static void Right(this RectTransform RT, float right)
		{
			RT.SetRight(0f - right);
		}

		public static void Top(this RectTransform RT, float top)
		{
			RT.SetTop(0f - top);
		}

		public static void Bottom(this RectTransform RT, float bottom)
		{
			RT.SetRight(bottom);
		}

		public static void SetLeftFrom(this RectTransform RT, MinMax anchor, float left)
		{
			RT.offsetMin = new Vector2(RT.AnchorToParentSpace(anchor.min - RT.anchorMin).x + left, RT.offsetMin.y);
		}

		public static void SetRightFrom(this RectTransform RT, MinMax anchor, float right)
		{
			RT.offsetMax = new Vector2(RT.AnchorToParentSpace(anchor.max - RT.anchorMax).x + right, RT.offsetMax.y);
		}

		public static void SetTopFrom(this RectTransform RT, MinMax anchor, float top)
		{
			Vector2 vector = RT.AnchorToParentSpace(anchor.max - RT.anchorMax);
			RT.offsetMax = new Vector2(RT.offsetMax.x, vector.y + top);
		}

		public static void SetBottomFrom(this RectTransform RT, MinMax anchor, float bottom)
		{
			Vector2 vector = RT.AnchorToParentSpace(anchor.min - RT.anchorMin);
			RT.offsetMin = new Vector2(RT.offsetMin.x, vector.y + bottom);
		}

		public static void SetRelativeLeft(this RectTransform RT, float left)
		{
			RT.offsetMin = new Vector2(RT.anchoredPosition.x + left, RT.offsetMin.y);
		}

		public static void SetRelativeRight(this RectTransform RT, float right)
		{
			RT.offsetMax = new Vector2(RT.anchoredPosition.x + right, RT.offsetMax.y);
		}

		public static void SetRelativeTop(this RectTransform RT, float top)
		{
			RT.offsetMax = new Vector2(RT.offsetMax.x, RT.anchoredPosition.y + top);
		}

		public static void SetRelativeBottom(this RectTransform RT, float bottom)
		{
			RT.offsetMin = new Vector2(RT.offsetMin.x, RT.anchoredPosition.y + bottom);
		}

		public static void MoveLeft(this RectTransform RT, float left = 0f)
		{
			float xMin = RT.GetParent().rect.xMin;
			float num = RT.anchorMax.x - RT.anchorMin.x;
			float num2 = RT.anchorMax.x * 2f - 1f;
			RT.anchoredPosition = new Vector2(xMin + xMin * num2 + left - num * xMin, RT.anchoredPosition.y);
		}

		public static void MoveRight(this RectTransform RT, float right = 0f)
		{
			float xMax = RT.GetParent().rect.xMax;
			float num = RT.anchorMax.x - RT.anchorMin.x;
			float num2 = RT.anchorMax.x * 2f - 1f;
			RT.anchoredPosition = new Vector2(xMax - xMax * num2 - right + num * xMax, RT.anchoredPosition.y);
		}

		public static void MoveTop(this RectTransform RT, float top = 0f)
		{
			float yMax = RT.GetParent().rect.yMax;
			float num = RT.anchorMax.y - RT.anchorMin.y;
			float num2 = RT.anchorMax.y * 2f - 1f;
			RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, yMax - yMax * num2 - top + num * yMax);
		}

		public static void MoveBottom(this RectTransform RT, float bottom = 0f)
		{
			float yMin = RT.GetParent().rect.yMin;
			float num = RT.anchorMax.y - RT.anchorMin.y;
			float num2 = RT.anchorMax.y * 2f - 1f;
			RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, yMin + yMin * num2 + bottom - num * yMin);
		}

		public static void MoveLeftInside(this RectTransform RT, float left = 0f)
		{
			RT.MoveLeft(left + RT.GetWidth() / 2f);
		}

		public static void MoveRightInside(this RectTransform RT, float right = 0f)
		{
			RT.MoveRight(right + RT.GetWidth() / 2f);
		}

		public static void MoveTopInside(this RectTransform RT, float top = 0f)
		{
			RT.MoveTop(top + RT.GetHeight() / 2f);
		}

		public static void MoveBottomInside(this RectTransform RT, float bottom = 0f)
		{
			RT.MoveBottom(bottom + RT.GetHeight() / 2f);
		}

		public static void MoveLeftOutside(this RectTransform RT, float left = 0f)
		{
			RT.MoveLeft(left - RT.GetWidth() / 2f);
		}

		public static void MoveRightOutside(this RectTransform RT, float right = 0f)
		{
			RT.MoveRight(right - RT.GetWidth() / 2f);
		}

		public static void MoveTopOutside(this RectTransform RT, float top = 0f)
		{
			RT.MoveTop(top - RT.GetHeight() / 2f);
		}

		public static void MoveBottomOutside(this RectTransform RT, float bottom = 0f)
		{
			RT.MoveBottom(bottom - RT.GetHeight() / 2f);
		}

		public static void Move(this RectTransform RT, float x, float y)
		{
			RT.MoveLeft(x);
			RT.MoveBottom(y);
		}

		public static void Move(this RectTransform RT, Vector2 point)
		{
			RT.MoveLeft(point.x);
			RT.MoveBottom(point.y);
		}

		public static void MoveInside(this RectTransform RT, float x, float y)
		{
			RT.MoveLeftInside(x);
			RT.MoveBottomInside(y);
		}

		public static void MoveInside(this RectTransform RT, Vector2 point)
		{
			RT.MoveLeftInside(point.x);
			RT.MoveBottomInside(point.y);
		}

		public static void MoveOutside(this RectTransform RT, float x, float y)
		{
			RT.MoveLeftOutside(x);
			RT.MoveBottomOutside(y);
		}

		public static void MoveOutside(this RectTransform RT, Vector2 point)
		{
			RT.MoveLeftOutside(point.x);
			RT.MoveBottomOutside(point.y);
		}

		public static void MoveFrom(this RectTransform RT, MinMax anchor, Vector2 point)
		{
			RT.MoveFrom(anchor, point.x, point.y);
		}

		public static void MoveFrom(this RectTransform RT, MinMax anchor, float x, float y)
		{
			Vector2 vector = RT.AnchorToParentSpace(AnchorOrigin(anchor) - RT.AnchorOrigin());
			RT.anchoredPosition = new Vector2(vector.x + x, vector.y + y);
		}

		public static Vector2 ParentToChildSpace(this RectTransform RT, Vector2 point)
		{
			return RT.ParentToChildSpace(point.x, point.y);
		}

		public static Vector2 ParentToChildSpace(this RectTransform RT, float x, float y)
		{
			float xMin = RT.GetParent().rect.xMin;
			float yMin = RT.GetParent().rect.yMin;
			float num = RT.anchorMin.x * 2f - 1f;
			float num2 = RT.anchorMin.y * 2f - 1f;
			return new Vector2(xMin + xMin * num + x, yMin + yMin * num2 + y);
		}

		public static Vector2 ChildToParentSpace(this RectTransform RT, float x, float y)
		{
			return RT.AnchorOriginParent() + new Vector2(x, y);
		}

		public static Vector2 ChildToParentSpace(this RectTransform RT, Vector2 point)
		{
			return RT.AnchorOriginParent() + point;
		}

		public static Vector2 ParentToAnchorSpace(this RectTransform RT, Vector2 point)
		{
			return RT.ParentToAnchorSpace(point.x, point.y);
		}

		public static Vector2 ParentToAnchorSpace(this RectTransform RT, float x, float y)
		{
			Rect rect = RT.GetParent().rect;
			x = ((rect.width == 0f) ? 0f : (x / rect.width));
			y = ((rect.height == 0f) ? 0f : (y / rect.height));
			return new Vector2(x, y);
		}

		public static Vector2 AnchorToParentSpace(this RectTransform RT, float x, float y)
		{
			return new Vector2(x * RT.GetParent().rect.width, y * RT.GetParent().rect.height);
		}

		public static Vector2 AnchorToParentSpace(this RectTransform RT, Vector2 point)
		{
			return new Vector2(point.x * RT.GetParent().rect.width, point.y * RT.GetParent().rect.height);
		}

		public static Vector2 AnchorOrigin(this RectTransform RT)
		{
			return AnchorOrigin(RT.GetAnchors());
		}

		public static Vector2 AnchorOrigin(MinMax anchor)
		{
			float x = anchor.min.x + (anchor.max.x - anchor.min.x) / 2f;
			float y = anchor.min.y + (anchor.max.y - anchor.min.y) / 2f;
			return new Vector2(x, y);
		}

		public static Vector2 AnchorOriginParent(this RectTransform RT)
		{
			return Vector2.Scale(RT.AnchorOrigin(), new Vector2(RT.GetParent().rect.width, RT.GetParent().rect.height));
		}

		public static Canvas GetRootCanvas(this RectTransform RT)
		{
			Canvas componentInParent = RT.GetComponentInParent<Canvas>();
			while (!componentInParent.isRootCanvas)
			{
				componentInParent = componentInParent.transform.parent.GetComponentInParent<Canvas>();
			}
			return componentInParent;
		}
	}
}
