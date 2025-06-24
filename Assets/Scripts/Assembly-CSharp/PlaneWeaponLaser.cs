using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlaneWeaponLaser : AbstractPlaneWeapon
{
	private const float Y_POS = 20f;

	private float[] yPositions = new float[2] { 20f, -20f };

	private int currentY;

	protected override bool rapidFire
	{
		get
		{
			return WeaponProperties.PlaneWeaponLaser.Basic.rapidFire;
		}
	}

	protected override float rapidFireRate
	{
		get
		{
			return WeaponProperties.PlaneWeaponLaser.Basic.rapidFireRate;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		BasicProjectile basicProjectile = base.fireBasic() as BasicProjectile;
		basicProjectile.Speed = WeaponProperties.PlaneWeaponLaser.Basic.speed;
		basicProjectile.Damage = WeaponProperties.PlaneWeaponLaser.Basic.damage;
		basicProjectile.PlayerId = player.id;
		float num = yPositions[currentY];
		currentY++;
		if (currentY >= yPositions.Length)
		{
			currentY = 0;
		}
		basicProjectile.transform.AddPosition(0f, num);
		if (player.Shrunk)
		{
			basicProjectile.Damage *= shrunkDamageMultiplier;
			basicProjectile.transform.AddPosition(0f, num * -0.5f);
			basicProjectile.DestroyDistance = Random.Range(200, 350);
			basicProjectile.DestroyDistanceAnimated = true;
		}
		return basicProjectile;
	}

	protected override AbstractProjectile fireEx()
	{
		StartCoroutine(ex_cr());
		return null;
	}

	private IEnumerator ex_cr()
	{
		for (int wave = 0; wave < WeaponProperties.PlaneWeaponLaser.Ex.counts.Length; wave++)
		{
			int count = WeaponProperties.PlaneWeaponLaser.Ex.counts[wave];
			float angle = WeaponProperties.PlaneWeaponLaser.Ex.angles[wave];
			for (int i = 0; i < count; i++)
			{
				float value = Mathf.Lerp(0f, angle, (float)i / (float)count) - 90f;
				BasicProjectile basicProjectile = _003CfireEx_003E__BaseCallProxy0() as BasicProjectile;
				basicProjectile.transform.SetEulerAngles(0f, 0f, value);
				basicProjectile.Speed = WeaponProperties.PlaneWeaponLaser.Ex.speed;
				basicProjectile.Damage = WeaponProperties.PlaneWeaponLaser.Ex.damage;
				basicProjectile.PlayerId = player.id;
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
