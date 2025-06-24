using System;
using System.Collections;
using UnityEngine;

public class PlanePlayerWeaponManager : AbstractPlanePlayerComponent
{
	public enum State
	{
		Inactive = 0,
		Ready = 1,
		Busy = 2
	}

	public delegate void OnWeaponChangeHandler(Weapon weapon);

	[Serializable]
	public class Weapons
	{
		public AbstractPlaneWeapon peashot;

		public AbstractPlaneWeapon laser;

		public AbstractPlaneWeapon bomb;

		public void Init(PlanePlayerWeaponManager manager)
		{
			peashot = UnityEngine.Object.Instantiate(peashot);
			peashot.Initialize(manager, 0);
			peashot.transform.SetParent(manager.transform);
			peashot.transform.ResetLocalTransforms();
			laser = UnityEngine.Object.Instantiate(laser);
			laser.Initialize(manager, 1);
			laser.transform.SetParent(manager.transform);
			laser.transform.ResetLocalTransforms();
			bomb = UnityEngine.Object.Instantiate(bomb);
			bomb.Initialize(manager, 2);
			bomb.transform.SetParent(manager.transform);
			bomb.transform.ResetLocalTransforms();
		}

		public AbstractPlaneWeapon GetWeapon(Weapon weapon)
		{
			switch (weapon)
			{
			case Weapon.plane_weapon_peashot:
				return peashot;
			case Weapon.plane_weapon_laser:
				return laser;
			case Weapon.plane_weapon_bomb:
				return bomb;
			default:
				return null;
			}
		}
	}

	public class States
	{
		public enum Basic
		{
			Ready = 0
		}

		public enum Ex
		{
			Ready = 0,
			Intro = 1,
			Fire = 2,
			Ending = 3
		}

		public enum Super
		{
			Ready = 0,
			Intro = 1,
			Countdown = 2,
			Ending = 3
		}

		public Super super;

		public Basic basic { get; internal set; }

		public Ex ex { get; internal set; }

		public States()
		{
			basic = Basic.Ready;
			ex = Ex.Ready;
			super = Super.Ready;
		}
	}

	[SerializeField]
	private Weapons weapons;

	[SerializeField]
	private AbstractPlaneSuper super;

	[Space(10f)]
	[SerializeField]
	private Transform bulletRoot;

	[NonSerialized]
	public bool IsShooting;

	private Weapon currentWeapon = Weapon.plane_weapon_peashot;

	private bool usingShrinkWeapon;

	public State state { get; protected set; }

	public States states { get; private set; }

	public bool CanInterupt { get; private set; }

	public event OnWeaponChangeHandler OnWeaponChangeEvent;

	public event Action OnExStartEvent;

	public event Action OnExFireEvent;

	public event Action OnSuperStartEvent;

	public event Action OnSuperCountdownEvent;

	public event Action OnSuperFireEvent;

	protected override void Start()
	{
		base.Start();
		weapons.Init(this);
		states = new States();
		base.player.animationController.OnExFireAnimEvent += OnExAnimFire;
		base.player.OnReviveEvent += OnRevive;
		base.player.stats.OnPlayerDeathEvent += StopSound;
		CanInterupt = true;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (state != State.Inactive && state != State.Busy)
		{
			CheckBasic();
			CheckEx();
			HandleWeaponSwitch();
		}
	}

	public override void OnLevelStart()
	{
		base.OnLevelStart();
		if (!(base.player.stats.StoneTime > 0f))
		{
			state = State.Ready;
			if (base.player.input.actions.GetButton(3))
			{
				StartBasic();
			}
		}
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
		EndBasic();
	}

	public void SwitchToWeapon(Weapon weapon)
	{
		Debug.Log("Switch Weapon: " + weapon);
		if (weapon == Weapon.None)
		{
			Debug.Log("Weapon is 'None', not switching");
			return;
		}
		weapons.GetWeapon(currentWeapon).EndBasic();
		weapons.GetWeapon(currentWeapon).EndEx();
		if (this.OnWeaponChangeEvent != null)
		{
			this.OnWeaponChangeEvent(weapon);
		}
		currentWeapon = weapon;
		if (base.player.input.actions.GetButton(3))
		{
			StartBasic();
		}
	}

	private void OnRevive(Vector3 pos)
	{
		IsShooting = false;
		state = State.Ready;
		states.basic = States.Basic.Ready;
		states.ex = States.Ex.Ready;
		CanInterupt = true;
	}

	private void CheckBasic()
	{
		if ((base.player.input.actions.GetButtonDown(3) || (base.player.input.actions.GetButtonTimePressed(3) > 0f && !IsShooting)) && base.player.stats.StoneTime <= 0f)
		{
			StartBasic();
		}
		else if (base.player.input.actions.GetButtonUp(3) || (IsShooting && base.player.stats.StoneTime > 0f))
		{
			EndBasic();
		}
		else if (!base.player.input.actions.GetButton(3) && IsShooting)
		{
			EndBasic();
		}
		else if ((!base.player.Shrunk && usingShrinkWeapon) || (base.player.Shrunk && !usingShrinkWeapon))
		{
			EndBasic();
			if (base.player.input.actions.GetButton(3))
			{
				StartBasic();
			}
		}
	}

