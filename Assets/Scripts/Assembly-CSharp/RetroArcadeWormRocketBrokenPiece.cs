using System.Collections;
using UnityEngine;

public class RetroArcadeWormRocketBrokenPiece : BasicProjectile
{
	protected override void Awake()
	{
		base.Awake();
		Damage = PlayerManager.DamageMultiplier;
		StartCoroutine(turnOnCollider_cr());
	}

	private IEnumerator turnOnCollider_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.25f);
		GetComponent<Collider2D>().enabled = true;
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
