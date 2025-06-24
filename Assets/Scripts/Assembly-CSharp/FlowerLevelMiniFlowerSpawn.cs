using System.Collections;
using UnityEngine;

public class FlowerLevelMiniFlowerSpawn : AbstractCollidableObject
{
	private const float easingValue = 0.03f;

	private const float strength = 4000f;

	private float attackTime;

	private int friendlyFireHits;

	private float currentHP;

	private int currentSpeed;

	private bool isFriendly;

	private bool isAttacking;

	private bool isActive;

	private Vector3 flightDirection;

	private Vector3 pivotPoint;

	private FlowerLevelFlower parent;

	[SerializeField]
	private GameObject bulletPrefab;

	[SerializeField]
	private GameObject spawnPoint;

	[SerializeField]
	private Transform explosion;

	[SerializeField]
	private GameObject petalA;

	[SerializeField]
	private GameObject petalB;

	private LevelProperties.Flower.EnemyPlants properties;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private bool isDead;

	public void OnMiniFlowerSpawn(FlowerLevelFlower parent, LevelProperties.Flower.EnemyPlants properties)
	{
		this.properties = properties;
		currentSpeed = this.properties.miniFlowerMovmentSpeed;
		currentHP = this.properties.miniFlowerPlantHP;
		friendlyFireHits = properties.miniFlowerFriendHP;
		this.parent = parent;
		this.parent.OnDeathEvent += HandleEnd;
		StartCoroutine(move_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		currentHP -= info.damage;
		if (currentHP <= 0f && !isDead)
		{
			isDead = true;
			parent.OnMiniFlowerDeath();
			base.animator.SetTrigger("OnDeath");
			explosion.GetComponent<Animator>().SetInteger("Variant", 1);
			explosion.GetComponent<Animator>().SetTrigger("OnDeath");
			GetComponent<Collider2D>().enabled = false;
			StartCoroutine(die_cr());
			currentSpeed = 0;
			explosion.Rotate(Vector3.forward, Random.Range(0, 360));
		}
	}

	public void SpawnPetals()
	{
		GetComponent<Collider2D>().enabled = false;
		Vector3 vector = spawnPoint.transform.position + Vector3.up * Random.Range(-10, 10);
		if (!isFriendly)
		{
			GameObject gameObject = Object.Instantiate(petalA, vector, Quaternion.identity);
			gameObject.GetComponent<Animator>().Play("PetalA_Red", Random.Range(0, 1));
			StartCoroutine(fade_cr(gameObject, 0.8f));
			gameObject = Object.Instantiate(petalB, vector + Vector3.down * 50f, Quaternion.identity);
			gameObject.GetComponent<Animator>().Play("PetalB_Red_Loop");
			StartCoroutine(fade_cr(gameObject, 1f));
		}
		else
		{
			GameObject gameObject2 = Object.Instantiate(petalA, vector, Quaternion.identity);
			gameObject2.GetComponent<Animator>().Play("PetalA_Blue", Random.Range(0, 1));
			StartCoroutine(fade_cr(gameObject2, 0.8f));
			gameObject2 = Object.Instantiate(petalB, vector + Vector3.down * 50f, Quaternion.identity);
			gameObject2.GetComponent<Animator>().Play("PetalB_Blue_Loop");
			StartCoroutine(fade_cr(gameObject2, 1f, true));
		}
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (true)
		{
			if (!isAttacking && isActive)
			{
				float value = Mathf.Sin(attackTime * (float)currentSpeed / 3f);
				value = Mathf.Clamp(value, -2f, 2f);
				attackTime += CupheadTime.FixedDelta;
				Vector3 position = Vector3.Lerp(base.transform.position, pivotPoint + flightDirection * value * 4000f * CupheadTime.FixedDelta, 0.03f * CupheadTime.GlobalSpeed);
				base.transform.position = position;
				float z = 15f * Mathf.Sin(value) * (0f - Mathf.Sign(flightDirection.x));
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(0f, 0f, z), 8f);
			}
			else
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(0f, 0f, 0f), 10f);
			}
			yield return wait;
		}
	}

	private IEnumerator die_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, "Death", 1, true);
		GetComponent<SpriteRenderer>().enabled = false;
	}

	private IEnumerator fade_cr(GameObject petal, float duration, bool lastPetal = false)
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		SpriteRenderer petalSprite = petal.GetComponent<SpriteRenderer>();
		float currentTime = duration;
		float pct = currentTime / duration;
		while (pct >= 0f)
		{
			Color c = petalSprite.material.color;
			c.a = pct;
			petalSprite.material.color = c;
			petalSprite.transform.position += Vector3.down * 100f * CupheadTime.FixedDelta;
			currentTime -= CupheadTime.FixedDelta;
			pct = currentTime / duration;
			yield return wait;
		}
		Object.Destroy(petal);
		if (lastPetal)
		{
			Die();
		}
	}

	public void FriendlyFireDamage()
	{
	}

	private void HandleEnd()
	{
		if (isFriendly)
		{
			GetComponent<Collider2D>().enabled = false;
			StopAllCoroutines();
		}
		else
		{
			Die();
		}
	}

	private void Die()
	{
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator initialFlight_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (base.transform.position.y < pivotPoint.y)
		{
			base.transform.position += flightDirection * currentSpeed * CupheadTime.GlobalSpeed;
			yield return wait;
		}
		isActive = true;
		attackTime = 0f;
		if (base.transform.position.x < pivotPoint.x)
		{
			flightDirection = Vector3.right * currentSpeed;
		}
		else
		{
			flightDirection = Vector3.left * currentSpeed;
		}
		StartCoroutine(attackDelay_cr());
		yield return wait;
	}

	private IEnumerator attackDelay_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(properties.miniFlowerShootDelay.min, properties.miniFlowerShootDelay.max));
			if (!isAttacking)
			{
				base.animator.SetTrigger("OnAttack");
				isAttacking = true;
			}
		}
	}

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		isFriendly = false;
		flightDirection = Vector3.up;
		pivotPoint = new Vector3((float)Level.Current.Left + (float)Level.Current.Width / 2.5f, Level.Current.Ceiling - Level.Current.Height / 8, 0f);
		base.Awake();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		damageReceiver.enabled = isAttacking;
		base.Update();
	}

	protected override void OnDestroy()
	{
		AudioManager.Play("flower_minion_simple_deathpop_low");
		emitAudioFromObject.Add("flower_minion_simple_deathpop_low");
		StopAllCoroutines();
		parent.OnDeathEvent -= HandleEnd;
		base.OnDestroy();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	private void OnIntroEnd()
	{
		StartCoroutine(initialFlight_cr());
	}

	private void StartedShooting()
	{
		GameObject gameObject = Object.Instantiate(bulletPrefab, spawnPoint.transform.position, Quaternion.identity);
		if (isFriendly)
		{
			gameObject.GetComponent<FlowerLevelMiniFlowerBullet>().OnBulletSpawned(parent.attackPoint.transform.position, properties.miniFlowerProjectileSpeed, properties.miniFlowerProjectileDamage, true);
		}
		else
		{
			gameObject.GetComponent<FlowerLevelMiniFlowerBullet>().OnBulletSpawned(PlayerManager.GetNext().center, properties.miniFlowerProjectileSpeed, properties.miniFlowerProjectileDamage);
		}
	}

	private void EndedShooting()
	{
		isAttacking = false;
	}
}
