using System.Collections;
using UnityEngine;

public class FlowerLevelVenusSpawn : AbstractCollidableObject
{
	[SerializeField]
	private GameObject petalA;

	[SerializeField]
	private GameObject petalB;

	private bool lockRotation;

	private float rotationSpeed;

	private int movementSpeed;

	private float rotationDelay;

	private float currentHP;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private FlowerLevelFlower parent;

	public void OnVenusSpawn(FlowerLevelFlower parent, int hp, float rotSpeed, int moveSpeed, float rotDelay)
	{
		AudioManager.Play("flower_venus_a_chomp");
		rotationDelay = rotDelay;
		rotationSpeed = rotSpeed;
		movementSpeed = moveSpeed;
		lockRotation = false;
		currentHP = hp;
		this.parent = parent;
		this.parent.OnDeathEvent += Die;
		base.animator.SetInteger("Variant", Random.Range(0, 2));
		StartCoroutine(move_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		currentHP -= info.damage;
		if (currentHP <= 0f)
		{
			Die();
			damageReceiver.enabled = false;
		}
	}

	private IEnumerator spawnPetals_cr()
	{
		yield return new WaitForEndOfFrame();
		yield return CupheadTime.WaitForSeconds(this, base.animator.GetCurrentAnimatorStateInfo(0).length / 4f);
		SpawnPetals();
	}

	public void SpawnPetals()
	{
		Vector3 vector = base.transform.position + Vector3.up * (Random.Range(-10, 10) + 70);
		GameObject gameObject = Object.Instantiate(petalA, vector, Quaternion.identity);
		gameObject.GetComponent<Animator>().Play("Plant_LeafA", Random.Range(0, 1));
		StartCoroutine(fade_cr(gameObject, 0.8f, 125f));
		gameObject = Object.Instantiate(petalB, vector + Vector3.down * 50f, Quaternion.identity);
		gameObject.GetComponent<Animator>().Play("Plant_LeafB");
		StartCoroutine(fade_cr(gameObject, 1f, 100f, true));
	}

	private IEnumerator die_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, "Death", 2);
		GetComponent<SpriteRenderer>().enabled = false;
	}

	private IEnumerator fade_cr(GameObject petal, float duration, float speed, bool lastPetal = false)
	{
		SpriteRenderer petalSprite = petal.GetComponent<SpriteRenderer>();
		float currentTime = duration;
		float pct = currentTime / duration;
		while (pct >= 0f)
		{
			Color c = petalSprite.material.color;
			c.a = pct;
			petalSprite.material.color = c;
			petalSprite.transform.position += Vector3.down * speed * CupheadTime.Delta;
			currentTime -= (float)CupheadTime.Delta;
			pct = currentTime / duration;
			yield return null;
		}
		Object.Destroy(petal);
		if (lastPetal)
		{
			StopAllCoroutines();
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		yield return base.animator.WaitForAnimationToEnd(this, true);
		while (true)
		{
			if (!lockRotation && rotationDelay <= 0f)
			{
				Vector3 vector = PlayerManager.GetNext().center - base.transform.position;
				base.transform.right = Vector3.RotateTowards(base.transform.right, -vector.normalized * base.transform.localScale.x, rotationSpeed * CupheadTime.FixedDelta, 0f);
				if (base.transform.localScale.x == 1f)
				{
					if (Vector3.Angle(base.transform.right, -vector.normalized) < 5f)
					{
						lockRotation = true;
					}
				}
				else if (Vector3.Angle(base.transform.right, -vector.normalized) > 175f)
				{
					lockRotation = true;
				}
			}
			base.transform.position -= base.transform.right * movementSpeed * CupheadTime.FixedDelta * base.transform.localScale.x;
			yield return wait;
		}
	}

	private void Die()
	{
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		base.animator.SetTrigger("OnDeath");
		AudioManager.Play("flower_minion_simple_deathpop");
		emitAudioFromObject.Add("flower_minion_simple_deathpop");
		StartCoroutine(die_cr());
		StartCoroutine(spawnPetals_cr());
	}

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.Awake();
	}

	protected override void Update()
	{
		if (rotationDelay > 0f)
		{
			rotationDelay -= CupheadTime.Delta;
		}
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionEnemy(hit, phase);
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		Die();
		base.OnCollisionGround(hit, phase);
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		StartCoroutine(offScreenDeath_cr());
		base.OnCollisionCeiling(hit, phase);
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		StartCoroutine(offScreenDeath_cr());
		base.OnCollisionWalls(hit, phase);
	}

	protected override void OnDestroy()
	{
		parent.OnDeathEvent -= Die;
		StopAllCoroutines();
		base.OnDestroy();
	}

	private IEnumerator offScreenDeath_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		Object.Destroy(base.gameObject);
	}

	private void RandomiseVariant()
	{
		base.animator.SetInteger("Variant", Random.Range(0, 3));
	}

	private void VenusGrowEndAudio()
	{
	}
}
