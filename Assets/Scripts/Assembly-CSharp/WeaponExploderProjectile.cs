using UnityEngine;

public class WeaponExploderProjectile : BasicSineProjectile
{
	[SerializeField]
	private WeaponExploderProjectileExplosion explosionPrefab;

	[SerializeField]
	private BasicProjectile shrapnelPrefab;

	[SerializeField]
	private bool isEx;

	public float explodeRadius;

	public WeaponExploder weapon;

	private MeterScoreTracker tracker;

	protected override void Awake()
	{
		base.Awake();
	}

	public void Init(float velocity, float sinVelocity, float sinSize)
	{
		base.velocity = velocity;
		base.sinVelocity = sinVelocity;
		base.sinSize = sinSize;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!isEx)
		{
			UpdateDamageState();
		}
	}

	private void UpdateDamageState()
	{
		if (base.lifetime < WeaponProperties.LevelWeaponExploder.Basic.timeStateTwo)
		{
			Damage = WeaponProperties.LevelWeaponExploder.Basic.baseDamage;
			base.transform.SetScale(1f, 1f);
			explodeRadius = WeaponProperties.LevelWeaponExploder.Basic.baseExplosionRadius;
		}
		else if (base.lifetime < WeaponProperties.LevelWeaponExploder.Basic.timeStateThree)
		{
			Damage = WeaponProperties.LevelWeaponExploder.Basic.damageStateTwo;
			base.transform.SetScale(1.5f, 1.5f);
			explodeRadius = WeaponProperties.LevelWeaponExploder.Basic.explosionRadiusStateTwo;
		}
		else
		{
			Damage = WeaponProperties.LevelWeaponExploder.Basic.damageStateThree;
			base.transform.SetScale(2.5f, 2.5f);
			explodeRadius = WeaponProperties.LevelWeaponExploder.Basic.explosionRadiusStateThree;
		}
	}

	protected override void Die()
	{
		base.Die();
		explosionPrefab.Create(base.transform.position, explodeRadius, Damage, base.DamageMultiplier, weapon, tracker);
		if (shrapnelPrefab != null)
		{
			shrapnelPrefab.Create(base.transform.position, base.transform.eulerAngles.z + 180f, WeaponProperties.LevelWeaponExploder.Ex.shrapnelSpeed);
		}
		Object.Destroy(base.gameObject);
	}

	public override void AddToMeterScoreTracker(MeterScoreTracker tracker)
	{
		base.AddToMeterScoreTracker(tracker);
		this.tracker = tracker;
	}
}
