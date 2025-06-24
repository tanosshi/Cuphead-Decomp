using System.Collections;
using UnityEngine;

public class AirshipJellyLevelJelly : LevelProperties.AirshipJelly.Entity
{
	public enum State
	{
		Init = 0,
		Running = 1,
		Hurt = 2,
		Dead = 3
	}

	public enum Direction
	{
		Right = 0,
		Left = 1
	}

	private Color startColor;

	private Color flashColor = Color.red;

	public Transform knobRoot;

	[SerializeField]
	private SpriteRenderer knobSprite;

	[Space(10f)]
	[SerializeField]
	private Effect smashEffect;

	private float speed;

	private float defaultSpeed;

	private float maxHealth;

	private SpriteRenderer renderer;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private AirshipJellyLevelKnob knobSwitch;

	private State state;

	private Direction direction = Direction.Left;

	private const float MIN_X = -550f;

	private const float MAX_X = 550f;

	private Coroutine moveCoroutine;

	private bool Moving
	{
		get
		{
			return state == State.Running;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		renderer = GetComponent<SpriteRenderer>();
		startColor = GetComponent<SpriteRenderer>().color;
		Level.Current.OnLevelStartEvent += OnLevelStart;
		CupheadLevelCamera.Current.StartFloat(25f, 3f);
	}

	public override void LevelInit(LevelProperties.AirshipJelly properties)
	{
		base.LevelInit(properties);
		knobSwitch = AirshipJellyLevelKnob.Create(this);
		knobSwitch.OnActivate += OnKnobParry;
		knobSwitch.OnPrePauseActivate += OnKnobPreParry;
		maxHealth = properties.CurrentHealth;
		defaultSpeed = properties.CurrentState.main.speed.min;
		speed = defaultSpeed;
		damageDealer = new DamageDealer(1f, 0.3f, DamageDealer.DamageSource.Enemy, true, false, false);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		StartCoroutine(flash_cr());
		GetNewSpeed();
		if (base.properties.CurrentHealth <= 0f && state != State.Dead)
		{
			state = State.Dead;
			base.animator.Play("Death");
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

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	private void OnLevelStart()
	{
		state = State.Running;
		StartCoroutine(start_cr());
	}

	private void GetNewSpeed()
	{
		MinMax minMax = base.properties.CurrentState.main.speed;
		float num = base.properties.CurrentHealth / maxHealth;
		float num2 = 1f - num;
		speed = defaultSpeed + minMax.max * num2;
	}

	private void OnTurnComplete()
	{
		base.transform.SetScale(0f - base.transform.localScale.x);
	}

	private void OnKnobParry()
	{
		StartCoroutine(hurt_cr());
	}

	private void OnKnobPreParry()
	{
		AudioManager.Play(Sfx.Levels_Airship_Jelly_Hit);
		smashEffect.Create(knobRoot.position);
		CupheadLevelCamera.Current.Shake(10f, 0.6f);
	}

	private void SfxWalk()
	{
		AudioManager.Play(Sfx.Levels_Airship_Jelly_Walk);
	}

	private void ResetMove()
	{
		if (moveCoroutine != null)
		{
			StopCoroutine(moveCoroutine);
			moveCoroutine = null;
		}
		moveCoroutine = StartCoroutine(jelly_cr());
	}

	private IEnumerator start_cr()
	{
		base.animator.SetTrigger("OnIntroComplete");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro_Transition");
		ResetMove();
	}

	private IEnumerator jelly_cr()
	{
		while (base.properties == null)
		{
		}
		float offset = 100f;
		while (true)
		{
			Vector3 pos = base.transform.position;
			if (direction == Direction.Left)
			{
				while (base.transform.position.x > -640f + offset)
				{
					if (!Moving)
					{
						yield return StartCoroutine(waitForMove_cr());
					}
					pos.x = Mathf.MoveTowards(base.transform.position.x, -640f + offset, speed * (float)CupheadTime.Delta);
					base.transform.position = pos;
					yield return null;
				}
				base.animator.SetTrigger("OnTurn");
				direction = Direction.Right;
			}
			else if (direction == Direction.Right)
			{
				while (base.transform.position.x < 640f - offset)
				{
					if (!Moving)
					{
						yield return StartCoroutine(waitForMove_cr());
					}
					pos.x = Mathf.MoveTowards(base.transform.position.x, 640f - offset, speed * (float)CupheadTime.Delta);
					base.transform.position = pos;
					yield return null;
				}
				base.animator.SetTrigger("OnTurn");
				direction = Direction.Left;
			}
			yield return null;
		}
	}

	private IEnumerator waitForMove_cr()
	{
		while (!Moving)
		{
			yield return null;
		}
	}

	private IEnumerator hurt_cr()
	{
		base.properties.DealDamage(base.properties.CurrentState.main.parryDamage);
		GetNewSpeed();
		knobSprite.enabled = false;
		knobSwitch.enabled = false;
		AudioManager.Play(Sfx.Levels_Airship_Jelly_Hurt);
		if (base.properties.CurrentHealth <= 0f)
		{
			state = State.Dead;
			base.animator.Play("Death");
			yield break;
		}
		base.animator.SetTrigger("OnHurt");
		State lastState = state;
		state = State.Hurt;
		ResetMove();
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.main.hurtDelay);
		state = lastState;
		base.animator.SetTrigger("OnHurtComplete");
		yield return base.animator.WaitForAnimationToEnd(this, "Hurt_Loop");
		state = State.Running;
		StartCoroutine(enableKnob_cr());
	}

	private IEnumerator enableKnob_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.main.orbDelay);
		knobSprite.enabled = true;
		knobSwitch.enabled = true;
	}

	private void SetColor(float t)
	{
		Color color = Color.Lerp(flashColor, Color.black, t);
		renderer.color = color;
	}

	private IEnumerator flash_cr()
	{
		float t = 0f;
		float time = 0.15f;
		while (t < time)
		{
			float val = t / time;
			SetColor(val);
			t += Time.deltaTime;
			yield return null;
		}
		GetComponent<SpriteRenderer>().color = startColor;
	}
}
