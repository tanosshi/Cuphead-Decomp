using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilLevelSittingDevil : LevelProperties.Devil.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Clap = 2,
		Head = 3,
		Pitchfork = 4,
		EndPhase1 = 5
	}

	public State state;

	[SerializeField]
	private GameObject middleGround;

	[SerializeField]
	private DevilLevelGiantHead giantHead;

	[SerializeField]
	private DevilLevelDemon demonPrefab;

	[SerializeField]
	private Transform leftDemonPeek;

	[SerializeField]
	private Transform leftDemonJumpRoot;

	[SerializeField]
	private Transform leftDemonRunRoot;

	[SerializeField]
	private Transform leftDemonPillar;

	[SerializeField]
	private Transform leftDemonFront;

	[SerializeField]
	private Transform rightDemonPeek;

	[SerializeField]
	private Transform rightDemonJumpRoot;

	[SerializeField]
	private Transform rightDemonRunRoot;

	[SerializeField]
	private Transform rightDemonPillar;

	[SerializeField]
	private Transform rightDemonFront;

	[SerializeField]
	private DevilLevelDevilArm[] arms;

	[SerializeField]
	private DevilLevelSpiderHead spiderHead;

	[SerializeField]
	private DevilLevelDragonHead dragonHead;

	[SerializeField]
	private Transform leftWall;

	private float leftWallPositionX;

	[SerializeField]
	private Transform rightWall;

	private float rightWallPositionX;

	[SerializeField]
	private DevilLevelSplitDevil[] splitDevils;

	[SerializeField]
	private Transform platforms;

	[SerializeField]
	private DevilLevelPitchforkWheelProjectile wheelProjectilePrefab;

	[SerializeField]
	private DevilLevelPitchforkOrbitingProjectile wheelOrbitingProjectilePrefab;

	[SerializeField]
	private DevilLevelPitchforkJumpingProjectile jumpingProjectilePrefab;

	[SerializeField]
	private DevilLevelPitchforkBouncingProjectile bouncingProjectilePrefab;

	[SerializeField]
	private DevilLevelPitchforkSpinnerProjectile spinnerProjectilePrefab;

	[SerializeField]
	private DevilLevelPitchforkOrbitingProjectile spinnerOrbitingProjectilePrefab;

	[SerializeField]
	private DevilLevelPitchforkRingProjectile ringProjectilePrefab;

	[SerializeField]
	private GameObject holeSign;

	private Vector3 dragonPos;

	private DamageReceiver damageReceiver;

	private bool isSpiderAttackNext;

	private bool endFire;

	private bool endPH1;

	private int spiderOffsetIndex;

	private string[] spiderOffsets;

	private int dragonOffsetIndex;

	private string[] dragonOffsets;

	private int pitchforkPatternIndex;

	private string[] pitchforkPattern;

	private DevilLevelPitchforkProjectileSpawner pitchforkTwoFlameWheelSpawner;

	private DevilLevelPitchforkProjectileSpawner pitchforkThreeFlameJumperSpawner;

	private DevilLevelPitchforkProjectileSpawner pitchforkFourFlameBouncerSpawner;

	private DevilLevelPitchforkProjectileSpawner pitchforkFiveFlameSpinnerSpawner;

	private DevilLevelPitchforkProjectileSpawner pitchforkSixFlameRingSpawner;

	public Action OnPhase1Death;

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		leftWallPositionX = leftWall.transform.position.x;
		rightWallPositionX = rightWall.transform.position.x;
	}

	protected override void Start()
	{
		base.Start();
		dragonPos = dragonHead.transform.position;
		StartCoroutine(intro_cr());
	}

	public override void LevelInit(LevelProperties.Devil properties)
	{
		base.LevelInit(properties);
		isSpiderAttackNext = Rand.Bool();
		spiderOffsets = properties.CurrentState.spider.positionOffset.Split(',');
		spiderOffsetIndex = UnityEngine.Random.Range(0, spiderOffsets.Length);
		dragonOffsets = properties.CurrentState.dragon.positionOffset.Split(',');
		dragonOffsetIndex = UnityEngine.Random.Range(0, dragonOffsets.Length);
		pitchforkPattern = properties.CurrentState.pitchfork.patternString.RandomChoice().Split(',');
		pitchforkPatternIndex = UnityEngine.Random.Range(0, pitchforkPattern.Length);
		pitchforkTwoFlameWheelSpawner = new DevilLevelPitchforkProjectileSpawner(2, properties.CurrentState.pitchforkTwoFlameWheel.angleOffset);
		pitchforkThreeFlameJumperSpawner = new DevilLevelPitchforkProjectileSpawner(3, properties.CurrentState.pitchforkThreeFlameJumper.angleOffset);
		pitchforkFourFlameBouncerSpawner = new DevilLevelPitchforkProjectileSpawner(4, properties.CurrentState.pitchforkFourFlameBouncer.angleOffset);
		pitchforkFiveFlameSpinnerSpawner = new DevilLevelPitchforkProjectileSpawner(4, properties.CurrentState.pitchforkFiveFlameSpinner.angleOffset);
		pitchforkSixFlameRingSpawner = new DevilLevelPitchforkProjectileSpawner(6, properties.CurrentState.pitchforkSixFlameRing.angleOffset);
		if (Level.Difficulty == Level.Mode.Easy)
		{
			properties.OnBossDeath += DeathEasy;
		}
	}

	private IEnumerator intro_cr()
	{
		state = State.Intro;
		yield return CupheadTime.WaitForSeconds(this, 0.25f);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro");
		state = State.Idle;
		yield return null;
	}

	public void StartDemons()
	{
		StartCoroutine(demon_cr());
	}

	private IEnumerator demon_cr()
	{
		bool fromLeft = Rand.Bool();
		bool playedFirstSound = false;
		yield return CupheadTime.WaitForSeconds(this, 3f);
		while (!endPH1)
		{
			yield return null;
			if (playedFirstSound)
			{
				AudioManager.Play("devil_small_flame_imp_spawn");
				emitAudioFromObject.Add("devil_small_flame_imp_spawn");
			}
			else
			{
				AudioManager.Play("devil_small_flame_imp_first_spawn");
				emitAudioFromObject.Add("devil_small_flame_imp_first_spawn");
				playedFirstSound = true;
			}
			DevilLevelDemon demon = demonPrefab.Create((!fromLeft) ? rightDemonPeek.position : leftDemonPeek.position, fromLeft ? 1 : (-1), base.properties.CurrentState.demons.speed, base.properties.CurrentState.demons.hp, this);
			if (fromLeft)
			{
				demon.JumpRoot = leftDemonJumpRoot.position;
				demon.RunRoot = leftDemonRunRoot.position;
				demon.PillarDestination = leftDemonPillar.position;
				demon.FrontSpawn = leftDemonFront.position;
			}
			else
			{
				demon.JumpRoot = rightDemonJumpRoot.position;
				demon.RunRoot = rightDemonRunRoot.position;
				demon.PillarDestination = rightDemonPillar.position;
				demon.FrontSpawn = rightDemonFront.position;
			}
			fromLeft = !fromLeft;
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.demons.delay);
		}
	}

	public void StartClap()
	{
		state = State.Clap;
		StartCoroutine(clap_cr());
	}

	private IEnumerator clap_cr()
	{
		LevelProperties.Devil.Clap p = base.properties.CurrentState.clap;
		base.animator.SetBool("StartRam", true);
		yield return base.animator.WaitForAnimationToEnd(this, "Ram_Start");
		yield return CupheadTime.WaitForSeconds(this, p.delay.RandomFloat());
		DevilLevelDevilArm[] array = arms;
		foreach (DevilLevelDevilArm devilLevelDevilArm in array)
		{
			devilLevelDevilArm.Attack(p.speed);
		}
		while (arms[0].state != DevilLevelDevilArm.State.Idle)
		{
			yield return null;
		}
		base.animator.SetBool("StartRam", false);
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
	}

	public void StartHead()
	{
		state = State.Head;
		if (isSpiderAttackNext)
		{
			StartCoroutine(spider_cr());
		}
		else
		{
			StartCoroutine(dragon_cr());
		}
		isSpiderAttackNext = !isSpiderAttackNext;
	}

	private IEnumerator spider_cr()
	{
		base.animator.SetBool("StartSpider", true);
		yield return base.animator.WaitForAnimationToStart(this, "Spider_Start");
		AudioManager.Play("devil_spider_head_intro");
		emitAudioFromObject.Add("devil_spider_head_intro");
		yield return base.animator.WaitForAnimationToEnd(this, "Spider_Start");
		LevelProperties.Devil.Spider p = base.properties.CurrentState.spider;
		int numAttacks = p.numAttacks.RandomInt();
		for (int i = 0; i < numAttacks; i++)
		{
			yield return CupheadTime.WaitForSeconds(this, p.entranceDelay.RandomFloat());
			spiderOffsetIndex = (spiderOffsetIndex + 1) % spiderOffsets.Length;
			float offset = 0f;
			float.TryParse(spiderOffsets[spiderOffsetIndex], out offset);
			spiderHead.Attack(Mathf.Clamp(PlayerManager.GetNext().center.x + offset, -620f, 620f), p.downSpeed, p.upSpeed);
			while (spiderHead.state != DevilLevelSpiderHead.State.Idle)
			{
				yield return null;
			}
		}
		base.animator.SetBool("StartSpider", false);
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
	}

	private IEnumerator dragon_cr()
	{
		base.animator.SetBool("StartDragon", true);
		bool isLeft = Rand.Bool();
		base.animator.SetBool("IsLeft", isLeft);
		dragonHead.Attack(this, isLeft);
		LevelProperties.Devil.Dragon p = base.properties.CurrentState.dragon;
		while (dragonHead.state != DevilLevelDragonHead.State.Idle)
		{
			yield return null;
		}
		base.animator.SetBool("StartDragon", false);
		yield return base.animator.WaitForAnimationToEnd(this, "Morph_End");
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
		yield return null;
	}

	private void DragonStop()
	{
		base.animator.SetTrigger("Continue");
		dragonHead.state = DevilLevelDragonHead.State.Stopped;
	}

	private void DragonReverse()
	{
		base.animator.SetTrigger("OnDragonEnd");
	}

	private void ResetPosition()
	{
		dragonHead.transform.position = dragonPos;
	}

	public void StartPitchfork()
	{
		state = State.Pitchfork;
		base.animator.SetBool("StartTrident", true);
	}

	private void SpawnProjectiles()
	{
		StartTridentHeadSFX();
		pitchforkPatternIndex = (pitchforkPatternIndex + 1) % pitchforkPattern.Length;
		int result = 0;
		int.TryParse(pitchforkPattern[pitchforkPatternIndex], out result);
		AudioManager.Play("devil_generic_projectile_start");
		emitAudioFromObject.Add("devil_generic_projectile_start");
		switch (result)
		{
		case 2:
			StartCoroutine(pitchforkTwoFlameWheel_cr());
			break;
		case 3:
			StartCoroutine(pitchforkThreeFlameJumper_cr());
			break;
		case 4:
			StartCoroutine(pitchforkFourFlameBouncer_cr());
			break;
		case 5:
			StartCoroutine(pitchforkFiveFlameSpinner_cr());
			break;
		case 6:
			StartCoroutine(pitchforkSixFlameRing_cr());
			break;
		}
	}

	private Vector2 getPitchforkFiringPos(float angle)
	{
		return new Vector2(0f, base.properties.CurrentState.pitchfork.spawnCenterY) + MathUtils.AngleToDirection(angle) * base.properties.CurrentState.pitchfork.spawnRadius;
	}

	private void StartParts()
	{
		base.animator.Play("Trident_Body", 2);
		base.animator.Play("Trident_Attack", 3);
	}

	private void StopParts()
	{
		base.animator.SetBool("StartTrident", false);
	}

	private IEnumerator pitchforkTwoFlameWheel_cr()
	{
		LevelProperties.Devil.PitchforkTwoFlameWheel p = base.properties.CurrentState.pitchforkTwoFlameWheel;
		List<DevilLevelPitchforkWheelProjectile> projectiles = new List<DevilLevelPitchforkWheelProjectile>();
		bool flipDelays = Rand.Bool();
		foreach (float spawnAngle in pitchforkTwoFlameWheelSpawner.getSpawnAngles())
		{
			bool flag = projectiles.Count == 0;
			if (flipDelays)
			{
				flag = !flag;
			}
			float attackDelay = ((!flag) ? p.secondAttackDelay : p.initialtAttackDelay);
			DevilLevelPitchforkWheelProjectile devilLevelPitchforkWheelProjectile = wheelProjectilePrefab.Create(getPitchforkFiringPos(spawnAngle), attackDelay, p.movementSpeed, this);
			wheelOrbitingProjectilePrefab.Create(devilLevelPitchforkWheelProjectile, 90f, (float)Rand.PosOrNeg() * p.rotationSpeed, 100f, this);
			projectiles.Add(devilLevelPitchforkWheelProjectile);
		}
		bool allProjectilesFinished = false;
		while (!allProjectilesFinished)
		{
			allProjectilesFinished = true;
			foreach (DevilLevelPitchforkWheelProjectile item in projectiles)
			{
				if (item != null && item.state != DevilLevelPitchforkWheelProjectile.State.Returning)
				{
					allProjectilesFinished = false;
				}
			}
			yield return null;
		}
		AudioManager.Play("devil_generic_projectile_stop");
		emitAudioFromObject.Add("devil_generic_projectile_stop");
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
	}

	private IEnumerator pitchforkThreeFlameJumper_cr()
	{
		LevelProperties.Devil.PitchforkThreeFlameJumper p = base.properties.CurrentState.pitchforkThreeFlameJumper;
		List<DevilLevelPitchforkJumpingProjectile> projectiles = new List<DevilLevelPitchforkJumpingProjectile>();
		foreach (float spawnAngle in pitchforkThreeFlameJumperSpawner.getSpawnAngles())
		{
			projectiles.Add(jumpingProjectilePrefab.Create(getPitchforkFiringPos(spawnAngle), p.launchAngle, p.launchSpeed, p.gravity, p.numJumps, this));
		}
		projectiles.Shuffle();
		float delay = p.initialAttackDelay.RandomFloat();
		for (int i = 0; i < p.numJumps; i++)
		{
			foreach (DevilLevelPitchforkJumpingProjectile projectile in projectiles)
			{
				yield return CupheadTime.WaitForSeconds(this, delay);
				projectile.Jump();
				delay = p.jumpDelay;
			}
		}
		AudioManager.Play("devil_generic_projectile_stop");
		emitAudioFromObject.Add("devil_generic_projectile_stop");
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
	}

	private IEnumerator pitchforkFourFlameBouncer_cr()
	{
		LevelProperties.Devil.PitchforkFourFlameBouncer p = base.properties.CurrentState.pitchforkFourFlameBouncer;
		List<DevilLevelPitchforkBouncingProjectile> projectiles = new List<DevilLevelPitchforkBouncingProjectile>();
		float delay = p.initialAttackDelay.RandomFloat();
		foreach (float spawnAngle in pitchforkFourFlameBouncerSpawner.getSpawnAngles())
		{
			projectiles.Add(bouncingProjectilePrefab.Create(getPitchforkFiringPos(spawnAngle), delay, p.speed, spawnAngle, p.numBounces, this, base.properties.CurrentState.pitchfork.dormantDuration));
		}
		projectiles[UnityEngine.Random.Range(0, projectiles.Count)].SetParryable(true);
		bool allProjectilesFinished = false;
		while (!allProjectilesFinished)
		{
			allProjectilesFinished = true;
			foreach (DevilLevelPitchforkBouncingProjectile item in projectiles)
			{
				if (item.BouncesRemaining > 0)
				{
					allProjectilesFinished = false;
				}
			}
			yield return null;
		}
		AudioManager.Play("devil_generic_projectile_stop");
		emitAudioFromObject.Add("devil_generic_projectile_stop");
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
	}

	private IEnumerator pitchforkFiveFlameSpinner_cr()
	{
		LevelProperties.Devil.PitchforkFiveFlameSpinner p = base.properties.CurrentState.pitchforkFiveFlameSpinner;
		DevilLevelPitchforkSpinnerProjectile centerProjectile = spinnerProjectilePrefab.Create(new Vector2(0f, base.properties.CurrentState.pitchfork.spawnCenterY), p.maxSpeed, p.acceleration, p.attackDuration, this, base.properties.CurrentState.pitchfork.dormantDuration);
		float rotationSpeed = (float)Rand.PosOrNeg() * p.rotationSpeed;
		foreach (float spawnAngle in pitchforkFiveFlameSpinnerSpawner.getSpawnAngles())
		{
			spinnerOrbitingProjectilePrefab.Create(centerProjectile, spawnAngle, rotationSpeed, base.properties.CurrentState.pitchfork.spawnRadius, this, base.properties.CurrentState.pitchfork.dormantDuration);
		}
		AudioManager.Play("devil_generic_projectile_stop");
		emitAudioFromObject.Add("devil_generic_projectile_stop");
		yield return CupheadTime.WaitForSeconds(this, p.attackDuration + p.hesitate);
		state = State.Idle;
	}

	private IEnumerator pitchforkSixFlameRing_cr()
	{
		LevelProperties.Devil.PitchforkSixFlameRing p = base.properties.CurrentState.pitchforkSixFlameRing;
		List<DevilLevelPitchforkRingProjectile> projectiles = new List<DevilLevelPitchforkRingProjectile>();
		foreach (float spawnAngle in pitchforkSixFlameRingSpawner.getSpawnAngles())
		{
			projectiles.Add(ringProjectilePrefab.Create(getPitchforkFiringPos(spawnAngle), p.speed, p.groundDuration, this, base.properties.CurrentState.pitchfork.dormantDuration));
		}
		projectiles[UnityEngine.Random.Range(0, projectiles.Count)].SetParryable(true);
		yield return CupheadTime.WaitForSeconds(this, p.initialAttackDelay.RandomFloat());
		projectiles[0].Attack();
		projectiles.RemoveAt(0);
		if (Rand.Bool())
		{
			projectiles.Reverse();
		}
		foreach (DevilLevelPitchforkRingProjectile projectile in projectiles)
		{
			yield return CupheadTime.WaitForSeconds(this, p.attackDelay);
			projectile.Attack();
		}
		AudioManager.Play("devil_generic_projectile_stop");
		emitAudioFromObject.Add("devil_generic_projectile_stop");
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
	}

	private void DeathEasy()
	{
		if (Level.Current.mode == Level.Mode.Easy)
		{
			base.properties.OnBossDeath -= DeathEasy;
		}
		GetComponent<LevelBossDeathExploder>().StartExplosion();
		base.animator.Play("DeathEasy");
	}

	public void StartTransform()
	{
		endPH1 = true;
		state = State.EndPhase1;
		base.animator.SetTrigger("OnPhase2");
		StartCoroutine(on_phase_2_cr());
	}

	private IEnumerator on_phase_2_cr()
	{
		yield return base.animator.WaitForAnimationToStart(this, "Death_Start");
		if (OnPhase1Death != null)
		{
			OnPhase1Death();
		}
		yield return base.animator.WaitForAnimationToStart(this, "Death_Hole");
		middleGround.SetActive(false);
		StartCoroutine(move_fire_cr());
		yield return null;
	}

	private IEnumerator move_fire_cr()
	{
		AudioManager.PlayLoop("devil_fire_wall");
		emitAudioFromObject.Add("devil_fire_wall");
		while (!endFire && leftWall.transform.position.x < -200f)
		{
			leftWall.transform.position += Vector3.right * base.properties.CurrentState.firewall.firewallSpeed * CupheadTime.FixedDelta;
			if (rightWall.transform.position.x > 200f)
			{
				rightWall.transform.position += Vector3.left * base.properties.CurrentState.firewall.firewallSpeed * CupheadTime.FixedDelta;
				yield return new WaitForFixedUpdate();
				continue;
			}
			break;
		}
	}

	public void RemoveFire()
	{
		StartCoroutine(remove_fire_cr());
	}

	private IEnumerator remove_fire_cr()
	{
		endFire = true;
		float t = 0f;
		float time = 1f;
		float startLeftPos = leftWall.transform.position.x;
		float startRightPos = rightWall.transform.position.x;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			leftWall.transform.SetPosition(Mathf.Lerp(startLeftPos, leftWallPositionX, val));
			rightWall.transform.SetPosition(Mathf.Lerp(startRightPos, rightWallPositionX, val));
			yield return null;
		}
		leftWall.transform.SetPosition(leftWallPositionX);
		rightWall.transform.SetPosition(rightWallPositionX);
		AudioManager.FadeSFXVolume("devil_fire_wall", 0f, 1f);
		yield return CupheadTime.WaitForSeconds(this, 1f);
		AudioManager.Stop("devil_fire_wall");
	}

	private void TridentStartSFX()
	{
		AudioManager.Play("devil_trident_start");
		emitAudioFromObject.Add("devil_trident_start");
	}

	private void TridentEndSFX()
	{
		AudioManager.Play("devil_trident_end");
		emitAudioFromObject.Add("devil_trident_end");
	}

	private void TridentAttackSFX()
	{
		AudioManager.Play("devil_trident_attack");
		emitAudioFromObject.Add("devil_trident_attack");
	}

	private void SpiderMorphEndSFX()
	{
		AudioManager.Play("devil_spider_morph_end");
		emitAudioFromObject.Add("devil_spider_morph_end");
	}

	private void DevilPhase1DeathSFX()
	{
		AudioManager.Play("devil_phase_1_death_start");
		emitAudioFromObject.Add("devil_phase_1_death_start");
	}

	private void DragonMorphEndSFX()
	{
		AudioManager.Play("devil_dragon_morph_end");
		emitAudioFromObject.Add("devil_dragon_morph_end");
	}

	private void HandclapSnakeSFX()
	{
		AudioManager.Play("devil_dragon_start");
		emitAudioFromObject.Add("devil_dragon_start");
	}

	private void IntroPupilsSFX()
	{
		AudioManager.Play("devil_intro_pupils");
		emitAudioFromObject.Add("devil_intro_pupils");
	}

	private void RamMorphStartSFX()
	{
		AudioManager.Play("devil_ram_morph_start");
		emitAudioFromObject.Add("devil_ram_morph_start");
	}

	private void RamMorphEndSFX()
	{
		AudioManager.Play("devil_ram_morph_end");
		emitAudioFromObject.Add("devil_ram_morph_end");
	}

	private void StartTridentHeadSFX()
	{
		AudioManager.Play("devil_trident_head");
		emitAudioFromObject.Add("devil_trident_head");
	}

	private void EndTridentHeadSFX()
	{
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public void ShowGoSign()
	{
		holeSign.SetActive(true);
	}
}
