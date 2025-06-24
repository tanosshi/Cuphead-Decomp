using System.Collections;
using UnityEngine;

public class BeeLevelAirplane : LevelProperties.Bee.Entity
{
	public enum State
	{
		Unspawned = 0,
		Intro = 1,
		Idle = 2,
		Wing = 3,
		EndWing = 4,
		Turbine = 5,
		Dead = 6
	}

	[SerializeField]
	private Transform rightShootRoot;

	[SerializeField]
	private Transform leftShootRoot;

	[SerializeField]
	private BeeLevelTurbineBullet bullet;

	[SerializeField]
	private SpriteRenderer topLayer;

	[SerializeField]
	private SpriteRenderer midLayer;

	private string[] countPattern;

	private int countIndex;

	private int blinkCount;

	private int blinkCountMax;

	private bool blinkOne;

	private bool attackOnRight;

	private bool movingRight;

	private bool isMoving;

	private bool isPreSFXPlaying;

	private float offset;

	private float speed;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Coroutine patternCoroutine;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		state = State.Unspawned;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		midLayer.GetComponentInChildren<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
		topLayer.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (base.properties.CurrentHealth <= 0f && state != State.Dead)
		{
			state = State.Dead;
			Dead();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public void StartIntro()
	{
		state = State.Intro;
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		float speed = 400f;
		countPattern = base.properties.CurrentState.wingSwipe.attackCount.GetRandom().Split(',');
		countIndex = Random.Range(0, countPattern.Length);
		while (base.transform.position.y < -60f)
		{
			base.transform.AddPosition(0f, speed * (float)CupheadTime.Delta);
			yield return null;
		}
		state = State.Idle;
		StartCoroutine(move_cr());
		yield return null;
	}

	private void IdleCount()
	{
		blinkCount++;
		if (blinkCount >= blinkCountMax)
		{
			blinkOne = Rand.Bool();
		}
	}

	private void Blink_One()
	{
		if (blinkCount >= blinkCountMax && blinkOne)
		{
			topLayer.enabled = true;
			blinkCount = 0;
			blinkCountMax = Random.Range(3, 7);
		}
		else
		{
			topLayer.enabled = false;
		}
	}

	private void Blink_Two()
	{
		if (blinkCount >= blinkCountMax && !blinkOne)
		{
			topLayer.enabled = true;
			blinkCount = 0;
			blinkCountMax = Random.Range(3, 7);
		}
		else
		{
			topLayer.enabled = false;
		}
	}

	private IEnumerator move_cr()
	{
		bool isLooping = false;
		LevelProperties.Bee.General p = base.properties.CurrentState.general;
		movingRight = false;
		isMoving = true;
		offset = p.movementOffset;
		speed = p.movementSpeed;
		while (true)
		{
			if (isMoving)
			{
				if (movingRight)
				{
					while (base.transform.position.x < 640f - offset && movingRight)
					{
						base.transform.AddPosition(speed * (float)CupheadTime.Delta * hitPauseCoefficient());
						yield return null;
					}
					if (state != State.Wing)
					{
						movingRight = !movingRight;
					}
				}
				else
				{
					while (base.transform.position.x > -640f + offset && !movingRight)
					{
						base.transform.AddPosition((0f - speed) * (float)CupheadTime.Delta * hitPauseCoefficient());
						yield return null;
					}
					if (state != State.Wing)
					{
						movingRight = !movingRight;
					}
				}
				if (!isLooping)
				{
					AudioManager.PlayLoop("bee_airplane_idle_loop");
					emitAudioFromObject.Add("bee_airplane_idle_loop");
					isLooping = true;
				}
			}
			else if (isLooping)
			{
				AudioManager.Stop("bee_airplane_idle_loop");
				isLooping = false;
			}
			yield return null;
		}
	}

	private float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	public void StartTurbine()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(turbine_cr());
	}

	private IEnumerator turbine_cr()
	{
		state = State.Turbine;
		LevelProperties.Bee.TurbineBlasters p = base.properties.CurrentState.turbineBlasters;
		string[] bulletPattern = p.attackDirectionString.GetRandom().Split(',');
		for (int i = 0; i < bulletPattern.Length; i++)
		{
			if (bulletPattern[i][0] == 'R')
			{
				base.animator.Play("Right_Pylon");
			}
			else if (bulletPattern[i][0] == 'L')
			{
				base.animator.Play("Left_Pylon");
			}
			else if (bulletPattern[i][0] == 'B')
			{
				base.animator.Play("Right_Pylon");
				base.animator.Play("Left_Pylon");
			}
			yield return CupheadTime.WaitForSeconds(this, p.repeatDealy);
		}
		yield return CupheadTime.WaitForSeconds(this, p.hesitateRange.RandomFloat());
		state = State.Idle;
	}

