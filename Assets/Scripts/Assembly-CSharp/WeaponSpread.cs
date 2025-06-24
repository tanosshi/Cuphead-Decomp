using UnityEngine;

public class WeaponSpread : AbstractLevelWeapon
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
			return WeaponProperties.LevelWeaponSpreadshot.Basic.rapidFireRate;
		}
	}

	protected override AbstractProjectile fireBasic()
	{
		float[] array = new float[2] { 0.5f, 0.75f };
		float damage = WeaponProperties.LevelWeaponSpreadshot.Basic.damage;
		for (int i = 0; i < 2; i++)
		{
			BasicProjectile basicProjectile = fireBasicNoEffect() as BasicProjectile;
			basicProjectile.Speed = WeaponProperties.LevelWeaponSpreadshot.Basic.speed * array[i];
			basicProjectile.DestroyDistance = WeaponProperties.LevelWeaponSpreadshot.Basic.distance - 20f * (float)(i + 1);
			basicProjectile.Damage = damage;
			basicProjectile.PlayerId = player.id;
			basicProjectile.transform.AddEulerAngles(0f, 0f, 15f * (float)(i + 1));
			Animator component = basicProjectile.GetComponent<Animator>();
			component.SetBool("Large", i == 1);
			BasicProjectile basicProjectile2 = fireBasicNoEffect() as BasicProjectile;
			basicProjectile2.Speed = WeaponProperties.LevelWeaponSpreadshot.Basic.speed * array[i];
			basicProjectile2.DestroyDistance = WeaponProperties.LevelWeaponSpreadshot.Basic.distance - 20f * (float)(i + 1);
			basicProjectile2.Damage = damage;
			basicProjectile2.PlayerId = player.id;
			basicProjectile2.transform.AddEulerAngles(0f, 0f, -15f * (float)(i + 1));
			Animator component2 = basicProjectile2.GetComponent<Animator>();
			component2.SetBool("Large", i == 1);
		}
		BasicProjectile basicProjectile3 = base.fireBasic() as BasicProjectile;
		basicProjectile3.Speed = WeaponProperties.LevelWeaponSpreadshot.Basic.speed;
		basicProjectile3.Damage = damage;
		basicProjectile3.PlayerId = player.id;
		basicProjectile3.DestroyDistance = WeaponProperties.LevelWeaponSpreadshot.Basic.distance;
		return basicProjectile3;
	}

	protected override AbstractProjectile fireEx()
	{
		AudioManager.Play("player_weapon_exploder_fire");
		PlayerLevelSpreadEx playerLevelSpreadEx = base.fireEx() as PlayerLevelSpreadEx;
		playerLevelSpreadEx.Init(WeaponProperties.LevelWeaponSpreadshot.Ex.speed, WeaponProperties.LevelWeaponSpreadshot.Ex.damage, WeaponProperties.LevelWeaponSpreadshot.Ex.childCount, WeaponProperties.LevelWeaponSpreadshot.Ex.radius);
		return playerLevelSpreadEx;
	}

	public override void BeginBasic()
	{
		base.BeginBasic();
		BasicSoundLoop("player_weapon_spread_loop", "player_weapon_spread_loop_p2");
	}

	public override void EndBasic()
	{
		base.EndBasic();
		StopLoopSound("player_weapon_spread_loop", "player_weapon_spread_loop_p2");
	}
}
