using UnityEngine;

public class VeggiesLevelBeetBabyBullet : AbstractProjectile
{
	public enum State
	{
		Go = 0,
		Dead = 1
	}

	private State state;

	private float speed;

	public VeggiesLevelBeetBabyBullet Create(float speed, Vector2 pos, float rot)
	{
		VeggiesLevelBeetBabyBullet veggiesLevelBeetBabyBullet = Create(pos, rot) as VeggiesLevelBeetBabyBullet;
		veggiesLevelBeetBabyBullet.CollisionDeath.OnlyPlayer();
		veggiesLevelBeetBabyBullet.DamagesType.OnlyPlayer();
		veggiesLevelBeetBabyBullet.speed = speed;
		return veggiesLevelBeetBabyBullet;
	}

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
	}

	protected override void Update()
	{
		base.Update();
		if (state != State.Dead)
		{
			base.transform.position += base.transform.right * speed * CupheadTime.Delta;
			if (base.transform.position.y < (float)Level.Current.Ground)
			{
				Die();
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void Die()
	{
		base.Die();
		state = State.Dead;
		animator.SetTrigger("Death");
		GetComponent<Collider2D>().enabled = false;
	}
}
