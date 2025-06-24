using System.Collections;
using UnityEngine;

public class FrogsLevelWaiter : AbstractMonoBehaviour
{
	private const float TIME = 8f;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(waiter_cr());
	}

	private IEnumerator waiter_cr()
	{
		float x = base.transform.localPosition.x;
		while (true)
		{
			base.transform.SetScale(1f);
			yield return StartCoroutine(move_cr(x, 0f - x));
			yield return CupheadTime.WaitForSeconds(this, 2f);
			base.transform.SetScale(-1f);
			yield return StartCoroutine(move_cr(0f - x, x));
			yield return CupheadTime.WaitForSeconds(this, 2f);
		}
	}

	private IEnumerator move_cr(float start, float end)
	{
		float t = 0f;
		base.transform.SetLocalPosition(start);
		while (t < 8f)
		{
			TransformExtensions.SetLocalPosition(x: Mathf.Lerp(start, end, t / 8f), transform: base.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetLocalPosition(end);
	}
}
