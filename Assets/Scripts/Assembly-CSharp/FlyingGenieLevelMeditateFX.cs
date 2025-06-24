using System.Collections;
using UnityEngine;

public class FlyingGenieLevelMeditateFX : Effect
{
	private const float FRAME_TIME = 1f / 12f;

	private float frameTime;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(effect_cr());
	}

	private IEnumerator effect_cr()
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		sprite.color = new Color(1f, 1f, 1f, 0f);
		float t = 0f;
		float time = 1f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			sprite.color = new Color(1f, 1f, 1f, t / time);
			yield return null;
		}
		sprite.color = new Color(1f, 1f, 1f, 1f);
		t = 0f;
		while (true)
		{
			frameTime += CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 12f)
			{
				base.transform.SetEulerAngles(null, null, 360f * t);
				frameTime -= 1f / 12f;
			}
			yield return null;
		}
	}

	public void EndEffect()
	{
		StopAllCoroutines();
		StartCoroutine(end_effect_cr());
	}

	private IEnumerator end_effect_cr()
	{
		float t = 0f;
		float time = 1f;
		base.transform.SetEulerAngles(null, null, 0f);
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetScale(1f - t / time, 1f - t / time);
			yield return null;
		}
		OnEffectComplete();
	}
}
