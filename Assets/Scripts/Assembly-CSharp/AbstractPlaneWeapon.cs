using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractPlaneWeapon : AbstractPausableComponent
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

	private const int ANIMATION_FRAMES = 10;

	[Header("Ex")]
	[SerializeField]
	protected AbstractProjectile exPrefab;

	[SerializeField]
	protected Effect exEffectPrefab;

	[Header("Basic")]
	[SerializeField]
	protected AbstractProjectile basicPrefab;

	[SerializeField]
	protected Effect basicEffectPrefab;

	[Header("Shrunk")]
	[SerializeField]
	protected AbstractProjectile shrunkPrefab;

	[SerializeField]
	protected float shrunkDamageMultiplier = 0.5f;

	protected FiringSwitches firing;

	protected PlanePlayerController player;

	protected PlanePlayerWeaponManager weaponManager;

	protected abstract bool rapidFire { get; }

	protected abstract float rapidFireRate { get; }

	public int index { get; private set; }

	public virtual void Initialize(PlanePlayerWeaponManager weaponManager, int index)
	{
		this.weaponManager = weaponManager;
		player = weaponManager.GetComponent<PlanePlayerController>();
		this.index = index;
		firing = new FiringSwitches();
		StartCoroutine(fireWeapon_cr(Mode.Basic));
		StartCoroutine(fireWeapon_cr(Mode.Ex));
		StartCoroutine(endFiringAnimation_cr());
		player.OnReviveEvent += OnRevive;
	}

	private void OnRevive(Vector3 pos)
	{
		StartCoroutine(fireWeapon_cr(Mode.Basic));
		StartCoroutine(fireWeapon_cr(Mode.Ex));
		StartCoroutine(endFiringAnimation_cr());
	}

	private void OnDealDamage(float damage, DamageReceiver receiver, DamageDealer dealer)
	{
		if (!(player == null) && !player.IsDead && !(player.stats == null) && receiver.enabled)
		{
			player.stats.OnDealDamage(damage, dealer);
		}
	}

	public virtual void BeginBasic()
	{
		beginFiring(Mode.Basic);
	}

	public virtual void EndBasic()
	{
		endFiring(Mode.Basic);
	}

	protected virtual AbstractProjectile fireBasic()
	{
		AbstractProjectile abstractProjectile = fireProjectile(Mode.Basic);
		abstractProjectile.PlayerId = player.id;
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
		StopCoroutine("endFiringAnimation_cr");
		weaponManager.IsShooting = true;
		firing.Set(mode, true);
	}

	protected virtual AbstractProjectile fireProjectile(Mode mode)
	{
		Vector2 vector = weaponManager.GetBulletPosition() + new Vector2(-10f, 0f) + new Vector2(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
		if (GetProjectile(mode) == null)
		{
			Debug.LogWarning(base.gameObject.name + ": " + mode.ToString() + " projectile is not set!");
			return null;
		}
		if (GetEffect(mode) != null && (mode == Mode.Basic || mode != Mode.Ex))
		{
			basicEffectPrefab.Create(vector, base.transform.localScale).transform.SetParent(base.transform);
		}
		AbstractProjectile abstractProjectile = GetProjectile(mode).Create(vector);
		if (mode == Mode.Ex)
		{
			abstractProjectile.DamageSource = DamageDealer.DamageSource.Ex;
			CupheadLevelCamera.Current.Shake(5f, 0.5f);
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
			if (player.Shrunk)
			{
				return shrunkPrefab;
			}
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

	private IEnumerator fireWeapon_cr(Mode mode)
	{
		float t = 0f;
		float time = rapidFireRate;
		while (true)
		{
			yield return new WaitForFixedUpdate();
			if (t < time)
			{
				t += CupheadTime.FixedDelta;
			}
			else if (firing.Get(mode))
			{
				getFiringMethod(mode)();
				if (mode == Mode.Ex || !rapidFire)
				{
					firing.Set(mode, false);
					StartCoroutine(endFiringAnimation_cr());
				}
				t = 0f;
			}
		}
	}

	private IEnumerator endFiringAnimation_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f / 6f);
		weaponManager.IsShooting = false;
	}
}
