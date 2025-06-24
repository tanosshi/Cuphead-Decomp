using System;
using System.Collections;
using UnityEngine;

public class TrainLevelEngineBoss : LevelProperties.Train.Entity
{
	public delegate void OnDamageTakenHandler(float damage);

	public enum DoorState
	{
		Open = 0,
		Closed = 1,
		Opening = 2,
		Closing = 3
	}

	[Serializable]
	public class DoorSprites
	{
		public SpriteRenderer open;

		public SpriteRenderer closed;

		public SpriteRenderer opening;

		public SpriteRenderer closing;

		public SpriteRenderer this[DoorState state]
		{
			get
			{
				switch (state)
				{
				default:
					return open;
				case DoorState.Closed:
					return closed;
				case DoorState.Opening:
					return opening;
				case DoorState.Closing:
					return closing;
				}
			}
		}

		public void DisableAll()
		{
			open.enabled = false;
			closed.enabled = false;
			opening.enabled = false;
			closing.enabled = false;
		}
	}

	public enum TailState
	{
		On = 0,
		Off = 1
	}

	[Serializable]
	public class TailSprites
	{
		public SpriteRenderer on;

		public SpriteRenderer off;

		public SpriteRenderer this[TailState state]
		{
			get
			{
				if (state == TailState.On || state != TailState.Off)
				{
					return on;
				}
				return off;
			}
		}

		public void DisableAll()
		{
			on.enabled = false;
			off.enabled = false;
		}
	}

	private const string HitParameterName = "Hit";

	private const string StopHitAnimName = "StopHitAnim";

	private const float StopHitAnimTime = 0.25f;

	[SerializeField]
	private DamageReceiverChild heartDamageReceiver;

	[SerializeField]
	private Transform footDustRoot;

	[SerializeField]
	private Effect footDustPrefab;

	private DamageReceiver damageReceiver;

	private float health;

	private bool dead;

	private bool TrainRunStep;

	[Header("Dropper")]
	[SerializeField]
	private Transform dropperRoot;

	[SerializeField]
	private TrainLevelEngineBossDropperProjectile dropperPrefab;

	private IEnumerator attackCoroutine;

	private SpriteRenderer smokeRenderer;

	private const string FireAttackParameterName = "FireAttack";

	[Header("Fire")]
	[SerializeField]
	private Transform fireRoot;

	[SerializeField]
	private TrainLevelEngineBossFireProjectile firePrefab;

	private const string OpenDoorParameterName = "Open";

	private const string CloseDoorParameterName = "Close";

	[Header("Door")]
	[SerializeField]
	private DoorSprites doorSprites;

	[SerializeField]
	private GameObject door;

	private DoorState _ds = DoorState.Closed;

	private DoorState desiredDoorState = DoorState.Closed;

	[Header("Tail")]
	[SerializeField]
	private TailSprites tailSprites;

	[SerializeField]
	private Transform tailRoot;

	private TailState _tailState = TailState.Off;

	private TrainLevelEngineBossTail tailSwitch;

	private DoorState doorState
	{
		get
		{
			return _ds;
		}
		set
		{
			if (value != _ds)
			{
				_ds = value;
				UpdateHeartDamageReceiver();
			}
		}
	}

	private TailState tailState
	{
		get
		{
			return _tailState;
		}
		set
		{
			ChangeTail(value);
		}
	}

	public event OnDamageTakenHandler OnDamageTakenEvent;

	public event Action OnDeathEvent;

	protected override void Awake()
	{
		base.Awake();
		tailSwitch = TrainLevelEngineBossTail.Create(tailRoot);
		tailSwitch.OnActivate += OnTailParried;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		smokeRenderer = dropperRoot.GetComponent<SpriteRenderer>();
	}

	protected override void Start()
	{
		base.Start();
		UpdateHeartDamageReceiver();
	}

	protected override void Update()
	{
		base.Update();
	}

	public override void LevelInit(LevelProperties.Train properties)
	{
		base.LevelInit(properties);
		health = properties.CurrentState.engine.health;
	}

	public void StartBoss()
	{
		AudioManager.Play("train_engine_boss_run_start");
		TrainRunStep = true;
		StartCoroutine(move_cr());
		StartCoroutine(tailTimer_cr());
		StartCoroutine(fireProjectiles_cr());
		StartAttack();
		UpdateHeartDamageReceiver();
	}

