using System;
using UnityEngine;

public abstract class AbstractProjectile : AbstractCollidableObject
{
	public enum Integers
	{
		Variant = 0,
		MaxVariants = 1
	}

	public enum Triggers
	{
		OnDeath = 0
	}

	public enum Booleans
	{
		Parry = 0
	}

	[Serializable]
	public class CollisionProperties
	{
		public bool Walls = true;

		public bool Ceiling = true;

		public bool Ground = true;

		public bool Enemies;

		public bool EnemyProjectiles;

		public bool Player;

		public bool PlayerProjectiles;

		public bool Other;

		public CollisionProperties Copy()
		{
			return MemberwiseClone() as CollisionProperties;
		}

		public void SetAll(bool b)
		{
			Walls = b;
			Ceiling = b;
			Ground = b;
			Enemies = b;
			EnemyProjectiles = b;
			Player = b;
			PlayerProjectiles = b;
			Other = b;
		}

		public void All()
		{
			SetAll(true);
		}

		public void None()
		{
			SetAll(false);
		}

		public void OnlyPlayer()
		{
			SetAll(false);
			Player = true;
		}

		public void OnlyEnemies()
		{
			SetAll(false);
			Player = true;
		}

		public void OnlyBounds()
		{
			SetAll(false);
			SetBounds(true);
		}

		public void SetBounds(bool b)
		{
			Walls = b;
			Ceiling = b;
			Ground = b;
		}

		public void PlayerProjectileDefault()
		{
			SetAll(false);
			SetBounds(true);
			Enemies = true;
			Other = true;
		}
	}

	protected new Animator animator;

	private Vector3 startPosition;

	private Vector3 lastPosition;

	private MeterScoreTracker tracker;

	private bool hasBeenRendered;

	[SerializeField]
	private bool _canParry;

	private DamageDealer.DamageSource damageSource;

	public float Damage = 1f;

	public float DamageRate;

	public PlayerId PlayerId = PlayerId.None;

	public DamageDealer.DamageTypesManager DamagesType;

	public CollisionProperties CollisionDeath;

	[NonSerialized]
	public float DestroyDistance = 3000f;

	[NonSerialized]
	public bool DestroyDistanceAnimated;

	private LevelPlayerWeaponFiringHitbox firingHitbox;

	private bool firstUpdate = true;

	public bool CanParry
	{
		get
		{
			return _canParry;
		}
	}

	protected float distance { get; private set; }

	protected float lifetime { get; private set; }

	public bool dead { get; private set; }

	protected DamageDealer damageDealer { get; private set; }

	public float StoneTime { get; private set; }

	public virtual float ParryMeterMultiplier
	{
		get
		{
			return 1f;
		}
	}

	public DamageDealer.DamageSource DamageSource
	{
		get
		{
			return damageSource;
		}
		set
		{
			damageSource = value;
		}
	}

	public float DamageMultiplier
	{
		get
		{
			float num = PlayerManager.DamageMultiplier;
			if (base.tag == Tags.PlayerProjectile.ToString())
			{
				if (PlayerManager.GetPlayer(PlayerId).stats.Loadout.charm == Charm.charm_health_up_1)
				{
					num *= 1f - WeaponProperties.CharmHealthUpOne.weaponDebuff;
				}
				else if (PlayerManager.GetPlayer(PlayerId).stats.Loadout.charm == Charm.charm_health_up_2)
				{
					num *= 1f - WeaponProperties.CharmHealthUpTwo.weaponDebuff;
				}
			}
			return num;
		}
	}

	protected virtual float DestroyLifetime
	{
		get
		{
			return 20f;
		}
	}

	protected virtual bool DestroyedAfterLeavingScreen
	{
		get
		{
			return false;
		}
	}

	protected virtual float SafeTime
	{
		get
		{
			return 0.005f;
		}
	}

	protected virtual float PlayerSafeTime
	{
		get
		{
			return 0f;
		}
	}

	protected virtual float EnemySafeTime
	{
		get
		{
			return 0f;
		}
	}

	protected override bool allowCollisionPlayer
	{
		get
		{
			return lifetime > PlayerSafeTime;
		}
	}

	protected override bool allowCollisionEnemy
	{
		get
		{
			return lifetime > EnemySafeTime;
		}
	}

	public event DamageDealer.OnDealDamageHandler OnDealDamageEvent;

