using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelTurtle : AbstractCollidableObject
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
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
	private FlyingMermaidLevelTurtleCannonBall cannonBallPrefab;

	[SerializeField]
	private Transform cannonBallRoot;

	[SerializeField]
	private Transform shootEffectRoot;

	[SerializeField]
	private Effect shootEffectPrefab;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private LevelProperties.FlyingMermaid.Turtle properties;

	private float hp;

	private bool moving = true;

	private string currentExplodePattern;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
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

	public void Init(LevelProperties.FlyingMermaid.Turtle properties)
	{
		this.properties = properties;
		hp = properties.hp;
	}

	private IEnumerator die_cr()
	{
		AudioManager.Play("level_mermaid_turtle_flag");
		state = State.Dying;
		Collider2D[] components = GetComponents<Collider2D>();
		foreach (Collider2D collider2D in components)
		{
			collider2D.enabled = false;
		}
		base.animator.SetTrigger("OnDeath");
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
		AudioManager.Play("level_mermaid_turtle_enter");
		Collider2D[] components = GetComponents<Collider2D>();
		foreach (Collider2D collider2D in components)
		{
			collider2D.enabled = false;
		}
		float t = 0f;
		while (t < riseTime)
		{
			t += (float)CupheadTime.Delta;
			Vector2 position = base.transform.localPosition;
			position.y += riseDistance / riseTime * (float)CupheadTime.Delta;
			base.transform.localPosition = position;
			yield return null;
		}
		Animator animator = GetComponent<Animator>();
		animator.SetTrigger("Continue");
		yield return animator.WaitForAnimationToEnd(this, "Intro");
		state = State.Idle;
		StartCoroutine(pattern_cr());
		StartCoroutine(move_cr());
		Collider2D[] components2 = GetComponents<Collider2D>();
		foreach (Collider2D collider2D2 in components2)
		{
			collider2D2.enabled = true;
		}
	}

	private IEnumerator move_cr()
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		while (true)
		{
			if (moving)
			{
				Vector2 vector = base.transform.localPosition;
				vector.x -= properties.speed * (float)CupheadTime.Delta;
				base.transform.localPosition = vector;
				if (vector.x < (float)Level.Current.Left - sprite.bounds.size.x / 2f)
				{
					Object.Destroy(base.gameObject);
				}
			}
			yield return null;
		}
	}

	private IEnumerator pattern_cr()
	{
		string[] pattern = properties.explodeSpreadshotString.GetRandom().Split(',');
		yield return CupheadTime.WaitForSeconds(this, properties.timeUntilShoot.RandomFloat());
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i][0] == 'D')
			{
				float waitTime = 0f;
				float.TryParse(pattern[i].Substring(1), out waitTime);
				yield return CupheadTime.WaitForSeconds(this, waitTime);
			}
			else if (!((FlyingMermaidLevel)Level.Current).MerdusaTransformStarted)
			{
				currentExplodePattern = pattern[i];
				AudioManager.Play("level_mermaid_turtle_shell_pop");
				base.animator.SetTrigger("Shoot");
				yield return base.animator.WaitForAnimationToEnd(this, "Idle");
				moving = false;
				yield return base.animator.WaitForAnimationToEnd(this, "Shoot_B");
				moving = true;
				AudioManager.Play("level_mermaid_turtle_post_cannon");
			}
		}
	}

	private void OnShootFX()
	{
		shootEffectPrefab.Create(shootEffectRoot.position);
		cannonBallPrefab.Create(cannonBallRoot.transform.position, currentExplodePattern, properties);
	}
}
