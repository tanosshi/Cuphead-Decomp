using System.Collections.Generic;

public class WeaponExploder : AbstractLevelWeapon
{
	public List<WeaponArcProjectile> projectilesOnGround = new List<WeaponArcProjectile>();

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
			return WeaponProperties.LevelWeaponExploder.Basic.fireRate;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		WeaponExploderProjectile weaponExploderProjectile = base.fireBasic() as WeaponExploderProjectile;
		weaponExploderProjectile.velocity = WeaponProperties.LevelWeaponExploder.Basic.speed;
		weaponExploderProjectile.sinVelocity = WeaponProperties.LevelWeaponExploder.Basic.sinSpeed;
		weaponExploderProjectile.sinSize = WeaponProperties.LevelWeaponExploder.Basic.sinSize;
		weaponExploderProjectile.rotation = weaponExploderProjectile.transform.eulerAngles.z;
		weaponExploderProjectile.PlayerId = player.id;
		weaponExploderProjectile.DamagesType.SetAll(false);
		weaponExploderProjectile.CollisionDeath.PlayerProjectileDefault();
		weaponExploderProjectile.weapon = this;
		return weaponExploderProjectile;
	}

	protected override AbstractProjectile fireEx()
	{
		WeaponExploderProjectile weaponExploderProjectile = base.fireEx() as WeaponExploderProjectile;
		weaponExploderProjectile.velocity = WeaponProperties.LevelWeaponExploder.Ex.speed;
		weaponExploderProjectile.sinVelocity = WeaponProperties.LevelWeaponExploder.Basic.sinSpeed;
		weaponExploderProjectile.sinSize = WeaponProperties.LevelWeaponExploder.Basic.sinSize;
		weaponExploderProjectile.rotation = weaponExploderProjectile.transform.eulerAngles.z;
		weaponExploderProjectile.Damage = WeaponProperties.LevelWeaponExploder.Ex.damage;
		weaponExploderProjectile.explodeRadius = WeaponProperties.LevelWeaponExploder.Ex.explodeRadius;
		weaponExploderProjectile.PlayerId = player.id;
		weaponExploderProjectile.DamagesType.SetAll(false);
		weaponExploderProjectile.CollisionDeath.PlayerProjectileDefault();
		MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
		meterScoreTracker.Add(weaponExploderProjectile);
		return weaponExploderProjectile;
	}
}
