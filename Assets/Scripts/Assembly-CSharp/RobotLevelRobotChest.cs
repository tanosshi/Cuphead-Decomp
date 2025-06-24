using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RobotLevelRobotChest : RobotLevelRobotBodyPart
{
	public enum AnimationLayers
	{
		Main = 0,
		Pilot = 1,
		Head = 2,
		Hose = 3,
		Waist = 4,
		BackArm = 5,
		FrontArm = 6,
		Torso = 7
	}

	private const int PANIC_ARMS_ANIM_FRAME_COUNT = 24;

	private int frameCount;

	private bool playedCrackedSound;

	private bool armsActive;

	private bool panicArmsLoop;

	private int armsTypeIndex;

	private int twistyArmsPositionIndex;

	private Vector3 spriteBounds;

	private Vector3 origin;

	private Animator secondaryAnimator;

	private PlanePlayerMotor.Force force;

	[SerializeField]
	private SpriteRenderer torsoTop;

	[SerializeField]
	private GameObject[] damagedPortholes;

	[SerializeField]
	private GameObject frontArm;

	[SerializeField]
	private GameObject backArm;

	[SerializeField]
	private Transform portholeRoot;

	[SerializeField]
	private Transform portholeOffsetRoot;

	[SerializeField]
	private Transform[] panicArmsPath;

	[SerializeField]
	private Transform magnetStartRoot;

	[SerializeField]
	private Transform magnetEndRoot;

	[SerializeField]
	private GameObject damageEffect;

	private IEnumerator damageEffectRoutine;

	private SpriteRenderer damageEffectRenderer;

	public override void InitBodyPart(RobotLevelRobot parent, LevelProperties.Robot properties, int primaryHP = 0, int secondaryHP = 1, float attackDelayMinus = 0f)
	{
		origin = base.transform.position;
		primaryAttackDelay = properties.CurrentState.orb.orbInitialSpawnDelay.RandomFloat();
		secondaryAttackDelay = properties.CurrentState.arms.attackDelayRange.RandomFloat();
		primaryHP = properties.CurrentState.orb.chestHP;
		secondaryHP = properties.CurrentState.heart.heartHP;
		attackDelayMinus = properties.CurrentState.orb.orbSpawnDelayMinus;
		armsTypeIndex = Random.Range(0, properties.CurrentState.arms.attackString.Split(',').Length);
		twistyArmsPositionIndex = Random.Range(0, properties.CurrentState.twistyArms.twistyPositionString.Split(',').Length);
		base.InitBodyPart(parent, properties, primaryHP, secondaryHP, attackDelayMinus);
		base.animator.Play("Porthole_Idle", 0, 0.75f);
		base.animator.Play("Off_Idle", 1, 0.75f);
		StartCoroutine(panicArmsLoop_cr());
		StartCoroutine(close_porthole_cr());
		StartPrimary();
		damageEffectRenderer = damageEffect.GetComponent<SpriteRenderer>();
	}

	protected override void OnPrimaryAttack()
	{
		if (current == state.primary)
		{
			base.animator.SetTrigger("OnPortOpen");
			base.OnPrimaryAttack();
		}
	}

	private void SpawnOrb()
	{
		StartCoroutine(spawn_orb_cr());
	}

	private IEnumerator spawn_orb_cr()
	{
		RobotLevelOrb orb = primary.GetComponent<RobotLevelOrb>().Create(portholeRoot.transform.position, portholeOffsetRoot.position);
		primaryAttackDelay = properties.CurrentState.orb.orbSpawnDelay;
		orb.InitOrb(this, properties);
		isAttacking = false;
		base.animator.SetTrigger("OnFirstClose");
		yield return null;
	}

	private IEnumerator close_porthole_cr()
	{
		base.animator.SetTrigger("OnFirstClose");
		yield return null;
	}

	protected override void OnPrimaryDeath()
	{
		if (current != state.secondary && currentHealth[0] <= 0f)
		{
			AudioManager.Play("robot_upper_chest_port_destroyed");
			emitAudioFromObject.Add("robot_upper_chest_port_destroyed");
			torsoTop.enabled = false;
			StartSecondary();
			GetComponent<Collider2D>().enabled = false;
			DeathEffect();
			panicArmsLoop = true;
			parent.animator.Play("Transition_Arms");
			SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				spriteRenderer.enabled = false;
			}
			GameObject[] array = damagedPortholes;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(true);
				gameObject.GetComponent<SpriteRenderer>().enabled = true;
			}
		}
		base.OnPrimaryDeath();
	}

	private void EnablePorthole()
	{
		if (current == state.primary)
		{
			SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				spriteRenderer.enabled = true;
			}
		}
	}

	private void DisablePorthole()
	{
		if (current == state.primary)
		{
			SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				spriteRenderer.enabled = false;
			}
		}
	}

	protected override void StartSecondary()
	{
		secondary = Object.Instantiate(secondary);
		secondaryAnimator = secondary.GetComponent<Animator>();
		StartCoroutine(secondaryStart_cr());
	}

	private IEnumerator secondaryStart_cr()
	{
		yield return parent.animator.WaitForAnimationToEnd(parent, "Extend", 5, true);
		_003CStartSecondary_003E__BaseCallProxy0();
	}

	protected override void OnSecondaryAttack()
	{
		if (!armsActive)
		{
			if ((float)Random.Range(0, 100) <= 25f && !AudioManager.CheckIfPlaying("robot_vocals_laugh"))
			{
				AudioManager.Play("robot_vocals_laugh");
				emitAudioFromObject.Add("robot_vocals_laugh");
			}
			parent.animator.Play("Fast", 5, parent.animator.GetCurrentAnimatorStateInfo(5).normalizedTime % 1f);
			secondaryAttackDelay = 0f;
			armsActive = true;
			switch (properties.CurrentState.arms.attackString.Split(',')[armsTypeIndex][0])
			{
			case 'M':
				StartCoroutine(magnetArmsIntro_cr());
				break;
			case 'T':
				StartCoroutine(twistyArmsEnter_cr());
				break;
			}
		}
		base.OnSecondaryAttack();
	}

	private IEnumerator twistyArmsEnter_cr()
	{
		secondary.GetComponent<RobotLevelSecondaryArms>().InitHelper(properties);
		secondaryAnimator.Play("Twisty_Arms_Enter", 0, 0f);
		AudioManager.Play("robot_arms_extend_appear");
		yield return null;
		spriteBounds = secondary.GetComponent<SpriteRenderer>().bounds.size;
		secondary.transform.position = new Vector3(-1248f, Level.Current.Ground + int.Parse(properties.CurrentState.twistyArms.twistyPositionString.Split(',')[twistyArmsPositionIndex]), 0f);
		secondary.transform.rotation = Quaternion.identity;
		while (secondary.transform.position.x < -1248f + properties.CurrentState.twistyArms.warningArmsMoveAmount)
		{
			secondary.transform.position += Vector3.right * properties.CurrentState.twistyArms.twistyMoveSpeed * CupheadTime.Delta;
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.twistyArms.warningDuration);
		secondary.GetComponent<BoxCollider2D>().enabled = true;
		AudioManager.Play("robot_arms_extend_across");
		emitAudioFromObject.Add("robot_arms_extend_across");
		while (secondary.transform.position.x < -1248f + spriteBounds.x / 8f * 6f)
		{
			secondary.transform.position += Vector3.right * properties.CurrentState.twistyArms.twistyMoveSpeed * CupheadTime.Delta;
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.twistyArms.twistyArmsStayDuration);
		AudioManager.Play("robot_arms_extend_back");
		emitAudioFromObject.Add("robot_arms_extend_back");
		StartCoroutine(twistyArmsExit_cr());
	}

	private IEnumerator twistyArmsExit_cr(bool isPermaDeath = false)
	{
		float speedMultiplier = 1f;
		if (isPermaDeath)
		{
			speedMultiplier = 2f;
		}
		float nt = base.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
		secondaryAnimator.Play("Twisty_Arms_Exit", -1, nt);
		while (secondary.transform.position.x > -1248f)
		{
			secondary.transform.position += Vector3.left * properties.CurrentState.twistyArms.twistyMoveSpeed * speedMultiplier * CupheadTime.Delta;
			yield return null;
		}
		secondaryAnimator.Play("Twisty_Arms_Enter");
		twistyArmsPositionIndex++;
		if (twistyArmsPositionIndex >= properties.CurrentState.twistyArms.twistyPositionString.Split(',').Length)
		{
			twistyArmsPositionIndex = 0;
		}
		secondaryAttackDelay = properties.CurrentState.arms.attackDelayRange.RandomFloat();
		yield return null;
		secondary.GetComponent<BoxCollider2D>().enabled = false;
		armsActive = false;
		armsTypeIndex++;
		if (armsTypeIndex >= properties.CurrentState.arms.attackString.Split(',').Length)
		{
			armsTypeIndex = 0;
		}
		parent.animator.Play("Slow", 5, parent.animator.GetCurrentAnimatorStateInfo(5).normalizedTime % 1f);
	}

	private IEnumerator magnetArmsIntro_cr()
	{
		secondaryAnimator.Play("Magnet_Arms", -1, 0f);
		yield return null;
		spriteBounds = secondary.GetComponent<SpriteRenderer>().bounds.size;
		secondary.transform.position = magnetStartRoot.transform.position;
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.magnetArms.magnetStartDelay);
		AudioManager.Play("robot_magnet_arms_start");
		while (AudioManager.CheckIfPlaying("robot_magnet_arms_start"))
		{
			yield return null;
		}
		AudioManager.PlayLoop("robot_magnet_arms_loop");
		float t = 0f;
		float time = 1.8f;
		float deltaRotation = 0f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			deltaRotation = Mathf.Lerp(60f, 0f, t / time);
			secondary.transform.position = Vector2.Lerp(magnetStartRoot.transform.position, magnetEndRoot.transform.position, t / time);
			secondary.transform.SetEulerAngles(null, null, deltaRotation);
			yield return null;
		}
		time = 0f;
		while (time < properties.CurrentState.magnetArms.magnetStayDelay)
		{
			time += (float)CupheadTime.Delta;
			PlanePlayerMotor[] array = Object.FindObjectsOfType<PlanePlayerMotor>();
			foreach (PlanePlayerMotor player in array)
			{
				Vector2 playerForce = (PlayerManager.GetNext().center - secondary.transform.GetChild(1).transform.position).normalized * properties.CurrentState.magnetArms.magnetForce * 0.5f;
				force = new PlanePlayerMotor.Force(playerForce, true);
				player.AddForce(force);
				yield return null;
				time += (float)CupheadTime.Delta;
				if (player != null)
				{
					player.RemoveForce(force);
				}
			}
			if (current == state.none)
			{
				time = properties.CurrentState.magnetArms.magnetStayDelay;
			}
			yield return null;
		}
		AudioManager.Stop("robot_magnet_arms_loop");
		AudioManager.Play("robot_magnet_arms_end");
		StartCoroutine(magnetArmsExit_cr());
	}

	private IEnumerator magnetArmsExit_cr(bool isPermaDeath = false)
	{
		PlanePlayerMotor[] array = Object.FindObjectsOfType<PlanePlayerMotor>();
		foreach (PlanePlayerMotor planePlayerMotor in array)
		{
			planePlayerMotor.RemoveForce(force);
		}
		float speedMultiplier = 1f;
		if (isPermaDeath)
		{
			speedMultiplier = 2f;
		}
		float delay = (1f - secondaryAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime % 1f) * secondaryAnimator.GetCurrentAnimatorStateInfo(1).length;
		secondaryAnimator.Play("Magnet_Arms", -1, 0f);
		yield return CupheadTime.WaitForSeconds(this, delay);
		float t = 0f;
		float time = 1.8f;
		float deltaRotation = 0f;
		Vector3 root = ((!isPermaDeath) ? magnetEndRoot.transform.position : secondary.transform.position);
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			deltaRotation = Mathf.Lerp(0f, 60f, t / time);
			secondary.transform.position = Vector2.Lerp(root, magnetStartRoot.transform.position, t / time);
			secondary.transform.SetEulerAngles(null, null, deltaRotation);
			yield return null;
		}
		secondaryAttackDelay = properties.CurrentState.arms.attackDelayRange.RandomFloat();
		yield return null;
		armsActive = false;
		armsTypeIndex++;
		if (armsTypeIndex >= properties.CurrentState.arms.attackString.Split(',').Length)
		{
			armsTypeIndex = 0;
		}
		parent.animator.Play("Slow", 5, parent.animator.GetCurrentAnimatorStateInfo(5).normalizedTime % 1f);
	}

	private IEnumerator panicArmsLoop_cr()
	{
		while (true)
		{
			float normalizedTime = parent.animator.GetCurrentAnimatorStateInfo(7).normalizedTime;
			normalizedTime %= 1f;
			int currentFrame = (int)(normalizedTime * 24f);
			if (panicArmsLoop)
			{
				frontArm.transform.position = panicArmsPath[currentFrame].position;
				int num = currentFrame + 10;
				if (num >= 24)
				{
					num -= 24;
				}
				backArm.transform.position = panicArmsPath[num].position;
			}
			currentFrame++;
			if (currentFrame >= 24)
			{
				currentFrame = 0;
			}
			Vector3 originalPos = base.transform.position;
			float frameRate = (float)currentFrame / 24f;
			yield return null;
		}
	}

	protected override void OnSecondaryDeath()
	{
		StartCoroutine(heart_cr());
		base.OnSecondaryDeath();
	}

	private void HeartIntroSFX()
	{
		AudioManager.Play("robot_heart_spring_out");
		emitAudioFromObject.Add("robot_heart_spring_out");
	}

	private IEnumerator heart_cr()
	{
		GetComponent<SpriteRenderer>().enabled = true;
		float waitDuration = parent.animator.GetCurrentAnimatorStateInfo(2).length;
		yield return CupheadTime.WaitForSeconds(this, waitDuration);
		base.animator.SetTrigger("OnHeartActive");
		GetComponent<Collider2D>().enabled = true;
		yield return base.animator.WaitForAnimationToEnd(this, "Intro", 1, true);
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (current == state.primary)
		{
			base.OnDamageTaken(info);
			if (damageEffectRoutine != null)
			{
				StopCoroutine(damageEffectRoutine);
			}
			damageEffectRoutine = damageEffect_cr();
			StartCoroutine(damageEffectRoutine);
			return;
		}
		currentHealth[1] -= info.damage;
		if (currentHealth[1] / (float)properties.CurrentState.heart.heartHP * 100f <= (float)properties.CurrentState.heart.heartDamageChangePercentage)
		{
			if (!playedCrackedSound)
			{
				AudioManager.Play("robot_heart_spring_cracked");
				emitAudioFromObject.Add("robot_heart_spring_cracked");
				playedCrackedSound = true;
			}
			base.animator.Play("Damaged Loop", 1, base.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f);
		}
		if (currentHealth[1] <= 0f)
		{
			AudioManager.Stop("robot_magnet_arms_loop");
			AudioManager.Play("robot_heart_spring_destroyed");
			emitAudioFromObject.Add("robot_heart_spring_destroyed");
			properties.DealDamageToNextNamedState();
		}
		base.OnDamageTaken(info);
	}

	protected override void ExitCurrentAttacks()
	{
		StopAllCoroutines();
		bool isPermaDeath = false;
		if (Level.Current.mode == Level.Mode.Easy)
		{
			isPermaDeath = true;
			secondary.GetComponent<RobotLevelSecondaryArms>().BossAlive = false;
		}
		switch (properties.CurrentState.arms.attackString.Split(',')[armsTypeIndex][0])
		{
		case 'T':
			StartCoroutine(twistyArmsExit_cr(isPermaDeath));
			break;
		case 'M':
			StartCoroutine(magnetArmsExit_cr(isPermaDeath));
			break;
		}
		base.ExitCurrentAttacks();
	}

	public void InitAnims()
	{
		base.animator.SetTrigger("OnRobotIntro");
	}

	protected override void Die()
	{
		if (damageEffectRoutine != null)
		{
			StopCoroutine(damageEffectRoutine);
		}
		damageEffect.SetActive(false);
		SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteRenderer.enabled = false;
		}
		base.Die();
	}

	private IEnumerator damageEffect_cr()
	{
		for (int i = 0; i < 3; i++)
		{
			damageEffectRenderer.enabled = true;
			damageEffect.SetActive(true);
			yield return CupheadTime.WaitForSeconds(this, 1f / 24f);
			damageEffect.SetActive(false);
			yield return CupheadTime.WaitForSeconds(this, 1f / 24f);
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CStartSecondary_003E__BaseCallProxy0()
	{
		base.StartSecondary();
	}
}
