using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractLevelWeapon : AbstractPausableComponent
{
	public enum Mode
	{
		Basic = 0,
		Ex = 1
	}

	private delegate AbstractProjectile FireProjectileDelegate();

	[Serializable]
	public class Prefabs
	{
		public AbstractProjectile basic;

		public AbstractProjectile ex;

		public AbstractProjectile Get(Mode mode)
		{
			switch (mode)
			{
			case Mode.Ex:
				return ex;
			default:
				return basic;
			}
		}
	}

	[Serializable]
	public class MuzzleEffects
	{
		public Effect basic;

		public Effect ex;

		public Effect Get(Mode mode)
		{
			switch (mode)
			{
			case Mode.Ex:
				return ex;
			default:
				return basic;
			}
		}
	}

	public class FiringSwitches
	{
		public bool basic;

		public bool ex;

		public bool Get(Mode mode)
		{
			switch (mode)
			{
			case Mode.Ex:
				return ex;
			default:
				return basic;
			}
		}

		public void Set(Mode mode, bool val)
		{
			switch (mode)
			{
			case Mode.Ex:
				ex = val;
				break;
			default:
				basic = val;
				break;
			}
		}
	}

	[Header("Ex")]
	[SerializeField]
	protected AbstractProjectile exPrefab;

	[SerializeField]
	protected Effect exEffectPrefab;

	[SerializeField]
	protected LevelPlayerWeaponFiringHitbox exFiringHitboxPrefab;

	[Header("Basic")]
	[SerializeField]
	protected AbstractProjectile basicPrefab;

	[SerializeField]
	protected Effect basicEffectPrefab;

	[SerializeField]
	protected LevelPlayerWeaponFiringHitbox basicFiringHitboxPrefab;

	protected FiringSwitches firing;

	protected LevelPlayerController player;

	protected LevelPlayerWeaponManager weaponManager;

	private Coroutine basicCoroutine;

	private Coroutine exCoroutine;

	private string WeaponSound;

	private bool isUsingLoop;

	public static bool ONE_PLAYER_FIRING { get; private set; }

	protected abstract bool rapidFire { get; }

	protected abstract float rapidFireRate { get; }

	public Weapon id { get; private set; }

	protected virtual bool isChargeWeapon
	{
		get
		{
			return false;
		}
	}

	protected override Transform emitTransform
	{
		get
		{
			return player.transform;
		}
	}

	public virtual void Initialize(LevelPlayerWeaponManager weaponManager, Weapon id)
	{
		this.weaponManager = weaponManager;
		player = weaponManager.GetComponent<LevelPlayerController>();
		this.id = id;
		firing = new FiringSwitches();
		StartCoroutines();
		player.OnReviveEvent += OnRevive;
	}

	public void OnDealDamage(float damage, DamageReceiver receiver, DamageDealer dealer)
	{
		if (!(player == null) && !player.IsDead && !(player.stats == null) && receiver.enabled)
		{
			player.stats.OnDealDamage(damage, dealer);
		}
	}

	private void OnRevive(Vector3 pos)
	{
		StartCoroutines();
	}

	private void StartCoroutines()
	{
		StopAllCoroutines();
		basicCoroutine = StartCoroutine((!isChargeWeapon) ? fireWeapon_cr(Mode.Basic) : chargeFireWeapon_cr(Mode.Basic));
		exCoroutine = StartCoroutine(fireWeapon_cr(Mode.Ex));
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		StartCoroutines();
	}

	protected override void Update()
	{
		base.Update();
		if (firing.Get(Mode.Basic) || firing.Get(Mode.Ex))
		{
			ONE_PLAYER_FIRING = true;
		}
		if (isUsingLoop && AudioManager.CheckIfPlaying(WeaponSound))
		{
			emitAudioFromObject.Add(WeaponSound);
		}
	}

	protected virtual void BasicSoundOneShot(string soundP1, string soundP2)
	{
		if (player.name == PlayerId.PlayerOne.ToString())
		{
			AudioManager.Play(soundP1);
			emitAudioFromObject.Add(soundP1);
		}
		else
		{
			AudioManager.Play(soundP2);
			emitAudioFromObject.Add(soundP2);
		}
	}

	protected virtual void BeginBasicCheckAttenuation(string soundP1, string soundP2)
	{
		if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
		{
			if (player.name == PlayerId.PlayerOne.ToString())
			{
				AudioManager.Attenuation(soundP1, ONE_PLAYER_FIRING, 0.1f);
			}
			else
			{
				AudioManager.Attenuation(soundP2, ONE_PLAYER_FIRING, 0.1f);
			}
		}
	}

	protected virtual void EndBasicCheckAttenuation(string soundP1, string soundP2)
	{
		if (!(PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null))
		{
			return;
		}
		if (player.name == PlayerId.PlayerOne.ToString())
		{
			if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
			{
				AudioManager.Attenuation(soundP2, ONE_PLAYER_FIRING, 0.1f);
				AudioManager.Attenuation(soundP1, false, 0.1f);
			}
		}
		else
		{
			AudioManager.Attenuation(soundP1, ONE_PLAYER_FIRING, 0.1f);
			AudioManager.Attenuation(soundP2, false, 0.1f);
		}
	}

	protected virtual void BasicSoundLoop(string loopP1, string loopP2)
	{
		if (player.name == PlayerId.PlayerOne.ToString())
		{
			WeaponSound = loopP1;
			AudioManager.PlayLoop(loopP1);
			AudioManager.Attenuation(loopP1, ONE_PLAYER_FIRING, 0.1f);
		}
		else
		{
			WeaponSound = loopP2;
			AudioManager.PlayLoop(loopP2);
			AudioManager.Attenuation(loopP2, ONE_PLAYER_FIRING, 0.1f);
		}
		isUsingLoop = true;
	}

	protected virtual void StopLoopSound(string loopP1, string loopP2)
	{
		if (player.name == PlayerId.PlayerOne.ToString())
		{
			AudioManager.Stop(loopP1);
			if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
			{
				AudioManager.Attenuation(loopP2, ONE_PLAYER_FIRING, 0.1f);
			}
		}
		else
		{
			AudioManager.Stop(loopP2);
			AudioManager.Attenuation(loopP1, ONE_PLAYER_FIRING, 0.1f);
		}
	}

	public virtual void BeginBasic()
	{
		beginFiring(Mode.Basic);
	}

	public virtual void EndBasic()
	{
		endFiring(Mode.Basic);
		ONE_PLAYER_FIRING = false;
	}

	protected virtual AbstractProjectile fireBasic()
	{
		AbstractProjectile abstractProjectile = fireProjectile(Mode.Basic);
		abstractProjectile.OnDealDamageEvent += OnDealDamage;
		return abstractProjectile;
	}

	protected AbstractProjectile fireBasicNoEffect()
	{
		AbstractProjectile abstractProjectile = fireProjectile(Mode.Basic, false);
		abstractProjectile.OnDealDamageEvent += OnDealDamage;
		return abstractProjectile;
	}

	public virtual void BeginEx()
	{
		beginFiring(Mode.Ex);
	}

	public virtual void EndEx()
	{
		endFiring(Mode.Ex);
	}

	protected virtual AbstractProjectile fireEx()
	{
		return fireProjectile(Mode.Ex);
	}

	protected virtual void beginFiring(Mode mode)
	{
		weaponManager.IsShooting = true;
		firing.Set(mode, true);
	}

	protected virtual AbstractProjectile fireProjectile(Mode mode, bool createEffect = true)
	{
		Vector2 vector = weaponManager.GetBulletPosition();
		if (mode == Mode.Ex)
		{
			vector = weaponManager.ExPosition;
		}
		if (mode == Mode.Basic)
		{
			weaponManager.UpdateAim();
		}
		if (GetProjectile(mode) == null)
		{
			Debug.LogWarning(base.gameObject.name + ": " + mode.ToString() + " projectile is not set!");
			return null;
		}
		if (GetEffect(mode) != null && createEffect)
		{
			if (mode == Mode.Basic || mode != Mode.Ex)
			{
				Effect effect = basicEffectPrefab.Create(vector, base.transform.localScale);
				WeaponSparkEffect weaponSparkEffect = effect as WeaponSparkEffect;
				if (weaponSparkEffect != null)
				{
					LevelPlayerWeaponManager.Pose directionPose = weaponManager.GetDirectionPose();
					if (directionPose == LevelPlayerWeaponManager.Pose.Forward || directionPose == LevelPlayerWeaponManager.Pose.Forward_R || directionPose == LevelPlayerWeaponManager.Pose.Up_D || directionPose == LevelPlayerWeaponManager.Pose.Up_D_R)
					{
						weaponSparkEffect.SetPlayer(player);
					}
					if (directionPose == LevelPlayerWeaponManager.Pose.Down)
					{
						weaponSparkEffect.BringToFrontOfPlayer();
					}
				}
			}
			else
			{
				weaponManager.CreateExDust(GetEffect(mode));
			}
		}
		AbstractProjectile abstractProjectile = GetProjectile(mode).Create(vector, weaponManager.GetBulletRotation(), weaponManager.GetBulletScale());
		if (mode == Mode.Ex)
		{
			abstractProjectile.DamageSource = DamageDealer.DamageSource.Ex;
			CupheadLevelCamera.Current.Shake(5f, 0.5f);
		}
		if (GetFiringHitbox(mode) != null)
		{
			abstractProjectile.AddFiringHitbox(GetFiringHitbox(mode).Create(vector, weaponManager.GetBulletRotation()));
		}
		abstractProjectile.PlayerId = player.id;
		return abstractProjectile;
	}

	protected virtual void endFiring(Mode mode)
	{
		weaponManager.IsShooting = false;
		firing.Set(mode, false);
	}

	private AbstractProjectile GetProjectile(Mode mode)
	{
		if (mode == Mode.Basic || mode != Mode.Ex)
		{
			return basicPrefab;
		}
		return exPrefab;
	}

	private Effect GetEffect(Mode mode)
	{
		if (mode == Mode.Basic || mode != Mode.Ex)
		{
			return basicEffectPrefab;
		}
		return exEffectPrefab;
	}

	private LevelPlayerWeaponFiringHitbox GetFiringHitbox(Mode mode)
	{
		if (mode == Mode.Basic || mode != Mode.Ex)
		{
			return basicFiringHitboxPrefab;
		}
		return exFiringHitboxPrefab;
	}

	private FireProjectileDelegate getFiringMethod(Mode mode)
	{
		switch (mode)
		{
		case Mode.Ex:
			return fireEx;
		default:
			return fireBasic;
		}
	}

	protected virtual IEnumerator fireWeapon_cr(Mode mode)
	{
		float t = 0f;
		while (true)
		{
			yield return new WaitForFixedUpdate();
			if (player.motor.Dashing)
			{
				continue;
			}
			if (t < rapidFireRate)
			{
				t += CupheadTime.FixedDelta;
			}
			else if (firing.Get(mode) && weaponManager.IsShooting)
			{
				weaponManager.TriggerWeaponFire();
				getFiringMethod(mode)();
				if (mode == Mode.Ex || !rapidFire)
				{
					firing.Set(mode, false);
					weaponManager.IsShooting = false;
				}
				t = 0f;
			}
		}
	}

	protected virtual IEnumerator chargeFireWeapon_cr(Mode mode)
	{
		float t = 0f;
		bool charging = false;
		bool alreadyHeld = false;
		bool alreadyReleased = false;
		while (true)
		{
			yield return new WaitForFixedUpdate();
			if (firing.Get(mode) && weaponManager.IsShooting)
			{
				alreadyHeld = true;
			}
			else if (alreadyHeld)
			{
				alreadyReleased = true;
			}
			if (t < rapidFireRate)
			{
				t += CupheadTime.FixedDelta;
				charging = false;
			}
			else if (firing.Get(mode) && weaponManager.IsShooting && !player.motor.Dashing && !player.motor.IsHit && !player.motor.IsUsingSuperOrEx && !alreadyReleased)
			{
				if (!charging)
				{
					StartCharging();
				}
				charging = true;
			}
			else if (charging || alreadyReleased)
			{
				charging = false;
				alreadyReleased = false;
				alreadyHeld = false;
				weaponManager.TriggerWeaponFire();
				getFiringMethod(mode)();
				if (!weaponManager.IsShooting)
				{
					firing.Set(mode, false);
				}
				t = 0f;
			}
		}
	}

	protected virtual void StartCharging()
	{
	}
}
