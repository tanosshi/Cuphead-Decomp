using System.Collections;
using UnityEngine;

public class BaronessLevelJawbreakerGhost : AbstractCollidableObject
{
	private const float ROTATE_FRAME_TIME = 1f / 12f;

	private Vector3 deathPosition;

	private float deathSpeed = 2.3f;

	private float sinWaveStrength = 0.4f;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(deathrotation_cr());
	}

	private IEnumerator deathrotation_cr()
	{
		float frameTime = 0f;
		float t = 0f;
		while (true)
		{
			frameTime += (float)CupheadTime.Delta;
			deathPosition = new Vector3(base.transform.position.x + Mathf.Sin(t) * sinWaveStrength * (float)CupheadTime.Delta * 60f, base.transform.position.y + deathSpeed, 0f);
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 12f)
			{
				frameTime -= 1f / 12f;
				base.transform.up = (deathPosition - base.transform.position).normalized * CupheadTime.Delta;
			}
			if ((float)CupheadTime.Delta != 0f)
			{
				base.transform.position = deathPosition;
			}
			if (base.transform.position.y > 540f)
			{
				break;
			}
			yield return CupheadTime.WaitForSeconds(this, CupheadTime.Delta);
		}
		Die();
	}

	private void Die()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
