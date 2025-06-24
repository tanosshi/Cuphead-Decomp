public class WeaponPeashot : AbstractLevelWeapon
{
	private const float Y_POS = 20f;

	private const float ROTATION_OFFSET = 3f;

	private float[] yPositions = new float[4] { 0f, 20f, 40f, 20f };

	private int currentY;

	protected override bool rapidFire
	{
		get
		{
			return WeaponProperties.LevelWeaponPeashot.Basic.rapidFire;
		}
	}

	protected override float rapidFireRate
	{
		get
		{
			return WeaponProperties.LevelWeaponPeashot.Basic.rapidFireRate;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		BasicProjectile basicProjectile = base.fireBasic() as BasicProjectile;
		basicProjectile.Speed = WeaponProperties.LevelWeaponPeashot.Basic.speed;
		basicProjectile.Damage = WeaponProperties.LevelWeaponPeashot.Basic.damage;
		basicProjectile.PlayerId = player.id;
		basicProjectile.DamagesType.PlayerProjectileDefault();
		basicProjectile.CollisionDeath.PlayerProjectileDefault();
		float y = yPositions[currentY];
		currentY++;
		if (currentY >= yPositions.Length)
		{
			currentY = 0;
		}
		basicProjectile.transform.AddPosition(0f, y);
		return basicProjectile;
	}

	protected override AbstractProjectile fireEx()
	{
		WeaponPeashotExProjectile weaponPeashotExProjectile = base.fireEx() as WeaponPeashotExProjectile;
		weaponPeashotExProjectile.moveSpeed = WeaponProperties.LevelWeaponPeashot.Ex.speed;
		weaponPeashotExProjectile.Damage = WeaponProperties.LevelWeaponPeashot.Ex.damage;
		weaponPeashotExProjectile.hitFreezeTime = WeaponProperties.LevelWeaponPeashot.Ex.freezeTime;
		weaponPeashotExProjectile.DamageRate = weaponPeashotExProjectile.hitFreezeTime + WeaponProperties.LevelWeaponPeashot.Ex.damageDistance / weaponPeashotExProjectile.moveSpeed;
		weaponPeashotExProjectile.maxDamage = WeaponProperties.LevelWeaponPeashot.Ex.maxDamage;
		weaponPeashotExProjectile.PlayerId = player.id;
		MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
		meterScoreTracker.Add(weaponPeashotExProjectile);
		return weaponPeashotExProjectile;
	}

	public override void BeginBasic()
	{
		AudioManager.Play("player_default_fire_start");
		emitAudioFromObject.Add("player_default_fire_start");
		BasicSoundLoop("player_default_fire_loop", "player_default_fire_loop_p2");
		base.BeginBasic();
	}

	public override void EndBasic()
	{
		base.EndBasic();
		StopLoopSound("player_default_fire_loop", "player_default_fire_loop_p2");
	}
}
