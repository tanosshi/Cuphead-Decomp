using System.Collections;
using UnityEngine;

public class SlimeLevelSlime : LevelProperties.Slime.Entity
{
	public enum State
	{
		Intro = 0,
		Jump = 1,
		Punch = 2,
		Dying = 3
	}

	private enum Direction
	{
		Left = 0,
		Right = 1
	}

	[SerializeField]
	private Animator[] questionMarks;

	private const float TRANSFORM_MAX_X = 350f;

	private Direction facingDirection;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private float onGroundY;

	private float shadowY;

	private bool wantsToTransform;

	private Direction punchDirection;

	private bool inAir;

	private float velocityY;

	private float gravity;

	private bool dieOnLand;

	private bool deathAudioPlayed;

	private bool firstTime = true;

	private bool firstPunch;

	private bool BigPunchPlaying;

	private int jumpsBeforeFirstPunch;

	private AbstractPlayerController playerToPunch;

	[SerializeField]
	private SpriteRenderer shadow;

	[SerializeField]
	private float shadowMaxY;

	[SerializeField]
	private SlimeLevelSlime bigSlime;

	[SerializeField]
	private SlimeLevelTombstone tombStone;

	[SerializeField]
	private Effect explosionPrefab;

	[SerializeField]
	private Effect dustPrefab;

	[SerializeField]
	private bool isBig;

	[SerializeField]
	private float punchMaxX;

	[SerializeField]
	private float punchMinX;

	[SerializeField]
	private float maxX;

	[SerializeField]
	private Transform eyeMaxPosition;

	private bool inNoDamageWindow;

	private bool inDamageWindow;

	private bool tookDamageDuringNoDamageWindow;

	private bool tookDamageDuringDamageWindow;

	public static bool TINIES { get; private set; }

	public State state { get; private set; }

