using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelMinerRope : AbstractPausableComponent
{
	public void PullRope(float ascendTime, Vector2 startPos)
	{
		StartCoroutine(pull_up_rope_cr(ascendTime, startPos));
	}

	public IEnumerator pull_up_rope_cr(float ascendTime, Vector2 startPos)
	{
		float t = 0f;
		Vector3 end = new Vector3(base.transform.position.x, startPos.y + 400f);
		Vector3 start = base.transform.position;
		while (t < ascendTime)
		{
			t += (float)CupheadTime.Delta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / ascendTime);
			base.transform.position = Vector2.Lerp(start, end, val);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield return null;
	}
}