	private void ShootBulletRight()
	{
		AudioManager.Play("bee_airplane_pylon");
		emitAudioFromObject.Add("bee_airplane_pylon");
		Vector3 vector = new Vector3(0f, 360f, 0f) - new Vector3(0f, base.transform.position.y, 0f);
		float rotation = MathUtils.DirectionToAngle(vector);
		bullet.Create(rightShootRoot.transform.position, rotation, true, base.properties.CurrentState.turbineBlasters);
	}

	private void ShootBulletLeft()
	{
		AudioManager.Play("bee_airplane_pylon");
		emitAudioFromObject.Add("bee_airplane_pylon");
		Vector3 vector = new Vector3(0f, 360f, 0f) - new Vector3(0f, base.transform.position.y, 0f);
		float rotation = MathUtils.DirectionToAngle(vector);
		bullet.Create(leftShootRoot.transform.position, rotation, false, base.properties.CurrentState.turbineBlasters);
	}

	public void StartWing()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(wing_cr());
	}

	private IEnumerator wing_cr()
	{
		state = State.Wing;
		LevelProperties.Bee.WingSwipe p = base.properties.CurrentState.wingSwipe;
		AbstractPlayerController player = PlayerManager.GetNext();
		attackOnRight = ((player.transform.position.x >= 0f) ? true : false);
		Vector3 startPos = Vector3.zero;
		int count = 0;
		int.TryParse(countPattern[countIndex], out count);
		for (int i = 0; i < count; i++)
		{
			base.animator.SetTrigger("OnSaw");
			yield return base.animator.WaitForAnimationToEnd(this, "Saw_Start");
			movingRight = (attackOnRight ? true : false);
			offset = p.warningMaxDistance;
			speed = p.warningMovementSpeed;
			if (attackOnRight)
			{
				while (base.transform.position.x < 640f - p.warningMaxDistance)
				{
					yield return null;
				}
			}
			else
			{
				while (base.transform.position.x > -640f + p.warningMaxDistance)
				{
					yield return null;
				}
			}
			isMoving = false;
			yield return CupheadTime.WaitForSeconds(this, p.warningDuration);
			base.animator.SetTrigger("Continue");
			isMoving = true;
			offset = p.maxDistance;
			movingRight = !movingRight;
			speed = p.movementSpeed;
			if (!attackOnRight)
			{
				while (base.transform.position.x < 640f - p.maxDistance)
				{
					yield return null;
				}
			}
			else
			{
				while (base.transform.position.x > -640f + p.maxDistance)
				{
					yield return null;
				}
			}
			isMoving = false;
			yield return CupheadTime.WaitForSeconds(this, p.attackDuration);
			base.animator.SetTrigger("End");
			isMoving = true;
			offset = base.properties.CurrentState.general.movementOffset;
			attackOnRight = !attackOnRight;
			speed = base.properties.CurrentState.general.movementSpeed;
		}
		state = State.EndWing;
		countIndex = (countIndex + 1) % countPattern.Length;
		yield return CupheadTime.WaitForSeconds(this, p.hesitateRange.RandomFloat());
		state = State.Idle;
	}

	private void SawLoopSFX()
	{
		SawStartSFX();
		AudioManager.PlayLoop("bee_airplane_saw_loop");
		emitAudioFromObject.Add("bee_airplane_saw_loop");
	}

	private void SawStartSFX()
	{
		AudioManager.Play("bee_airplane_saw_start");
		emitAudioFromObject.Add("bee_airplane_saw_start");
	}

	private void SawEndSFX()
	{
		AudioManager.Stop("bee_airplane_saw_loop");
		AudioManager.Play("bee_airplane_saw_end");
		emitAudioFromObject.Add("bee_airplane_saw_end");
	}

	private void DeathHeadSFX()
	{
		AudioManager.Play("bee_airplane_death_head");
		emitAudioFromObject.Add("bee_airplane_death_head");
	}

	private void SawContinueSFX()
	{
		if (!isPreSFXPlaying)
		{
			AudioManager.Play("bee_airplane_saw_continue");
			emitAudioFromObject.Add("bee_airplane_saw_continue");
			isPreSFXPlaying = true;
		}
	}

	private void SawContinueSFXEnd()
	{
		isPreSFXPlaying = false;
	}

	private void Flip()
	{
		if (attackOnRight)
		{
			base.transform.SetScale(1f, 1f, 1f);
		}
		else
		{
			base.transform.SetScale(-1f, 1f, 1f);
		}
	}

	private void Dead()
	{
		StopAllCoroutines();
		AudioManager.Stop("bee_airplane_saw_loop");
		AudioManager.Play("bee_airplane_death");
		emitAudioFromObject.Add("bee_airplane_death");
		base.animator.SetTrigger("Death");
	}

	private void DeadHead()
	{
		base.animator.Play("Death_Head");
	}
}
