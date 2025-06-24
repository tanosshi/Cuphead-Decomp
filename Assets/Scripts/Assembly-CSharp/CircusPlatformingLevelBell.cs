using System.Collections;
using UnityEngine;

public class CircusPlatformingLevelBell : PlatformingLevelPathMovementEnemy
{
	public float coolDown = 0.4f;

	public override void OnParry(AbstractPlayerController player)
	{
		base.animator.Play("Ring");
		AudioManager.Play("circus_bell_ding");
		GetComponent<Collider2D>().enabled = false;
		StartCoroutine(timer_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
	}

	private IEnumerator timer_cr()
	{
		float t = 0f;
		while (t < coolDown)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		Collider2D collider = GetComponent<Collider2D>();
		collider.enabled = true;
		yield return null;
	}

	protected override void CalculateCollider()
	{
	}
}
