using UnityEngine;

public class WeaponExploderProjectileExplosion : Effect
{
	private DamageDealer damageDealer;

	private const float BASE_RADIUS = 15f;

	private WeaponExploder weapon;

	public void Create(Vector2 position, float radius, float damage, float damageMultiplier, WeaponExploder weapon, MeterScoreTracker tracker)
	{
		float num = radius / 15f;
		WeaponExploderProjectileExplosion weaponExploderProjectileExplosion = base.Create(position, new Vector3(num, num, 1f)) as WeaponExploderProjectileExplosion;
		weaponExploderProjectileExplosion.damageDealer.SetDamage(damage);
		weaponExploderProjectileExplosion.damageDealer.DamageMultiplier *= damageMultiplier;
		weaponExploderProjectileExplosion.damageDealer.SetDamageFlags(false, true, false);
		weaponExploderProjectileExplosion.weapon = weapon;
		weaponExploderProjectileExplosion.damageDealer.OnDealDamage += weaponExploderProjectileExplosion.OnDealDamage;
		if (tracker != null)
		{
			tracker.Add(weaponExploderProjectileExplosion.damageDealer);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		damageDealer = new DamageDealer(1f, 0f);
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionEnemy(hit, phase);
		if (phase == CollisionPhase.Enter && damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	private void OnDealDamage(float damage, DamageReceiver damageReceiver, DamageDealer damageDealer)
	{
		if (weapon != null)
		{
			weapon.OnDealDamage(damage, damageReceiver, damageDealer);
		}
	}
}
