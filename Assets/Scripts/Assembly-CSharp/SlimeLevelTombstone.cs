using System;
using System.Collections;
using UnityEngine;

public class SlimeLevelTombstone : LevelProperties.Slime.Entity
{
	public enum State
	{
		Init = 0,
		Intro = 1,
		Move = 2,
		Smash = 3
	}

	private enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float startY = 550f;

	private const float onGroundY = -80f;

	private const float maxX = 500f;

	private const float fallTime = 0.2f;

	private const float crushSlimeY = 70f;

	private int offsetIndex;

	private bool dealDamage;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	[SerializeField]
	private Transform dirt;

	[SerializeField]
	private Transform dust;

	[SerializeField]
	private Transform dust2;

	[SerializeField]
	private Effect dustPrefab;

	[SerializeField]
	private SlimeLevelSlime bigSlime;

	[SerializeField]
	private SlimeLevelTinySlime tinySlime;

	[SerializeField]
	private Effect smashDustBackPrefab;

	[SerializeField]
	private Effect smashDustFrontPrefab;

	private Direction direction;

	public Action onDeath;

	private bool wantsToSmash;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		Collider2D[] components = GetComponents<Collider2D>();
		foreach (Collider2D collider2D in components)
		{
			collider2D.enabled = false;
		}
		GetComponent<LevelBossDeathExploder>().enabled = false;
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
		if (dealDamage)
		{
			damageDealer.DealDamage(hit);
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public override void LevelInit(LevelProperties.Slime properties)
	{
		base.LevelInit(properties);
	}

	public void StartIntro(float x)
	{
		state = State.Intro;
		base.transform.SetPosition(x);
		offsetIndex = UnityEngine.Random.Range(0, base.properties.CurrentState.tombstone.attackOffsetString.Length);
		Collider2D[] components = GetComponents<Collider2D>();
		foreach (Collider2D collider2D in components)
		{
			collider2D.enabled = true;
		}
		base.properties.OnBossDeath += OnBossDeath;
		GetComponent<LevelBossDeathExploder>().enabled = true;
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		StartCoroutine(crush_slime_cr());
		AudioManager.Play("slime_tombstone_drop_onto_slime");
		emitAudioFromObject.Add("slime_tombstone_drop_onto_slime");
		yield return TweenPositionY(550f, -80f, 0.2f, EaseUtils.EaseType.linear);
		base.animator.SetTrigger("Continue");
		dustPrefab.Create(base.transform.position);
		StartMove();
	}

	private IEnumerator crush_slime_cr()
	{
		while (base.transform.position.y > 70f)
		{
			yield return null;
		}
		bigSlime.Explode();
		if (SlimeLevelSlime.TINIES)
		{
			SlimeLevelTinySlime slimeLevelTinySlime = UnityEngine.Object.Instantiate(tinySlime);
			SlimeLevelTinySlime slimeLevelTinySlime2 = UnityEngine.Object.Instantiate(tinySlime);
			slimeLevelTinySlime.Init(dust.transform.position, base.properties.CurrentState.tombstone, true, this);
			slimeLevelTinySlime2.Init(dust.transform.position, base.properties.CurrentState.tombstone, false, this);
		}
		CupheadLevelCamera.Current.Shake(20f, 0.7f);
	}

	private void StartMove()
	{
		state = State.Move;
		wantsToSmash = false;
		StartCoroutine(move_cr());
		StartCoroutine(waitForSmash_cr());
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		direction = ((!MathUtils.RandomBool()) ? Direction.Right : Direction.Left);
		string[] offsets = base.properties.CurrentState.tombstone.attackOffsetString.Split(',');
		offsetIndex = (offsetIndex + 1) % offsets.Length;
		float offset = 0f;
		float.TryParse(offsets[offsetIndex], out offset);
		bool justStarted = true;
		while (!wantsToSmash)
		{
			base.animator.SetTrigger((direction != Direction.Right) ? "MoveLeft" : "MoveRight");
			yield return base.animator.WaitForAnimationToStart(this, (direction != Direction.Right) ? "Move_Left" : "Move_Right");
			base.animator.Play("Dirt");
			if (justStarted)
			{
				base.animator.Play("Dust_Start");
			}
			else
			{
				base.animator.Play("Dust_Start_End");
			}
			AudioManager.Play("slime_tombstone_slide");
			emitAudioFromObject.Add("slime_tombstone_slide");
			float startX = base.transform.position.x;
			float endX = ((direction != Direction.Right) ? (-500f) : 500f);
			float moveTime = Mathf.Abs(startX - endX) / base.properties.CurrentState.tombstone.moveSpeed;
			yield return TweenPositionX(startX, endX, moveTime, EaseUtils.EaseType.easeInOutSine);
			direction = ((direction != Direction.Right) ? Direction.Right : Direction.Left);
			justStarted = false;
		}
		base.animator.SetTrigger((direction != Direction.Right) ? "MoveLeft" : "MoveRight");
		yield return base.animator.WaitForAnimationToStart(this, (direction != Direction.Right) ? "Move_Left" : "Move_Right");
		base.animator.Play("Dust_Start_End");
		AudioManager.Play("slime_tombstone_slide");
		emitAudioFromObject.Add("slime_tombstone_slide");
		AbstractPlayerController player = PlayerManager.GetNext();
		float startX2 = base.transform.position.x;
		float endX2 = ((direction != Direction.Right) ? (-500f) : 500f);
		float moveTime2 = Mathf.Abs(startX2 - endX2) / base.properties.CurrentState.tombstone.moveSpeed;
		float targetX = 0f;
		float t = 0f;
		bool centeredOnPlayer = false;
		while (!centeredOnPlayer && t < moveTime2)
		{
			yield return wait;
			t += CupheadTime.FixedDelta * hitPauseCoefficient();
			base.transform.SetPosition(EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, startX2, endX2, t / moveTime2));
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			targetX = player.center.x + offset;
			if ((direction == Direction.Right && base.transform.position.x > targetX) || (direction == Direction.Left && base.transform.position.x < targetX))
			{
				centeredOnPlayer = true;
			}
		}
		base.transform.SetPosition(Mathf.Clamp(targetX, -500f, 500f));
		base.animator.Play("Dust_End");
		base.animator.Play("Dirt_Off");
		StartSmash();
	}

