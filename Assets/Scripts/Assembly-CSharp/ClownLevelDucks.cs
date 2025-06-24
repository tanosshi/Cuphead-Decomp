using System.Collections;
using UnityEngine;

public class ClownLevelDucks : AbstractProjectile
{
	public bool isBombDuck;

	[SerializeField]
	private Effect explosionPrefab;

	[SerializeField]
	private Effect smokePrefab;

	[SerializeField]
	private Effect sparkPrefab;

	[SerializeField]
	private SpriteDeathParts[] deathParts;

	[SerializeField]
	private Transform bomb;

	[SerializeField]
	private GameObject body;

	private LevelProperties.Clown.Duck properties;

	private AbstractPlayerController player;

	private DamageReceiver damageReceiver;

	private CollisionChild collisionChild;

	private float maxYPos;

	private float speedY;

	private float originalSpeed;

	private bool slowDown;

	private bool bombDropped;

	protected override void Awake()
	{
		base.Awake();
		bombDropped = false;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		body.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		collisionChild = body.GetComponent<CollisionChild>();
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
		if (isBombDuck)
		{
			bomb.GetComponent<Transform>();
			bomb.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		}
	}

	public ClownLevelDucks Init(Vector2 pos, LevelProperties.Clown.Duck properties, float maxYPos, float speedY)
	{
		this.properties = properties;
		base.transform.position = pos;
		this.maxYPos = maxYPos;
		this.speedY = speedY;
		originalSpeed = this.speedY;
		return this;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		VaryingSpeed();
		if (isBombDuck)
		{
			BombCheck();
		}
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		StartCoroutine(spin_cr());
		if (isBombDuck && !bombDropped)
		{
			StartCoroutine(drop_bomb_cr());
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		bool goingUp = false;
		float stopDist = 100f;
		float startPos = base.transform.position.y;
		float endPos = 360f - maxYPos;
		slowDown = true;
		while (base.transform.position.x > -840f)
		{
			Vector3 pos = base.transform.position;
			while (base.transform.position.y != endPos)
			{
				float dist = base.transform.position.y - endPos;
				dist = Mathf.Abs(dist);
				if (dist < stopDist)
				{
					slowDown = true;
				}
				pos.x -= properties.duckXMovementSpeed * (float)CupheadTime.Delta;
				pos.y = Mathf.MoveTowards(base.transform.position.y, endPos, speedY * (float)CupheadTime.Delta);
				base.transform.position = pos;
				yield return null;
			}
			goingUp = !goingUp;
			endPos = ((!goingUp) ? (360f - maxYPos) : 360f);
			yield return null;
		}
		Die();
		yield return null;
	}

	private void VaryingSpeed()
	{
		float num = 4f;
		if (slowDown)
		{
			if (speedY <= originalSpeed / 3f)
			{
				slowDown = false;
			}
			else
			{
				speedY -= num;
			}
		}
		else if (speedY < originalSpeed)
		{
			speedY += num;
		}
		else
		{
			speedY = originalSpeed;
		}
	}

	private IEnumerator spin_cr()
	{
		AudioManager.Play("clown_regular_duck_spin");
		emitAudioFromObject.Add("clown_regular_duck_spin");
		Effect spark = Object.Instantiate(sparkPrefab);
		spark.transform.position = base.transform.position;
		animator.SetBool("Spin", true);
		base.transform.GetComponent<Collider2D>().enabled = false;
		body.transform.GetComponent<Collider2D>().enabled = false;
		yield return CupheadTime.WaitForSeconds(this, properties.spinDuration);
		animator.SetBool("Spin", false);
		base.transform.GetComponent<Collider2D>().enabled = true;
		body.transform.GetComponent<Collider2D>().enabled = true;
		yield return null;
	}

	private void BombCheck()
	{
		player = PlayerManager.GetNext();
		float num = 10f;
		float num2 = player.transform.position.x - base.transform.position.x;
		if (num2 > 0f - num && num2 < num && !bombDropped)
		{
			StartCoroutine(drop_bomb_cr());
		}
	}

	private IEnumerator drop_bomb_cr()
	{
		float vel = properties.bombSpeed;
		float acceleration = 5f;
		bombDropped = true;
		bomb.transform.parent = null;
		bomb.GetComponent<Animator>().SetBool("Fall", true);
		while (bomb.transform.position.y > (float)Level.Current.Ground)
		{
			bomb.transform.AddPosition(0f, (0f - vel) * (float)CupheadTime.Delta);
			vel += acceleration;
			yield return null;
		}
		bomb.GetComponent<Animator>().SetBool("Fall", false);
		Effect explosion = Object.Instantiate(explosionPrefab);
		explosion.transform.position = bomb.transform.position;
		int num = Random.Range(0, 3);
		if (num == 3)
		{
			Effect effect = Object.Instantiate(smokePrefab);
			effect.transform.position = bomb.transform.position;
		}
		AudioManager.Play("clown_bulb_explosion");
		emitAudioFromObject.Add("clown_bulb_explosion");
		CreatePieces();
		Object.Destroy(bomb.gameObject);
	}

	private void CreatePieces()
	{
		SpriteDeathParts[] array = deathParts;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(bomb.transform.position);
		}
	}

	public override void OnParry(AbstractPlayerController player)
	{
		animator.SetBool("Spin", true);
		base.transform.GetComponent<Collider2D>().enabled = false;
		body.transform.GetComponent<Collider2D>().enabled = false;
	}

	protected override void Die()
	{
		StopAllCoroutines();
		base.Die();
	}
}
