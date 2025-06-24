using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelSparkle : ScrollingSprite
{
	[SerializeField]
	private float minSecond;

	[SerializeField]
	private float maxSecond;

	private float getSecond;

	private float fadeTime;

	private float setSpeed;

	private bool fadeIn;

	private bool change;

	[SerializeField]
	private FlyingBlimpLevelMoonLady moonLady;

	private SpriteRenderer twinkleSprite;

	[SerializeField]
	private SpriteRenderer starSprite;

	private SpriteRenderer[] twinkleClones;

	private SpriteRenderer[] starClones;

	protected override void Start()
	{
		base.Start();
		FrameDelayedCallback(DisableStars, 1);
	}

	private void DisableStars()
	{
		twinkleSprite = GetComponent<SpriteRenderer>();
		starClones = starSprite.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
		twinkleClones = base.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
		starSprite.enabled = false;
		twinkleSprite.enabled = false;
		change = true;
		fadeTime = 0.8f;
		for (int i = 0; i < starClones.Length; i++)
		{
			starClones[i].enabled = false;
		}
		for (int j = 0; j < twinkleClones.Length; j++)
		{
			twinkleClones[j].enabled = false;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (moonLady.state == FlyingBlimpLevelMoonLady.State.Morph && change)
		{
			StartCoroutine(fadein_cr());
			change = false;
		}
	}

	private IEnumerator fadein_cr()
	{
		float t = 0f;
		starSprite.enabled = true;
		while (t < fadeTime)
		{
			starSprite.color = new Color(1f, 1f, 1f, t / fadeTime);
			for (int i = 0; i < starClones.Length; i++)
			{
				starClones[i].enabled = true;
				starClones[i].color = new Color(1f, 1f, 1f, t / fadeTime);
			}
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		starSprite.color = new Color(1f, 1f, 1f, 1f);
		for (int j = 0; j < starClones.Length; j++)
		{
			starClones[j].color = new Color(1f, 1f, 1f, 1f);
		}
		for (int k = 0; k < twinkleClones.Length; k++)
		{
			twinkleClones[k].color = new Color(1f, 1f, 1f, 1f);
		}
		StartCoroutine(twinkle_cr());
		yield return null;
	}

	private IEnumerator twinkle_cr()
	{
		twinkleSprite.enabled = true;
		for (int i = 0; i < twinkleClones.Length; i++)
		{
			twinkleClones[i].enabled = true;
		}
		while (true)
		{
			getSecond = Random.Range(minSecond, maxSecond);
			if (fadeIn)
			{
				float t = 0f;
				while (t < fadeTime)
				{
					twinkleSprite.color = new Color(1f, 1f, 1f, t / fadeTime);
					for (int j = 0; j < twinkleClones.Length; j++)
					{
						twinkleClones[j].color = new Color(1f, 1f, 1f, t / fadeTime);
					}
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				twinkleSprite.color = new Color(1f, 1f, 1f, 1f);
				for (int k = 0; k < twinkleClones.Length; k++)
				{
					twinkleClones[k].color = new Color(1f, 1f, 1f, 1f);
				}
			}
			else
			{
				float t2 = 0f;
				while (t2 < fadeTime)
				{
					twinkleSprite.color = new Color(1f, 1f, 1f, 1f - t2 / fadeTime);
					for (int l = 0; l < twinkleClones.Length; l++)
					{
						twinkleClones[l].color = new Color(1f, 1f, 1f, 1f - t2 / fadeTime);
					}
					t2 += (float)CupheadTime.Delta;
					yield return null;
				}
				twinkleSprite.color = new Color(1f, 1f, 1f, 0f);
				for (int m = 0; m < twinkleClones.Length; m++)
				{
					twinkleClones[m].color = new Color(1f, 1f, 1f, 0f);
				}
				yield return CupheadTime.WaitForSeconds(this, getSecond);
			}
			fadeIn = !fadeIn;
		}
	}
}
