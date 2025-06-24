using System.Collections;
using UnityEngine;

public class DragonLevelRain : AbstractPausableComponent
{
	[SerializeField]
	private float fadeTime;

	[SerializeField]
	private SpriteRenderer[] rainRenderers;

	public void StartRain()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(fade_cr());
	}

	private IEnumerator fade_cr()
	{
		float alpha = 0f;
		while (alpha < 1f)
		{
			for (int i = 0; i < rainRenderers.Length; i++)
			{
				Color color = rainRenderers[i].color;
				color.a = alpha;
				rainRenderers[i].color = color;
			}
			alpha += (float)CupheadTime.Delta / fadeTime;
			yield return null;
		}
	}
}