	private void UpdateHeartDamageReceiver()
	{
		heartDamageReceiver.enabled = doorState == DoorState.Open || doorState == DoorState.Closing;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!dead)
		{
			if (this.OnDamageTakenEvent != null)
			{
				this.OnDamageTakenEvent(info.damage);
			}
			base.animator.SetBool("Hit", true);
			CancelInvoke("StopHitAnim");
			Invoke("StopHitAnim", 0.25f);
			health -= info.damage;
			if (health <= 0f)
			{
				Die();
			}
		}
	}

	private void Die()
	{
		if (!dead)
		{
			dead = true;
			damageReceiver.enabled = false;
			StopAllCoroutines();
			StartCoroutine(die_cr());
		}
	}

	private IEnumerator die_cr()
	{
		AudioManager.Play("train_engine_boss_die");
		emitAudioFromObject.Add("train_engine_boss_die");
		base.animator.SetTrigger("OnDeath");
		door.SetActive(false);
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
		yield return TweenPositionX(base.transform.position.x, -300f, 2.5f * Mathf.Abs(-300f - base.transform.position.x) / 400f, EaseUtils.EaseType.easeInOutSine);
		while (true)
		{
			yield return TweenPositionX(base.transform.position.x, 100f, 2.5f, EaseUtils.EaseType.easeInOutSine);
			yield return TweenPositionX(base.transform.position.x, -300f, 2.5f, EaseUtils.EaseType.easeInOutSine);
		}
	}

	private void StopHitAnim()
	{
		base.animator.SetBool("Hit", false);
	}

	public void SpawnDustOnFeet()
	{
		footDustPrefab.Create(footDustRoot.position, footDustRoot.localScale).Play();
	}

	private void StartAttack()
	{
		StopAttack();
		attackCoroutine = attack_cr();
		StartCoroutine(attackCoroutine);
	}

	private void StopAttack()
	{
		if (attackCoroutine != null)
		{
			StopCoroutine(attackCoroutine);
		}
	}

	private void OnAttackAnimComplete()
	{
		dropperPrefab.Create(dropperRoot.position, base.properties.CurrentState.engine.projectileUpSpeed, base.properties.CurrentState.engine.projectileXSpeed, base.properties.CurrentState.engine.projectileGravity);
	}

	public void SmokeFX()
	{
		smokeRenderer.flipX = Rand.Bool();
		base.animator.SetTrigger("Smoke");
	}

	private IEnumerator attack_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.engine.projectileDelay);
			base.animator.SetTrigger("OnAttack");
			AudioManager.Play("train_engine_boss_attack");
			emitAudioFromObject.Add("train_engine_boss_attack");
			yield return base.animator.WaitForAnimationToStart(this, "Attack");
			yield return base.animator.WaitForAnimationToEnd(this, "Attack");
		}
	}

	private IEnumerator fireProjectiles_cr()
	{
		while (true)
		{
			if (doorState == DoorState.Open)
			{
				base.animator.SetTrigger("FireAttack");
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.engine.fireDelay);
			}
			yield return null;
		}
	}

	public void SpawnProjectile()
	{
		Vector2 zero = Vector2.zero;
		zero.y = base.properties.CurrentState.engine.fireVelocityY;
		zero.x = base.properties.CurrentState.engine.fireVelocityX;
		firePrefab.Create(fireRoot.position, zero, base.properties.CurrentState.engine.fireGravity);
	}

	private void DoorAnimOpenStarted()
	{
		if (desiredDoorState == DoorState.Open && doorState == DoorState.Closed)
		{
			doorState = DoorState.Opening;
			base.animator.SetTrigger("Open");
		}
		UpdateDoorSprite();
	}

	private void DoorAnimCloseStarted()
	{
		if (desiredDoorState == DoorState.Closed && doorState == DoorState.Open)
		{
			doorState = DoorState.Closing;
			base.animator.SetTrigger("Close");
		}
		UpdateDoorSprite();
	}

	private void DoorOpenAnimComplete()
	{
		if (doorState == DoorState.Opening)
		{
			AudioManager.Play("train_engine_boss_door");
			emitAudioFromObject.Add("train_engine_boss_door");
			doorState = DoorState.Open;
		}
		UpdateDoorSprite();
	}

	private void DoorCloseAnimComplete()
	{
		if (doorState == DoorState.Closing)
		{
			AudioManager.Play("train_engine_boss_door_shut");
			emitAudioFromObject.Add("train_engine_boss_door_shut");
			doorState = DoorState.Closed;
		}
		UpdateDoorSprite();
	}

	private void IronStepSFX()
	{
		if (TrainRunStep)
		{
			AudioManager.Play("train_engine_step");
			emitAudioFromObject.Add("train_engine_step");
		}
	}

	private void UpdateDoorSprite()
	{
		doorSprites.DisableAll();
		doorSprites[doorState].enabled = true;
	}

	private IEnumerator doorTimer_cr()
	{
		desiredDoorState = DoorState.Open;
		float time = base.properties.CurrentState.engine.doorTime.GetFloatAt(health / base.properties.CurrentState.engine.health);
		while (doorState != DoorState.Open)
		{
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, time);
		desiredDoorState = DoorState.Closed;
		while (doorState != DoorState.Closed)
		{
			yield return null;
		}
		StartCoroutine(tailTimer_cr());
	}

	private void ChangeTail(TailState state)
	{
		if (state != tailState)
		{
			tailSwitch.tailEnabled = state == TailState.On;
			_tailState = state;
			tailSprites.DisableAll();
			tailSprites[state].enabled = true;
		}
	}

	private void OnTailParried()
	{
		tailState = TailState.Off;
		StartCoroutine(doorTimer_cr());
	}

	private IEnumerator tailTimer_cr()
	{
		tailState = TailState.Off;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.engine.tailDelay);
		tailState = TailState.On;
	}

	private IEnumerator move_cr()
	{
		float max_x = base.properties.CurrentState.engine.maxDist;
		float min_x = base.properties.CurrentState.engine.minDist;
		float forwardTime = base.properties.CurrentState.engine.forwardTime;
		float backTime = base.properties.CurrentState.engine.backTime;
		yield return TweenLocalPositionX(base.transform.position.x, min_x, 3f, EaseUtils.EaseType.easeOutSine);
		AudioManager.FadeSFXVolume("train_engine_boss_run_start", 0f, 3f);
		AudioManager.PlayLoop("train_engine_boss_run_loop");
		emitAudioFromObject.Add("train_engine_boss_run_loop");
		AudioManager.PlayLoop("train_engine_boss_fire_idle");
		emitAudioFromObject.Add("train_engine_boss_fire_idle");
		while (true)
		{
			yield return TweenLocalPositionX(base.transform.position.x, max_x, forwardTime, EaseUtils.EaseType.easeInOutSine);
			yield return TweenLocalPositionX(base.transform.position.x, min_x, backTime, EaseUtils.EaseType.easeInOutSine);
		}
	}
}
