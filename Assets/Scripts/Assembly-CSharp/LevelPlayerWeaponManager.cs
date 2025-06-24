using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LevelPlayerWeaponManager : AbstractLevelPlayerComponent
{
	public enum Pose
	{
		Forward = 0,
		Forward_R = 1,
		Up = 2,
		Up_D = 3,
		Up_D_R = 4,
		Down = 5,
		Down_D = 6,
		Duck = 7,
		Jump = 8,
		Ex = 9
	}

	public delegate void OnWeaponChangeHandler(Weapon weapon);

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ProjectilePosition
	{
		public static Vector2 Get(Pose pose, Pose direction)
		{
			switch (pose)
			{
			case Pose.Jump:
				switch (direction)
				{
				case Pose.Forward:
					return new Vector2(78f, 64f);
				case Pose.Up:
					return new Vector2(0f, 158f);
				case Pose.Up_D:
					return new Vector2(71f, 107f);
				case Pose.Down:
					return new Vector2(0f, -11f);
				case Pose.Down_D:
					return new Vector2(71f, 20f);
				default:
					return new Vector2(0f, 0f);
				}
			case Pose.Forward:
				return new Vector2(78f, 64f);
			case Pose.Forward_R:
				return new Vector2(70f, 46f);
			case Pose.Up:
				return new Vector2(27f, 158f);
			case Pose.Up_D:
				return new Vector2(71f, 107f);
			case Pose.Up_D_R:
				return new Vector2(73f, 107f);
			case Pose.Down:
				return new Vector2(28f, -11f);
			case Pose.Down_D:
				return new Vector2(71f, 20f);
			case Pose.Duck:
				return new Vector2(102f, 24f);
			default:
				return new Vector2(0f, 54f);
			}
		}
	}

	public class WeaponState
	{
		public enum State
		{
			Ready = 0,
			Firing = 1,
			Fired = 2,
			Ended = 3
		}

		public State state;

		public bool firing;

		public bool holding;
	}

	public class ExState
	{
		public bool airAble = true;

		public bool firing;

		public bool Able
		{
			get
			{
				return airAble && !firing;
			}
		}
	}

	[Serializable]
	public class WeaponPrefabs
	{
		[SerializeField]
		private WeaponPeashot peashot;

		[SerializeField]
		private WeaponSpread spread;

		[SerializeField]
		private WeaponArc arc;

		[SerializeField]
		private WeaponHoming homing;

		[SerializeField]
		private WeaponExploder exploder;

		[SerializeField]
		private WeaponCharge charge;

		[SerializeField]
		private WeaponBoomerang boomerang;

		[SerializeField]
		private WeaponBouncer bouncer;

		[SerializeField]
		private WeaponWideShot wideShot;

		private PlayerId player;

		private Transform root;

		private LevelPlayerWeaponManager weaponManager;

		private Dictionary<Weapon, AbstractLevelWeapon> weapons;

		public void Init(LevelPlayerWeaponManager weaponManager, Transform root)
		{
			this.weaponManager = weaponManager;
			this.root = root;
			player = weaponManager.player.id;
			weapons = new Dictionary<Weapon, AbstractLevelWeapon>();
			Weapon[] values = EnumUtils.GetValues<Weapon>();
			for (int i = 0; i < values.Length; i++)
			{
				Weapon id = values[i];
				if (id.ToString().ToLower().Contains("level"))
				{
					InitWeapon(id);
				}
			}
		}

		public AbstractLevelWeapon GetWeapon(Weapon weapon)
		{
			return weapons[weapon];
		}

		private void InitWeapon(Weapon id)
		{
			AbstractLevelWeapon abstractLevelWeapon = null;
			switch (id)
			{
			case Weapon.level_weapon_peashot:
				abstractLevelWeapon = peashot;
				break;
			case Weapon.level_weapon_spreadshot:
				abstractLevelWeapon = spread;
				break;
			case Weapon.level_weapon_arc:
				abstractLevelWeapon = arc;
				break;
			case Weapon.level_weapon_homing:
				abstractLevelWeapon = homing;
				break;
			case Weapon.level_weapon_exploder:
				abstractLevelWeapon = exploder;
				break;
			case Weapon.level_weapon_charge:
				abstractLevelWeapon = charge;
				break;
			case Weapon.level_weapon_boomerang:
				abstractLevelWeapon = boomerang;
				break;
			case Weapon.level_weapon_bouncer:
				abstractLevelWeapon = bouncer;
				break;
			case Weapon.level_weapon_wide_shot:
				abstractLevelWeapon = wideShot;
				break;
			case Weapon.None:
				return;
			default:
				Debug.LogWarning(string.Concat("Weapon '", id, "' is not configured!"));
				return;
			}
			AbstractLevelWeapon abstractLevelWeapon2 = UnityEngine.Object.Instantiate(abstractLevelWeapon);
			abstractLevelWeapon2.transform.parent = root.transform;
			abstractLevelWeapon2.Initialize(weaponManager, id);
			abstractLevelWeapon2.name = abstractLevelWeapon2.name.Replace("(Clone)", string.Empty);
			weapons[id] = abstractLevelWeapon2;
		}
	}

	[Serializable]
	public class SuperPrefabs
	{
		[SerializeField]
		private AbstractPlayerSuper beam;

		[SerializeField]
		private AbstractPlayerSuper ghost;

		[SerializeField]
		private AbstractPlayerSuper invincible;

		public void Init(LevelPlayerController player)
		{
		}

		public AbstractPlayerSuper GetPrefab(Super super)
		{
			switch (super)
			{
			default:
				return beam;
			case Super.level_super_ghost:
				return ghost;
			case Super.level_super_invincible:
				return invincible;
			}
		}
	}

	[SerializeField]
	private WeaponPrefabs weaponPrefabs;

	[SerializeField]
	private SuperPrefabs superPrefabs;

	[SerializeField]
	private CharmTurret turretPrefab;

	[Space(10f)]
	[SerializeField]
	private Effect exDustEffect;

	[SerializeField]
	private Effect exChargeEffect;

	[SerializeField]
	private Transform exRoot;

	private Weapon currentWeapon = Weapon.None;

	private Pose currentPose;

	private WeaponState basic;

	private ExState ex;

	private Transform weaponsRoot;

	private Transform aim;

	private bool allowInput = true;

	public bool IsShooting { get; set; }

	public bool FreezePosition { get; set; }

	public Vector2 ExPosition
	{
		get
		{
			return exRoot.position;
		}
	}

	public event OnWeaponChangeHandler OnWeaponChangeEvent;

	public event Action OnBasicStart;

	public event Action OnExStart;

	public event Action OnSuperStart;

	public event Action OnBasicFire;

	public event Action OnExFire;

	public event Action OnWeaponFire;

	public event Action OnBasicEnd;

	public event Action OnExEnd;

	public event Action OnSuperEnd;

	protected override void OnAwake()
	{
		base.OnAwake();
		base.basePlayer.damageReceiver.OnDamageTaken += OnDamageTaken;
		Level.Current.OnLevelEndEvent += OnLevelEnd;
		base.player.motor.OnDashStartEvent += OnDash;
		basic = new WeaponState();
		ex = new ExState();
		weaponsRoot = new GameObject("Weapons").transform;
		weaponsRoot.parent = base.transform;
		weaponsRoot.localPosition = Vector3.zero;
		weaponsRoot.localEulerAngles = Vector3.zero;
		weaponsRoot.localScale = Vector3.one;
		aim = new GameObject("Aim").transform;
		aim.SetParent(base.transform);
		aim.ResetLocalTransforms();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Level.Current != null)
		{
			Level.Current.OnLevelEndEvent -= OnLevelEnd;
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.player.levelStarted && allowInput)
		{
			HandleWeaponSwitch();
			HandleWeaponFiring();
			if (base.player.motor.Grounded)
			{
				ex.airAble = true;
			}
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		EnableInput();
	}

	public override void OnLevelEnd()
	{
		EndBasic();
		base.OnLevelEnd();
	}

	private void OnDash()
	{
		EndBasic();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (ex.firing && !base.player.stats.SuperInvincible)
		{
			ex.firing = false;
		}
	}

	public void ParrySuccess()
	{
	}

	public void LevelInit(PlayerId id)
	{
		PlayerData.PlayerLoadouts.PlayerLoadout playerLoadout = PlayerData.Data.Loadouts.GetPlayerLoadout(base.player.id);
		currentWeapon = playerLoadout.primaryWeapon;
		weaponPrefabs.Init(this, weaponsRoot);
		superPrefabs.Init(base.player);
	}

	public void EnableInput()
	{
		allowInput = true;
	}

	public void DisableInput()
	{
		Debug.Log("DisableInput");
		allowInput = false;
		IsShooting = false;
	}

	private void _WeaponFireEx()
	{
		FireEx();
	}

	private void _WeaponEndEx()
	{
		EndEx();
	}

	private void StartBasic()
	{
		UpdateAim();
		weaponPrefabs.GetWeapon(currentWeapon).BeginBasic();
		if (this.OnBasicStart != null)
		{
			this.OnBasicStart();
		}
	}

	private void EndBasic()
	{
		if (currentWeapon != Weapon.None)
		{
			weaponPrefabs.GetWeapon(currentWeapon).EndBasic();
			basic.firing = false;
		}
	}

	public void TriggerWeaponFire()
	{
		this.OnWeaponFire();
	}

	private void StartEx()
	{
		EndBasic();
		UpdateAim();
		ex.firing = true;
		ex.airAble = false;
		base.player.stats.OnEx();
		exChargeEffect.Create(base.player.center);
		if (this.OnExStart != null)
		{
			this.OnExStart();
		}
	}

	private void FireEx()
	{
		weaponPrefabs.GetWeapon(currentWeapon).BeginEx();
		if (this.OnExFire != null)
		{
			this.OnExFire();
		}
	}

	private void EndEx()
	{
		ex.firing = false;
		if (this.OnExEnd != null)
		{
			this.OnExEnd();
		}
	}

	public void CreateExDust(Effect starsEffect)
	{
		Transform transform = new GameObject("ExRootTemp").transform;
		transform.ResetLocalTransforms();
		transform.position = exRoot.position;
		Vector2 vector = transform.position;
		if (starsEffect != null)
		{
			Transform transform2 = starsEffect.Create(vector).transform;
			transform2.SetParent(transform);
			transform2.ResetLocalTransforms();
			transform2.SetParent(null);
			transform2.SetEulerAngles(0f, 0f, GetBulletRotation());
			transform2.localScale = GetBulletScale();
			transform2.AddPositionForward2D(-100f);
		}
		if (exDustEffect != null)
		{
			Transform transform3 = exDustEffect.Create(vector).transform;
			transform3.SetParent(transform);
			transform3.ResetLocalTransforms();
			transform3.SetParent(null);
			transform3.SetEulerAngles(0f, 0f, GetBulletRotation());
			transform3.localScale = GetBulletScale();
			transform3.AddPositionForward2D(-15f);
		}
		UnityEngine.Object.Destroy(transform.gameObject);
	}

	private void StartSuper()
	{
		EndBasic();
		UpdateAim();
		base.player.stats.OnSuper();
		Super super = PlayerData.Data.Loadouts.GetPlayerLoadout(base.player.id).super;
		AbstractPlayerSuper abstractPlayerSuper = superPrefabs.GetPrefab(super).Create(base.player);
		abstractPlayerSuper.OnEndedEvent += EndSuperFromSuper;
		if (this.OnSuperStart != null)
		{
			this.OnSuperStart();
		}
	}

	private void EndSuper()
	{
		if (this.OnSuperEnd != null)
		{
			this.OnSuperEnd();
		}
	}

	public void EndSuperFromSuper()
	{
		EndSuper();
	}

	private void HandleWeaponFiring()
	{
		if (base.player.motor.Dashing || base.player.motor.IsHit)
		{
			return;
		}
		if (base.player.input.actions.GetButtonDown(4) || base.player.motor.HasBufferedInput(LevelPlayerMotor.BufferedInput.Super))
		{
			base.player.motor.ClearBufferedInput();
			Super super = PlayerData.Data.Loadouts.GetPlayerLoadout(base.player.id).super;
			if (base.player.stats.SuperMeter >= base.player.stats.SuperMeterMax && super != Super.None)
			{
				StartSuper();
				return;
			}
			if (base.player.stats.CanUseEx && ex.Able)
			{
				StartEx();
				return;
			}
		}
		if (ex.firing)
		{
			return;
		}
		if (basic.firing != base.player.input.actions.GetButton(3))
		{
			if (base.player.input.actions.GetButton(3))
			{
				StartBasic();
			}
			else
			{
				EndBasic();
			}
		}
		basic.firing = base.player.input.actions.GetButton(3);
	}

	private void HandleWeaponSwitch()
	{
		if (!base.player.input.actions.GetButtonDown(5))
		{
			return;
		}
		PlayerData.PlayerLoadouts.PlayerLoadout playerLoadout = PlayerData.Data.Loadouts.GetPlayerLoadout(base.player.id);
		if (playerLoadout.secondaryWeapon != Weapon.None)
		{
			if (currentWeapon == playerLoadout.primaryWeapon)
			{
				SwitchWeapon(playerLoadout.secondaryWeapon);
			}
			else
			{
				SwitchWeapon(playerLoadout.primaryWeapon);
			}
		}
	}

	private void SwitchWeapon(Weapon weapon)
	{
		Debug.Log("Switch Weapon: " + weapon);
		if (weapon == Weapon.None)
		{
			Debug.Log("Weapon is 'None', not switching");
			return;
		}
		weaponPrefabs.GetWeapon(currentWeapon).EndBasic();
		weaponPrefabs.GetWeapon(currentWeapon).EndEx();
		currentWeapon = weapon;
		if (this.OnWeaponChangeEvent != null)
		{
			this.OnWeaponChangeEvent(weapon);
		}
		if (base.player.input.actions.GetButton(3))
		{
			StartBasic();
		}
	}

	private Pose GetCurrentPose()
	{
		if (ex.firing)
		{
			return Pose.Ex;
		}
		if (base.player.motor.Ducking)
		{
			return Pose.Duck;
		}
		if (!base.player.motor.Grounded)
		{
			return Pose.Jump;
		}
		if (base.player.motor.Locked)
		{
			if ((int)base.player.motor.LookDirection.y > 0)
			{
				if ((int)base.player.motor.LookDirection.x != 0)
				{
					return Pose.Up_D;
				}
				return Pose.Up;
			}
			if ((int)base.player.motor.LookDirection.y < 0)
			{
				if ((int)base.player.motor.LookDirection.x != 0)
				{
					return Pose.Down_D;
				}
				return Pose.Down;
			}
		}
		else
		{
			if ((int)base.player.motor.LookDirection.x != 0)
			{
				if ((int)base.player.motor.LookDirection.y > 0)
				{
					return Pose.Up_D_R;
				}
				return Pose.Forward_R;
			}
			if ((int)base.player.motor.LookDirection.y < 0)
			{
				return Pose.Duck;
			}
			if ((int)base.player.motor.LookDirection.y > 0)
			{
				return Pose.Up;
			}
		}
		return Pose.Forward;
	}

	public Pose GetDirectionPose()
	{
		if (base.player.motor.Dashing)
		{
			return Pose.Forward;
		}
		if ((int)base.player.motor.LookDirection.y > 0)
		{
			if ((int)base.player.motor.LookDirection.x != 0)
			{
				return Pose.Up_D;
			}
			return Pose.Up;
		}
		if ((int)base.player.motor.LookDirection.y < 0)
		{
			if ((int)base.player.motor.LookDirection.x != 0)
			{
				return Pose.Down_D;
			}
			return Pose.Down;
		}
		return Pose.Forward;
	}

	public void UpdateAim()
	{
		float num = 0f;
		Pose directionPose = GetDirectionPose();
		if (base.transform.localScale.x > 0f)
		{
			switch (directionPose)
			{
			default:
				num = 0f;
				break;
			case Pose.Up:
				num = 90f;
				break;
			case Pose.Down:
				num = -90f;
				break;
			case Pose.Up_D:
				num = 45f;
				break;
			case Pose.Down_D:
				num = -45f;
				break;
			}
		}
		else
		{
			switch (directionPose)
			{
			default:
				num = 180f;
				break;
			case Pose.Up:
				num = 90f;
				break;
			case Pose.Down:
				num = -90f;
				break;
			case Pose.Up_D:
				num = 135f;
				break;
			case Pose.Down_D:
				num = -135f;
				break;
			}
		}
		num *= base.player.motor.GravityReversalMultiplier;
		aim.SetEulerAngles(0f, 0f, num);
	}

	public Vector2 GetBulletPosition()
	{
		Vector2 vector = base.transform.position;
		Vector2 vector2 = ProjectilePosition.Get(GetCurrentPose(), GetDirectionPose());
		return new Vector2(vector.x + vector2.x * (float)base.player.motor.TrueLookDirection.x, vector.y + vector2.y * base.player.motor.GravityReversalMultiplier);
	}

	public float GetBulletRotation()
	{
		Pose pose = GetCurrentPose();
		if (pose == Pose.Duck)
		{
			if (base.transform.localScale.x < 0f)
			{
				return 180f;
			}
			return 0f;
		}
		return aim.eulerAngles.z;
	}

	public Vector3 GetBulletScale()
	{
		return new Vector3(1f, base.player.motor.TrueLookDirection.x, 1f);
	}
}
