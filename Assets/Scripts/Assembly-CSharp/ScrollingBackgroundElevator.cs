using System.Collections;
using UnityEngine;

public class ScrollingBackgroundElevator : AbstractPausableComponent
{
	[SerializeField]
	private bool isClouds;

	[SerializeField]
	private bool isBackground;

	[SerializeField]
	private SpriteRenderer firstSprite;

	[SerializeField]
	private SpriteRenderer lastSprite;

	private float speed;

	private Vector3 startPos;

	private Vector3 endPos;

	private Vector3 direction;

	public bool ending;

	public void SetUp(Vector3 direction, float speed)
	{
		if (isBackground)
		{
			startPos = firstSprite.transform.position;
		}
		else
		{
			startPos = base.transform.position + direction.normalized * -800f;
		}
		endPos = lastSprite.transform.position;
		this.direction = direction;
		this.speed = speed;
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		while (!ending)
		{
			if (isBackground)
			{
				while (base.transform.position != endPos && !ending)
				{
					base.transform.position = Vector3.MoveTowards(base.transform.position, endPos, speed * (float)CupheadTime.Delta);
					yield return null;
				}
			}
			else
			{
				while ((base.transform.position.y < endPos.y && base.transform.position.x > endPos.x && !ending) || (isClouds && ending))
				{
					base.transform.position -= direction * speed * CupheadTime.Delta;
					yield return null;
				}
			}
			if (!ending)
			{
				base.transform.position = startPos;
			}
			yield return null;
		}
	}

	public void EaseoutSpeed(float time)
	{
		StartCoroutine(ease_speed_cr(time));
	}

	private IEnumerator ease_speed_cr(float time)
	{
		float startSpeed = speed;
		float t = 0f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			speed = Mathf.Lerp(startSpeed, 0f, t / time);
			yield return null;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(0f, 0f, 1f, 1f);
		Gizmos.DrawWireSphere(base.transform.position, 100f);
	}
}
