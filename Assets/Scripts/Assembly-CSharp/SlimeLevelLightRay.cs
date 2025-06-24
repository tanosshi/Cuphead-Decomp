using System.Collections;
using UnityEngine;

public class SlimeLevelLightRay : AbstractPausableComponent
{
	[SerializeField]
	private float holdTime;

	[SerializeField]
	private float fadeTime;

	[SerializeField]
	private bool startVisible;

	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(main_cr());
	}

	private IEnumerator main_cr()
	{
		bool fadingOut = startVisible;
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		if (!startVisible)
		{
			sprite.color = new Color(1f, 1f, 1f, 0f);
		}
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, holdTime);
			if (fadingOut)
			{
				float t = 0f;
				while (t < fadeTime)
				{
					sprite.color = new Color(1f, 1f, 1f, 1f - t / fadeTime);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				sprite.color = new Color(1f, 1f, 1f, 0f);
			}
			else
			{
				float t2 = 0f;
				while (t2 < fadeTime)
				{
					sprite.color = new Color(1f, 1f, 1f, t2 / fadeTime);
					t2 += (float)CupheadTime.Delta;
					yield return null;
				}
				sprite.color = new Color(1f, 1f, 1f, 1f);
			}
			fadingOut = !fadingOut;
		}
	}
}
