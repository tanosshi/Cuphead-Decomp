using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelEnemy : AbstractCollidableObject
{
	public enum State
	{
		Unspawned = 0,
		Spawned = 1
	}

	private LevelProperties.FlyingBlimp.Enemy enemyProperties;

	private LevelProperties.FlyingBlimp properties;

	private bool shoot;

	private Vector3 startPoint;

	[SerializeField]
	private Effect bulletEffect;

	[SerializeField]
	private FlyingBlimpLevelEnemyDeathPart[] deathPieces;

	[SerializeField]
	private FlyingBlimpLevelEnemyProjectile projectilePrefab;

	[SerializeField]
	private FlyingBlimpLevelEnemyProjectile parryablePrefab;

	[SerializeField]
	private Transform projectileRoot;

	private AbstractPlayerController player;

	private FlyingBlimpLevelBlimpLady parent;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private float hp;

	private float stopPoint;

	private bool parryable;

	private int animationPicker;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
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
		if (hp < 0f && state == State.Spawned)
		{
			StopAllCoroutines();
			StartCoroutine(dying_cr());
		}
	}

	protected override void Start()
	{
		base.Start();
		shoot = false;
		startPoint = base.transform.position;
	}

	public void Init(LevelProperties.FlyingBlimp properties, Vector3 startPoint, float stopPoint, bool Aparryable, FlyingBlimpLevelBlimpLady parent)
	{
		enemyProperties = properties.CurrentState.enemy;
		this.properties = properties;
		this.parent = parent;
		this.startPoint = startPoint;
		this.stopPoint = stopPoint;
		this.parent.OnDeathEvent += Die;
		parryable = Aparryable;
		StartCoroutine(emerge_cr());
	}

	private void CreatePieces()
	{
		FlyingBlimpLevelEnemyDeathPart[] array = deathPieces;
		foreach (FlyingBlimpLevelEnemyDeathPart flyingBlimpLevelEnemyDeathPart in array)
		{
			flyingBlimpLevelEnemyDeathPart.CreatePart(base.transform.position, properties.CurrentState.gear);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		parent.OnDeathEvent -= Die;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public IEnumerator emerge_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		state = State.Spawned;
		hp = enemyProperties.hp;
		Collider2D collider = GetComponent<Collider2D>();
		collider.enabled = true;
		while (base.transform.position.x > stopPoint)
		{
			base.transform.position += base.transform.right * (0f - enemyProperties.speed) * CupheadTime.FixedDelta;
			yield return wait;
		}
		yield return CupheadTime.WaitForSeconds(this, enemyProperties.shotDelay);
		base.animator.Play("Enemy_Attack");
		AudioManager.Play("level_flying_blimp_cannon_ship_fire");
		yield return base.animator.WaitForAnimationToEnd(this, "Enemy_Attack");
		while (base.transform.position.x <= startPoint.x)
		{
			base.transform.position += base.transform.right * enemyProperties.speed * CupheadTime.FixedDelta;
			yield return wait;
			if (base.transform.position.x > startPoint.x)
			{
				Die();
			}
		}
	}

	private void FireSpreadshot()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		float x = next.transform.position.x - base.transform.position.x;
		float y = next.transform.position.y - base.transform.position.y;
		Effect effect = Object.Instantiate(bulletEffect);
		effect.transform.position = projectileRoot.transform.position;
		effect.GetComponent<Animator>().SetInteger("PickAni", Random.Range(0, 3));
		for (int i = 0; i < enemyProperties.numBullets; i++)
		{
			float floatAt = enemyProperties.spreadAngle.GetFloatAt((float)i / ((float)enemyProperties.numBullets - 1f));
			float num = enemyProperties.spreadAngle.max / 2f;
			floatAt -= num;
			float num2 = Mathf.Atan2(y, x) * 57.29578f;
			animationPicker = Random.Range(0, 3);
			if (next.transform.position.x > base.transform.position.x)
			{
				num2 = ((!(next.transform.position.y > base.transform.position.y)) ? (-90) : 90);
			}
			switch (animationPicker)
			{
			case 0:
				projectilePrefab.Create(projectileRoot.position, num2 + floatAt, enemyProperties.BSpeed).GetComponent<Animator>().Play("Bullet_1");
				break;
			case 1:
				projectilePrefab.Create(projectileRoot.position, num2 + floatAt, enemyProperties.BSpeed).GetComponent<Animator>().Play("Bullet_2");
				break;
			default:
				projectilePrefab.Create(projectileRoot.position, num2 + floatAt, enemyProperties.BSpeed).GetComponent<Animator>().Play("Bullet_3");
				break;
			}
		}
	}

	private void FireSingle()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		float x = next.transform.position.x - base.transform.position.x;
		float y = next.transform.position.y - base.transform.position.y;
		float num = -3f;
		float num2 = Mathf.Atan2(y, x) * 57.29578f;
		Effect effect = Object.Instantiate(bulletEffect);
		effect.transform.position = projectileRoot.transform.position;
		effect.GetComponent<Animator>().SetInteger("PickAni", Random.Range(0, 3));
		if (next.transform.position.x > base.transform.position.x)
		{
			num2 = ((!(next.transform.position.y > base.transform.position.y)) ? (-90) : 90);
		}
		if (!parryable)
		{
			animationPicker = Random.Range(0, 3);
		}
		else
		{
			animationPicker = Random.Range(0, 2);
		}
		if (!parryable)
		{
			switch (animationPicker)
			{
			case 0:
				projectilePrefab.Create(projectileRoot.position, num2 + num, enemyProperties.ASpeed).GetComponent<Animator>().Play("Bullet_1");
				break;
			case 1:
				projectilePrefab.Create(projectileRoot.position, num2 + num, enemyProperties.ASpeed).GetComponent<Animator>().Play("Bullet_2");
				break;
			default:
				projectilePrefab.Create(projectileRoot.position, num2 + num, enemyProperties.ASpeed).GetComponent<Animator>().Play("Bullet_3");
				break;
			}
		}
		else if (animationPicker == 0)
		{
			parryablePrefab.Create(projectileRoot.position, num2 + num, enemyProperties.ASpeed).GetComponent<Animator>().Play("Bullet_1");
		}
		else
		{
			parryablePrefab.Create(projectileRoot.position, num2 + num, enemyProperties.ASpeed).GetComponent<Animator>().Play("Bullet_2");
		}
	}

	private void FlipEnemy()
	{
		GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
	}

	private IEnumerator dying_cr()
	{
		GetComponent<Collider2D>().enabled = false;
		AudioManager.Play("level_flying_blimp_cannon_ship_death");
		base.animator.Play("Enemy_Explode");
		yield return base.animator.WaitForAnimationToEnd(this, "Enemy_Explode");
		Die();
	}

	private void Die()
	{
		state = State.Unspawned;
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
