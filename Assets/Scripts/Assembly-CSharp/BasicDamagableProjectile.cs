using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DamageReceiver))]
public class BasicDamagableProjectile : BasicProjectile
{
	public float health = 10f;

	private DamageReceiver damageReceiver;

	public virtual BasicDamagableProjectile Create(Vector2 position, float rotation, float speed, float health)
	{
		BasicDamagableProjectile basicDamagableProjectile = Create(position, rotation, speed) as BasicDamagableProjectile;
		basicDamagableProjectile.health = health;
		return basicDamagableProjectile;
	}

	public virtual BasicDamagableProjectile Create(Vector2 position, float rotation, Vector2 scale, float speed, float health)
	{
		BasicDamagableProjectile basicDamagableProjectile = Create(position, rotation, scale, speed) as BasicDamagableProjectile;
		basicDamagableProjectile.health = health;
		return basicDamagableProjectile;
	}

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void OnDestroy()
	{
		damageReceiver.OnDamageTaken -= OnDamageTaken;
		base.OnDestroy();
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f)
		{
			Die();
		}
	}
}
