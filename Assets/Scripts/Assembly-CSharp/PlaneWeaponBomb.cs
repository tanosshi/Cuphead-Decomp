using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlaneWeaponBomb : AbstractPlaneWeapon
{
	protected override bool rapidFire
	{
		get
		{
			return WeaponProperties.PlaneWeaponBomb.Basic.rapidFire;
		}
	}

	protected override float rapidFireRate
	{
		get
		{
			return WeaponProperties.PlaneWeaponBomb.Basic.rapidFireRate;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		PlaneWeaponBombProjectile planeWeaponBombProjectile = base.fireBasic() as PlaneWeaponBombProjectile;
		planeWeaponBombProjectile.shootsUp = false;
		planeWeaponBombProjectile.velocity = WeaponProperties.PlaneWeaponBomb.Basic.speed * MathUtils.AngleToDirection(planeWeaponBombProjectile.transform.rotation.eulerAngles.z);
		planeWeaponBombProjectile.gravity = WeaponProperties.PlaneWeaponBomb.Basic.gravity;
		planeWeaponBombProjectile.Damage = WeaponProperties.PlaneWeaponBomb.Basic.damage;
		planeWeaponBombProjectile.PlayerId = player.id;
		planeWeaponBombProjectile.bulletSize = WeaponProperties.PlaneWeaponBomb.Basic.size;
		planeWeaponBombProjectile.explosionSize = WeaponProperties.PlaneWeaponBomb.Basic.sizeExplosion;
		planeWeaponBombProjectile.SetAnimation(player.id);
		if (WeaponProperties.PlaneWeaponBomb.Basic.Up)
		{
			PlaneWeaponBombProjectile planeWeaponBombProjectile2 = base.fireBasic() as PlaneWeaponBombProjectile;
			planeWeaponBombProjectile2.shootsUp = true;
			planeWeaponBombProjectile2.velocity = WeaponProperties.PlaneWeaponBomb.Basic.speed * MathUtils.AngleToDirection(planeWeaponBombProjectile.transform.rotation.eulerAngles.z);
			planeWeaponBombProjectile2.gravity = WeaponProperties.PlaneWeaponBomb.Basic.gravity;
			planeWeaponBombProjectile2.Damage = WeaponProperties.PlaneWeaponBomb.Basic.damage;
			planeWeaponBombProjectile2.PlayerId = player.id;
			planeWeaponBombProjectile2.bulletSize = WeaponProperties.PlaneWeaponBomb.Basic.size;
			planeWeaponBombProjectile2.explosionSize = WeaponProperties.PlaneWeaponBomb.Basic.sizeExplosion;
			planeWeaponBombProjectile2.SetAnimation(player.id);
		}
		return planeWeaponBombProjectile;
	}

	protected override AbstractProjectile fireEx()
	{
		StartCoroutine(ex_cr());
		return null;
	}

	private IEnumerator ex_cr()
	{
		for (int wave = 0; wave < WeaponProperties.PlaneWeaponBomb.Ex.counts.Length; wave++)
		{
			int count = WeaponProperties.PlaneWeaponBomb.Ex.counts[wave];
			float angle = WeaponProperties.PlaneWeaponBomb.Ex.angles[wave];
			MeterScoreTracker tracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
			for (int i = 0; i < count; i++)
			{
				float num = Mathf.Lerp(0f, angle, (float)i / (float)count) - 90f;
				PlaneWeaponBombExProjectile planeWeaponBombExProjectile = _003CfireEx_003E__BaseCallProxy0() as PlaneWeaponBombExProjectile;
				planeWeaponBombExProjectile.transform.SetEulerAngles(0f, 0f, num);
				planeWeaponBombExProjectile.rotation = num;
				planeWeaponBombExProjectile.speed = WeaponProperties.PlaneWeaponBomb.Ex.speed;
				planeWeaponBombExProjectile.Damage = WeaponProperties.PlaneWeaponBomb.Ex.damage;
				planeWeaponBombExProjectile.PlayerId = player.id;
				planeWeaponBombExProjectile.rotationSpeed = WeaponProperties.PlaneWeaponBomb.Ex.rotationSpeed;
				planeWeaponBombExProjectile.rotationSpeedEaseTime = WeaponProperties.PlaneWeaponBomb.Ex.rotationSpeedEaseTime;
				planeWeaponBombExProjectile.timeBeforeEaseRotationSpeed = WeaponProperties.PlaneWeaponBomb.Ex.timeBeforeEaseRotationSpeed;
				tracker.Add(planeWeaponBombExProjectile);
				planeWeaponBombExProjectile.Init();
				planeWeaponBombExProjectile.FindTarget();
			}
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private AbstractProjectile _003CfireEx_003E__BaseCallProxy0()
	{
		return base.fireEx();
	}
}
