using System.Collections.Generic;
using UnityEngine;

public class WeaponArc : AbstractLevelWeapon
{
	public List<WeaponArcProjectile> projectilesOnGround = new List<WeaponArcProjectile>();

	private bool isDiagonal;

	protected override bool rapidFire
	{
		get
		{
			return WeaponProperties.LevelWeaponArc.Basic.rapidFire;
		}
	}

	protected override float rapidFireRate
	{
		get
		{
			return WeaponProperties.LevelWeaponArc.Basic.fireRate;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		AudioManager.Play("player_weapon_arc");
		emitAudioFromObject.Add("player_weapon_arc");
		WeaponArcProjectile weaponArcProjectile = base.fireBasic() as WeaponArcProjectile;
		weaponArcProjectile.PlayerId = player.id;
		float num = weaponArcProjectile.transform.rotation.eulerAngles.z;
		if (num == 0f)
		{
			num += WeaponProperties.LevelWeaponArc.Basic.straightShotAngle;
			isDiagonal = false;
		}
		else if (num == 180f)
		{
			num -= WeaponProperties.LevelWeaponArc.Basic.straightShotAngle;
			isDiagonal = false;
		}
		else if (Mathf.Approximately(num, 45f) || Mathf.Approximately(num, 135f))
		{
			num += WeaponProperties.LevelWeaponArc.Basic.diagShotAngle;
			isDiagonal = true;
		}
		else
		{
			isDiagonal = false;
		}
		weaponArcProjectile.transform.SetEulerAngles(null, null, num);
		if (isDiagonal)
		{
			weaponArcProjectile.velocity = WeaponProperties.LevelWeaponArc.Basic.diagLaunchSpeed * MathUtils.AngleToDirection(weaponArcProjectile.transform.rotation.eulerAngles.z);
			weaponArcProjectile.gravity = WeaponProperties.LevelWeaponArc.Basic.diagGravity;
		}
		else
		{
			weaponArcProjectile.velocity = WeaponProperties.LevelWeaponArc.Basic.launchSpeed * MathUtils.AngleToDirection(weaponArcProjectile.transform.rotation.eulerAngles.z);
			weaponArcProjectile.gravity = WeaponProperties.LevelWeaponArc.Basic.gravity;
		}
		weaponArcProjectile.weapon = this;
		return weaponArcProjectile;
	}

	protected override AbstractProjectile fireEx()
	{
		AudioManager.Play(Sfx.Player_Weapon_Peashot_Ex);
		WeaponArcProjectile weaponArcProjectile = base.fireEx() as WeaponArcProjectile;
		weaponArcProjectile.velocity = WeaponProperties.LevelWeaponArc.Basic.launchSpeed * MathUtils.AngleToDirection(weaponArcProjectile.transform.rotation.eulerAngles.z);
		weaponArcProjectile.gravity = WeaponProperties.LevelWeaponArc.Basic.gravity;
		weaponArcProjectile.weapon = this;
		weaponArcProjectile.Damage = WeaponProperties.LevelWeaponArc.Ex.damage;
		weaponArcProjectile.PlayerId = player.id;
		MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
		meterScoreTracker.Add(weaponArcProjectile);
		return weaponArcProjectile;
	}
}
