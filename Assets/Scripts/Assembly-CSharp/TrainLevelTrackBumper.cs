using System;
using System.Collections;
using UnityEngine;

public class TrainLevelTrackBumper : AbstractPausableComponent
{
	private const float bumpSpeed = 6000f;

	private const float bumpHeight = 3f;

	private const float bumpDuration = 0.1f;

	private const float bumpPeriod = 0.75f;

	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(main_cr());
	}

	private IEnumerator main_cr()
	{
		float startingY = base.transform.position.y;
		float t = 0f;
		while (true)
		{
			t += (float)CupheadTime.Delta;
			float d = (base.transform.position.x + 6000f * t) % 4500f;
			float y = startingY;
			if (d < 600f)
			{
				float num = d / 600f;
				y += Mathf.Sin(num * (float)Math.PI) * 3f;
			}
			base.transform.SetPosition(null, y);
			yield return null;
		}
	}
}
