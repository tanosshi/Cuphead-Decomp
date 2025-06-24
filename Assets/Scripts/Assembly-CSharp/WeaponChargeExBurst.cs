using UnityEngine;

public class WeaponChargeExBurst : AbstractProjectile
{
	public const float Offset = 125f;

	protected override void Start()
	{
		base.Start();
		GetComponent<SpriteRenderer>().flipX = Rand.Bool();
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionEnemy(hit, phase);
		if (phase == CollisionPhase.Enter && base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	private void OnEffectComplete()
	{
		Object.Destroy(base.gameObject);
	}
}
