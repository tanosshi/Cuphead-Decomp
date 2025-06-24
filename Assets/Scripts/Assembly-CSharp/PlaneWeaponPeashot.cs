using UnityEngine;

public class PlaneWeaponPeashot : AbstractPlaneWeapon
{
	private const float Y_POS = 20f;

	private float[] yPositions = new float[2] { 20f, -20f };

	private int currentY;

	protected override bool rapidFire
	{
		get
		{
			return WeaponProperties.PlaneWeaponPeashot.Basic.rapidFire;
		}
	}

	protected override float rapidFireRate
	{
		get
		{
			return WeaponProperties.PlaneWeaponPeashot.Basic.rapidFireRate;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		if (player.id == PlayerId.PlayerOne)
		{
			if (!AudioManager.CheckIfPlaying("player_plane_weapon_fire_loop_cuphead"))
			{
				AudioManager.PlayLoop("player_plane_weapon_fire_loop_cuphead");
			}
		}
		else if (!AudioManager.CheckIfPlaying("player_plane_weapon_fire_loop_mugman"))
		{
			AudioManager.PlayLoop("player_plane_weapon_fire_loop_mugman");
		}
		emitAudioFromObject.Add("player_plane_weapon_fire_loop_cuphead");
		emitAudioFromObject.Add("player_plane_weapon_fire_loop_mugman");
		BasicProjectile basicProjectile = base.fireBasic() as BasicProjectile;
		basicProjectile.Speed = WeaponProperties.PlaneWeaponPeashot.Basic.speed;
		basicProjectile.Damage = WeaponProperties.PlaneWeaponPeashot.Basic.damage;
		basicProjectile.PlayerId = player.id;
		float num = yPositions[currentY];
		currentY++;
		if (currentY >= yPositions.Length)
		{
			currentY = 0;
		}
		basicProjectile.transform.AddPosition(0f, num);
		Animator component = basicProjectile.GetComponent<Animator>();
		component.SetInteger("Variant", Random.Range(0, component.GetInteger("MaxVariants")));
		component.SetBool("isCH", basicProjectile.PlayerId == PlayerId.PlayerOne);
		if (player.Shrunk)
		{
			basicProjectile.Damage *= shrunkDamageMultiplier;
			basicProjectile.transform.AddPosition(0f, num * -0.5f);
			basicProjectile.DestroyDistance = Random.Range(200, 350);
			basicProjectile.DestroyDistanceAnimated = true;
			basicProjectile.DamageSource = DamageDealer.DamageSource.SmallPlane;
		}
		return basicProjectile;
	}

	protected override AbstractProjectile fireEx()
	{
		PlaneWeaponPeashotExProjectile planeWeaponPeashotExProjectile = base.fireEx() as PlaneWeaponPeashotExProjectile;
		planeWeaponPeashotExProjectile.MaxSpeed = WeaponProperties.PlaneWeaponPeashot.Ex.maxSpeed;
		planeWeaponPeashotExProjectile.Acceleration = WeaponProperties.PlaneWeaponPeashot.Ex.acceleration;
		planeWeaponPeashotExProjectile.FreezeTime = WeaponProperties.PlaneWeaponPeashot.Ex.freezeTime;
		planeWeaponPeashotExProjectile.Damage = WeaponProperties.PlaneWeaponPeashot.Ex.damage;
		planeWeaponPeashotExProjectile.DamageRate = WeaponProperties.PlaneWeaponPeashot.Ex.freezeTime + WeaponProperties.PlaneWeaponPeashot.Ex.damageDistance / planeWeaponPeashotExProjectile.MaxSpeed;
		planeWeaponPeashotExProjectile.PlayerId = player.id;
		planeWeaponPeashotExProjectile.speed = Mathf.Clamp(player.motor.Velocity.x, 0f, planeWeaponPeashotExProjectile.MaxSpeed);
		MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
		meterScoreTracker.Add(planeWeaponPeashotExProjectile);
		planeWeaponPeashotExProjectile.Init();
		return planeWeaponPeashotExProjectile;
	}
}