	private void DustDirection()
	{
		dirt.SetScale((direction == Direction.Right) ? 1 : (-1));
		dust.SetScale((direction != Direction.Right) ? 1 : (-1));
		dust2.SetScale((direction == Direction.Right) ? 1 : (-1));
	}

	private float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	private IEnumerator waitForSmash_cr()
	{
		float timeUntilAttack = base.properties.CurrentState.tombstone.attackDelay.RandomFloat();
		yield return CupheadTime.WaitForSeconds(this, timeUntilAttack);
		wantsToSmash = true;
	}

	private void StartSmash()
	{
		state = State.Smash;
		StartCoroutine(smash_cr());
	}

	private IEnumerator smash_cr()
	{
		base.animator.SetTrigger("StartSmash");
		yield return base.animator.WaitForAnimationToStart(this, "Smash_Pre_Hold");
		AudioManager.Play("slime_tombstone_splat");
		emitAudioFromObject.Add("slime_tombstone_splat");
		AudioManager.Play("slime_tombstone_splat_start");
		emitAudioFromObject.Add("slime_tombstone_splat_start");
		AudioManager.Stop("slime_tombstone_slide");
		emitAudioFromObject.Add("slime_tombstone_slide");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.tombstone.anticipationHold);
		base.animator.SetTrigger("Continue");
		StartMove();
	}

	private void DisableDamageReceiver()
	{
		damageReceiver.enabled = false;
	}

	private void EnableDamageReceiver()
	{
		damageReceiver.enabled = true;
	}

	private void EnableDamageDealer()
	{
		dealDamage = true;
	}

	private void DisableDamageDealer()
	{
		dealDamage = false;
	}

	private void OnSmash()
	{
		CupheadLevelCamera.Current.Shake(30f, 0.7f);
		smashDustFrontPrefab.Create(base.transform.position);
		smashDustBackPrefab.Create(base.transform.position);
	}

	private void OnBossDeath()
	{
		if (onDeath != null)
		{
			onDeath();
		}
		StopAllCoroutines();
		base.animator.SetTrigger("Death");
		AudioManager.Play("slime_tombstone_death");
	}

	private void TombstoneTauntsAudio()
	{
		AudioManager.Play("slime_tombstone_taunts");
	}
}
