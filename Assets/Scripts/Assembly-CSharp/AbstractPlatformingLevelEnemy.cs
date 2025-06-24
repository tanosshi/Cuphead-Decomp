using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DamageReceiver))]
[RequireComponent(typeof(HitFlash))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class AbstractPlatformingLevelEnemy : AbstractLevelEntity
{
	public enum StartCondition
	{
		LevelStart = 0,
		TriggerVolume = 1,
		Instant = 2,
		Custom = 3
	}

	public static readonly Vector2 CAMERA_DEATH_PADDING = new Vector2(100f, 100f);

	[SerializeField]
	private EnemyID _id;

	[SerializeField]
	protected StartCondition _startCondition;

	[SerializeField]
	private float _startDelay;

	[SerializeField]
	protected Vector2 _triggerPosition = Vector2.zero;

	[SerializeField]
	protected Vector2 _triggerSize = Vector2.one * 100f;

	[SerializeField]
	private PlatformingLevelGenericExplosion[] explosionPrefabs;

	[SerializeField]
	private Effect parryEffectPrefab;

	private EnemyProperties _properties;

	protected bool IdleSounds = true;

	public EnemyID ID
	{
		get
		{
			return _id;
		}
	}

	public float StartDelay
	{
		get
		{
			return _startDelay;
		}
	}

	public EnemyProperties Properties
	{
		get
		{
			if (_properties == null)
			{
				_properties = EnemyDatabase.GetProperties(_id);
			}
			return _properties;
		}
	}

	public float Health { get; protected set; }

	public bool Dead { get; private set; }

	public string DeathSound { get; protected set; }

	protected DamageReceiver _damageReceiver { get; private set; }

	protected DamageDealer _damageDealer { get; private set; }

	protected bool _started { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		DeathSound = "none";
		if (Properties == null)
		{
			Health = 10f;
			_canParry = false;
		}
		else
		{
			Health = Properties.Health;
			_canParry = Properties.CanParry;
		}
		_damageReceiver = GetComponent<DamageReceiver>();
		_damageDealer = DamageDealer.NewEnemy();
		_damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		StartWithCondition(StartCondition.Instant);
		Level.Current.OnLevelStartEvent += OnLevelStart;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Level.Current != null)
		{
			Level.Current.OnLevelStartEvent -= OnLevelStart;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (_startCondition == StartCondition.TriggerVolume && !_started)
		{
			Rect rect = RectUtils.NewFromCenter(_triggerPosition.x, _triggerPosition.y, _triggerSize.x, _triggerSize.y);
			if (rect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerOne).center) || (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null && rect.Contains(PlayerManager.GetPlayer(PlayerId.PlayerTwo).center)))
			{
				StartWithCondition(StartCondition.TriggerVolume);
			}
		}
		if (_damageDealer != null)
		{
			_damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (_damageDealer != null)
		{
			_damageDealer.DealDamage(hit);
		}
	}

	protected virtual void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		Health -= info.damage;
		if (Health <= 0f)
		{
			Level.ScoringData.pacifistRun = false;
			Die();
		}
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
		Die();
	}

	protected virtual void Die()
	{
		IdleSounds = false;
		if (!Dead)
		{
			Dead = true;
			Explode();
			Object.Destroy(base.gameObject);
		}
	}

	protected void Explode()
	{
		if (explosionPrefabs.Length > 0 && CupheadLevelCamera.Current.ContainsPoint(base.transform.position, CAMERA_DEATH_PADDING))
		{
			AudioManager.Play(DeathSound);
			explosionPrefabs.RandomChoice().Create(GetComponent<Collider2D>().bounds.center);
		}
	}

	public override void OnParry(AbstractPlayerController player)
	{
		base.OnParry(player);
		if (parryEffectPrefab != null)
		{
			parryEffectPrefab.Create(GetComponent<Collider2D>().bounds.center);
		}
		player.stats.OnParry();
		Die();
	}

	protected virtual IEnumerator idle_audio_delayer_cr(string key, float delayMin, float delayMax)
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		while (true)
		{
			if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, CAMERA_DEATH_PADDING))
			{
				float delay = Random.Range(delayMin, delayMax);
				yield return CupheadTime.WaitForSeconds(this, delay);
				yield return null;
				if (IdleSounds)
				{
					AudioManager.Play(key);
					while (AudioManager.CheckIfPlaying(key))
					{
						yield return null;
					}
				}
			}
			yield return null;
		}
	}

	public void StartFromCustom()
	{
		if (!_started)
		{
			StartWithCondition(StartCondition.Custom);
		}
	}

	public void ResetStartingCondition()
	{
		_started = false;
	}

	protected abstract void OnStart();

	private void OnLevelStart()
	{
		StartWithCondition(StartCondition.LevelStart);
	}

	private void StartWithCondition(StartCondition condition)
	{
		if (!Dead && condition == _startCondition && !_started)
		{
			_started = true;
			StartCoroutine(startWithCondition_cr());
		}
	}

	private IEnumerator startWithCondition_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, _startDelay);
		OnStart();
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.2f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		if (_startCondition == StartCondition.TriggerVolume)
		{
			Gizmos.color = new Color(0f, 1f, 0f, a);
			Gizmos.DrawWireCube(_triggerPosition, _triggerSize);
		}
	}
}
