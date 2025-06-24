using UnityEngine;

public class BaronessLevelCandyCornMini : AbstractProjectile
{
	public enum State
	{
		Unspawned = 0,
		Spawned = 1,
		Dying = 2
	}

	[SerializeField]
	private Effect deathEffect;

	private float speed;

	private float health;

	private LevelProperties.Baroness.CandyCorn properties;

	private Transform targetPosition;

	private Vector3 lastPos;

	private Vector3 distFromLeaderX;

	private DamageReceiver damageReceiver;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		GetComponent<SpriteRenderer>().flipX = Rand.Bool();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	public void Init(LevelProperties.Baroness.CandyCorn properties, Vector2 pos, Transform targetPos, float speed, float health)
	{
		this.properties = properties;
		this.speed = speed;
		base.transform.position = pos;
		targetPosition = targetPos;
		this.health = health;
		state = State.Spawned;
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		float num = 2f;
		Vector3 position = base.transform.position;
		position.y = Mathf.MoveTowards(base.transform.position.y, 720f + num, speed * CupheadTime.FixedDelta * hitPauseCoefficient());
		base.transform.position = position;
		if (base.transform.position.y == 720f + num)
		{
			Die();
		}
	}

	private float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f)
		{
			deathEffect.Create(base.transform.position);
			Die();
		}
	}

	protected override void Die()
	{
		base.Die();
		state = State.Unspawned;
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private void SoundCandyCornMiniBite()
	{
		AudioManager.Play("level_baroness_candycorn_mini_bite");
		emitAudioFromObject.Add("level_baroness_candycorn_mini_bite");
	}
}
