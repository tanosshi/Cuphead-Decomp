using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ArcadePlayerWeaponManager : AbstractArcadePlayerComponent
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
			if (pose == Pose.Jump)
			{
				return new Vector2(0f, 105f);
			}
			return new Vector2(4f, 115f);
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
		private ArcadeWeaponPeashot peashot;

		private PlayerId player;

		private Transform root;

		private ArcadePlayerWeaponManager weaponManager;

		private Dictionary<Weapon, AbstractArcadeWeapon> weapons;

		public void Init(ArcadePlayerWeaponManager weaponManager, Transform root)
		{
			this.weaponManager = weaponManager;
			this.root = root;
			player = weaponManager.player.id;
			weapons = new Dictionary<Weapon, AbstractArcadeWeapon>();
			InitWeapon(Weapon.arcade_weapon_peashot);
		}

		public AbstractArcadeWeapon GetWeapon(Weapon weapon)
		{
			return weapons[weapon];
		}

		private void InitWeapon(Weapon id)
		{
			AbstractArcadeWeapon abstractArcadeWeapon = null;
			abstractArcadeWeapon = peashot;
			AbstractArcadeWeapon abstractArcadeWeapon2 = UnityEngine.Object.Instantiate(abstractArcadeWeapon);
			abstractArcadeWeapon2.transform.parent = root.transform;
			abstractArcadeWeapon2.Initialize(weaponManager, id);
			abstractArcadeWeapon2.name = abstractArcadeWeapon2.name.Replace("(Clone)", string.Empty);
			weapons[id] = abstractArcadeWeapon2;
		}
	}

	[Serializable]
	public class SuperPrefabs
	{
		public void Init(ArcadePlayerController player)
		{
		}

		public AbstractPlayerSuper GetPrefab(Super super)
		{
			return null;
		}
	}

	[SerializeField]
	private WeaponPrefabs weaponPrefabs;

	[SerializeField]
	private SuperPrefabs superPrefabs;

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

	public bool shotBullet { get; set; }

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

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.player.levelStarted && allowInput)
		{
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
		if (ex.firing)
		{
			ex.firing = false;
		}
	}

	public void ParrySuccess()
	{
	}

	public void LevelInit(PlayerId id)
	{
		currentWeapon = Weapon.arcade_weapon_peashot;
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
			transform3.AddPositionForward2D(-100f);
		}
		UnityEngine.Object.Destroy(transform.gameObject);
	}

	private void StartSuper()
	{
	}

	private void EndSuper()
	{
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
		if (base.player.input.actions.GetButtonDown(4))
		{
			if (base.player.stats.SuperMeter >= base.player.stats.SuperMeterMax)
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

	private Pose GetCurrentPose()
	{
		if (ex.firing)
		{
			return Pose.Ex;
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
		aim.SetEulerAngles(0f, 0f, 90f);
	}

	public Vector2 GetBulletPosition()
	{
		Vector2 vector = base.transform.position;
		Vector2 vector2 = ProjectilePosition.Get(GetCurrentPose(), GetDirectionPose());
		return new Vector2(vector.x + vector2.x * (float)base.player.motor.TrueLookDirection.x, vector.y + vector2.y);
	}

	public float GetBulletRotation()
	{
		Pose pose = GetCurrentPose();
		return aim.eulerAngles.z;
	}

	public Vector3 GetBulletScale()
	{
		return new Vector3(1f, base.player.motor.TrueLookDirection.x, 1f);
	}
}
