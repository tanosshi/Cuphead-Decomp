using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelPufferfish : AbstractProjectile
{
	public enum State
	{
		Idle = 0,
		Dying = 1
	}

	private const float GRAVITY = -100f;

	[SerializeField]
	private Effect deathFX;

	[SerializeField]
	private float spawnY;

	[SerializeField]
	private bool parryable;

	private DamageReceiver damageReceiver;

	private LevelProperties.FlyingMermaid.Pufferfish properties;

	private float hp;

	private float accumulatedGravity;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		Vector2 vector = base.transform.position;
		vector.y = spawnY;
		base.transform.position = vector;
		SetParryable(parryable);
		animator.Play("Idle", 0, Random.Range(0f, 1f));
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		AudioManager.Play("level_mermaid_merdusa_puffer_fish_hit");
		hp -= info.damage;
		if (hp < 0f && state != State.Dying)
		{
			state = State.Dying;
			StartDeath();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (state != State.Dying)
		{
			base.damageDealer.DealDamage(hit);
		}
	}

	public void Init(LevelProperties.FlyingMermaid.Pufferfish properties)
	{
		this.properties = properties;
		hp = properties.hp;
		StartCoroutine(loop_cr());
	}

	private IEnumerator loop_cr()
	{
		float speed = properties.floatSpeed * Random.Range(0.9f, 1.1f);
		while (true)
		{
			Vector2 position = base.transform.position;
			position.y += speed * (float)CupheadTime.Delta;
			base.transform.position = position;
			yield return null;
		}
	}

	private void StartDeath()
	{
		StopAllCoroutines();
		StartCoroutine(dying_cr());
	}

	private IEnumerator dying_cr()
	{
		base.gameObject.tag = Tags.EnemyProjectile.ToString();
		deathFX.Create(base.transform.position);
		animator.Play("Death");
		float velocity = 100f;
		while (base.transform.position.y > -660f)
		{
			velocity += (float)CupheadTime.Delta * 300f;
			base.transform.AddPosition(0f, (0f - velocity + accumulatedGravity) * (float)CupheadTime.Delta);
			accumulatedGravity += -100f;
			yield return null;
		}
		Die();
	}

	protected override void Die()
	{
		base.transform.GetComponent<SpriteRenderer>().enabled = false;
		base.Die();
	}
}
