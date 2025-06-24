using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelSally : LevelProperties.SallyStagePlay.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Attack = 2,
		Transition = 3
	}

	private enum JumpType
	{
		DiveKick = 1,
		DoubleJump = 2
	}

	[Header("Projectiles")]
	private const float FRAME_TIME = 1f / 24f;

	[SerializeField]
	public CollisionChild collisionChild;

	[SerializeField]
	private Transform husband;

	[SerializeField]
	private SallyStagePlayLevelAngel angel;

	[SerializeField]
	private SallyStagePlayLevelShurikenBomb shurikenPrefab;

	[SerializeField]
	private SallyStagePlayLevelProjectile projectilePrefab;

	[SerializeField]
	private SallyStagePlayLevelUmbrellaProjectile umbrellaProjectilePrefab;

	[SerializeField]
	private SallyStagePlayLevelHeart heartPrefab;

	[SerializeField]
	private SallyStagePlayLevelHouse house;

	private const float SALLY_INIT_JUMP_TIME = 0.1665f;

	private JumpType jumpType;

	private int jumpTypeIndex;

	private int jumpCountIndex;

	private int currentJumpAttackCount;

	private int jumpRollAttackTypeIndex;

	private int heartTypeIndex;

	private int teleportOffsetIndex;

	private float teleportOffset = 500f;

	private bool getOutOfJump;

	private bool isTeleporting;

	private bool isInvincible;

	private Vector2 bounds;

	private Vector3 ground;

	private SallyStagePlayLevel parent;

	private AbstractPlayerController target;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	[Space(10f)]
	[SerializeField]
	private GameObject shadowPrefab;

	[SerializeField]
	private Transform centerPoint;

	[SerializeField]
	private Transform[] spawnPoints;

	public State state { get; private set; }

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (isInvincible)
		{
			return;
		}
		base.properties.DealDamage(info.damage);
		if (!SallyStagePlayLevelBackgroundHandler.HUSBAND_GONE)
		{
			if (!AudioManager.CheckIfPlaying("sally_bg_church_fiance_ohno"))
			{
				AudioManager.Play("sally_bg_church_fiance_ohno");
				emitAudioFromObject.Add("sally_bg_church_fiance_ohno");
			}
			husband.GetComponent<Animator>().Play("OhNo");
		}
	}

	public override void LevelInit(LevelProperties.SallyStagePlay properties)
	{
		base.LevelInit(properties);
		bounds = GetComponent<BoxCollider2D>().bounds.size;
		jumpTypeIndex = Random.Range(0, properties.CurrentState.jump.JumpAttackString.Split(',').Length);
		jumpCountIndex = Random.Range(0, properties.CurrentState.jump.JumpAttackCountString.Split(',').Length);
		jumpRollAttackTypeIndex = Random.Range(0, properties.CurrentState.jumpRoll.JumpAttackTypeString.Split(',').Length);
		heartTypeIndex = Random.Range(0, properties.CurrentState.kiss.heartType.Split(',').Length);
		teleportOffsetIndex = Random.Range(0, properties.CurrentState.teleport.appearOffsetString.Split(',').Length);
		base.transform.position = ground;
		StartCoroutine(intro_cr());
	}

	public void GetParent(SallyStagePlayLevel parent)
	{
		this.parent = parent;
		parent.OnPhase2 += OnPhase2;
	}

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
		collisionChild.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		ground = new Vector3(base.transform.position.x, (float)Level.Current.Ground + 300f, 0f);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		ground.x = base.transform.position.x;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator intro_cr()
	{
		state = State.Intro;
		yield return CupheadTime.WaitForSeconds(this, 2f);
		base.animator.SetTrigger("Continue");
		AudioManager.Play("sally_sally_intro_phase1");
		emitAudioFromObject.Add("sally_sally_intro_phase1");
		state = State.Idle;
		yield return null;
	}

	public void OnJumpAttack()
	{
		state = State.Attack;
		jumpType = (JumpType)int.Parse(base.properties.CurrentState.jump.JumpAttackString.Split(',')[jumpTypeIndex]);
		target = PlayerManager.GetNext();
		currentJumpAttackCount = 0;
		StartCoroutine(jump_cr());
		jumpCountIndex = (jumpCountIndex + 1) % base.properties.CurrentState.jump.JumpAttackCountString.Split(',').Length;
	}

	private IEnumerator jump_cr()
	{
		if (currentJumpAttackCount >= int.Parse(base.properties.CurrentState.jump.JumpAttackCountString.Split(',')[jumpCountIndex]))
		{
			float rand = base.properties.CurrentState.jump.JumpHesitate.RandomFloat();
			if (rand > 0f)
			{
				yield return CupheadTime.WaitForSeconds(this, rand);
			}
			state = State.Idle;
			yield return null;
			yield break;
		}
		jumpTypeIndex = (jumpTypeIndex + 1) % base.properties.CurrentState.jump.JumpAttackString.Split(',').Length;
		jumpType = (JumpType)int.Parse(base.properties.CurrentState.jump.JumpAttackString.Split(',')[jumpTypeIndex]);
		if (currentJumpAttackCount > 0 && base.properties.CurrentState.jump.JumpDelay > 0f)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.jump.JumpDelay);
		}
		base.animator.SetInteger("JumpType", (int)jumpType);
		base.animator.SetTrigger("Jump");
		yield return base.animator.WaitForAnimationToEnd(this, "Jump_TakeOff", true);
		Vector3 start = base.transform.position;
		Vector3 end = start;
		switch (jumpType)
		{
		case JumpType.DiveKick:
			end += Vector3.up * base.properties.CurrentState.diveKick.DiveAttackHeight.RandomFloat();
			break;
		case JumpType.DoubleJump:
			end += Vector3.up * base.properties.CurrentState.jumpRoll.JumpHeight.RandomFloat();
			break;
		}
		StartCoroutine(shadow_cr());
		float timePassed = 0f;
		while (timePassed / 0.1665f < 1f)
		{
			if ((float)CupheadTime.Delta != 0f)
			{
				base.transform.position = start + (end - start) * (timePassed / 0.1665f);
				timePassed += (float)CupheadTime.Delta;
			}
			yield return null;
		}
		base.animator.SetTrigger("OnAttack");
		currentJumpAttackCount++;
		StartJumpAttack(jumpType);
	}

	private void StartJumpAttack(JumpType type)
	{
		switch (type)
		{
		case JumpType.DiveKick:
			StartCoroutine(diveKick_cr());
			break;
		case JumpType.DoubleJump:
			StartCoroutine(jumpRoll_cr());
			break;
		}
	}

	private IEnumerator landing_cr(bool useTrigger = true)
	{
		StartCoroutine(shadow_cr(false));
		if (target == null || target.IsDead)
		{
			target = PlayerManager.GetNext();
		}
		if (target.center.x > base.transform.position.x)
		{
			if (base.transform.right.x > 0f)
			{
				if (useTrigger)
				{
					yield return new WaitForEndOfFrame();
					base.animator.SetTrigger("OnTurnLanding");
					yield return base.animator.WaitForAnimationToEnd(this, true);
					base.transform.right *= -1f;
					yield return base.animator.WaitForAnimationToEnd(this, "Land_and_Turn");
				}
				else
				{
					yield return new WaitForEndOfFrame();
					base.animator.Play("Land_and_Turn");
					yield return base.animator.WaitForAnimationToEnd(this, true);
					base.transform.right *= -1f;
					yield return base.animator.WaitForAnimationToEnd(this, "Land_and_Turn");
				}
			}
			else if (useTrigger)
			{
				base.animator.SetTrigger("OnLanding");
				yield return base.animator.WaitForAnimationToEnd(this, "Land", true);
			}
			else
			{
				base.animator.Play("Land");
				yield return base.animator.WaitForAnimationToEnd(this, "Land", true);
			}
		}
		else if (base.transform.right.x < 0f)
		{
			if (useTrigger)
			{
				yield return new WaitForEndOfFrame();
				base.animator.SetTrigger("OnTurnLanding");
				yield return base.animator.WaitForAnimationToEnd(this, true);
				base.transform.right *= -1f;
				yield return base.animator.WaitForAnimationToEnd(this, "Land_and_Turn");
			}
			else
			{
				yield return new WaitForEndOfFrame();
				base.animator.Play("Land_and_Turn");
				yield return base.animator.WaitForAnimationToEnd(this, true);
				base.transform.right *= -1f;
				yield return base.animator.WaitForAnimationToEnd(this, "Land_and_Turn");
			}
		}
		else if (useTrigger)
		{
			base.animator.SetTrigger("OnLanding");
			yield return base.animator.WaitForAnimationToEnd(this, "Land", true);
		}
		else
		{
			base.animator.Play("Land");
			yield return base.animator.WaitForAnimationToEnd(this, "Land", true);
		}
		if (!getOutOfJump)
		{
			StartCoroutine(jump_cr());
		}
		else
		{
			state = State.Idle;
		}
	}

	private void LandSFX()
	{
		AudioManager.Play("sally_sally_land");
		emitAudioFromObject.Add("sally_sally_land");
	}

	private IEnumerator shadow_cr(bool fadeOut = true)
	{
		GameObject shadow = Object.Instantiate(shadowPrefab, new Vector3(base.transform.position.x, Level.Current.Ground, 0f), Quaternion.identity);
		if (fadeOut)
		{
			shadow.GetComponent<Animator>().Play("FadeOut");
		}
		else
		{
			shadow.GetComponent<Animator>().Play("FadeIn");
		}
		yield return shadow.GetComponent<Animator>().WaitForAnimationToEnd(this, true);
		Object.Destroy(shadow);
	}

	private IEnumerator diveKick_cr()
	{
		base.animator.Play("DiveKick_Transition");
		Vector2 direction = -base.transform.right;
		float angle = (float)base.properties.CurrentState.diveKick.DiveAngleRange.RandomInt() / 100f;
		if (angle == 0f)
		{
			angle = 0.001f;
		}
		direction.x = direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle);
		direction.y = direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle);
		direction.y = 0f - Mathf.Abs(direction.y);
		bool attacking = true;
		AudioManager.Play("sally_divekick_loop");
		emitAudioFromObject.Add("sally_divekick_loop");
		Vector3 deltaPosition = Vector3.zero;
		while (attacking)
		{
			if ((float)CupheadTime.Delta != 0f)
			{
				deltaPosition = Vector3.zero;
			}
			if (Mathf.Sign(direction.x) > 0f)
			{
				if (base.transform.position.x + bounds.x / 2f < (float)Level.Current.Right)
				{
					deltaPosition.x = direction.x * base.properties.CurrentState.diveKick.DiveSpeed * (float)CupheadTime.Delta;
				}
			}
			else if (base.transform.position.x - bounds.x / 2f > (float)Level.Current.Left)
			{
				deltaPosition.x = direction.x * base.properties.CurrentState.diveKick.DiveSpeed * (float)CupheadTime.Delta;
			}
			if (base.transform.position.y > ground.y)
			{
				deltaPosition.y = Mathf.Sign(direction.y) * base.properties.CurrentState.diveKick.DiveSpeed * (float)CupheadTime.Delta;
			}
			else
			{
				deltaPosition.y = 0f;
			}
			if (deltaPosition.y == 0f)
			{
				if ((float)CupheadTime.Delta != 0f)
				{
					attacking = false;
				}
			}
			else
			{
				base.transform.position += deltaPosition;
			}
			yield return null;
		}
		StartCoroutine(landing_cr(false));
	}

	private IEnumerator jumpRoll_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, "JumpRoll_Transition", true);
		if (!getOutOfJump)
		{
			StartCoroutine(rollAttack_cr());
			AudioManager.PlayLoop("sally_double_jump_roll_loop");
			emitAudioFromObject.Add("sally_double_jump_roll_loop");
		}
		Vector3 start = base.transform.position;
		Vector3 end = start + Vector3.up * base.properties.CurrentState.jumpRoll.RollJumpVerticalMovement;
		end += -base.transform.right * base.properties.CurrentState.jumpRoll.RollJumpHorizontalMovement.RandomFloat();
		if (end.x - bounds.x / 2f < (float)Level.Current.Left)
		{
			end.x = (float)Level.Current.Left + bounds.x / 2f;
		}
		else if (end.x + bounds.x / 2f > (float)Level.Current.Right)
		{
			end.x = (float)Level.Current.Right - bounds.x / 2f;
		}
		float pct = 0f;
		while (pct < base.properties.CurrentState.jumpRoll.JumpRollDuration)
		{
			base.transform.position = start + (end - start) * pct;
			pct += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return base.animator.WaitForAnimationToEnd(this, "JumpRoll_Roll", true);
		AudioManager.Stop("sally_double_jump_roll_loop");
		StartCoroutine(fall_cr());
		jumpRollAttackTypeIndex++;
		if (jumpRollAttackTypeIndex >= base.properties.CurrentState.jumpRoll.JumpAttackTypeString.Split(',').Length)
		{
			jumpRollAttackTypeIndex = 0;
		}
	}

	private IEnumerator fall_cr()
	{
		float speed = base.properties.CurrentState.teleport.fallingSpeed.RandomFloat();
		int iteration = 1;
		float offset = 150f;
		bool useTrigger = false;
		if (isTeleporting)
		{
			offset = 180f;
			useTrigger = true;
		}
		else
		{
			offset = ground.y;
			useTrigger = false;
		}
		while (base.transform.position.y > offset)
		{
			base.transform.position += Vector3.down * speed * CupheadTime.Delta;
			if ((float)CupheadTime.Delta != 0f)
			{
				speed += base.properties.CurrentState.teleport.acceleration * (float)iteration;
				iteration++;
			}
			yield return null;
		}
		base.transform.position = new Vector3(base.transform.position.x, offset, 0f);
		if (isTeleporting)
		{
			base.animator.SetTrigger("OnSawEnd");
		}
		StartCoroutine(landing_cr(useTrigger));
		yield return null;
	}

	private IEnumerator rollAttack_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.jumpRoll.RollShotDelayRange.RandomFloat());
		switch (base.properties.CurrentState.jumpRoll.JumpAttackTypeString.Split(',')[jumpRollAttackTypeIndex][0])
		{
		case 'S':
			SpawnShuriken();
			break;
		case 'B':
			SpawnProjectile();
			break;
		}
	}

	private void SpawnShuriken()
	{
		for (int i = -1; i < 1; i++)
		{
			AbstractProjectile abstractProjectile = shurikenPrefab.Create(base.transform.position + Vector3.up * 0.5f);
			abstractProjectile.GetComponent<SallyStagePlayLevelShurikenBomb>().InitShuriken(base.properties, i, target);
		}
	}

	private void SpawnProjectile()
	{
		Vector3 vector = target.transform.position - centerPoint.transform.position;
		SallyStagePlayLevelProjectile sallyStagePlayLevelProjectile = Object.Instantiate(projectilePrefab);
		sallyStagePlayLevelProjectile.Init(centerPoint.transform.position, MathUtils.DirectionToAngle(vector), base.properties.CurrentState.projectile);
	}

	public void OnUmbrellaAttack()
	{
		state = State.Attack;
		StartCoroutine(startUmbrella_cr());
	}

	private IEnumerator startUmbrella_cr()
	{
		base.animator.SetBool("UmbrellaAttack", true);
		yield return base.animator.WaitForAnimationToEnd(this, "Umbrella_Spin_Start");
		AudioManager.Play("sally_umbrella_spin");
		emitAudioFromObject.Add("sally_umbrella_spin");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.umbrella.initialAttackDelay);
		for (int i = 0; i < base.properties.CurrentState.umbrella.objectCount; i++)
		{
			if (i != 0)
			{
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.umbrella.objectDelay);
			}
			AudioManager.Play("sally_umbrella_spin_shoot");
			emitAudioFromObject.Add("sally_umbrella_spin_shoot");
			AbstractProjectile proj = umbrellaProjectilePrefab.Create(spawnPoints[0].position);
			proj.GetComponent<SallyStagePlayLevelUmbrellaProjectile>().InitProjectile(base.properties, (int)(0f - base.transform.right.x), parent);
			proj = umbrellaProjectilePrefab.Create(spawnPoints[1].position);
			proj.GetComponent<SallyStagePlayLevelUmbrellaProjectile>().InitProjectile(base.properties, (int)base.transform.right.x, parent);
			if (getOutOfJump)
			{
				break;
			}
		}
		base.animator.SetBool("UmbrellaAttack", false);
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.umbrella.hesitate);
		AudioManager.Play("sally_umbrella_spin_end");
		emitAudioFromObject.Add("sally_umbrella_spin_end");
		state = State.Idle;
	}

	private void UmbrellaIntroSFX()
	{
		AudioManager.Play("sally_sally_umbrella_intro");
		emitAudioFromObject.Add("sally_sally_umbrella_intro");
	}

	public void OnKissAttack()
	{
		state = State.Attack;
		base.animator.SetTrigger("OnKissAttack");
		target = PlayerManager.GetNext();
		if (target.center.x > centerPoint.position.x)
		{
			if (base.transform.eulerAngles.y == 0f)
			{
				base.transform.right *= -1f;
			}
		}
		else if (base.transform.eulerAngles.y == 180f)
		{
			base.transform.right *= -1f;
		}
	}

	private void SpawnHeart()
	{
		AbstractProjectile abstractProjectile = heartPrefab.Create(spawnPoints[0].position);
		bool isParryable = ((base.properties.CurrentState.kiss.heartType.Split(',')[heartTypeIndex][0] != 'R') ? true : false);
		int direction = (((int)base.transform.eulerAngles.y != 180) ? 1 : (-1));
		abstractProjectile.GetComponent<SallyStagePlayLevelHeart>().InitHeart(base.properties, direction, isParryable);
		abstractProjectile.GetComponent<Transform>().SetScale((base.transform.right.x > 0f) ? 1 : (-1));
		heartTypeIndex++;
		if (heartTypeIndex >= base.properties.CurrentState.kiss.heartType.Split(',').Length)
		{
			heartTypeIndex = 0;
		}
		StartCoroutine(endKiss_cr());
	}

	private void KissSFX()
	{
		AudioManager.Play("sally_sally_kiss");
		emitAudioFromObject.Add("sally_sally_kiss");
	}

	private IEnumerator endKiss_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.kiss.hesitate);
		state = State.Idle;
	}

	public void OnTeleportAttack()
	{
		state = State.Attack;
		isTeleporting = true;
		base.animator.SetTrigger("OnTeleport");
	}

	private void Teleport()
	{
		base.transform.SetPosition(null, (float)Level.Current.Ceiling + teleportOffset);
		StartCoroutine(delay_cr());
	}

	private IEnumerator delay_cr()
	{
		Vector3 pos = default(Vector3);
		pos.y = (float)Level.Current.Ceiling + teleportOffset;
		pos.z = 0f;
		base.animator.SetTrigger("OnTeleport");
		yield return base.animator.WaitForAnimationToStart(this, "Teleport_Loop");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.teleport.offScreenDelay);
		target = PlayerManager.GetNext();
		pos.x = target.center.x + (float)int.Parse(base.properties.CurrentState.teleport.appearOffsetString.Split(',')[teleportOffsetIndex]);
		if (int.Parse(base.properties.CurrentState.teleport.appearOffsetString.Split(',')[teleportOffsetIndex]) <= 0)
		{
			if (pos.x - 75f < (float)Level.Current.Left)
			{
				pos.x = Level.Current.Left + 75;
			}
			base.transform.right *= -1f;
		}
		else
		{
			if (pos.x + 75f > (float)Level.Current.Right)
			{
				pos.x = Level.Current.Right - 75;
			}
			base.transform.right *= 1f;
		}
		base.transform.position = pos;
		StartCoroutine(fall_cr());
		yield return base.animator.WaitForAnimationToStart(this, "Idle");
		teleportOffsetIndex++;
		if (teleportOffsetIndex >= base.properties.CurrentState.teleport.appearOffsetString.Split(',').Length)
		{
			teleportOffsetIndex = 0;
		}
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.teleport.hesitate);
		base.transform.position = ground;
		isTeleporting = false;
		yield return null;
	}

	private void MovePosition()
	{
		StartCoroutine(move_position_cr());
	}

	private IEnumerator move_position_cr()
	{
		Vector3 pos = base.transform.position;
		float speed = 700f;
		while (base.transform.position.y > ground.y)
		{
			pos.y -= speed * (float)CupheadTime.Delta;
			base.transform.position = pos;
			yield return null;
		}
		base.transform.position = ground;
		yield return null;
	}

	private void TeleportOutSFX()
	{
		AudioManager.Play("sally_sally_teleport_out");
		emitAudioFromObject.Add("sally_sally_teleport_out");
	}

	private void TeleportEndSFX()
	{
		AudioManager.Play("sally_sally_teleport_end");
		emitAudioFromObject.Add("sally_sally_teleport_end");
	}

	public void OnPhase3(bool killedHusband)
	{
		StopAllCoroutines();
		StartCoroutine(phase2_death_cr(killedHusband));
	}

	private IEnumerator phase2_death_cr(bool killedHusband)
	{
		float speed = 300f;
		GetComponent<LevelBossDeathExploder>().StartExplosion();
		base.animator.SetTrigger("OnDeath");
		AudioManager.Play("sally_vox_death_cry");
		emitAudioFromObject.Add("sally_vox_death_cry");
		yield return base.animator.WaitForAnimationToEnd(this, "Death_Ph2_Start");
		while (base.transform.position.y < 660f)
		{
			base.transform.position += Vector3.up * speed * CupheadTime.Delta;
			yield return null;
		}
		GetComponent<LevelBossDeathExploder>().StopExplosions();
		angel.StartPhase3(killedHusband);
		Object.Destroy(base.gameObject);
		yield return null;
	}

	public void PrePhase2()
	{
		getOutOfJump = true;
		isInvincible = true;
	}

	public void OnPhase2()
	{
		state = State.Transition;
		jumpTypeIndex = 0;
		jumpRollAttackTypeIndex = 0;
	}

	public void StartPhase2()
	{
		getOutOfJump = true;
		base.animator.SetTrigger("OnIntro");
		StartCoroutine(phase_2_cr());
	}

	private void Intro2SFX()
	{
		AudioManager.Play("sally_sally_intro_phase2");
		emitAudioFromObject.Add("sally_sally_intro_phase2");
	}

	private IEnumerator phase_2_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, "Teleport_GONE");
		StartCoroutine(slide_cr());
		yield return base.animator.WaitForAnimationToStart(this, "Idle");
		isInvincible = false;
		getOutOfJump = false;
		yield return CupheadTime.WaitForSeconds(this, 1f);
		state = State.Idle;
		house.StartAttacks();
		yield return null;
	}

	private IEnumerator slide_cr()
	{
		float startPos = 0f;
		float endPos = 0f;
		float appearPos = 300f;
		AbstractPlayerController player1 = PlayerManager.GetPlayer(PlayerId.PlayerOne);
		AbstractPlayerController player2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
		if (player2 == null || player1.IsDead || player2.IsDead)
		{
			if (target == null || target.IsDead)
			{
				target = PlayerManager.GetNext();
			}
			if (target.transform.position.x > 0f)
			{
				if (base.transform.right.x > 0f)
				{
					base.transform.right *= -1f;
				}
				startPos = -840f;
				endPos = -640f + appearPos;
			}
			else
			{
				if (base.transform.right.x < 0f)
				{
					base.transform.right *= -1f;
				}
				startPos = 840f;
				endPos = 640f - appearPos;
			}
		}
		else
		{
			float num = -640f - player1.transform.position.x;
			float num2 = 640f - player1.transform.position.x;
			float num3 = -640f - player2.transform.position.x;
			float num4 = 640f - player2.transform.position.x;
			if (player1.transform.position.x < 0f)
			{
				if (player2.transform.position.x < 0f)
				{
					if (base.transform.right.x < 0f)
					{
						base.transform.right *= -1f;
					}
					startPos = 840f;
					endPos = 640f - appearPos;
				}
				else if (num < num4)
				{
					if (base.transform.right.x < 0f)
					{
						base.transform.right *= -1f;
					}
					startPos = 840f;
					endPos = 640f - appearPos;
				}
				else
				{
					if (base.transform.right.x > 0f)
					{
						base.transform.right *= -1f;
					}
					startPos = -840f;
					endPos = -640f + appearPos;
				}
			}
			else if (player2.transform.position.x > 0f)
			{
				if (base.transform.right.x > 0f)
				{
					base.transform.right *= -1f;
				}
				startPos = -840f;
				endPos = -640f + appearPos;
			}
			else if (num2 < num3)
			{
				if (base.transform.right.x < 0f)
				{
					base.transform.right *= -1f;
				}
				startPos = 840f;
				endPos = 640f - appearPos;
			}
			else
			{
				if (base.transform.right.x > 0f)
				{
					base.transform.right *= -1f;
				}
				startPos = -840f;
				endPos = -640f + appearPos;
			}
		}
		base.transform.position = new Vector3(startPos, base.transform.position.y, base.transform.position.z);
		float t = 0f;
		float time = 0.75f;
		YieldInstruction wait = new WaitForFixedUpdate();
		float frameTime = 0f;
		while (t < time)
		{
			t += CupheadTime.FixedDelta;
			frameTime += CupheadTime.FixedDelta;
			if (frameTime > 1f / 24f)
			{
				frameTime -= 1f / 24f;
				float t2 = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
				base.transform.SetPosition(Mathf.Lerp(startPos, endPos, t2));
			}
			yield return wait;
		}
		yield return null;
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}

	private void SoundSallyVoxAttackMmmYoh()
	{
		AudioManager.Play("sally_vox_attack_mmm_yoh");
		emitAudioFromObject.Add("sally_vox_attack_mmm_yoh");
	}

	private void SoundSallyVoxAttackQuick()
	{
		AudioManager.Play("sally_vox_attack_quick");
		emitAudioFromObject.Add("sally_vox_attack_quick");
		AudioManager.Stop("sally_vox_maniacal");
	}

	private void SoundSallyVoxAttackDeathCry()
	{
		AudioManager.Play("sally_vox_death_cry");
		emitAudioFromObject.Add("sally_vox_death_cry");
	}

	private void SoundSallyVoxFrustrated()
	{
		AudioManager.Play("sally_vox_frustrated");
		emitAudioFromObject.Add("sally_vox_frustrated");
	}

	private void SoundSallyVoxLaughBig()
	{
		AudioManager.Play("sally_vox_laugh_big");
		emitAudioFromObject.Add("sally_vox_laugh_big");
	}

	private void SoundSallyVoxLaughSmall()
	{
		AudioManager.Play("sally_vox_laugh_small");
		emitAudioFromObject.Add("sally_vox_laugh_small");
	}

	private void SoundSallyVoxLaughManiacal()
	{
		AudioManager.Play("sally_vox_maniacal");
		emitAudioFromObject.Add("sally_vox_maniacal");
	}

	private void SoundSallyVoxDeathOperatic()
	{
		AudioManager.Play("sally_vox_operatic_death");
		emitAudioFromObject.Add("sally_vox_operatic_death");
	}

	private void SoundSallyVoxPainGrowl()
	{
		AudioManager.Play("sally_vox_pain_growl");
		emitAudioFromObject.Add("sally_vox_pain_growl");
	}
}
