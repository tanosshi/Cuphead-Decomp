using UnityEngine;

public class WeaponCharge : AbstractLevelWeapon
{
	[SerializeField]
	private WeaponChargeChargingEffect chargeEffectPrefab;

	[SerializeField]
	private Effect fullChargeFx;

	private WeaponChargeChargingEffect chargeEffect;

	private bool fullyCharged;

	private float timeCharged;

	private int damageState;

	private float damage;

	private bool AllowChargeSound = true;

	protected override bool rapidFire
	{
		get
		{
			return false;
		}
	}

	protected override float rapidFireRate
	{
		get
		{
			return WeaponProperties.LevelWeaponCharge.Basic.fireRate;
		}
	}

	protected override bool isChargeWeapon
	{
		get
		{
			return true;
		}
	}

	protected override void StartCharging()
	{
		base.StartCharging();
		BasicSoundOneShot("player_weapon_charge_start", "player_weapon_charge_start_p2");
		if (chargeEffect != null)
		{
			Object.Destroy(chargeEffect.gameObject);
			chargeEffect = null;
		}
		chargeEffect = chargeEffectPrefab.Create(base.transform.position);
		chargeEffect.transform.parent = player.transform;
		timeCharged = 0f;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (chargeEffect == null)
		{
			fullyCharged = false;
			damage = WeaponProperties.LevelWeaponCharge.Basic.baseDamage;
			return;
		}
		chargeEffect.transform.position = player.weaponManager.GetBulletPosition();
		timeCharged += CupheadTime.FixedDelta;
		if (timeCharged > WeaponProperties.LevelWeaponCharge.Basic.timeStateThree)
		{
			fullyCharged = true;
			if (AllowChargeSound)
			{
				AudioManager.Play("player_weapon_charge_ready");
				AllowChargeSound = false;
			}
			chargeEffect.animator.SetTrigger("IsFull");
			damage = WeaponProperties.LevelWeaponCharge.Basic.damageStateThree;
		}
		else
		{
			fullyCharged = false;
			damage = WeaponProperties.LevelWeaponCharge.Basic.baseDamage;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		WeaponChargeProjectile weaponChargeProjectile;
		if (fullyCharged)
		{
			Effect effect = basicEffectPrefab;
			basicEffectPrefab = null;
			weaponChargeProjectile = base.fireBasic() as WeaponChargeProjectile;
			basicEffectPrefab = effect;
		}
		else
		{
			weaponChargeProjectile = base.fireBasic() as WeaponChargeProjectile;
		}
		weaponChargeProjectile.Speed = ((!fullyCharged) ? WeaponProperties.LevelWeaponCharge.Basic.speed : WeaponProperties.LevelWeaponCharge.Basic.speedStateTwo);
		weaponChargeProjectile.Damage = damage;
		weaponChargeProjectile.PlayerId = player.id;
		weaponChargeProjectile.DamagesType.PlayerProjectileDefault();
		weaponChargeProjectile.CollisionDeath.PlayerProjectileDefault();
		if (fullyCharged && player.motor.Ducking)
		{
			weaponChargeProjectile.CollisionDeath.Ground = false;
			weaponChargeProjectile.CollisionDeath.Walls = false;
			weaponChargeProjectile.CollisionDeath.Other = false;
		}
		weaponChargeProjectile.fullyCharged = fullyCharged;
		weaponChargeProjectile.animator.SetBool("FullCharge", fullyCharged);
		if (chargeEffect != null)
		{
			Object.Destroy(chargeEffect.gameObject);
			chargeEffect = null;
			timeCharged = 0f;
		}
		if (fullyCharged)
		{
			fullChargeFx.Create(weaponChargeProjectile.transform.position);
			BasicSoundOneShot("player_weapon_charge_full_fireball", "player_weapon_charge_full_fireball_p2");
			AllowChargeSound = true;
		}
		else
		{
			BasicSoundOneShot("player_weapon_charge_fire_small", "player_weapon_charge_fire_small_p2");
		}
		return weaponChargeProjectile;
	}

	protected override AbstractProjectile fireEx()
	{
		WeaponChargeExBurst weaponChargeExBurst = base.fireEx() as WeaponChargeExBurst;
		Vector2 vector = 125f * MathUtils.AngleToDirection(weaponChargeExBurst.transform.eulerAngles.z);
		weaponChargeExBurst.transform.AddPosition(vector.x, vector.y);
		weaponChargeExBurst.transform.SetEulerAngles(0f, 0f, 0f);
		weaponChargeExBurst.transform.SetScale((!Rand.Bool()) ? 1 : (-1));
		weaponChargeExBurst.PlayerId = player.id;
		weaponChargeExBurst.Damage = WeaponProperties.LevelWeaponCharge.Ex.damage;
		MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
		meterScoreTracker.Add(weaponChargeExBurst);
		return weaponChargeExBurst;
	}

	public override void BeginBasic()
	{
		if (fullyCharged)
		{
			BeginBasicCheckAttenuation("player_weapon_charge_full_fireball", "player_weapon_charge_full_fireball_p2");
		}
		else
		{
			BeginBasicCheckAttenuation("player_weapon_charge_fire_small", "player_weapon_charge_fire_small_p2");
		}
		base.BeginBasic();
	}

	public override void EndBasic()
	{
		if (fullyCharged)
		{
			EndBasicCheckAttenuation("player_weapon_charge_full_fireball", "player_weapon_charge_full_fireball_p2");
		}
		else
		{
			EndBasicCheckAttenuation("player_weapon_charge_fire_small", "player_weapon_charge_fire_small_p2");
		}
		base.EndBasic();
	}
}