	public event Action<AbstractProjectile> OnDie;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		distance = 0f;
		lifetime = 0f;
		StoneTime = -1f;
		if (base.tag != Tags.PlayerProjectile.ToString() && base.tag != Tags.EnemyProjectile.ToString())
		{
			Debug.LogWarning("Projectile " + base.name.Replace("(Clone)", string.Empty) + " must have tag PlayerProjectile or EnemyProjectile!");
		}
		if (base.gameObject.layer != 12)
		{
			base.gameObject.layer = 12;
		}
		RandomizeVariant();
	}

	protected override void Start()
	{
		base.Start();
		damageDealer = new DamageDealer(this);
		damageDealer.OnDealDamage += OnDealDamage;
		damageDealer.SetStoneTime(StoneTime);
		damageDealer.PlayerId = PlayerId;
		if (tracker != null)
		{
			tracker.Add(damageDealer);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (lifetime == 0f)
		{
			lastPosition = (startPosition = base.transform.position);
		}
		if (DestroyDistance > 0f && Vector3.Distance(startPosition, base.transform.position) >= DestroyDistance)
		{
			OnDieDistance();
		}
		distance += Vector3.Distance(lastPosition, base.transform.position);
		lastPosition = base.transform.position;
		if (DestroyLifetime > 0f && lifetime >= DestroyLifetime)
		{
			OnDieLifetime();
		}
		lifetime += Time.deltaTime;
		bool flag = CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(150f, 150f));
		if (DestroyedAfterLeavingScreen && hasBeenRendered && !flag)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (!hasBeenRendered)
		{
			hasBeenRendered = flag;
		}
	}

	protected void ResetLifetime()
	{
		lifetime = 0f;
	}

	protected void ResetDistance()
	{
		distance = 0f;
	}

	protected override void checkCollision(Collider2D col, CollisionPhase phase)
	{
		if (!(lifetime < SafeTime))
		{
			base.checkCollision(col, phase);
		}
	}

	public virtual AbstractProjectile Create()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject);
		return gameObject.GetComponent<AbstractProjectile>();
	}

	public virtual AbstractProjectile Create(Vector2 position)
	{
		AbstractProjectile abstractProjectile = Create();
		abstractProjectile.transform.position = position;
		return abstractProjectile;
	}

	public virtual AbstractProjectile Create(Vector2 position, float rotation)
	{
		AbstractProjectile abstractProjectile = Create(position);
		abstractProjectile.transform.SetEulerAngles(0f, 0f, rotation);
		return abstractProjectile;
	}

	public virtual AbstractProjectile Create(Vector2 position, float rotation, Vector2 scale)
	{
		AbstractProjectile abstractProjectile = Create(position, rotation);
		abstractProjectile.transform.SetScale(scale.x, scale.y, 1f);
		return abstractProjectile;
	}

	protected virtual void OnDealDamage(float damage, DamageReceiver receiver, DamageDealer damageDealer)
	{
		if (this.OnDealDamageEvent != null)
		{
			this.OnDealDamageEvent(damage, receiver, damageDealer);
		}
	}

	public bool GetDamagesType(DamageReceiver.Type type)
	{
		return DamagesType.GetType(type);
	}

	public virtual void SetParryable(bool parryable)
	{
		_canParry = parryable;
		SetBool(Booleans.Parry, parryable);
	}

	public void SetStoneTime(float stoneTime)
	{
		StoneTime = stoneTime;
		if (damageDealer != null)
		{
			damageDealer.SetStoneTime(stoneTime);
		}
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		if (CollisionDeath.Walls)
		{
			OnCollisionDie(hit, phase);
		}
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		if (CollisionDeath.Ceiling)
		{
			OnCollisionDie(hit, phase);
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		if (CollisionDeath.Ground)
		{
			OnCollisionDie(hit, phase);
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (CollisionDeath.Enemies)
		{
			OnCollisionDie(hit, phase);
		}
	}

	protected override void OnCollisionEnemyProjectile(GameObject hit, CollisionPhase phase)
	{
		if (CollisionDeath.EnemyProjectiles)
		{
			OnCollisionDie(hit, phase);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (CollisionDeath.Player)
		{
			OnCollisionDie(hit, phase);
		}
	}

	protected override void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
	{
		if (CollisionDeath.PlayerProjectiles)
		{
			OnCollisionDie(hit, phase);
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (CollisionDeath.Other)
		{
			OnCollisionDie(hit, phase);
		}
	}

	protected virtual void OnCollisionDie(GameObject hit, CollisionPhase phase)
	{
		if (!dead)
		{
			Die();
		}
	}

	protected virtual void OnDieAnimationComplete()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public virtual void OnParry(AbstractPlayerController player)
	{
		if (CanParry)
		{
			OnParryDie();
		}
	}

	public virtual void OnParryDie()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		Die();
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
		Die();
	}

	protected virtual void Die()
	{
		dead = true;
		if (GetComponent<Collider2D>() != null)
		{
			GetComponent<Collider2D>().enabled = false;
		}
		RandomizeVariant();
		SetTrigger(Triggers.OnDeath);
		if (this.OnDie != null)
		{
			this.OnDie(this);
		}
	}

	protected virtual void OnDieDistance()
	{
		if (DestroyDistanceAnimated)
		{
			Die();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected virtual void OnDieLifetime()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected virtual void SetTrigger(Triggers trigger)
	{
		animator.SetTrigger(trigger.ToString());
	}

	protected virtual void SetInt(Integers integer, int i)
	{
		animator.SetInteger(integer.ToString(), i);
	}

	protected virtual void SetBool(Booleans boolean, bool b)
	{
		animator.SetBool(boolean.ToString(), b);
	}

	protected virtual int GetVariants()
	{
		return animator.GetInteger(Integers.MaxVariants.ToString());
	}

	protected virtual void RandomizeVariant()
	{
		int i = UnityEngine.Random.Range(0, GetVariants());
		SetInt(Integers.Variant, i);
	}

	public void AddFiringHitbox(LevelPlayerWeaponFiringHitbox hitbox)
	{
		firingHitbox = hitbox;
		RegisterCollisionChild(hitbox);
		GetComponent<Collider2D>().enabled = false;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (firingHitbox != null)
		{
			if (firstUpdate)
			{
				firstUpdate = false;
			}
			else
			{
				if (!dead)
				{
					GetComponent<Collider2D>().enabled = true;
				}
				UnityEngine.Object.Destroy(firingHitbox.gameObject);
				firingHitbox = null;
			}
		}
		if (damageDealer != null)
		{
			damageDealer.FixedUpdate();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (firingHitbox != null)
		{
			UnityEngine.Object.Destroy(firingHitbox.gameObject);
		}
	}

	public virtual void AddToMeterScoreTracker(MeterScoreTracker tracker)
	{
		this.tracker = tracker;
		if (damageDealer != null)
		{
			tracker.Add(damageDealer);
		}
	}
}
