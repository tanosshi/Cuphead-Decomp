using System.Collections;
using UnityEngine;

public class FlowerLevelChomperSeed : AbstractCollidableObject
{
	[SerializeField]
	private GameObject petalA;

	[SerializeField]
	private GameObject petalB;

	private float currentHP;

	private Transform explosion;

	private FlowerLevelFlower parent;

	[SerializeField]
	private SpriteRenderer chomperSprite;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private bool isDead;

	public void OnChomperStart(FlowerLevelFlower parent, LevelProperties.Flower.EnemyPlants properties)
	{
		AudioManager.Play("flower_plants_chomper");
		currentHP = properties.chomperPlantHP;
		this.parent = parent;
		this.parent.OnDeathEvent += StartDeath;
		explosion = base.transform.GetChild(0);
		int integer = base.animator.GetInteger("MaxVariants");
		base.animator.SetInteger("Variant", Random.Range(0, integer));
	}

	private IEnumerator grow_cr()
	{
		float pct = 0.3f;
		while (pct < 1f)
		{
			chomperSprite.transform.localScale = Vector3.one * pct;
			pct += (float)CupheadTime.Delta * 6f;
			if (pct > 1f)
			{
				pct = 1f;
			}
			yield return null;
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		currentHP -= info.damage;
		if (currentHP <= 0f)
		{
			AudioManager.Stop("flower_plants_chomper");
			if (!isDead)
			{
				isDead = true;
				StopAllCoroutines();
				GetComponent<BoxCollider2D>().enabled = false;
				base.animator.Play("Death");
				StartCoroutine(die_cr());
				StartCoroutine(spawnPetals_cr());
			}
		}
	}

	private IEnumerator spawnPetals_cr()
	{
		Animator child = explosion.GetComponent<Animator>();
		child.SetInteger("Variant", 0);
		yield return CupheadTime.WaitForSeconds(this, base.animator.GetCurrentAnimatorStateInfo(0).length / 4f);
		child.Play("Death");
		yield return new WaitForEndOfFrame();
		float delay = child.GetCurrentAnimatorStateInfo(0).length;
		yield return CupheadTime.WaitForSeconds(this, delay / 4f);
		SpawnPetals();
		yield return CupheadTime.WaitForSeconds(this, delay);
		Object.Destroy(base.gameObject);
	}

	public void SpawnPetals()
	{
		Vector3 vector = base.transform.position + Vector3.up * (Random.Range(-10, 10) + 70);
		GameObject gameObject = Object.Instantiate(petalA, vector, Quaternion.identity);
		gameObject.GetComponent<Animator>().Play("Plant_LeafA", Random.Range(0, 1));
		StartCoroutine(fade_cr(gameObject, 0.8f, 125f));
		gameObject = Object.Instantiate(petalB, vector + Vector3.down * 50f, Quaternion.identity);
		gameObject.GetComponent<Animator>().Play("Plant_LeafB");
		StartCoroutine(fade_cr(gameObject, 1f, 100f));
	}

	private IEnumerator die_cr()
	{
		yield return new WaitForEndOfFrame();
		yield return base.animator.WaitForAnimationToEnd(this, "Death", 2);
		explosion.GetComponent<Animator>().Play("Death");
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
			Die();
		}
	}

	private void StartDeath()
	{
		AudioManager.Play("flower_minion_simple_deathpop_low");
		emitAudioFromObject.Add("flower_minion_simple_deathpop_low");
		StopAllCoroutines();
		GetComponent<BoxCollider2D>().enabled = false;
		base.animator.Play("Death");
		StartCoroutine(die_cr());
		StartCoroutine(spawnPetals_cr());
	}

	private void Die()
	{
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.transform.localScale = new Vector3(base.transform.localScale.x * (float)MathUtils.PlusOrMinus(), base.transform.localScale.y, base.transform.localScale.z);
		base.Awake();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (hit.GetComponent<FlowerLevelFlowerDamageRegion>() != null)
		{
			DamageDealer.DamageInfo info = new DamageDealer.DamageInfo(1f, DamageDealer.Direction.Neutral, hit.transform.position, DamageDealer.DamageSource.Enemy);
			OnDamageTaken(info);
		}
		base.OnCollisionEnemy(hit, phase);
	}

	protected override void OnDestroy()
	{
		parent.OnDeathEvent -= StartDeath;
		base.OnDestroy();
	}

	private void OnDeath()
	{
	}

	private void SpawnChomper()
	{
		base.animator.Play("Trigger_Plant", 1);
		StartCoroutine(grow_cr());
	}

	private void GroundBurstStartAudio()
	{
		AudioManager.Play("flower_ground_pop");
	}
}