	private void StartBasic()
	{
		if (currentWeapon == Weapon.plane_weapon_bomb && base.player.Shrunk)
		{
			usingShrinkWeapon = true;
			weapons.GetWeapon(Weapon.plane_weapon_peashot).BeginBasic();
		}
		else
		{
			weapons.GetWeapon(currentWeapon).BeginBasic();
		}
	}

	private void EndBasic()
	{
		if (currentWeapon == Weapon.plane_weapon_bomb && usingShrinkWeapon)
		{
			usingShrinkWeapon = false;
			weapons.GetWeapon(Weapon.plane_weapon_peashot).EndBasic();
		}
		else
		{
			weapons.GetWeapon(currentWeapon).EndBasic();
		}
		StopSound(base.player.id);
	}

	private void StopSound(PlayerId id)
	{
		if (id.ToString() == PlayerId.PlayerOne.ToString())
		{
			if (AudioManager.CheckIfPlaying("player_plane_weapon_fire_loop_cuphead"))
			{
				AudioManager.Stop("player_plane_weapon_fire_loop_cuphead");
				AudioManager.Play("player_plane_weapon_fire_loop_end_cuphead");
				emitAudioFromObject.Add("player_plane_weapon_fire_loop_end_cuphead");
			}
		}
		else if (AudioManager.CheckIfPlaying("player_plane_weapon_fire_loop_mugman"))
		{
			AudioManager.Stop("player_plane_weapon_fire_loop_mugman");
			AudioManager.Play("player_plane_weapon_fire_loop_end_mugman");
			emitAudioFromObject.Add("player_plane_weapon_fire_loop_end_mugman");
		}
	}

	private void CheckEx()
	{
		if (base.player.stats.CanUseEx && !base.player.Parrying && !base.player.Shrunk && !(base.player.stats.StoneTime > 0f) && (base.player.input.actions.GetButtonDown(4) || base.player.motor.HasBufferedInput(PlanePlayerMotor.BufferedInput.Super)))
		{
			if (base.player.stats.SuperMeter >= base.player.stats.SuperMeterMax)
			{
				StartSuper();
			}
			else
			{
				StartEx();
			}
			base.player.motor.ClearBufferedInput();
		}
	}

	private void StartEx()
	{
		Debug.Log("START_EX");
		StartCoroutine(ex_cr());
	}

	private void OnExAnimFire()
	{
		states.ex = States.Ex.Fire;
	}

	private IEnumerator ex_cr()
	{
		AudioManager.Play("player_plane_weapon_special_fire");
		state = State.Inactive;
		states.ex = States.Ex.Intro;
		CanInterupt = false;
		EndBasic();
		base.player.stats.OnEx();
		if (this.OnExStartEvent != null)
		{
			this.OnExStartEvent();
		}
		while (states.ex != States.Ex.Fire)
		{
			if (base.player.stats.StoneTime > 0f)
			{
				CancelEX();
				yield return null;
			}
			yield return null;
		}
		weapons.GetWeapon(currentWeapon).BeginEx();
		if (this.OnExFireEvent != null)
		{
			this.OnExFireEvent();
		}
		AudioManager.Play("player_plane_up_ex_end");
		states.ex = States.Ex.Ending;
		while (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Ex_End"))
		{
			if (base.player.stats.StoneTime > 0f)
			{
				CancelEX();
				yield return null;
			}
			yield return null;
		}
		state = State.Ready;
		states.ex = States.Ex.Ready;
		if (base.player.input.actions.GetButtonDown(3))
		{
			StartBasic();
		}
		CanInterupt = true;
	}

	private void CancelEX()
	{
		StopCoroutine(ex_cr());
		if (base.player.input.actions.GetButtonDown(3))
		{
			StartBasic();
		}
		CanInterupt = true;
	}

	public void StartSuper()
	{
		StartCoroutine(super_cr());
	}

	private IEnumerator super_cr()
	{
		state = State.Inactive;
		states.super = States.Super.Ready;
		CanInterupt = false;
		EndBasic();
		base.player.stats.OnSuper();
		AbstractPlaneSuper s = super.Create(base.player);
		if (this.OnSuperStartEvent != null)
		{
			this.OnSuperStartEvent();
		}
		while (states.super != States.Super.Ending && states.super != States.Super.Countdown)
		{
			states.super = s.State;
			yield return null;
		}
		if (this.OnSuperCountdownEvent != null)
		{
			this.OnSuperCountdownEvent();
		}
		while (states.super != States.Super.Ending)
		{
			states.super = s.State;
			yield return null;
		}
		if (this.OnSuperFireEvent != null)
		{
			this.OnSuperFireEvent();
		}
		base.player.stats.OnSuperEnd();
		state = State.Ready;
		states.super = States.Super.Ready;
		if (base.player.input.actions.GetButtonDown(3))
		{
			StartBasic();
		}
		CanInterupt = true;
	}

	public Vector2 GetBulletPosition()
	{
		return bulletRoot.position;
	}

	private void HandleWeaponSwitch()
	{
		if (base.player.input.actions.GetButtonDown(5) && PlayerData.Data.IsUnlocked(base.player.id, Weapon.plane_weapon_bomb))
		{
			if (currentWeapon == Weapon.plane_weapon_peashot)
			{
				SwitchWeapon(Weapon.plane_weapon_bomb);
			}
			else
			{
				SwitchWeapon(Weapon.plane_weapon_peashot);
			}
		}
	}

	private void SwitchWeapon(Weapon weapon)
	{
		EndBasic();
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
}
