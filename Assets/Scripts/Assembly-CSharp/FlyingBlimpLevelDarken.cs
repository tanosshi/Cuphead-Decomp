using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelDarken : AbstractPausableComponent
{
	[SerializeField]
	private FlyingBlimpLevelBlimpLady blimpLady;

	private SpriteRenderer[] children;

	private float fadeMax = 0.75f;

	private bool startedFade;

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
		children = base.transform.GetComponentsInChildren<SpriteRenderer>();
		if (blimpLady.fading && !startedFade)
		{
			startedFade = true;
			StartFade();
		}
	}

	private void StartFade()
	{
		StartCoroutine(fade_cr());
	}

	private IEnumerator fade_cr()
	{
		float t = 0f;
		float fadeTime = 0.005f;
		float fadeVal = 1f;
		while (t < fadeTime && fadeVal > fadeMax)
		{
			for (int i = 0; i < children.Length; i++)
			{
				if (children[i].transform != null)
				{
					children[i].color = new Color(fadeVal - t / fadeTime, fadeVal - t / fadeTime, fadeVal - t / fadeTime, 1f);
				}
			}
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		StartCoroutine(dark_cr());
		yield return null;
	}

	private IEnumerator dark_cr()
	{
		while (true)
		{
			for (int i = 0; i < children.Length; i++)
			{
				if (children[i].transform != null)
				{
					children[i].color = new Color(fadeMax, fadeMax, fadeMax, 1f);
				}
			}
			if (!blimpLady.fading)
			{
				break;
			}
			yield return null;
		}
		StartCoroutine(light_cr());
	}

	private IEnumerator light_cr()
	{
		float fadeVal = fadeMax;
		float fadeMid = 0.87f;
		while (true)
		{
			for (int i = 0; i < children.Length; i++)
			{
				if (children[i].transform != null)
				{
					children[i].color = new Color(fadeMid, fadeMid, fadeMid, 1f);
				}
			}
			if (blimpLady.state == FlyingBlimpLevelBlimpLady.State.Idle)
			{
				break;
			}
			yield return null;
		}
		for (int j = 0; j < children.Length; j++)
		{
			if (children[j].transform != null)
			{
				children[j].color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}
}
