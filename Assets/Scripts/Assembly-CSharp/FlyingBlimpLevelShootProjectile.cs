using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelShootProjectile : AbstractProjectile
{
	private LevelProperties.FlyingBlimp.Shoot properties;

	private float velocity;

	public FlyingBlimpLevelShootProjectile Create(Vector2 pos, float rotation, LevelProperties.FlyingBlimp.Shoot properties)
	{
		FlyingBlimpLevelShootProjectile flyingBlimpLevelShootProjectile = base.Create() as FlyingBlimpLevelShootProjectile;
		flyingBlimpLevelShootProjectile.properties = properties;
		flyingBlimpLevelShootProjectile.velocity = properties.speedMin;
		flyingBlimpLevelShootProjectile.transform.position = pos;
		return flyingBlimpLevelShootProjectile;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (velocity < properties.speedMax)
		{
			velocity += properties.accelerationTime * CupheadTime.FixedDelta;
			yield return wait;
			base.transform.AddPosition((0f - velocity) * CupheadTime.FixedDelta);
		}
		Die();
		yield return wait;
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
