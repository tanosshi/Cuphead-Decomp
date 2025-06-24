using UnityEngine;

public class VeggiesLevelCarrotRegularProjectile : AbstractProjectile
{
	private float speed;

	private VeggiesLevelCarrot parent;

	protected override float DestroyLifetime
	{
		get
		{
			return 1000f;
		}
	}

	public VeggiesLevelCarrotRegularProjectile Create(VeggiesLevelCarrot parent, Vector2 pos, float speed, float rotation)
	{
		VeggiesLevelCarrotRegularProjectile veggiesLevelCarrotRegularProjectile = Create() as VeggiesLevelCarrotRegularProjectile;
		veggiesLevelCarrotRegularProjectile.CollisionDeath.None();
		veggiesLevelCarrotRegularProjectile.DamagesType.OnlyPlayer();
		veggiesLevelCarrotRegularProjectile.Init(parent, pos, speed, rotation);
		return veggiesLevelCarrotRegularProjectile;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.dead)
		{
			base.transform.position += base.transform.right * (speed * CupheadTime.FixedDelta);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		parent.OnDeathEvent -= Die;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private void Init(VeggiesLevelCarrot parent, Vector2 pos, float speed, float rotation)
	{
		this.parent = parent;
		this.speed = speed;
		parent.OnDeathEvent += Die;
		base.transform.position = pos;
		base.transform.SetLocalEulerAngles(0f, 0f, rotation);
	}

	protected override void Die()
	{
		AudioManager.Play("level_veggies_carrot_projectile_death");
		base.Die();
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
	}
}
