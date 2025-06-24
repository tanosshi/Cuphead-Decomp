using System.Collections;
using UnityEngine;

public class RetroArcadeWormPlatform : LevelPlatform
{
	private const float MOVE_Y = 50f;

	private const float MOVE_Y_SPEED = 50f;

	public void Rise()
	{
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		float moveTime = 1f;
		float t = 0f;
		while (t < moveTime)
		{
			t += CupheadTime.FixedDelta;
			base.transform.AddPosition(0f, 50f * CupheadTime.FixedDelta);
			yield return new WaitForFixedUpdate();
		}
	}
}
