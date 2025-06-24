using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageDealer
{
	public enum Direction
	{
		Neutral = 0,
		Left = 1,
		Right = 2
	}

	public enum DamageSource
	{
		Neutral = 0,
		Enemy = 1,
		Ex = 2,
		SmallPlane = 3,
		Super = 4,
		Pit = 5
	}

	public delegate void OnDealDamageHandler(float damage, DamageReceiver receiver, DamageDealer dealer);

	public class DamageInfo
	{
		public float damage { get; private set; }

		public Direction direction { get; private set; }

		public Vector2 origin { get; private set; }

		public DamageSource damageSource { get; private set; }

		public float stoneTime { get; private set; }

		public DamageInfo(float damage, Direction direction, Vector2 origin, DamageSource source)
		{
			this.direction = direction;
			this.origin = origin;
			damageSource = source;
			this.damage = damage;
			stoneTime = -1f;
		}

		public void SetStoneTime(float stoneTime)
		{
			this.stoneTime = stoneTime;
		}

		public void SetEditorPlayer()
		{
			damage *= 10f;
		}
	}

	[Serializable]
	public class DamageTypesManager
	{
		public bool Player;

		public bool Enemies;

		public bool Other;

		public DamageTypesManager Copy()
		{
			return MemberwiseClone() as DamageTypesManager;
		}

		public void SetAll(bool b)
		{
			Player = b;
			Enemies = b;
			Other = b;
		}

		public DamageTypesManager OnlyPlayer()
		{
			SetAll(false);
			Player = true;
			return this;
		}

		public DamageTypesManager OnlyEnemies()
		{
			SetAll(false);
			Enemies = true;
			return this;
		}

		public DamageTypesManager PlayerProjectileDefault()
		{
			SetAll(false);
			Enemies = true;
			Other = true;
			return this;
		}

		public bool GetType(DamageReceiver.Type type)
		{
			switch (type)
			{
			case DamageReceiver.Type.Player:
				return Player;
			case DamageReceiver.Type.Enemy:
				return Enemies;
			case DamageReceiver.Type.Other:
				return Other;
			default:
				Debug.LogWarning(string.Concat("DamageType ", type, " not set up!"));
				return false;
			}
		}

		public override string ToString()
		{
			return "Player:" + Player + ", Enemies:" + Enemies + ", Other:" + Other;
		}
	}

	public static DamageSource lastPlayerDamageSource;

	public static PlayerId lastPlayer;

	public static bool didDamageWithNonSmallPlaneWeapon;

	private Dictionary<int, float> timers;

	private float damage = 1f;

	private float damageRate = 1f;

	private float damageMultiplier = 1f;

	private Direction direction;

	private Transform origin;

	private DamageSource damageSource;

	private DamageTypesManager damageTypes;

	private PlayerId playerId = PlayerId.None;

	public float DamageDealt { get; private set; }

	public float DamageMultiplier
	{
		get
		{
			return damageMultiplier;
		}
		set
		{
			damageMultiplier = value;
		}
	}

	public PlayerId PlayerId
	{
		get
		{
			return playerId;
		}
		set
		{
			playerId = value;
		}
	}

	public float StoneTime { get; private set; }

	public event OnDealDamageHandler OnDealDamage;

	public DamageDealer(float damage, float damageRate)
	{
		Setup(damage, damageRate);
	}

	public DamageDealer(float damage, float damageRate, bool damagesPlayer, bool damagesEnemy, bool damagesOther)
	{
		Setup(damage, damageRate, DamageSource.Neutral, damagesPlayer, damagesEnemy, damagesOther);
	}

	public DamageDealer(float damage, float damageRate, DamageSource damageSource, bool damagesPlayer, bool damagesEnemy, bool damagesOther)
	{
		Setup(damage, damageRate, damageSource, damagesPlayer, damagesEnemy, damagesOther);
	}

	public DamageDealer(AbstractProjectile projectile)
	{
		Setup(projectile.Damage, projectile.DamageRate, projectile.DamageSource, projectile.GetDamagesType(DamageReceiver.Type.Player), projectile.GetDamagesType(DamageReceiver.Type.Enemy), projectile.GetDamagesType(DamageReceiver.Type.Other), projectile.DamageMultiplier);
		SetDirection(Direction.Neutral, projectile.transform);
	}

	public static DamageDealer NewEnemy()
	{
		return NewEnemy(0.2f);
	}

	public static DamageDealer NewEnemy(float rate)
	{
		return new DamageDealer(1f, rate, DamageSource.Enemy, true, false, false);
	}

	private void Setup(float damage, float damageRate)
	{
		Setup(damage, damageRate, DamageSource.Neutral, true, false, false);
	}

	private void Setup(float damage, float damageRate, DamageSource damageSource)
	{
		Setup(damage, damageRate, damageSource, true, false, false);
	}

	private void Setup(float damage, float damageRate, DamageSource damageSource, bool damagesPlayer, bool damagesEnemy, bool damagesOther, float damageMultiplier = 1f)
	{
		this.damage = damage;
		this.damageRate = damageRate;
		this.damageMultiplier = damageMultiplier;
		damageTypes = new DamageTypesManager();
		SetDamageFlags(damagesPlayer, damagesEnemy, damagesOther);
		SetDamageSource(damageSource);
		timers = new Dictionary<int, float>();
		StoneTime = -1f;
	}

	public void SetDamage(float damage)
	{
		this.damage = damage;
	}

	public void SetRate(float rate)
	{
		damageRate = rate;
	}

	public void SetDamageSource(DamageSource source)
	{
		damageSource = source;
	}

	public void SetDamageFlags(bool damagesPlayer, bool damagesEnemy, bool damagesOther)
	{
		damageTypes.Player = damagesPlayer;
		damageTypes.Enemies = damagesEnemy;
		damageTypes.Other = damagesOther;
	}

	public void SetDirection(Direction direction, Transform origin)
	{
		this.direction = direction;
		this.origin = origin;
	}

	public float DealDamage(GameObject hit)
	{
		DamageReceiver damageReceiver = hit.GetComponent<DamageReceiver>();
		if (damageReceiver == null)
		{
			DamageReceiverChild component = hit.GetComponent<DamageReceiverChild>();
			if (component != null && component.enabled)
			{
				damageReceiver = component.Receiver;
			}
		}
		if (damageReceiver != null)
		{
			int instanceID = damageReceiver.GetInstanceID();
			if (!damageTypes.GetType(damageReceiver.type))
			{
				return 0f;
			}
			if (!timers.ContainsKey(instanceID))
			{
				timers.Add(instanceID, damageRate);
			}
			else if (damageRate == 0f)
			{
				return 0f;
			}
			if (timers[instanceID] < damageRate)
			{
				return 0f;
			}
			Vector2 vector = ((!(origin != null)) ? Vector2.zero : ((Vector2)origin.position));
			DamageInfo damageInfo = new DamageInfo(damage * damageMultiplier, direction, vector, damageSource);
			damageInfo.SetStoneTime(StoneTime);
			damageReceiver.TakeDamage(damageInfo);
			DamageDealt += damage * damageMultiplier;
			timers[damageReceiver.GetInstanceID()] = 0f;
			if (this.OnDealDamage != null)
			{
				this.OnDealDamage(damage * damageMultiplier, damageReceiver, this);
			}
			if (playerId != PlayerId.None && damageReceiver.type == DamageReceiver.Type.Enemy)
			{
				lastPlayer = playerId;
				lastPlayerDamageSource = damageSource;
				if (damageSource != DamageSource.SmallPlane)
				{
					didDamageWithNonSmallPlaneWeapon = true;
				}
			}
			return damage;
		}
		return 0f;
	}

	public void Update()
	{
		foreach (int item in timers.Keys.ToList())
		{
			timers[item] += CupheadTime.Delta;
		}
	}

	public void FixedUpdate()
	{
		foreach (int item in timers.Keys.ToList())
		{
			timers[item] += CupheadTime.FixedDelta;
		}
	}

	public void SetStoneTime(float stoneTime)
	{
		StoneTime = stoneTime;
	}
}
