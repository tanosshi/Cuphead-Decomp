using System.Collections;
using UnityEngine;

public class ClownLevelClown : LevelProperties.Clown.Entity
{
	public enum State
	{
		BumperCar = 0,
		Helium = 1,
		Death = 2
	}

	private const float FALL_GRAVITY = -100f;

	[SerializeField]
	private Transform forwardYPos;

	[SerializeField]
	private ClownLevelDucks regularDuck;

	[SerializeField]
	private ClownLevelDucks pinkDuck;

	[SerializeField]
	private ClownLevelDucks bombDuck;

	[SerializeField]
	private ClownLevelClownHelium clownHelium;

	private bool notDashing;

	private bool firstSelection;

	private bool stop;

	private float speed;

	private float fallAccumulatedGravity;

	private int timerIndex;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private DamageReceiver damageReceiverChild;

	private Vector3 startPos;

	private Vector2 fallVelocity;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		startPos = base.transform.position;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		state = State.BumperCar;
		notDashing = true;
		StartCoroutine(intro_cr());
	}

	public override void LevelInit(LevelProperties.Clown properties)
	{
		base.LevelInit(properties);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (state != State.Helium)
		{
			damageDealer.DealDamage(hit);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected virtual float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2f);
		base.animator.SetTrigger("Continue");
		AudioManager.Play("clown_intro_continue");
		emitAudioFromObject.Add("clown_intro_continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro_End");
		StartBumperCar();
	}

	public void StartBumperCar()
	{
		state = State.BumperCar;
		base.animator.SetBool("BumperDeath", false);
		StartCoroutine(bumper_car_cr());
		StartCoroutine(ducks_cr());
	}

	public void EndBumperCar()
	{
		base.animator.SetBool("BumperDeath", true);
	}

	private void SwitchLayer()
	{
		GetComponent<SpriteRenderer>().sortingLayerName = "Background";
		GetComponent<SpriteRenderer>().sortingOrder = 101;
	}

	private IEnumerator end_bumper_car_cr()
	{
		AudioManager.Play("clown_bumper_death");
		emitAudioFromObject.Add("clown_bumper_death");
		while (base.transform.position.y > -660f)
		{
			if ((float)CupheadTime.Delta != 0f)
			{
				base.transform.position += (Vector3)(fallVelocity + new Vector2(-300f, fallAccumulatedGravity)) * (float)CupheadTime.Delta;
				fallAccumulatedGravity += -100f;
			}
			yield return null;
		}
		clownHelium.StartHeliumTank();
		Object.Destroy(base.gameObject);
		yield return null;
	}

	private IEnumerator dash_timer_cr(string[] delayPattern)
	{
		LevelProperties.Clown.BumperCar p = base.properties.CurrentState.bumperCar;
		float waitTime;
		float.TryParse(delayPattern[timerIndex], out waitTime);
		yield return CupheadTime.WaitForSeconds(this, waitTime);
		notDashing = false;
		timerIndex = (timerIndex + 1) % delayPattern.Length;
		yield return null;
	}

	private IEnumerator bumper_car_cr()
	{
		notDashing = true;
		bool isFlipped = false;
		Vector3 bumperPos = base.transform.position;
		float offsetDash = 150f;
		float offsetMove = 250f;
		LevelProperties.Clown.BumperCar p = base.properties.CurrentState.bumperCar;
		string[] movementPattern = p.movementStrings.GetRandom().Split(',');
		string[] dashDelayPattern = p.attackDelayString.GetRandom().Split(',');
		float t = 0f;
		float speed = p.movementSpeed;
		int movementIndex = Random.Range(0, movementPattern.Length);
		timerIndex = Random.Range(0, dashDelayPattern.Length);
		StartCoroutine(dash_timer_cr(dashDelayPattern));
		emitAudioFromObject.Add("clown_bumper_move");
		emitAudioFromObject.Add("clown_dash_start");
		emitAudioFromObject.Add("clown_dash_end");
		while (true)
		{
			if (notDashing)
			{
				if (movementPattern[movementIndex][0] == 'F')
				{
					base.animator.SetTrigger("Move");
					while (t < p.movementDuration && notDashing && !stop)
					{
						TransformExtensions.AddPosition(x: ((!isFlipped) ? (0f - p.movementSpeed) : p.movementSpeed) * (float)CupheadTime.Delta, transform: base.transform);
						t += (float)CupheadTime.Delta;
						yield return null;
					}
					AudioManager.Play("clown_bumper_move");
					if (notDashing)
					{
						yield return CupheadTime.WaitForSeconds(this, p.movementDelay);
					}
				}
				else if (movementPattern[movementIndex][0] == 'B')
				{
					base.animator.SetTrigger("Move");
					while (t < p.movementDuration && notDashing && !stop)
					{
						speed = ((!isFlipped) ? p.movementSpeed : (0f - p.movementSpeed));
						if (base.transform.position.x >= (float)Level.Current.Left + offsetDash && base.transform.position.x <= (float)Level.Current.Right - offsetDash)
						{
							base.transform.AddPosition(speed * (float)CupheadTime.Delta);
							t += (float)CupheadTime.Delta;
							yield return null;
						}
						yield return null;
					}
					AudioManager.Play("clown_bumper_move");
					if (notDashing)
					{
						yield return CupheadTime.WaitForSeconds(this, p.movementDelay);
					}
				}
				stop = false;
				t = 0f;
				movementIndex = (movementIndex + 1) % movementPattern.Length;
				yield return null;
			}
			else
			{
				float dist = 640f - base.transform.position.x;
				if (dist < 50f)
				{
					base.animator.Play("Move_Forward");
					while (t < p.movementDuration)
					{
						TransformExtensions.AddPosition(x: ((!isFlipped) ? (0f - p.movementSpeed) : p.movementSpeed) * (float)CupheadTime.Delta, transform: base.transform);
						t += (float)CupheadTime.Delta;
						yield return null;
					}
				}
				AudioManager.Play("clown_dash_start");
				base.animator.Play("Dash_Start");
				yield return CupheadTime.WaitForSeconds(this, p.bumperDashWarning);
				base.animator.SetTrigger("Continue");
				float endPos = (isFlipped ? ((float)Level.Current.Right - offsetMove) : ((float)Level.Current.Left + offsetMove));
				while (base.transform.position.x != endPos)
				{
					bumperPos.x = Mathf.MoveTowards(base.transform.position.x, endPos, p.dashSpeed * (float)CupheadTime.Delta * hitPauseCoefficient());
					base.transform.position = bumperPos;
					yield return null;
				}
				AudioManager.Play("clown_dash_end");
				base.animator.SetTrigger("End");
				isFlipped = !isFlipped;
				yield return base.animator.WaitForAnimationToEnd(this, "Dash_End");
				notDashing = true;
				t = 0f;
				StartCoroutine(dash_timer_cr(dashDelayPattern));
			}
			yield return null;
		}
	}

	private void FlipSprite()
	{
		base.transform.SetScale(0f - base.transform.localScale.x, 1f, 1f);
	}

	private void MoveAStop()
	{
		float num = 10f;
		Vector2 vector = base.transform.position;
		stop = true;
		while (base.transform.position.y != startPos.y)
		{
			vector.y = Mathf.MoveTowards(base.transform.position.y, startPos.y, num * (float)CupheadTime.Delta);
			base.transform.position = vector;
		}
	}

	private void MoveBStop()
	{
		stop = true;
	}

	private void AnimationOffsetUp()
	{
		float num = 10f;
		Vector2 vector = base.transform.position;
		while (base.transform.position.y != forwardYPos.position.y)
		{
			vector.y = Mathf.MoveTowards(base.transform.position.y, forwardYPos.position.y, num * (float)CupheadTime.Delta);
			base.transform.position = vector;
		}
	}

	private void SpawnDuck(ClownLevelDucks prefab, float startPercent)
	{
		if (prefab != null)
		{
			float num = 100f;
			LevelProperties.Clown.Duck duck = base.properties.CurrentState.duck;
			float maxYPos = duck.duckYHeightRange.RandomFloat();
			float num2 = startPercent / 100f * duck.duckYHeightRange.max;
			Vector2 pos = Vector3.zero;
			pos.y = 360f - num2;
			pos.x = 640f + num;
			ClownLevelDucks clownLevelDucks = Object.Instantiate(prefab).Init(pos, base.properties.CurrentState.duck, maxYPos, duck.duckYMovementSpeed);
			clownLevelDucks.Init(pos, base.properties.CurrentState.duck, maxYPos, duck.duckYMovementSpeed);
		}
	}

	private IEnumerator ducks_cr()
	{
		LevelProperties.Clown.Duck p = base.properties.CurrentState.duck;
		string[] positionPattern = p.duckYStartPercentString.GetRandom().Split(',');
		string[] typePattern = p.duckTypeString.GetRandom().Split(',');
		int typeIndex = Random.Range(0, typePattern.Length);
		int posPercentIndex = Random.Range(0, positionPattern.Length);
		while (true)
		{
			float spawnY = 0f;
			float.TryParse(positionPattern[posPercentIndex], out spawnY);
			ClownLevelDucks toSpawn = null;
			if (typePattern[typeIndex][0] == 'R')
			{
				toSpawn = regularDuck;
			}
			else if (typePattern[typeIndex][0] == 'P')
			{
				toSpawn = pinkDuck;
			}
			else if (typePattern[typeIndex][0] == 'B')
			{
				toSpawn = bombDuck;
			}
			if (state != State.Death)
			{
				SpawnDuck(toSpawn, spawnY);
			}
			yield return CupheadTime.WaitForSeconds(this, p.duckDelay);
			typeIndex = (typeIndex + 1) % typePattern.Length;
			posPercentIndex = (posPercentIndex + 1) % positionPattern.Length;
			yield return null;
		}
	}
}
