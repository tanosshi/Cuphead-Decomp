using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelSeahorse : AbstractCollidableObject
{
	public enum State
	{
		Intro = 0,
		Spit = 1,
		Dying = 2
	}

	[SerializeField]
	private float spawnY;

	[SerializeField]
	private float riseTime;

	[SerializeField]
	private float riseDistance;

	[SerializeField]
	private float deathStayTime;

	[SerializeField]
	private float deathMoveTime;

	[SerializeField]
	private float deathMoveDistance;

	[SerializeField]
	private FlyingMermaidLevelSeahorseSpray spray;

	[SerializeField]
	private Transform deathFxRoot;

	[SerializeField]
	private Transform deathFxPrefab;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private LevelProperties.FlyingMermaid.Seahorse properties;

	private float hp;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		spray.enabled = false;
		StartCoroutine(intro_cr());
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		Vector2 vector = base.transform.position;
		vector.y = spawnY;
		base.transform.position = vector;
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp < 0f && state != State.Dying)
		{
			AudioManager.Play("level_mermaid_seahorse_death");
			state = State.Dying;
			StopAllCoroutines();
			StartCoroutine(die_cr());
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void Init(LevelProperties.FlyingMermaid.Seahorse properties)
	{
		this.properties = properties;
		GroundHomingMovement component = GetComponent<GroundHomingMovement>();
		component.acceleration = properties.acceleration;
		component.maxSpeed = properties.maxSpeed;
		component.bounceRatio = properties.bounceRatio;
		hp = properties.hp;
	}

	private IEnumerator die_cr()
	{
		state = State.Dying;
		GroundHomingMovement homer = GetComponent<GroundHomingMovement>();
		Collider2D collider = GetComponent<Collider2D>();
		homer.enabled = false;
		collider.enabled = false;
		base.animator.SetTrigger("SprayDeath");
		spray.End();
		AudioManager.Play("level_mermaid_seahorse_death");
		base.animator.SetTrigger("OnDeath");
		Transform deathFx = Object.Instantiate(deathFxPrefab);
		deathFx.SetParent(deathFxRoot);
		deathFx.ResetLocalTransforms();
		yield return CupheadTime.WaitForSeconds(this, deathStayTime);
		float t = 0f;
		while (t < deathMoveTime)
		{
			t += (float)CupheadTime.Delta;
			Vector2 position = base.transform.localPosition;
			position.y -= deathMoveDistance * (float)CupheadTime.Delta / deathMoveTime;
			base.transform.localPosition = position;
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator intro_cr()
	{
		GroundHomingMovement homer = GetComponent<GroundHomingMovement>();
		Collider2D collider = GetComponent<Collider2D>();
		homer.enabled = false;
		collider.enabled = false;
		float t = 0f;
		while (t < riseTime)
		{
			t += (float)CupheadTime.Delta;
			Vector2 position = base.transform.localPosition;
			position.y += riseDistance / riseTime * (float)CupheadTime.Delta;
			base.transform.localPosition = position;
			yield return null;
		}
		AudioManager.Play("level_mermaid_seahorse_intro");
		Animator animator = GetComponent<Animator>();
		animator.SetTrigger("Continue");
		yield return animator.WaitForAnimationToStart(this, "Spit_Start");
		spray.enabled = true;
		spray.Init(properties);
		yield return animator.WaitForAnimationToEnd(this, "Spit_Start");
		state = State.Spit;
		StartCoroutine(spit_cr());
	}

	private IEnumerator spit_cr()
	{
		AudioManager.Play("level_mermaid_seahorse_spit");
		GroundHomingMovement homer = GetComponent<GroundHomingMovement>();
		Collider2D collider = GetComponent<Collider2D>();
		homer.enabled = true;
		collider.enabled = true;
		float t = 0f;
		while (true)
		{
			t += (float)CupheadTime.Delta;
			if (t > properties.homingDuration || ((FlyingMermaidLevel)Level.Current).MerdusaTransformStarted)
			{
				break;
			}
			yield return null;
		}
		homer.EnableHoming = false;
		homer.bounceEnabled = false;
		base.animator.SetTrigger("SprayDeath");
		spray.End();
	}
}