	public LevelProperties.Slime.State CurrentPropertyState { get; set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		onGroundY = base.transform.position.y;
		shadowY = shadow.transform.position.y;
		TINIES = false;
		shadow.enabled = false;
		if (isBig)
		{
			Collider2D[] components = GetComponents<Collider2D>();
			foreach (Collider2D collider2D in components)
			{
				collider2D.enabled = false;
			}
		}
		Animator[] array = questionMarks;
		foreach (Animator animator in array)
		{
			animator.GetComponent<Collider2D>().enabled = false;
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

	protected override void LateUpdate()
	{
		base.LateUpdate();
		updateShadow(base.transform.position.y - onGroundY);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public override void LevelInit(LevelProperties.Slime properties)
	{
		base.LevelInit(properties);
		CurrentPropertyState = properties.CurrentState;
		CircleCollider2D component = GetComponent<CircleCollider2D>();
		jumpsBeforeFirstPunch = CurrentPropertyState.jump.bigSlimeInitialJumpPunchCount;
		if (isBig)
		{
			properties.OnBossDeath += OnBossDeath;
		}
	}

	private void QuestionMarksOn()
	{
		Animator[] array = questionMarks;
		foreach (Animator animator in array)
		{
			animator.transform.SetScale(base.transform.localScale.x);
			animator.SetBool("IsOn", true);
			animator.GetComponent<Collider2D>().enabled = true;
		}
	}

	private void QuestionMarksOff()
	{
		Animator[] array = questionMarks;
		foreach (Animator animator in array)
		{
			if (animator != null)
			{
				animator.SetBool("IsOn", false);
				animator.GetComponent<Collider2D>().enabled = false;
			}
		}
	}

	public void IntroContinue()
	{
		StartJump();
	}

	public void StartJump()
	{
		state = State.Jump;
		StartCoroutine(jump_cr());
	}

	private IEnumerator jump_cr()
	{
		LevelProperties.Slime.Jump p = CurrentPropertyState.jump;
		string[] pattern = p.patternString.Split(',');
		int i = Random.Range(0, pattern.Length);
		int numJumpsLeft = 0;
		if (firstTime && isBig)
		{
			numJumpsLeft = p.bigSlimeInitialJumpPunchCount;
			firstTime = false;
		}
		else
		{
			numJumpsLeft = p.numJumps.RandomInt();
		}
		base.animator.SetTrigger("Jump");
		float delay = p.groundDelay;
		playerToPunch = PlayerManager.GetNext();
		while (true)
		{
			if (pattern[i][0] == 'D')
			{
				float.TryParse(pattern[i].Substring(1), out delay);
			}
			else
			{
				yield return base.animator.WaitForAnimationToStart(this, "Jump_Squish_Loop");
				if (isBig)
				{
					BigJumpAudio();
				}
				else
				{
					SmallJumpAudio();
				}
				yield return CupheadTime.WaitForSeconds(this, delay);
				base.animator.SetTrigger("Continue");
				yield return base.animator.WaitForAnimationToStart(this, "Up");
				bool goingUp = true;
				bool highJump = pattern[i][0] == 'H';
				if (pattern[i][0] == 'R')
				{
					highJump = Rand.Bool();
				}
				velocityY = ((!highJump) ? p.lowJumpVerticalSpeed : p.highJumpVerticalSpeed);
				float speedX = ((!highJump) ? p.lowJumpHorizontalSpeed : p.highJumpHorizontalSpeed);
				gravity = ((!highJump) ? p.lowJumpGravity : p.highJumpGravity);
				inAir = true;
				Direction moveDir = facingDirection;
				shadow.enabled = true;
				while (goingUp || base.transform.position.y > onGroundY)
				{
					velocityY -= gravity * CupheadTime.FixedDelta * hitPauseCoefficient();
					float velocityX = ((moveDir != Direction.Left) ? speedX : (0f - speedX));
					base.transform.AddPosition(velocityX * CupheadTime.FixedDelta * hitPauseCoefficient(), velocityY * CupheadTime.FixedDelta * hitPauseCoefficient());
					if (velocityY < 0f && goingUp)
					{
						goingUp = false;
						base.animator.SetTrigger("Apex");
					}
					if ((moveDir == Direction.Left && base.transform.position.x < 0f - maxX) || (moveDir == Direction.Right && base.transform.position.x > maxX))
					{
						if (moveDir == Direction.Left)
						{
							base.transform.SetPosition(0f - maxX);
							moveDir = Direction.Right;
						}
						else
						{
							base.transform.SetPosition(maxX);
							moveDir = Direction.Left;
						}
						if (!goingUp)
						{
							speedX = 0f;
						}
						Turn();
					}
					yield return new WaitForFixedUpdate();
				}
				base.transform.SetPosition(null, onGroundY);
				shadow.enabled = false;
				inAir = false;
				delay = p.groundDelay;
				float screenShakeCoefficient = ((!highJump) ? 1f : 1.5f);
				screenShakeCoefficient *= ((!isBig) ? 1f : 2f);
				CupheadLevelCamera.Current.Shake(5f * screenShakeCoefficient, 0.2f * screenShakeCoefficient);
				dustPrefab.Create(base.transform.position);
				if (wantsToTransform && base.transform.position.x > -350f && base.transform.position.x < 350f)
				{
					base.animator.SetTrigger("Transform");
					yield break;
				}
				if (dieOnLand)
				{
					base.animator.SetTrigger("LandingDeath");
					state = State.Dying;
					yield break;
				}
				base.animator.SetTrigger("Land");
				if (isBig && !firstPunch)
				{
					jumpsBeforeFirstPunch--;
					if (jumpsBeforeFirstPunch == 0)
					{
						firstPunch = true;
						yield return Punch();
						yield break;
					}
				}
				else
				{
					numJumpsLeft--;
					if (numJumpsLeft <= 0 && inPunchPosition())
					{
						break;
					}
				}
			}
			i = (i + 1) % pattern.Length;
		}
		yield return Punch();
	}

	private IEnumerator Punch()
	{
		if (playerToPunch == null || playerToPunch.IsDead)
		{
			playerToPunch = PlayerManager.GetNext();
		}
		punchDirection = ((playerToPunch.transform.position.x > base.transform.position.x) ? Direction.Right : Direction.Left);
		if (!isBig)
		{
			base.animator.SetTrigger("Continue");
			yield return base.animator.WaitForAnimationToEnd(this, "Jump_Squish_Loop");
		}
		StartPunch();
	}

	private void updateShadow(float jumpY)
	{
		shadow.transform.SetPosition(null, shadowY);
		float normalizedTime = Mathf.Clamp01(jumpY / shadowMaxY);
		Animator component = shadow.GetComponent<Animator>();
		component.Play("Idle", 0, normalizedTime);
		component.speed = 0f;
	}

	private void Turn()
	{
		base.animator.SetTrigger("Turn");
		StartCoroutine(turn_cr());
	}

	private IEnumerator turn_cr()
	{
		int upTurn = Animator.StringToHash(base.animator.GetLayerName(0) + ".Up_Turn");
		int downTurn = Animator.StringToHash(base.animator.GetLayerName(0) + ".Down_Turn");
		int startSquish = Animator.StringToHash(base.animator.GetLayerName(0) + ".Jump_Squish_Start");
		while (base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash != upTurn && base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash != downTurn && base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash != startSquish)
		{
			yield return new WaitForEndOfFrame();
		}
		while (base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash == upTurn || base.animator.GetCurrentAnimatorStateInfo(0).fullPathHash == downTurn)
		{
			yield return new WaitForEndOfFrame();
		}
		facingDirection = ((facingDirection == Direction.Left) ? Direction.Right : Direction.Left);
		base.transform.SetScale((facingDirection != Direction.Right) ? 1 : (-1));
	}

	private bool inPunchPosition()
	{
		if (playerToPunch == null || playerToPunch.IsDead)
		{
			playerToPunch = PlayerManager.GetNext();
		}
		return (playerToPunch.transform.position.x > base.transform.position.x && base.transform.position.x < punchMaxX && base.transform.position.x > punchMinX) || (playerToPunch.transform.position.x < base.transform.position.x && base.transform.position.x > 0f - punchMaxX && base.transform.position.x < 0f - punchMinX);
	}

	public void StartPunch()
	{
		state = State.Punch;
		StartCoroutine(punch_cr());
	}

	private IEnumerator punch_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0f);
		bool turn = punchDirection != facingDirection;
		base.animator.SetTrigger((!turn) ? "StartPunch" : "StartPunchTurn");
		yield return base.animator.WaitForAnimationToStart(this, "Punch_Pre_Hold");
		yield return CupheadTime.WaitForSeconds(this, CurrentPropertyState.punch.preHold);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToStart(this, "Punch_Hold");
		yield return CupheadTime.WaitForSeconds(this, CurrentPropertyState.punch.mainHold);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToStart(this, "Punch_End");
		BigPunchPlaying = false;
		StartJump();
		if (isBig)
		{
			base.animator.SetBool("FirstPunch", false);
		}
	}

	private void PunchTurn()
	{
		base.animator.SetTrigger("Continue");
		facingDirection = ((facingDirection == Direction.Left) ? Direction.Right : Direction.Left);
		base.transform.SetScale((facingDirection != Direction.Right) ? 1 : (-1));
	}

	public void Transform()
	{
		wantsToTransform = true;
	}

	public void TurnBig()
	{
		bigSlime.transform.position = base.transform.position;
		bigSlime.StartJump();
		bigSlime.facingDirection = facingDirection;
		bigSlime.transform.localScale = base.transform.localScale;
		Collider2D[] components = bigSlime.GetComponents<Collider2D>();
		foreach (Collider2D collider2D in components)
		{
			collider2D.enabled = true;
		}
		Object.Destroy(base.gameObject);
	}

	private void OnBossDeath()
	{
		Die(false);
	}

	public void DeathTransform()
	{
		if (state != State.Dying)
		{
			base.properties.OnBossDeath -= OnBossDeath;
			if (inAir)
			{
				dieOnLand = true;
			}
			else
			{
				Die(false);
			}
		}
		StartCoroutine(transformToTombstone_cr());
	}

	private IEnumerator transformToTombstone_cr()
	{
		yield return base.animator.WaitForAnimationToStart(this, "Death_Loop");
		yield return CupheadTime.WaitForSeconds(this, 3.5f);
		tombStone.StartIntro(base.transform.position.x);
	}

	private void Die(bool earlyKnockout)
	{
		StopAllCoroutines();
		state = State.Dying;
		base.animator.ResetTrigger("Continue");
		if (earlyKnockout)
		{
			base.animator.SetTrigger("EarlyKnockout");
			if (Level.Current.mode == Level.Mode.Easy)
			{
				base.properties.WinInstantly();
			}
			else
			{
				base.properties.DealDamageToNextNamedState();
			}
		}
		else if (inAir)
		{
			base.animator.SetTrigger("AirDeath");
			StartCoroutine(airDeath_cr());
		}
		else
		{
			base.animator.SetTrigger("GroundDeath");
		}
	}

	private IEnumerator airDeath_cr()
	{
		velocityY = Mathf.Min(0f, velocityY);
		while (base.transform.position.y > onGroundY)
		{
			velocityY -= gravity * CupheadTime.FixedDelta * hitPauseCoefficient();
			base.transform.AddPosition(0f, velocityY * CupheadTime.FixedDelta * hitPauseCoefficient());
			yield return new WaitForFixedUpdate();
		}
		base.transform.SetPosition(null, onGroundY);
		float screenShakeCoefficient = 2.5f;
		CupheadLevelCamera.Current.Shake(5f * screenShakeCoefficient, 0.2f * screenShakeCoefficient);
		dustPrefab.Create(base.transform.position);
		shadow.enabled = false;
		base.animator.SetTrigger("Continue");
	}

	private float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	public void Explode()
	{
		explosionPrefab.Create(base.transform.position);
		Object.Destroy(base.gameObject);
	}

	private void StartNoDamageWindow()
	{
		inNoDamageWindow = true;
	}

	private void StartDamageWindow()
	{
		inNoDamageWindow = false;
		inDamageWindow = true;
	}

	private void EndDamageWindow()
	{
		inDamageWindow = false;
	}

	private void DoPotentialKnockout()
	{
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (inNoDamageWindow)
		{
			tookDamageDuringNoDamageWindow = true;
		}
		if (!inDamageWindow)
		{
			return;
		}
		if (base.transform.localScale.x > 0f)
		{
			if (info.origin.x < eyeMaxPosition.position.x && info.origin.y > eyeMaxPosition.position.y)
			{
				tookDamageDuringDamageWindow = true;
			}
		}
		else if (info.origin.x > eyeMaxPosition.position.x && info.origin.y > eyeMaxPosition.position.y)
		{
			tookDamageDuringDamageWindow = true;
		}
	}

	private void IntroAudio()
	{
		AudioManager.Play("slime_small_intro_anim");
		emitAudioFromObject.Add("slime_small_intro_anim");
		AudioManager.Play("slime_tiphat");
		emitAudioFromObject.Add("slime_tiphat");
	}

	private void BlinkAudio()
	{
		AudioManager.Play("slime_blink");
	}

	private void SmallJumpAudio()
	{
		AudioManager.Play("slime_small_jump");
	}

	private void SmallLandAudio()
	{
		AudioManager.Play("slime_small_land");
	}

	private void SmallStretchPunchAudio()
	{
		AudioManager.Play("slime_small_stretch_punch");
		emitAudioFromObject.Add("slime_small_stretch_punch");
	}

	private void SmallTransformAudio()
	{
		AudioManager.Play("slime_small_transform");
	}

	private void BigJumpAudio()
	{
		AudioManager.Play("slime_big_jump");
	}

	private void BigLandAudio()
	{
		AudioManager.Play("slime_big_land");
	}

	private void BigPunchAudio()
	{
		if (!BigPunchPlaying)
		{
			AudioManager.Play("slime_big_punch");
			emitAudioFromObject.Add("slime_big_punch");
			AudioManager.Play("slime_big_punch_voice");
			emitAudioFromObject.Add("slime_big_punch_voice");
			BigPunchPlaying = true;
		}
	}

	private void BigDeathAudio()
	{
		if (!deathAudioPlayed)
		{
			AudioManager.Play("slime_big_death");
			AudioManager.Play("slime_big_death_voice");
			deathAudioPlayed = true;
		}
	}
}
