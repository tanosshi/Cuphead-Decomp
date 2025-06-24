using System.Collections;
using UnityEngine;

public class ClownLevelBackgroundLights : AbstractPausableComponent
{
	private float waitMinSecond;

	private float waitMaxSecond = 1f;

	private float fadeDurationMin = 0.5f;

	private float fadeDurationMax = 2f;

	[SerializeField]
	private bool occasionalFlicker;

	private float fadeWaitMinSecond = 5f;

	private float fadeWaitMaxSecond = 10f;

	private float flickerMinTime = 1f;

	private float flickerMaxTime = 5f;

	private SpriteRenderer lightVersion;

	private float getSecond;

	private float getFadeTime;

	private float fadeTime;

	private bool fadeIn;

	protected override void Start()
	{
		base.Start();
		lightVersion = GetComponent<SpriteRenderer>();
		StartCoroutine(lights_cr());
		if (occasionalFlicker)
		{
			StartCoroutine(flicker_cr());
		}
	}

	private IEnumerator lights_cr()
	{
		while (true)
		{
			fadeTime = Random.Range(fadeDurationMin, fadeDurationMax);
			getSecond = Random.Range(waitMinSecond, waitMaxSecond);
			if (fadeIn)
			{
				float t = 0f;
				while (t < fadeTime)
				{
					lightVersion.color = new Color(1f, 1f, 1f, t / fadeTime);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				lightVersion.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				float t2 = 0f;
				while (t2 < fadeTime)
				{
					lightVersion.color = new Color(1f, 1f, 1f, 1f - t2 / fadeTime);
					t2 += (float)CupheadTime.Delta;
					yield return null;
				}
				lightVersion.color = new Color(1f, 1f, 1f, 0f);
				yield return CupheadTime.WaitForSeconds(this, getSecond);
			}
			fadeIn = !fadeIn;
		}
	}

	private IEnumerator flicker_cr()
	{
		while (true)
		{
			float waitTime = Random.Range(fadeWaitMinSecond, fadeWaitMaxSecond);
			float flickerTime = Random.Range(flickerMinTime, flickerMaxTime);
			float t = 0f;
			while (t < flickerTime)
			{
				fadeTime = 0.08f;
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			fadeTime = getFadeTime;
			yield return CupheadTime.WaitForSeconds(this, waitTime);
		}
	}
}
