using System;
using System.Collections;
using UnityEngine;

public class FlyingGenieLevelBackgroundShade : AbstractPausableComponent
{
	[SerializeField]
	private SpriteRenderer darkSprite;

	private SpriteRenderer[] darkClones;

	[SerializeField]
	private float fullSunOpacity;

	[SerializeField]
	private float fullShadeOpactity = 1f;

	private Color color;

	protected override void Start()
	{
		base.Start();
		color = GetComponent<SpriteRenderer>().color;
		FrameDelayedCallback(GetSprites, 1);
	}

	private void GetSprites()
	{
		darkClones = darkSprite.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
		StartCoroutine(fade_sprite_cr());
	}

	private IEnumerator fade_sprite_cr()
	{
		while (true)
		{
			float t = FlyingGenieLevel.mainTimer;
			float period = 12f;
			float shade = Mathf.Lerp(t: (Mathf.Sin(t * (float)Math.PI * 2f / period) + 1f) / 2f, a: fullSunOpacity, b: fullShadeOpactity);
			SpriteRenderer[] array = darkClones;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				spriteRenderer.color = new Color(spriteRenderer.GetComponent<SpriteRenderer>().color.r, spriteRenderer.GetComponent<SpriteRenderer>().color.g, spriteRenderer.GetComponent<SpriteRenderer>().color.b, shade);
			}
			yield return null;
		}
	}
}
