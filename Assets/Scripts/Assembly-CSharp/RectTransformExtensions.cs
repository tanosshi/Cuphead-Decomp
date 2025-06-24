using UnityEngine;

public static class RectTransformExtensions
{
	public static RectTransform Copy(this RectTransform transform, RectTransform target)
	{
		transform.SetParent(target.parent);
		transform.position = target.position;
		transform.localScale = target.localScale;
		transform.rotation = target.rotation;
		transform.anchoredPosition3D = target.anchoredPosition3D;
		transform.anchorMax = target.anchorMax;
		transform.anchorMin = target.anchorMin;
		transform.offsetMax = target.offsetMax;
		transform.offsetMin = target.offsetMin;
		transform.pivot = target.pivot;
		transform.sizeDelta = target.sizeDelta;
		return transform;
	}
}
