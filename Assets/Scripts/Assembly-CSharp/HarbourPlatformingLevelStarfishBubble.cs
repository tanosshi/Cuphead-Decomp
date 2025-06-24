using System;
using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelStarfishBubble : Effect
{
	private const float ROTATE_FRAME_TIME = 1f / 12f;

	private Vector3 pos;

	private float speed;

	private float sinWaveStrength;

	protected override void Start()
	{
		base.Start();
		sinWaveStrength = UnityEngine.Random.Range(0.4f, 0.9f);
		speed = UnityEngine.Random.Range(50f, 100f);
		StartCoroutine(deathrotation_cr());
	}

	private IEnumerator deathrotation_cr()
	{
		float totalTime = 0f;
		float maxTime = UnityEngine.Random.Range(4f, 7f);
		float t = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		while (totalTime < maxTime)
		{
			totalTime += (float)CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(base.transform.position.x + Mathf.Sin(t) * sinWaveStrength * (float)CupheadTime.Delta * 60f);
			base.transform.AddPosition(0f, speed * (float)CupheadTime.Delta);
			yield return null;
		}
		animator.Play("Pop");
		yield return null;
	}
}
