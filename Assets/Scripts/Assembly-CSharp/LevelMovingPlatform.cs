using System.Collections;
using UnityEngine;

public class LevelMovingPlatform : LevelPlatform
{
	[SerializeField]
	private float time;

	[SerializeField]
	private Vector2 end;

	private Vector3 startPos;

	private Vector3 endPos;

	public virtual EaseUtils.EaseType Ease
	{
		get
		{
			return EaseUtils.EaseType.linear;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		startPos = base.transform.position;
		endPos = startPos + (Vector3)end;
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			yield return StartCoroutine(goTo_cr(startPos, endPos));
			yield return new WaitForSeconds(1f);
			yield return StartCoroutine(goTo_cr(endPos, startPos));
			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator goTo_cr(Vector3 start, Vector3 end)
	{
		float t = 0f;
		base.transform.position = start;
		while (t < time)
		{
			float val = t / time;
			Vector3 pos = base.transform.position;
			pos.x = EaseUtils.Ease(Ease, start.x, end.x, val);
			pos.y = EaseUtils.Ease(Ease, start.y, end.y, val);
			base.transform.position = pos;
			t += Time.deltaTime;
			yield return StartCoroutine(WaitForPause_CR());
		}
		base.transform.position = end;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = Color.magenta;
		if (Application.isPlaying)
		{
			Gizmos.DrawLine(startPos, endPos);
			Gizmos.DrawWireSphere(base.transform.position, 5f);
		}
		else
		{
			Gizmos.DrawLine(base.baseTransform.position, base.baseTransform.position + (Vector3)end);
		}
	}
}
