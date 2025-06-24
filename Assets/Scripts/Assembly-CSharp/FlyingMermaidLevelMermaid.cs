using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelMermaid : LevelProperties.FlyingMermaid.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Yell = 2,
		Summon = 3,
		Fish = 4,
		Transform = 5
	}

	public enum SummonPossibility
	{
		Seahorse = 0,
		Pufferfish = 1,
		Turtle = 2
	}

	public enum FishPossibility
	{
		Spreadshot = 0,
		Spinner = 1,
		Homer = 2
	}

	[SerializeField]
	private Transform[] walkingPositions;

	[SerializeField]
	private float introRiseTime;

	[SerializeField]
	private float tuckdownMoveTime;

	[SerializeField]
	private float tuckdownWaitTime;

	[SerializeField]
	private float tuckdownRiseTime;

	[SerializeField]
	private float regularY;

	[SerializeField]
	private float startUnderwaterY;

	[SerializeField]
	private float fishUnderwaterY;

	[SerializeField]
	private float transformMoveTime;

	[SerializeField]
	private float transformMoveX;

	[SerializeField]
	private float eelSinkTime;

	[SerializeField]
	private float eelUnderwaterY;

	[SerializeField]
	private FlyingMermaidLevelYellProjectile yellProjectilePrefab;

	[SerializeField]
	private FlyingMermaidLevelSeahorse seahorsePrefab;

	[SerializeField]
	private Effect FishSpitEffectPrefab;

	private bool introEnded;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private Transform yellFxRoot;

	[SerializeField]
	private FlyingMermaidLevelPufferfish[] pufferfishPrefabs;

	[SerializeField]
	private FlyingMermaidLevelPufferfish pinkPufferfishPrefab;

	[SerializeField]
	private FlyingMermaidLevelTurtle turtlePrefab;

	[SerializeField]
	private SpriteRenderer blinkOverlaySprite;

	[SerializeField]
	private SpriteRenderer spreadshotFishSprite;

	[SerializeField]
	private SpriteRenderer spinnerFishSprite;

	[SerializeField]
	private SpriteRenderer homerFishSprite;

	[SerializeField]
	private SpriteRenderer spreadshotFishOverlaySprite;

	[SerializeField]
	private SpriteRenderer spinnerFishOverlaySprite;

	[SerializeField]
	private SpriteRenderer homerFishOverlaySprite;

	[SerializeField]
	private FlyingMermaidLevelFish spreadshotFishPrefab;

	[SerializeField]
	private FlyingMermaidLevelFish spinnerFishPrefab;

	[SerializeField]
	private FlyingMermaidLevelFish homerFishPrefab;

	[SerializeField]
	private BasicProjectile fishSpreadshotBulletPrefab;

	[SerializeField]
	private FlyingMermaidLevelFishSpinner fishSpinnerBulletPrefab;

	[SerializeField]
	private FlyingMermaidLevelHomingProjectile fishHomerBulletPrefab;

	[SerializeField]
	private Transform fishLaunchRoot;

	[SerializeField]
	private Transform fishProjectileRoot;

	[SerializeField]
	private FlyingMermaidLevelMerdusa merdusa;

	[SerializeField]
	private Transform blockingColliders;

	[SerializeField]
	private Effect splashRight;

	[SerializeField]
	private Effect splashLeft;

	[SerializeField]
	private Transform splashRoot;

	[SerializeField]
	private Effect yellEffect;

	private SummonPossibility[] summonPattern = new SummonPossibility[3]
	{
		SummonPossibility.Seahorse,
		SummonPossibility.Pufferfish,
		SummonPossibility.Turtle
	};

	private FishPossibility[] fishPattern = new FishPossibility[2]
	{
		FishPossibility.Homer,
		FishPossibility.Spreadshot
	};

	private int summonIndex;

	private int fishIndex;

	private int spreadshotPatternIndex;

	private int spinnerPatternIndex;

	private int homerPatternIndex;

	private bool stopPufferfish;

	private bool transformationStarting;

	private bool stopMoving;

	private float walkPCT;

	private float walkTime;

	private float walkDuration;

	private int blinks;

	private int maxBlinks = 3;

	private FishPossibility fish;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		summonPattern.Shuffle();
		fishPattern.Shuffle();
		StartCoroutine(intro_cr());
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		CollisionChild collisionChild = blockingColliders.gameObject.AddComponent<CollisionChild>();
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		if (stopMoving)
		{
			return;
		}
		if (introEnded && !transformationStarting)
		{
			float num = Mathf.Max(PlayerManager.GetNext().center.x, PlayerManager.GetNext().center.x);
			if (num > base.transform.position.x)
			{
				Position(true);
			}
			else
			{
				Position(false);
			}
		}
		else if (transformationStarting)
		{
			Position(false);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public override void LevelInit(LevelProperties.FlyingMermaid properties)
	{
		base.LevelInit(properties);
		initFishPatternIndices();
	}

	private void Position(bool closeGap)
	{
		walkDuration = ((!transformationStarting) ? 4 : 2);
		float levelTime = Level.Current.LevelTime;
		float num = Level.Current.LevelTime + walkDuration;
		if (closeGap)
		{
			float x = walkingPositions[0].position.x;
			float x2 = walkingPositions[1].position.x;
			Move(x, x2, walkDuration, 1);
		}
		else
		{
			float x = walkingPositions[1].position.x;
			float x2 = walkingPositions[0].position.x;
			Move(x, x2, walkDuration, -1);
		}
	}

	private void Move(float startPosition, float endPosition, float duration, int direction)
	{
		walkTime += (float)CupheadTime.Delta * (float)direction;
		if (direction < 0)
		{
			if (walkTime <= 0f)
			{
				walkTime = 0f;
			}
		}
		else if (walkTime >= duration)
		{
			walkTime = duration;
		}
		walkPCT = walkTime / duration;
		if (walkPCT >= 1f)
		{
			walkPCT = 1f;
		}
		if (direction < 0)
		{
			walkPCT = 1f - walkPCT;
		}
		base.transform.SetPosition(startPosition + (endPosition - startPosition) * walkPCT);
	}

	private void PlayIntroSound()
	{
		AudioManager.Play("level_mermaid_intro");
		emitAudioFromObject.Add("level_mermaid_intro");
	}

	private IEnumerator intro_cr()
	{
		float t = 0f;
		base.transform.SetPosition(null, startUnderwaterY);
		yield return CupheadTime.WaitForSeconds(this, introRiseTime * 0.5f);
		StartCoroutine(spawn_splash_cr());
		while (t < introRiseTime * 0.5f)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(null, Mathf.Lerp(startUnderwaterY, regularY, t / (introRiseTime * 0.5f)));
			yield return null;
		}
		base.transform.SetPosition(null, regularY);
		while (!introEnded)
		{
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, 1f);
		state = State.Idle;
	}

	private IEnumerator spawn_splash_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.35f);
		FlyingMermaidLevelSplashManager.Instance.SpawnMegaSplashMedium(base.gameObject, -50f, true, -200f);
		yield return null;
	}

	public void IntroContinue()
	{
		Animator component = GetComponent<Animator>();
		component.SetTrigger("Continue");
		state = State.Intro;
	}

	private void OnIntroAnimComplete()
	{
		introEnded = true;
	}

	private void BlinkMaybe()
	{
		blinks++;
		if (blinks >= maxBlinks)
		{
			blinks = 0;
			maxBlinks = Random.Range(2, 5);
			blinkOverlaySprite.enabled = true;
		}
		else
		{
			blinkOverlaySprite.enabled = false;
		}
	}

	public void StartYell()
	{
		state = State.Yell;
		StartCoroutine(yell_cr());
	}

	private IEnumerator yell_cr()
	{
		LevelProperties.FlyingMermaid.Yell p = base.properties.CurrentState.yell;
		string[] pattern = p.patternString.GetRandom().Split(',');
		base.animator.SetTrigger("StartYell");
		base.animator.SetBool("Repeat", true);
		yield return base.animator.WaitForAnimationToEnd(this, "Yell_Start");
		float waitTime = p.anticipateInitialHold;
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i][0] == 'D')
			{
				float.TryParse(pattern[i].Substring(1), out waitTime);
				continue;
			}
			int repeatTimes = 0;
			int.TryParse(pattern[i].Substring(1), out repeatTimes);
			for (int n = 0; n < repeatTimes; n++)
			{
				yield return CupheadTime.WaitForSeconds(this, waitTime);
				base.animator.SetTrigger("Continue");
				yield return base.animator.WaitForAnimationToEnd(this, "Yell_Anticipation_End");
				FireProjectiles();
				yellEffect.Create(yellFxRoot.position);
				yield return CupheadTime.WaitForSeconds(this, p.mouthHold);
				base.animator.SetTrigger("Continue");
				waitTime = p.anticipateHold;
				if (i < pattern.Length - 1 || n < repeatTimes - 1)
				{
					yield return base.animator.WaitForAnimationToEnd(this, "Yell_Back");
				}
			}
		}
		base.animator.SetBool("Repeat", false);
		yield return base.animator.WaitForAnimationToEnd(this, "Yell_End");
		yield return CupheadTime.WaitForSeconds(this, p.hesitateAfterAttack);
		state = State.Idle;
	}

	private void FireProjectiles()
	{
		LevelProperties.FlyingMermaid.Yell yell = base.properties.CurrentState.yell;
		AbstractPlayerController next = PlayerManager.GetNext();
		for (int i = 0; i < yell.numBullets; i++)
		{
			float floatAt = yell.spreadAngle.GetFloatAt((float)i / ((float)yell.numBullets - 1f));
			FlyingMermaidLevelYellProjectile flyingMermaidLevelYellProjectile = yellProjectilePrefab.Create(projectileRoot.position, yell.bulletSpeed, floatAt, next);
			flyingMermaidLevelYellProjectile.animator.SetInteger("Variant", i);
		}
	}

	public void StartSummon()
	{
		state = State.Summon;
		StartCoroutine(summon_cr());
	}

	private IEnumerator summon_cr()
	{
		LevelProperties.FlyingMermaid.Summon p = base.properties.CurrentState.summon;
		base.animator.SetBool("Summon", true);
		yield return base.animator.WaitForAnimationToEnd(this, "Summon_Start");
		AudioManager.Play("level_mermaid_summon_loop_start");
		yield return CupheadTime.WaitForSeconds(this, p.holdBeforeCreature);
		SummonPossibility summon = nextSummon();
		AudioManager.Play("level_mermaid_summon_loop");
		switch (summon)
		{
		case SummonPossibility.Seahorse:
			SummonSeahorse();
			break;
		case SummonPossibility.Pufferfish:
			AudioManager.Play("level_mermaid_merdusa_puffer_fish_bubble_up");
			StartCoroutine(summonPufferFish_cr());
			break;
		case SummonPossibility.Turtle:
			SummonTurtle();
			break;
		}
		yield return CupheadTime.WaitForSeconds(this, p.holdAfterCreature);
		AudioManager.Stop("level_mermaid_summon_loop");
		AudioManager.Play("level_mermaid_summon_loop_end");
		base.animator.SetBool("Summon", false);
		yield return base.animator.WaitForAnimationToEnd(this, "Summon_End");
		yield return CupheadTime.WaitForSeconds(this, p.hesitateAfterAttack);
		state = State.Idle;
	}

	private SummonPossibility nextSummon()
	{
		summonIndex = (summonIndex + 1) % summonPattern.Length;
		return summonPattern[summonIndex];
	}

	private IEnumerator summonPufferFish_cr()
	{
		LevelProperties.FlyingMermaid.Pufferfish p = base.properties.CurrentState.pufferfish;
		string[] pattern = p.spawnString.GetRandom().Split(',');
		int i = Random.Range(0, pattern.Length);
		float t = 0f;
		float waitTime = 0f;
		int spawnsUntilPinkPufferfish = p.pinkPufferSpawnRange.RandomInt();
		while (t < p.spawnDuration && !stopPufferfish)
		{
			if (pattern[i][0] == 'D')
			{
				float.TryParse(pattern[i].Substring(1), out waitTime);
			}
			else
			{
				if (waitTime > 0f)
				{
					yield return CupheadTime.WaitForSeconds(this, waitTime);
					t += waitTime;
				}
				string[] spawnLocations = pattern[i].Split('-');
				string[] array = spawnLocations;
				foreach (string s in array)
				{
					float result = 0f;
					float.TryParse(s, out result);
					spawnsUntilPinkPufferfish--;
					FlyingMermaidLevelPufferfish prefab;
					if (spawnsUntilPinkPufferfish == 0)
					{
						spawnsUntilPinkPufferfish = p.pinkPufferSpawnRange.RandomInt();
						prefab = pinkPufferfishPrefab;
					}
					else
					{
						prefab = pufferfishPrefabs[Random.Range(0, pufferfishPrefabs.Length)];
					}
					StartCoroutine(summon_pufferfish_cr(prefab, result));
				}
				waitTime = p.delay;
			}
			i = (i + 1) % pattern.Length;
		}
	}

	private void SummonSeahorse()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		FlyingMermaidLevelSeahorse flyingMermaidLevelSeahorse = Object.Instantiate(seahorsePrefab);
		Vector2 vector = flyingMermaidLevelSeahorse.transform.position;
		vector.x = next.transform.position.x;
		flyingMermaidLevelSeahorse.transform.position = vector;
		flyingMermaidLevelSeahorse.Init(base.properties.CurrentState.seahorse);
		GroundHomingMovement component = flyingMermaidLevelSeahorse.GetComponent<GroundHomingMovement>();
		component.TrackingPlayer = next;
	}

	private void SummonTurtle()
	{
		FlyingMermaidLevelTurtle flyingMermaidLevelTurtle = Object.Instantiate(turtlePrefab);
		Vector2 vector = flyingMermaidLevelTurtle.transform.position;
		vector.x = (float)Level.Current.Left + base.properties.CurrentState.turtle.appearPosition.RandomFloat();
		flyingMermaidLevelTurtle.transform.position = vector;
		flyingMermaidLevelTurtle.Init(base.properties.CurrentState.turtle);
	}

	private IEnumerator summon_pufferfish_cr(FlyingMermaidLevelPufferfish prefab, float x)
	{
		yield return CupheadTime.WaitForSeconds(this, Random.Range(0f, 0.15f));
		FlyingMermaidLevelPufferfish pufferfish = Object.Instantiate(prefab);
		Vector2 position = pufferfish.transform.position;
		position.x = x + (float)Level.Current.Left;
		pufferfish.transform.position = position;
		pufferfish.Init(base.properties.CurrentState.pufferfish);
	}

	public void StartFish()
	{
		StartCoroutine(fish_cr());
	}

	private void PlayMermaidTuckdownSound()
	{
		AudioManager.Play("level_mermaid_tuckdown_laugh");
		emitAudioFromObject.Add("level_mermaid_tuckdown_laugh");
	}

	private IEnumerator fish_cr()
	{
		state = State.Fish;
		base.animator.SetTrigger("StartFish");
		yield return base.animator.WaitForAnimationToEnd(this, "Tuckdown_Start");
		float t = 0f;
		FlyingMermaidLevelSplashManager.Instance.SpawnMegaSplashLarge(base.gameObject);
		while (t < tuckdownMoveTime)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(null, Mathf.Lerp(regularY, fishUnderwaterY, t / tuckdownMoveTime));
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, tuckdownWaitTime);
		fish = nextFish();
		spreadshotFishSprite.enabled = fish == FishPossibility.Spreadshot;
		spreadshotFishOverlaySprite.enabled = fish == FishPossibility.Spreadshot;
		spinnerFishSprite.enabled = fish == FishPossibility.Spinner;
		spinnerFishOverlaySprite.enabled = fish == FishPossibility.Spinner;
		homerFishSprite.enabled = fish == FishPossibility.Homer;
		homerFishOverlaySprite.enabled = fish == FishPossibility.Homer;
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Tuckdown_Loop");
		t = 0f;
		FlyingMermaidLevelSplashManager.Instance.SpawnMegaSplashLarge(base.gameObject, 50f, true);
		while (t < tuckdownRiseTime)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(null, Mathf.Lerp(fishUnderwaterY, regularY, t / tuckdownRiseTime));
			yield return null;
		}
		base.animator.SetBool("Repeat", true);
		string[] pattern = nextFishPatternString().Split(',');
		float waitTime = base.properties.CurrentState.fish.delayBeforeFirstAttack;
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i][0] == 'D')
			{
				float.TryParse(pattern[i].Substring(1), out waitTime);
				continue;
			}
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			base.animator.SetTrigger("Continue");
			yield return base.animator.WaitForAnimationToEnd(this, "Fish_Attack_Start");
			doFishAttack(pattern[i]);
			if (i < pattern.Length - 1)
			{
				yield return base.animator.WaitForAnimationToEnd(this, "Fish_Attack_Repeat");
				waitTime = waitTimeBetweenFishAttacks();
			}
		}
		base.animator.SetBool("Repeat", false);
		yield return base.animator.WaitForAnimationToEnd(this, "Fish_Attack");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.fish.delayBeforeFly);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Fish_Launch");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.fish.hesitateAfterAttack);
		state = State.Idle;
	}

	private FishPossibility nextFish()
	{
		fishIndex = (fishIndex + 1) % fishPattern.Length;
		return fishPattern[fishIndex];
	}

	private void initFishPatternIndices()
	{
		spreadshotPatternIndex = Random.Range(0, base.properties.CurrentState.spreadshotFish.shootString.Length);
		spinnerPatternIndex = Random.Range(0, base.properties.CurrentState.spinnerFish.shootString.Length);
		homerPatternIndex = Random.Range(0, base.properties.CurrentState.homerFish.shootString.Length);
	}

	private string nextFishPatternString()
	{
		switch (fish)
		{
		case FishPossibility.Spreadshot:
			spreadshotPatternIndex = (spreadshotPatternIndex + 1) % base.properties.CurrentState.spreadshotFish.shootString.Length;
			return base.properties.CurrentState.spreadshotFish.shootString[spreadshotPatternIndex];
		case FishPossibility.Spinner:
			spinnerPatternIndex = (spinnerPatternIndex + 1) % base.properties.CurrentState.spinnerFish.shootString.Length;
			return base.properties.CurrentState.spinnerFish.shootString[spinnerPatternIndex];
		case FishPossibility.Homer:
			homerPatternIndex = (homerPatternIndex + 1) % base.properties.CurrentState.homerFish.shootString.Length;
			return base.properties.CurrentState.homerFish.shootString[homerPatternIndex];
		default:
			return string.Empty;
		}
	}

	private float waitTimeBetweenFishAttacks()
	{
		switch (fish)
		{
		case FishPossibility.Spreadshot:
			return base.properties.CurrentState.spreadshotFish.attackDelay;
		case FishPossibility.Spinner:
			return base.properties.CurrentState.spinnerFish.attackDelay;
		case FishPossibility.Homer:
			return base.properties.CurrentState.homerFish.attackDelay;
		default:
			return 0f;
		}
	}

	private void doFishAttack(string attackString)
	{
		AudioManager.Play("level_mermaid_fish_attack");
		emitAudioFromObject.Add("level_mermaid_fish_attack");
		switch (fish)
		{
		case FishPossibility.Spreadshot:
			fishSpreadshot(attackString);
			break;
		case FishPossibility.Spinner:
			fishSpinner();
			break;
		case FishPossibility.Homer:
			fishHomer();
			break;
		}
	}

	private void fishSpreadshot(string attackString)
	{
		int result = 0;
		int.TryParse(attackString.Substring(1), out result);
		result--;
		string[] array = base.properties.CurrentState.spreadshotFish.spreadVariableGroups[result].Split(',');
		float result2 = 0f;
		int result3 = 0;
		MinMax minMax = new MinMax(0f, 0f);
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text[0] == 'S')
			{
				float.TryParse(text.Substring(1), out result2);
				continue;
			}
			if (text[0] == 'N')
			{
				int.TryParse(text.Substring(1), out result3);
				continue;
			}
			string[] array3 = text.Split('-');
			float.TryParse(array3[0], out minMax.min);
			float.TryParse(array3[1], out minMax.max);
		}
		for (int j = 0; j < result3; j++)
		{
			float floatAt = minMax.GetFloatAt((float)j / ((float)result3 - 1f));
			BasicProjectile basicProjectile = fishSpreadshotBulletPrefab.Create(fishProjectileRoot.position, floatAt, result2);
			basicProjectile.animator.SetInteger("Variant", j % 2);
		}
	}

	private void fishSpinner()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector2 direction = next.transform.position - fishProjectileRoot.position;
		direction.Normalize();
		if (next.transform.position.x > fishProjectileRoot.transform.position.x)
		{
			direction = MathUtils.AngleToDirection(90f);
		}
		fishSpinnerBulletPrefab.Create(fishProjectileRoot.position, direction, base.properties.CurrentState.spinnerFish);
	}

	private void fishHomer()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector2 direction = next.transform.position - fishProjectileRoot.position;
		float rotation = MathUtils.DirectionToAngle(direction) + Random.Range(-15f, 15f);
		LevelProperties.FlyingMermaid.HomerFish homerFish = base.properties.CurrentState.homerFish;
		if (next.transform.position.x > fishProjectileRoot.transform.position.x)
		{
			rotation = 90f;
		}
		fishHomerBulletPrefab.Create(fishProjectileRoot.position, rotation, next, homerFish);
	}

	public void LaunchFish()
	{
		FlyingMermaidLevelFish flyingMermaidLevelFish = null;
		switch (fish)
		{
		case FishPossibility.Spreadshot:
			flyingMermaidLevelFish = spreadshotFishPrefab;
			break;
		case FishPossibility.Spinner:
			flyingMermaidLevelFish = spinnerFishPrefab;
			break;
		case FishPossibility.Homer:
			flyingMermaidLevelFish = homerFishPrefab;
			break;
		}
		flyingMermaidLevelFish.Create(fishLaunchRoot.position, base.properties.CurrentState.fish);
	}

	private void OnFishSpitFx()
	{
		FishSpitEffectPrefab.Create(fishProjectileRoot.position);
	}

	public void StartTransform()
	{
		transformationStarting = true;
		StartCoroutine(transform_cr());
	}

	private IEnumerator transform_cr()
	{
		while (base.transform.position.x != walkingPositions[0].position.x)
		{
			yield return null;
		}
		stopMoving = true;
		float startX = base.transform.position.x;
		float t = 0f;
		while (t < transformMoveTime)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(Mathf.Lerp(startX, startX - transformMoveX, t / transformMoveTime));
			yield return null;
		}
		base.animator.SetTrigger("Transform");
		if (state == State.Summon)
		{
			yield return base.animator.WaitForAnimationToStart(this, "Idle");
			stopPufferfish = true;
		}
		if (state == State.Idle)
		{
			stopPufferfish = true;
		}
		state = State.Transform;
		yield return base.animator.WaitForAnimationToStart(this, "Transform");
		AudioManager.Play("level_mermaid_transform");
		((FlyingMermaidLevel)Level.Current).MerdusaTransformStarted = true;
		stopPufferfish = true;
		yield return base.animator.WaitForAnimationToEnd(this, "Transform");
		t = 0f;
		while (t < eelSinkTime)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(null, Mathf.Lerp(regularY, eelUnderwaterY, t / eelSinkTime));
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	public void DisableColliders()
	{
		Collider2D[] components = GetComponents<Collider2D>();
		Collider2D[] array = components;
		foreach (Collider2D collider2D in array)
		{
			collider2D.enabled = false;
		}
		blockingColliders.gameObject.SetActive(false);
	}

	public void SpawnMerdusa()
	{
		merdusa.StartIntro(base.transform.position);
	}

	private void RightSplash()
	{
		splashRight.Create(splashRoot.transform.position);
	}

	private void LeftSplash()
	{
		splashLeft.Create(splashRoot.transform.position);
	}

	private void SoundMermaidFishLaunch()
	{
		AudioManager.Play("level_mermaid_fish_launch");
		emitAudioFromObject.Add("level_mermaid_fish_launch");
	}

	private void SoundMermaidAttackYellStart()
	{
		AudioManager.Play("level_mermaid_yell_start");
		emitAudioFromObject.Add("level_mermaid_yell_start");
	}

	private void SoundMermaidAttackYell()
	{
		AudioManager.Play("level_mermaid_yell_attack");
		emitAudioFromObject.Add("level_mermaid_yell_attack");
	}
}
