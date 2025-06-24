using System.Collections;
using UnityEngine;

public class DicePalaceRouletteLevelRoulette : LevelProperties.DicePalaceRoulette.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Twirl = 2,
		Marble = 3,
		Death = 4
	}

	[SerializeField]
	private BasicProjectile marble;

	[SerializeField]
	private DicePalaceRouletteLevelMarblesLaunch marbleLaunch;

	[SerializeField]
	private Transform marbleRoot;

	private bool onRight = true;

	private bool slowDown;

	private bool stopTwirl;

	private bool firstLaunch = true;

	private bool stopMarbles;

	private int index;

	private float speed;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Coroutine patternCoroutine;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		state = State.Intro;
		StartCoroutine(intro_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (base.properties.CurrentHealth <= 0f && state != State.Death)
		{
			state = State.Death;
			StartDeath();
		}
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2f);
		AudioManager.Play("dice_palace_roulette_intro");
		emitAudioFromObject.Add("dice_palace_roulette_intro");
		base.animator.Play("Roulette_Intro");
		state = State.Idle;
	}

	protected virtual float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	public void StartTwirl()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(twirl_cr());
	}

	private IEnumerator twirl_cr()
	{
		state = State.Twirl;
		base.animator.Play("Roulette_Travel");
		LevelProperties.DicePalaceRoulette.Twirl p = base.properties.CurrentState.twirl;
		string[] amountPattern = p.twirlAmount.GetRandom().Split(',');
		StartCoroutine(twirl_vary_speed_cr());
		float twirlAmount = 0f;
		float stopDist = 200f;
		float.TryParse(amountPattern[index], out twirlAmount);
		Vector3 pos = base.transform.position;
		for (int i = 0; (float)i < twirlAmount; i++)
		{
			if (onRight)
			{
				slowDown = false;
				float maxPoint = -630f;
				while (base.transform.position.x > maxPoint)
				{
					if (!stopTwirl)
					{
						float f = maxPoint - base.transform.position.x;
						f = Mathf.Abs(f);
						pos.x = Mathf.MoveTowards(base.transform.position.x, maxPoint, speed * (float)CupheadTime.Delta * hitPauseCoefficient());
						if (f < stopDist)
						{
							slowDown = true;
						}
						base.transform.position = pos;
					}
					yield return null;
				}
				onRight = !onRight;
				continue;
			}
			slowDown = false;
			float maxPoint2 = 490f;
			while (base.transform.position.x < maxPoint2)
			{
				if (!stopTwirl)
				{
					float f2 = maxPoint2 - base.transform.position.x;
					f2 = Mathf.Abs(f2);
					pos.x = Mathf.MoveTowards(base.transform.position.x, maxPoint2, speed * (float)CupheadTime.Delta * hitPauseCoefficient());
					if (f2 < stopDist)
					{
						slowDown = true;
					}
					base.transform.position = pos;
				}
				yield return null;
			}
			onRight = !onRight;
		}
		twirlAmount = (twirlAmount + 1f) % (float)amountPattern.Length;
		StopCoroutine(twirl_vary_speed_cr());
		state = State.Idle;
	}

	private void TwirlStop()
	{
		stopTwirl = true;
	}

	private void TwirlStart()
	{
		stopTwirl = false;
	}

	private IEnumerator twirl_vary_speed_cr()
	{
		LevelProperties.DicePalaceRoulette.Twirl p = base.properties.CurrentState.twirl;
		float incrementspeed = p.movementSpeed / 50f;
		while (true)
		{
			if (slowDown)
			{
				if (speed <= 50f)
				{
					slowDown = false;
				}
				else
				{
					speed -= incrementspeed;
				}
			}
			else if (speed < p.movementSpeed)
			{
				speed += incrementspeed;
			}
			yield return null;
		}
	}

	public void StartMarbleDrop()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(marble_drop_cr());
	}

	private void SpawnMarble(float xOffset)
	{
		LevelProperties.DicePalaceRoulette.MarbleDrop marbleDrop = base.properties.CurrentState.marbleDrop;
		float rotation = Mathf.Atan2(Level.Current.Ground, 0f) * 57.29578f;
		Vector2 position = base.transform.position;
		position.y = 360f;
		position.x = ((!onRight) ? (640f - xOffset) : (-640f + xOffset));
		marble.Create(position, rotation, marbleDrop.marbleSpeed);
	}

	private IEnumerator marble_drop_cr()
	{
		LevelProperties.DicePalaceRoulette.MarbleDrop p = base.properties.CurrentState.marbleDrop;
		string[] spawnPattern = p.marblePositionStrings.GetRandom().Split(',');
		float waitTime = 0f;
		state = State.Marble;
		firstLaunch = true;
		stopMarbles = false;
		base.animator.Play("Roulette_Attack_Start");
		AudioManager.Play("dice_palace_roulette_attack_start");
		emitAudioFromObject.Add("dice_palace_roulette_attack_start");
		yield return base.animator.WaitForAnimationToStart(this, "Roulette_Attack_Loop");
		AudioManager.PlayLoop("dice_palace_roulette_attack_loop");
		emitAudioFromObject.Add("dice_palace_roulette_attack_loop");
		StartCoroutine(marble_sound_cr());
		yield return CupheadTime.WaitForSeconds(this, p.marbleInitalDelay);
		int i;
		for (i = 0; i < spawnPattern.Length; i++)
		{
			if (spawnPattern[i][0] == 'D')
			{
				float.TryParse(spawnPattern[i].Substring(1), out waitTime);
				yield return CupheadTime.WaitForSeconds(this, waitTime);
			}
			else
			{
				string[] array = spawnPattern[i].Split('-');
				string[] array2 = array;
				foreach (string s in array2)
				{
					float result = 0f;
					float.TryParse(s, out result);
					SpawnMarble(result);
				}
			}
			i %= spawnPattern.Length;
			yield return CupheadTime.WaitForSeconds(this, p.marbleDelay);
		}
		stopMarbles = true;
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		base.animator.SetTrigger("Continue");
		AudioManager.Stop("dice_palace_roulette_attack_loop");
		AudioManager.Play("dice_palace_roulette_attack_end");
		emitAudioFromObject.Add("dice_palace_roulette_attack_end");
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		state = State.Idle;
	}

	public void SpawnMarbleAnimation()
	{
		if (!stopMarbles)
		{
			DicePalaceRouletteLevelMarblesLaunch dicePalaceRouletteLevelMarblesLaunch = Object.Instantiate(marbleLaunch, marbleRoot, false);
			dicePalaceRouletteLevelMarblesLaunch.IsFirstTime = firstLaunch;
			firstLaunch = false;
		}
	}

	private IEnumerator marble_sound_cr()
	{
		AudioManager.Play("dice_palace_roulette_balls_start");
		emitAudioFromObject.Add("dice_palace_roulette_balls_start");
		AudioManager.PlayLoop("dice_palace_roulette_balls_shoot_loop");
		emitAudioFromObject.Add("dice_palace_roulette_balls_shoot_loop");
		while (!stopMarbles)
		{
			yield return null;
		}
		AudioManager.Stop("dice_palace_roulette_balls_shoot_loop");
		AudioManager.Play("dice_palace_roulette_balls_end");
		emitAudioFromObject.Add("dice_palace_roulette_balls_end");
	}

	private void StartDeath()
	{
		StopAllCoroutines();
		GetComponent<Collider2D>().enabled = false;
		base.animator.Play("Roulette_Death");
		AudioManager.Play("dice_palace_roulette_death");
		emitAudioFromObject.Add("dice_palace_roulette_death");
	}

	private void TravelSFX()
	{
		AudioManager.Play("dice_palace_roulette_travel");
		emitAudioFromObject.Add("dice_palace_roulette_travel");
	}
}
