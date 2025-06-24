using System.Collections.Generic;
using UnityEngine;

public class WeaponHoming : AbstractLevelWeapon
{
	private float fireRate = 1f;

	private float fireRateLockRatio;

	public static Transform target;

	private List<WeaponHomingProjectile> swirlingProjectiles = new List<WeaponHomingProjectile>();

	protected override bool rapidFire
	{
		get
		{
			return true;
		}
	}

	protected override float rapidFireRate
	{
		get
		{
			return fireRate;
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (player.motor.Locked)
		{
			fireRateLockRatio = Mathf.Clamp01(fireRateLockRatio + CupheadTime.FixedDelta / WeaponProperties.LevelWeaponHoming.Basic.lockedShotAccelerationTime);
		}
		else
		{
			fireRateLockRatio = 0f;
		}
		fireRate = WeaponProperties.LevelWeaponHoming.Basic.fireRate.GetFloatAt(1f - fireRateLockRatio);
	}

	protected override AbstractProjectile fireBasic()
	{
		WeaponHomingProjectile weaponHomingProjectile = base.fireBasic() as WeaponHomingProjectile;
		weaponHomingProjectile.rotation = weaponHomingProjectile.transform.rotation.eulerAngles.z + Random.Range(0f - WeaponProperties.LevelWeaponHoming.Basic.angleVariation, WeaponProperties.LevelWeaponHoming.Basic.angleVariation);
		weaponHomingProjectile.speed = WeaponProperties.LevelWeaponHoming.Basic.speed + Random.Range(0f - WeaponProperties.LevelWeaponHoming.Basic.speedVariation, WeaponProperties.LevelWeaponHoming.Basic.speedVariation);
		weaponHomingProjectile.rotationSpeed = WeaponProperties.LevelWeaponHoming.Basic.rotationSpeed;
		weaponHomingProjectile.rotationSpeedEaseTime = WeaponProperties.LevelWeaponHoming.Basic.rotationSpeedEaseTime;
		weaponHomingProjectile.timeBeforeEaseRotationSpeed = WeaponProperties.LevelWeaponHoming.Basic.timeBeforeEaseRotationSpeed;
		weaponHomingProjectile.Damage = WeaponProperties.LevelWeaponHoming.Basic.damage;
		weaponHomingProjectile.PlayerId = player.id;
		weaponHomingProjectile.DamagesType.PlayerProjectileDefault();
		weaponHomingProjectile.CollisionDeath.PlayerProjectileDefault();
		weaponHomingProjectile.trailFollowFrames = Mathf.Clamp(WeaponProperties.LevelWeaponHoming.Basic.trailFrameDelay, 1, 10);
		if (Random.Range(0, 4) == 0)
		{
			weaponHomingProjectile.transform.SetScale(0.8f, 0.8f);
		}
		if (MathUtils.RandomBool())
		{
			weaponHomingProjectile.transform.SetScale(0f - weaponHomingProjectile.transform.localScale.x);
		}
		weaponHomingProjectile.FindTarget();
		return weaponHomingProjectile;
	}

	protected override AbstractProjectile fireEx()
	{
		foreach (WeaponHomingProjectile swirlingProjectile in swirlingProjectiles)
		{
			if (swirlingProjectile != null)
			{
				swirlingProjectile.StopSwirling();
			}
		}
		MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
		swirlingProjectiles.Clear();
		for (int i = 0; i < WeaponProperties.LevelWeaponHoming.Ex.bulletCount; i++)
		{
			WeaponHomingProjectile weaponHomingProjectile = base.fireEx() as WeaponHomingProjectile;
			weaponHomingProjectile.speed = WeaponProperties.LevelWeaponHoming.Ex.speed;
			weaponHomingProjectile.rotationSpeed = WeaponProperties.LevelWeaponHoming.Basic.rotationSpeed;
			weaponHomingProjectile.rotationSpeedEaseTime = WeaponProperties.LevelWeaponHoming.Basic.rotationSpeedEaseTime;
			weaponHomingProjectile.timeBeforeEaseRotationSpeed = WeaponProperties.LevelWeaponHoming.Basic.timeBeforeEaseRotationSpeed;
			weaponHomingProjectile.Damage = WeaponProperties.LevelWeaponHoming.Ex.damage;
			weaponHomingProjectile.PlayerId = player.id;
			weaponHomingProjectile.DamagesType.PlayerProjectileDefault();
			weaponHomingProjectile.CollisionDeath.PlayerProjectileDefault();
			weaponHomingProjectile.CollisionDeath.SetBounds(false);
			weaponHomingProjectile.swirlDistance = WeaponProperties.LevelWeaponHoming.Ex.swirlDistance;
			weaponHomingProjectile.swirlEaseTime = WeaponProperties.LevelWeaponHoming.Ex.swirlEaseTime;
			weaponHomingProjectile.trailFollowFrames = Mathf.Clamp(WeaponProperties.LevelWeaponHoming.Ex.trailFrameDelay, 1, 10);
			weaponHomingProjectile.StartSwirling(i, WeaponProperties.LevelWeaponHoming.Ex.bulletCount, WeaponProperties.LevelWeaponHoming.Ex.spread, player);
			weaponHomingProjectile.isEx = true;
			swirlingProjectiles.Add(weaponHomingProjectile);
			meterScoreTracker.Add(weaponHomingProjectile);
		}
		return swirlingProjectiles[0];
	}

	public override void BeginBasic()
	{
		base.BeginBasic();
		AudioManager.Play("player_weapon_homing_fire_start");
		emitAudioFromObject.Add("player_weapon_homing_fire_start");
		BasicSoundLoop("player_weapon_homing_loop", "player_weapon_homing_loop_p2");
	}

	public override void EndBasic()
	{
		base.EndBasic();
		StopLoopSound("player_weapon_homing_loop", "player_weapon_homing_loop_p2");
	}
}
