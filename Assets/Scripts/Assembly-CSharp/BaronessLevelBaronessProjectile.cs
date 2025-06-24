using UnityEngine;

public class BaronessLevelBaronessProjectile : AbstractProjectile
{
	[SerializeField]
	private Effect deathFX;

	[SerializeField]
	private GameObject FX;

	private DamageReceiver damageReceiver;

	private float health;

	protected override void Start()
	{
		base.Start();
		health = GetComponentInParent<BaronessLevelBaronessProjectileBunch>().properties.projectileHP;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
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

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f)
		{
			Die();
		}
	}

	protected override void Die()
	{
		deathFX.Create(base.transform.position);
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		FX.SetActive(false);
		StopAllCoroutines();
		base.Die();
	}
}
