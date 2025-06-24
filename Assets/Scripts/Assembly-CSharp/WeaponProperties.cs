using UnityEngine;

public static class WeaponProperties
{
	public static class ArcadeWeaponPeashot
	{
		public static class Basic
		{
			public static readonly float damage = 4f;

			public static readonly float speed = 850f;

			public static readonly bool rapidFire;

			public static readonly float rapidFireRate;
		}

		public static class Ex
		{
		}

		public static readonly int value = 2;

		public static readonly string iconPath = "Icons/";

		public static readonly Weapon id = Weapon.arcade_weapon_peashot;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.arcade_weapon_peashot);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.arcade_weapon_peashot);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.arcade_weapon_peashot);
			}
		}
	}

	public static class CharmCharmParryPlus
	{
		public static readonly int value = 3;

		public static readonly string iconPath = "Icons/equip_icon_charm_parry_slapper";

		public static readonly Charm id = Charm.charm_parry_plus;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Charm.charm_parry_plus);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Charm.charm_parry_plus);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Charm.charm_parry_plus);
			}
		}
	}

	public static class CharmHealthUpOne
	{
		public static readonly int value = 3;

		public static readonly string iconPath = "Icons/equip_icon_charm_hp1";

		public static readonly Charm id = Charm.charm_health_up_1;

		public static readonly int healthIncrease = 1;

		public static readonly float weaponDebuff = 0.05f;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Charm.charm_health_up_1);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Charm.charm_health_up_1);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Charm.charm_health_up_1);
			}
		}
	}

	public static class CharmHealthUpTwo
	{
		public static readonly int value = 5;

		public static readonly string iconPath = "Icons/equip_icon_charm_hp2";

		public static readonly Charm id = Charm.charm_health_up_2;

		public static readonly int healthIncrease = 2;

		public static readonly float weaponDebuff = 0.1f;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Charm.charm_health_up_2);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Charm.charm_health_up_2);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Charm.charm_health_up_2);
			}
		}
	}

	public static class CharmParryAttack
	{
		public static readonly int value = 3;

		public static readonly string iconPath = "Icons/equip_icon_charm_parry_attack";

		public static readonly Charm id = Charm.charm_parry_attack;

		public static readonly float damage = 16f;

		public static readonly float bounce;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Charm.charm_parry_attack);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Charm.charm_parry_attack);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Charm.charm_parry_attack);
			}
		}
	}

	public static class CharmPitSaver
	{
		public static readonly int value = 3;

		public static readonly string iconPath = "Icons/equip_icon_charm_pitsaver";

		public static readonly Charm id = Charm.charm_pit_saver;

		public static readonly float meterAmount = 10f;

		public static readonly float invulnerabilityMultiplier = 2f;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Charm.charm_pit_saver);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Charm.charm_pit_saver);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Charm.charm_pit_saver);
			}
		}
	}

	public static class CharmSmokeDash
	{
		public static readonly int value = 3;

		public static readonly string iconPath = "Icons/equip_icon_charm_smoke-dash";

		public static readonly Charm id = Charm.charm_smoke_dash;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Charm.charm_smoke_dash);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Charm.charm_smoke_dash);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Charm.charm_smoke_dash);
			}
		}
	}

	public static class CharmSuperBuilder
	{
		public static readonly int value = 3;

		public static readonly string iconPath = "Icons/equip_icon_charm_coffee";

		public static readonly Charm id = Charm.charm_super_builder;

		public static readonly float delay = 1f;

		public static readonly float amount = 0.4f;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Charm.charm_super_builder);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Charm.charm_super_builder);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Charm.charm_super_builder);
			}
		}
	}

	public static class LevelSuperBeam
	{
		public static readonly int value;

		public static readonly string iconPath = "Icons/equip_icon_super_beam";

		public static readonly Super id = Super.level_super_beam;

		public static readonly float time = 1.25f;

		public static readonly float damage = 14.5f;

		public static readonly float damageRate = 0.25f;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Super.level_super_beam);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Super.level_super_beam);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Super.level_super_beam);
			}
		}
	}

	public static class LevelSuperGhost
	{
		public static readonly int value;

		public static readonly string iconPath = "Icons/equip_icon_super_ghost";

		public static readonly Super id = Super.level_super_ghost;

		public static readonly float initialSpeed = 700f;

		public static readonly float maxSpeed = 1250f;

		public static readonly float initialSpeedTime = 2f;

		public static readonly float maxSpeedTime = 4.2f;

		public static readonly float noHeartMaxSpeedTime = 4f;

		public static readonly float accelerationTime = 1f;

		public static readonly float heartSpeed = 100f;

		public static readonly float damage = 4f;

		public static readonly float damageRate = 0.25f;

		public static readonly float turnaroundEaseMultiplier = 4f;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Super.level_super_ghost);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Super.level_super_ghost);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Super.level_super_ghost);
			}
		}
	}

	public static class LevelSuperInvincibility
	{
		public static readonly int value;

		public static readonly string iconPath = "Icons/equip_icon_super_invincible";

		public static readonly Super id = Super.level_super_invincible;

		public static readonly float durationInvincible = 5.4f;

		public static readonly float durationFX = 5.1f;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Super.level_super_invincible);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Super.level_super_invincible);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Super.level_super_invincible);
			}
		}
	}

	public static class LevelWeaponArc
	{
		public static class Basic
		{
			public static readonly int Movement;

			public static readonly float launchSpeed = 1600f;

			public static readonly float gravity = 2750f;

			public static readonly float straightShotAngle = 65f;

			public static readonly float fireRate = 0.4f;

			public static readonly bool rapidFire = true;

			public static readonly int maxNumMines = 1;

			public static readonly float baseDamage = 14f;

			public static readonly float timeStateTwo = 1.25f;

			public static readonly float damageStateTwo = 7.5f;

			public static readonly float timeStateThree = 2.5f;

			public static readonly float damageStateThree = 11.25f;

			public static readonly float diagLaunchSpeed = 600f;

			public static readonly float diagGravity = 1000f;

			public static readonly float diagShotAngle = 45f;
		}

		public static class Ex
		{
			public static readonly float launchSpeed = 1600f;

			public static readonly float gravity = 2750f;

			public static readonly float damage = 28f;

			public static readonly float explodeDelay = 2f;
		}

		public static readonly int value = 2;

		public static readonly string iconPath = "Icons/equip_icon_weapon_peashot";

		public static readonly Weapon id = Weapon.level_weapon_arc;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_arc);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_arc);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_arc);
			}
		}
	}

	public static class LevelWeaponBoomerang
	{
		public static class Basic
		{
			public static readonly float fireRate = 0.25f;

			public static readonly float speed = 1400f;

			public static readonly float damage = 8.5f;

			public static readonly string xDistanceString = "550,450,520,480";

			public static readonly string yDistanceString = "100,  50,  80, 70";
		}

		public static class Ex
		{
			public static readonly float speed = 1000f;

			public static readonly float damage = 5f;

			public static readonly float damageRate = 0.2f;

			public static readonly float maxDamage = 35f;

			public static readonly float xDistance = 400f;

			public static readonly float yDistance = 110f;

			public static readonly string pinkString = "2,3,2,4";

			public static readonly float hitFreezeTime = 0.1f;
		}

		public static readonly int value = 4;

		public static readonly string iconPath = "Icons/equip_icon_weapon_boomerang";

		public static readonly Weapon id = Weapon.level_weapon_boomerang;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_boomerang);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_boomerang);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_boomerang);
			}
		}
	}

	public static class LevelWeaponBouncer
	{
		public static class Basic
		{
			public static readonly float launchSpeed = 1200f;

			public static readonly float gravity = 3600f;

			public static readonly float bounceRatio = 1.3f;

			public static readonly float bounceSpeedDampening = 800f;

			public static readonly float straightExtraAngle = 22.5f;

			public static readonly float diagonalUpExtraAngle;

			public static readonly float diagonalDownExtraAngle = 10f;

			public static readonly float damage = 11.6f;

			public static readonly float fireRate = 0.33f;

			public static readonly int numBounces = 2;
		}

		public static class Ex
		{
			public static readonly float launchSpeed = 1600f;

			public static readonly float gravity = 2750f;

			public static readonly float damage = 28f;

			public static readonly float explodeDelay = 2f;
		}

		public static readonly int value = 4;

		public static readonly string iconPath = "Icons/equip_icon_weapon_bouncer";

		public static readonly Weapon id = Weapon.level_weapon_bouncer;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_bouncer);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_bouncer);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_bouncer);
			}
		}
	}

	public static class LevelWeaponCharge
	{
		public static class Basic
		{
			public static readonly float fireRate = 0.25f;

			public static readonly float baseDamage = 6f;

			public static readonly float speed = 1050f;

			public static readonly float timeStateTwo = 9999f;

			public static readonly float damageStateTwo = 20f;

			public static readonly float speedStateTwo = 1300f;

			public static readonly float timeStateThree = 1f;

			public static readonly float damageStateThree = 52f;
		}

		public static class Ex
		{
			public static readonly float damage = 27f;

			public static readonly float radius = 300f;
		}

		public static readonly int value = 4;

		public static readonly string iconPath = "Icons/equip_icon_weapon_charge";

		public static readonly Weapon id = Weapon.level_weapon_charge;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_charge);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_charge);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_charge);
			}
		}
	}

	public static class LevelWeaponExploder
	{
		public static class Basic
		{
			public static readonly float fireRate = 0.3f;

			public static readonly bool rapideFire = true;

			public static readonly float speed = 800f;

			public static readonly float sinSpeed = 10f;

			public static readonly float sinSize = 2.5f;

			public static readonly float baseDamage = 8f;

			public static readonly float baseExplosionRadius = 15f;

			public static readonly float baseScale = 0.1f;

			public static readonly float timeStateTwo = 0.3f;

			public static readonly float damageStateTwo = 9.5f;

			public static readonly float explosionRadiusStateTwo = 70f;

			public static readonly float scaleStateTwo = 0.5f;

			public static readonly float timeStateThree = 0.6f;

			public static readonly float damageStateThree = 10.5f;

			public static readonly float explosionRadiusStateThree = 130f;

			public static readonly float scaleStateThree = 1f;
		}

		public static class Ex
		{
			public static readonly float speed = 1200f;

			public static readonly float damage = 35f;

			public static readonly float hitRate;

			public static readonly float explodeRadius = 300f;

			public static readonly float shrapnelSpeed = 1450f;
		}

		public static readonly int value = 2;

		public static readonly string iconPath = "Icons/";

		public static readonly Weapon id = Weapon.level_weapon_exploder;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_exploder);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_exploder);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_exploder);
			}
		}
	}

	public static class LevelWeaponHoming
	{
		public static class Basic
		{
			public static readonly MinMax fireRate = new MinMax(0.15f, 0.15f);

			public static readonly float speed = 1000f;

			public static readonly float damage = 2.6f;

			public static readonly MinMax rotationSpeed = new MinMax(0f, 500f);

			public static readonly float timeBeforeEaseRotationSpeed = 0f;

			public static readonly float rotationSpeedEaseTime = 0.4f;

			public static readonly float lockedShotAccelerationTime = 0.5f;

			public static readonly float speedVariation = 100f;

			public static readonly float angleVariation = 5f;

			public static readonly int trailFrameDelay = 2;

			public static readonly float maxHomingTime = 2.5f;
		}

		public static class Ex
		{
			public static readonly float speed = 1500f;

			public static readonly float damage = 6f;

			public static readonly float spread = 90f;

			public static readonly int bulletCount = 4;

			public static readonly float swirlDistance = 100f;

			public static readonly float swirlEaseTime = 0.75f;

			public static readonly int trailFrameDelay = 2;
		}

		public static readonly int value = 4;

		public static readonly string iconPath = "Icons/equip_icon_weapon_homing";

		public static readonly Weapon id = Weapon.level_weapon_homing;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_homing);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_homing);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_homing);
			}
		}
	}

	public static class LevelWeaponPeashot
	{
		public static class Basic
		{
			public static readonly float damage = 4f;

			public static readonly float speed = 2250f;

			public static readonly bool rapidFire = true;

			public static readonly float rapidFireRate = 0.11f;
		}

		public static class Ex
		{
			public static readonly float damage = 8.334f;

			public static readonly float maxDamage = 25f;

			public static readonly float damageDistance = 80f;

			public static readonly float speed = 1500f;

			public static readonly float freezeTime = 0.05f;
		}

		public static readonly int value = 2;

		public static readonly string iconPath = "Icons/equip_icon_weapon_peashot";

		public static readonly Weapon id = Weapon.level_weapon_peashot;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_peashot);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_peashot);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_peashot);
			}
		}
	}

	public static class LevelWeaponSpreadshot
	{
		public static class Basic
		{
			public static readonly float damage = 1.24f;

			public static readonly float speed = 2250f;

			public static readonly float distance = 375f;

			public static readonly float rapidFireRate = 0.13f;
		}

		public static class Ex
		{
			public static readonly float damage = 4.5f;

			public static readonly float speed = 500f;

			public static readonly int childCount = 8;

			public static readonly float radius = 100f;
		}

		public static readonly int value = 4;

		public static readonly string iconPath = "Icons/equip_icon_weapon_spread";

		public static readonly Weapon id = Weapon.level_weapon_spreadshot;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_spreadshot);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_spreadshot);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_spreadshot);
			}
		}
	}

	public static class LevelWeaponWideShot
	{
		public static class Basic
		{
			public static readonly float damage = 2.65f;

			public static readonly float speed = 1700f;

			public static readonly float distance = 2000f;

			public static readonly float rapidFireRate = 0.22f;

			public static readonly MinMax angleRange = new MinMax(30f, 5f);

			public static readonly float closingAngleSpeed = 1.3f;

			public static readonly float openingAngleSpeed = 1f;

			public static readonly float projectileSpeed = 300f;
		}

		public static class Ex
		{
		}

		public static readonly int value = 4;

		public static readonly string iconPath = "Icons/equip_icon_weapon_wideshot";

		public static readonly Weapon id = Weapon.level_weapon_wide_shot;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.level_weapon_wide_shot);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.level_weapon_wide_shot);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.level_weapon_wide_shot);
			}
		}
	}

	public static class PlaneSuperBomb
	{
		public static readonly int value = 10;

		public static readonly string iconPath = "Icons/";

		public static readonly Super id = Super.plane_super_bomb;

		public static readonly float damage = 38f;

		public static readonly float damageRate = 0.25f;

		public static readonly float countdownTime = 3f;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Super.plane_super_bomb);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Super.plane_super_bomb);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Super.plane_super_bomb);
			}
		}
	}

	public static class PlaneWeaponBomb
	{
		public static class Basic
		{
			public static readonly float damage = 11.5f;

			public static readonly float speed = 1200f;

			public static readonly bool Up;

			public static readonly float sizeExplosion = 1f;

			public static readonly float size = 1f;

			public static readonly float angle = 45f;

			public static readonly float gravity = 4500f;

			public static readonly bool rapidFire = true;

			public static readonly float rapidFireRate = 0.6f;
		}

		public static class Ex
		{
			public static readonly float damage = 6f;

			public static readonly float speed = 700f;

			public static readonly float[] angles = new float[2] { 180f, 170f };

			public static readonly int[] counts = new int[2] { 6, 3 };

			public static readonly MinMax rotationSpeed = new MinMax(0f, 250f);

			public static readonly float timeBeforeEaseRotationSpeed = 0f;

			public static readonly float rotationSpeedEaseTime = 1f;

			public static readonly float maxHomingTime = 2.5f;
		}

		public static readonly int value = 2;

		public static readonly string iconPath = "Icons/";

		public static readonly Weapon id = Weapon.plane_weapon_bomb;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.plane_weapon_bomb);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.plane_weapon_bomb);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.plane_weapon_bomb);
			}
		}
	}

	public static class PlaneWeaponLaser
	{
		public static class Basic
		{
			public static readonly float damage = 8f;

			public static readonly float speed = 4000f;

			public static readonly bool rapidFire = true;

			public static readonly float rapidFireRate = 0.1f;
		}

		public static class Ex
		{
			public static readonly float damage = 3f;

			public static readonly float speed = 2000f;

			public static readonly float[] angles = new float[2] { 180f, 170f };

			public static readonly int[] counts = new int[2] { 12, 6 };
		}

		public static readonly int value = 2;

		public static readonly string iconPath = "Icons/";

		public static readonly Weapon id = Weapon.plane_weapon_laser;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.plane_weapon_laser);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.plane_weapon_laser);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.plane_weapon_laser);
			}
		}
	}

	public static class PlaneWeaponPeashot
	{
		public static class Basic
		{
			public static readonly float damage = 4f;

			public static readonly float speed = 1800f;

			public static readonly bool rapidFire = true;

			public static readonly float rapidFireRate = 0.07f;
		}

		public static class Ex
		{
			public static readonly float damage = 15f;

			public static readonly float damageDistance = 100f;

			public static readonly float acceleration = 2500f;

			public static readonly float maxSpeed = 1500f;

			public static readonly float freezeTime = 0.125f;
		}

		public static readonly int value = 2;

		public static readonly string iconPath = "Icons/equip_icon_weapon_peashot";

		public static readonly Weapon id = Weapon.plane_weapon_peashot;

		public static string displayName
		{
			get
			{
				return GetDisplayName(Weapon.plane_weapon_peashot);
			}
		}

		public static string subtext
		{
			get
			{
				return GetSubtext(Weapon.plane_weapon_peashot);
			}
		}

		public static string description
		{
			get
			{
				return GetDescription(Weapon.plane_weapon_peashot);
			}
		}
	}

	public static string GetDisplayName(Weapon weapon)
	{
		TranslationElement translationElement = Localization.Find(weapon.ToString() + "_name");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static string GetDisplayName(Super super)
	{
		TranslationElement translationElement = Localization.Find(super.ToString() + "_name");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static string GetDisplayName(Charm charm)
	{
		TranslationElement translationElement = Localization.Find(charm.ToString() + "_name");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static string GetSubtext(Weapon weapon)
	{
		TranslationElement translationElement = Localization.Find(weapon.ToString() + "_subtext");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static string GetSubtext(Super super)
	{
		TranslationElement translationElement = Localization.Find(super.ToString() + "_subtext");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static string GetSubtext(Charm charm)
	{
		TranslationElement translationElement = Localization.Find(charm.ToString() + "_subtext");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static string GetIconPath(Weapon weapon)
	{
		switch (weapon)
		{
		case Weapon.level_weapon_peashot:
			return "Icons/equip_icon_weapon_peashot";
		case Weapon.level_weapon_spreadshot:
			return "Icons/equip_icon_weapon_spread";
		case Weapon.level_weapon_arc:
			return "Icons/equip_icon_weapon_peashot";
		case Weapon.level_weapon_homing:
			return "Icons/equip_icon_weapon_homing";
		case Weapon.level_weapon_exploder:
			return "Icons/";
		case Weapon.level_weapon_charge:
			return "Icons/equip_icon_weapon_charge";
		case Weapon.level_weapon_boomerang:
			return "Icons/equip_icon_weapon_boomerang";
		case Weapon.level_weapon_bouncer:
			return "Icons/equip_icon_weapon_bouncer";
		case Weapon.level_weapon_wide_shot:
			return "Icons/equip_icon_weapon_wideshot";
		case Weapon.plane_weapon_peashot:
			return "Icons/equip_icon_weapon_peashot";
		case Weapon.plane_weapon_laser:
			return "Icons/";
		case Weapon.plane_weapon_bomb:
			return "Icons/";
		case Weapon.arcade_weapon_peashot:
			return "Icons/";
		case Weapon.None:
			return "Icons/equip_icon_empty";
		default:
			Debug.LogWarning(string.Concat("Weapon '", weapon, "' not yet configured!"));
			return "ERROR";
		}
	}

	public static string GetIconPath(Super super)
	{
		switch (super)
		{
		case Super.level_super_beam:
			return "Icons/equip_icon_super_beam";
		case Super.level_super_ghost:
			return "Icons/equip_icon_super_ghost";
		case Super.level_super_invincible:
			return "Icons/equip_icon_super_invincible";
		case Super.plane_super_bomb:
			return "Icons/";
		case Super.None:
			return "Icons/equip_icon_empty";
		default:
			Debug.LogWarning(string.Concat("Super '", super, "' not yet configured!"));
			return "ERROR";
		}
	}

	public static string GetIconPath(Charm charm)
	{
		switch (charm)
		{
		case Charm.charm_health_up_1:
			return "Icons/equip_icon_charm_hp1";
		case Charm.charm_health_up_2:
			return "Icons/equip_icon_charm_hp2";
		case Charm.charm_super_builder:
			return "Icons/equip_icon_charm_coffee";
		case Charm.charm_smoke_dash:
			return "Icons/equip_icon_charm_smoke-dash";
		case Charm.charm_parry_plus:
			return "Icons/equip_icon_charm_parry_slapper";
		case Charm.charm_pit_saver:
			return "Icons/equip_icon_charm_pitsaver";
		case Charm.charm_parry_attack:
			return "Icons/equip_icon_charm_parry_attack";
		case Charm.None:
			return "Icons/equip_icon_empty";
		default:
			Debug.LogWarning(string.Concat("Charm '", charm, "' not yet configured!"));
			return "ERROR";
		}
	}

	public static string GetDescription(Weapon weapon)
	{
		TranslationElement translationElement = Localization.Find(weapon.ToString() + "_description");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static string GetDescription(Super super)
	{
		TranslationElement translationElement = Localization.Find(super.ToString() + "_description");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static string GetDescription(Charm charm)
	{
		TranslationElement translationElement = Localization.Find(charm.ToString() + "_description");
		if (translationElement == null)
		{
			return "ERROR";
		}
		return translationElement.translation.text;
	}

	public static int GetValue(Weapon weapon)
	{
		switch (weapon)
		{
		case Weapon.level_weapon_peashot:
			return 2;
		case Weapon.level_weapon_spreadshot:
			return 4;
		case Weapon.level_weapon_arc:
			return 2;
		case Weapon.level_weapon_homing:
			return 4;
		case Weapon.level_weapon_exploder:
			return 2;
		case Weapon.level_weapon_charge:
			return 4;
		case Weapon.level_weapon_boomerang:
			return 4;
		case Weapon.level_weapon_bouncer:
			return 4;
		case Weapon.level_weapon_wide_shot:
			return 4;
		case Weapon.plane_weapon_peashot:
			return 2;
		case Weapon.plane_weapon_laser:
			return 2;
		case Weapon.plane_weapon_bomb:
			return 2;
		case Weapon.arcade_weapon_peashot:
			return 2;
		default:
			Debug.LogWarning(string.Concat("Weapon '", weapon, "' not yet configured!"));
			return 0;
		}
	}

	public static int GetValue(Super super)
	{
		switch (super)
		{
		case Super.level_super_beam:
			return 0;
		case Super.level_super_ghost:
			return 0;
		case Super.level_super_invincible:
			return 0;
		case Super.plane_super_bomb:
			return 10;
		default:
			Debug.LogWarning(string.Concat("Super '", super, "' not yet configured!"));
			return 0;
		}
	}

	public static int GetValue(Charm charm)
	{
		switch (charm)
		{
		case Charm.charm_health_up_1:
			return 3;
		case Charm.charm_health_up_2:
			return 5;
		case Charm.charm_super_builder:
			return 3;
		case Charm.charm_smoke_dash:
			return 3;
		case Charm.charm_parry_plus:
			return 3;
		case Charm.charm_pit_saver:
			return 3;
		case Charm.charm_parry_attack:
			return 3;
		default:
			Debug.LogWarning(string.Concat("Charm '", charm, "' not yet configured!"));
			return 0;
		}
	}
}
