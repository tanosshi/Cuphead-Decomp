using UnityEngine;

public class FlyingMermaidLevelSplashEffect : Effect
{
	private void LayerUp()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		int sortingOrder = component.sortingOrder;
		string text = component.sortingLayerName;
		if (text == "Foreground" || (text == "Background" && sortingOrder < 80))
		{
			sortingOrder = sortingOrder - sortingOrder % 20 + 21;
		}
		else
		{
			text = "Foreground";
			sortingOrder = 1;
		}
		component.sortingLayerName = text;
		component.sortingOrder = sortingOrder;
	}
}
