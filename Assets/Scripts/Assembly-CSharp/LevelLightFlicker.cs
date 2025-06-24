using System.Collections;
using UnityEngine;

public class LevelLightFlicker : AbstractPausableComponent
{
	[SerializeField]
	private float fadeWaitMinSecond = 8f;

	[SerializeField]
	private float fadeWaitMaxSecond = 25f;

	[SerializeField]
	private int countUntilPause = 3;

	private SpriteRenderer sprite;

	protected override void Start()
	{
		base.Start();
		sprite = GetComponent<SpriteRenderer>();
	}

	private IEnumerator flicker_cr()
	{
		float flickerTime = 0.3f;
		while (true)
		{
			int counter = 0;
			float waitTime = Random.Range(fadeWaitMinSecond, fadeWaitMaxSecond);
			float t = 0f;
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			while (counter < countUntilPause)
			{
				while (t < flickerTime)
				{
					sprite.color = new Color(1f, 1f, 1f, 1f - t / flickerTime);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				t = 0f;
				sprite.color = new Color(1f, 1f, 1f, 0f);
				while (t < flickerTime)
				{
					sprite.color = new Color(1f, 1f, 1f, t / flickerTime);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				sprite.color = new Color(1f, 1f, 1f, 1f);
				counter++;
				yield return null;
			}
			yield return null;
		}
	}
}
