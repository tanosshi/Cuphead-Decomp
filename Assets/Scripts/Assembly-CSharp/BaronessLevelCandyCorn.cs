using System.Collections;
using UnityEngine;

public class BaronessLevelCandyCorn : BaronessLevelMiniBossBase
{
	public enum State
	{
		Move = 0,
		Dying = 1
	}

	[SerializeField]
	private BaronessLevelCandyCornMini miniCandyPrefab;

	private LevelProperties.Baroness.CandyCorn properties;

	private AbstractPlayerController player;

	private Transform targetPos;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private float health;

	private float speed;

	private float bottomPoint;

	private int firstIndex;

	private bool isTop;

	private bool moveY;

	private bool firstTime;

	private bool justSwitchedMiddle;

	private bool movingLeft;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		firstTime = true;
		isDying = false;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		state = State.Move;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void Init(LevelProperties.Baroness.CandyCorn properties, AbstractPlayerController player, Vector2 pos, float speed, float health)
	{
		this.properties = properties;
		this.player = player;
		this.speed = speed;
		this.health = health;
		base.transform.position = pos;
		bottomPoint = pos.y;
		isTop = false;
		movingLeft = true;
		if (this.properties.spawnMinis)
		{
			StartCoroutine(spawnMinis_cr());
		}
		StartCoroutine(switchLayer_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (state == State.Move)
		{
			if (moveY)
			{
				MoveAlongY();
			}
			else
			{
				MoveAlongX();
			}
		}
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.OnDamageTaken(info);
		health -= info.damage;
		if (health <= 0f && state != State.Dying)
		{
			state = State.Dying;
			StartDeath();
		}
	}

	private IEnumerator switchLayer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 3f);
		base.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Enemies.ToString();
		base.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
	}

	private IEnumerator spawnMinis_cr()
	{
		targetPos = base.transform;
		Transform targetPos2 = targetPos;
		SpriteRenderer thisRenderer = base.gameObject.GetComponent<SpriteRenderer>();
		while (true)
		{
			if (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Turn_A") || base.animator.GetCurrentAnimatorStateInfo(0).IsName("Turn_B"))
			{
				BaronessLevelCandyCornMini miniCandyCorn = Object.Instantiate(miniCandyPrefab);
				miniCandyCorn.Init(properties, base.transform.position, targetPos, properties.miniCornMovementSpeed, properties.miniCornHP);
				targetPos2 = miniCandyCorn.transform;
				SpriteRenderer r = miniCandyCorn.GetComponent<SpriteRenderer>();
				r.sortingLayerName = thisRenderer.sortingLayerName;
				r.sortingOrder = thisRenderer.sortingOrder - 1;
				yield return CupheadTime.WaitForSeconds(this, properties.miniCornSpawnDelay);
			}
			else
			{
				yield return null;
			}
		}
	}

	private void MoveAlongX()
	{
		float num = 50f;
		float num2 = 10f;
		Vector3 vector = new Vector3(properties.centerPosition, 0f, 0f);
		Vector3 vector2 = vector - base.transform.position;
		float num3 = Mathf.Abs(vector2.x);
		if (movingLeft)
		{
			if (base.transform.position.x > -640f + num)
			{
				base.transform.position -= base.transform.right * (speed * CupheadTime.FixedDelta * hitPauseCoefficient());
				if (vector2.x < num2 && vector2.x > 0f - num2 && !justSwitchedMiddle)
				{
					checkIfSwitch();
				}
			}
			else
			{
				moveY = true;
				justSwitchedMiddle = false;
				movingLeft = false;
			}
		}
		else
		{
			if (movingLeft)
			{
				return;
			}
			if (base.transform.position.x < (float)Level.Current.Right - num)
			{
				base.transform.position += base.transform.right * (speed * CupheadTime.FixedDelta * hitPauseCoefficient());
				if (vector2.x < num2 && vector2.x > 0f - num2 && !justSwitchedMiddle)
				{
					checkIfSwitch();
				}
			}
			else
			{
				moveY = true;
				justSwitchedMiddle = false;
				movingLeft = true;
			}
		}
	}

	private void MoveAlongY()
	{
		float num = 125f;
		if (!isTop)
		{
			if (base.transform.position.y < 360f - num)
			{
				base.transform.position += base.transform.up * (speed * CupheadTime.FixedDelta * hitPauseCoefficient());
				return;
			}
			isTop = true;
			moveY = false;
		}
		else if (base.transform.position.y > bottomPoint)
		{
			base.transform.position -= base.transform.up * (speed * CupheadTime.FixedDelta * hitPauseCoefficient());
		}
		else
		{
			isTop = false;
			moveY = false;
		}
	}

	private void checkIfSwitch()
	{
		StartCoroutine(switch_cr());
	}

	private IEnumerator switch_cr()
	{
		string[] pattern = properties.changeLevelString.GetRandom().Split(',');
		if (firstTime)
		{
			firstIndex = Random.Range(0, pattern.Length);
			firstTime = false;
		}
		if (pattern[firstIndex][0] == 'Y')
		{
			moveY = true;
			justSwitchedMiddle = true;
		}
		else if (pattern[firstIndex][0] == 'N')
		{
			moveY = false;
			justSwitchedMiddle = true;
		}
		if (firstIndex < pattern.Length - 1)
		{
			firstIndex++;
		}
		else
		{
			firstIndex = 0;
		}
		yield return null;
	}

	private void StartDeath()
	{
		state = State.Dying;
		StopAllCoroutines();
		StartCoroutine(death_cr());
	}

	private IEnumerator death_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		float speed = properties.deathMoveSpeed;
		StartExplosions();
		isDying = true;
		base.animator.SetTrigger("Death");
		GetComponent<Collider2D>().enabled = false;
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		while (base.transform.position.y < 560f)
		{
			base.transform.position += Vector3.up * speed * CupheadTime.FixedDelta;
			speed += properties.deathAcceleration;
			yield return wait;
		}
		Die();
		yield return null;
	}

	private void SoundCandyCornBite()
	{
		AudioManager.Play("level_baroness_candycorn_bite");
		emitAudioFromObject.Add("level_baroness_candycorn_bite");
	}
}
