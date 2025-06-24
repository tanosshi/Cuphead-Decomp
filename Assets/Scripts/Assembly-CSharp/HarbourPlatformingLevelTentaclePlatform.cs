using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelTentaclePlatform : LevelPlatform
{
	[SerializeField]
	private Transform drag;

	public float timeToSink = 5f;

	private bool startedSinking;

	public override void AddChild(Transform player)
	{
		base.AddChild(player);
		if (!startedSinking)
		{
			StartCoroutine(sink_cr());
		}
	}

	private IEnumerator sink_cr()
	{
		startedSinking = true;
		float t = 0f;
		Vector2 start = drag.transform.position;
		Vector2 end = new Vector2(drag.transform.position.x, drag.transform.position.y - 500f);
		while (t < timeToSink)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / timeToSink);
			drag.transform.position = Vector2.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		Object.Destroy(drag.gameObject);
		yield return null;
	}
}
