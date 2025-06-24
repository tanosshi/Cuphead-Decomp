using UnityEngine;

public class WeaponBouncer : AbstractLevelWeapon
{
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
			return WeaponProperties.LevelWeaponBouncer.Basic.fireRate;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		BasicSoundOneShot("player_weapon_bouncer", "player_weapon_bouncer_p2");
		WeaponBouncerProjectile weaponBouncerProjectile = base.fireBasic() as WeaponBouncerProjectile;
		float adjustedAngle = getAdjustedAngle(weaponBouncerProjectile.transform.rotation.eulerAngles.z);
		weaponBouncerProjectile.transform.SetEulerAngles(null, null, 0f);
		weaponBouncerProjectile.transform.SetScale(1f, 1f, 1f);
		weaponBouncerProjectile.velocity = WeaponProperties.LevelWeaponBouncer.Basic.launchSpeed * MathUtils.AngleToDirection(adjustedAngle);
		weaponBouncerProjectile.gravity = WeaponProperties.LevelWeaponBouncer.Basic.gravity;
		weaponBouncerProjectile.bounceRatio = WeaponProperties.LevelWeaponBouncer.Basic.bounceRatio;
		weaponBouncerProjectile.bounceSpeedDampening = WeaponProperties.LevelWeaponBouncer.Basic.bounceSpeedDampening;
		weaponBouncerProjectile.Damage = WeaponProperties.LevelWeaponBouncer.Basic.damage;
		weaponBouncerProjectile.PlayerId = player.id;
		weaponBouncerProjectile.weapon = this;
		return weaponBouncerProjectile;
	}

	private float getAdjustedAngle(float angle)
	{
		switch (Mathf.RoundToInt(angle))
		{
		case 0:
			angle += WeaponProperties.LevelWeaponBouncer.Basic.straightExtraAngle;
			break;
		case 45:
			angle += WeaponProperties.LevelWeaponBouncer.Basic.diagonalUpExtraAngle;
			break;
		case 135:
			angle -= WeaponProperties.LevelWeaponBouncer.Basic.diagonalUpExtraAngle;
			break;
		case 180:
			angle -= WeaponProperties.LevelWeaponBouncer.Basic.straightExtraAngle;
			break;
		case 225:
			angle -= WeaponProperties.LevelWeaponBouncer.Basic.diagonalDownExtraAngle;
			break;
		case 315:
			angle += WeaponProperties.LevelWeaponBouncer.Basic.diagonalDownExtraAngle;
			break;
		}
		return angle;
	}

	protected override AbstractProjectile fireEx()
	{
		WeaponBouncerProjectile weaponBouncerProjectile = base.fireEx() as WeaponBouncerProjectile;
		float adjustedAngle = getAdjustedAngle(weaponBouncerProjectile.transform.rotation.eulerAngles.z);
		weaponBouncerProjectile.transform.SetEulerAngles(null, null, 0f);
		weaponBouncerProjectile.transform.SetScale(1f, 1f, 1f);
		weaponBouncerProjectile.velocity = WeaponProperties.LevelWeaponBouncer.Basic.launchSpeed * MathUtils.AngleToDirection(adjustedAngle);
		weaponBouncerProjectile.gravity = WeaponProperties.LevelWeaponBouncer.Basic.gravity;
		weaponBouncerProjectile.weapon = this;
		weaponBouncerProjectile.Damage = WeaponProperties.LevelWeaponBouncer.Ex.damage;
		weaponBouncerProjectile.PlayerId = player.id;
		return weaponBouncerProjectile;
	}

	public override void BeginBasic()
	{
		BeginBasicCheckAttenuation("player_weapon_bouncer", "player_weapon_bouncer_p2");
		base.BeginBasic();
	}

	public override void EndBasic()
	{
		EndBasicCheckAttenuation("player_weapon_bouncer", "player_weapon_bouncer_p2");
		base.EndBasic();
	}
}
