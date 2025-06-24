using System;
using System.Collections;
using UnityEngine;

public class BaronessLevelCastle : LevelProperties.Baroness.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		ChaseIntro = 2,
		Chase = 3,
		Open = 4,
		Dead = 5,
		EasyFinal = 6
	}

	public enum TeethState
	{
		Unspawned = 0,
		Idle = 1,
		Off = 2,
		StartOpen = 3,
		Open = 4
	}

	public enum BossPossibility
	{
		Gumball = 1,
		Waffle = 2,
		CandyCorn = 3,
		Cupcake = 4,
		Jawbreaker = 5
	}

	public bool pauseScrolling;

	private Transform[] homingRoots;

	[SerializeField]
	private SpriteRenderer teeth;

	[SerializeField]
	private SpriteRenderer blink;

	[SerializeField]
	private BaronessLevelPlatform platform;

	[SerializeField]
	private BaronessLevelBaroness baronessPhase1;

	[SerializeField]
	private Transform baronessPhase2;

	[SerializeField]
	private BaronessLevelPeppermint peppermintPrefab;

	[SerializeField]
	private Transform blackCastleHole;

	[SerializeField]
	private Transform castlePhase2TopLayer;

	[SerializeField]
	private BaronessLevelCupcake cupcakePrefab;

	[SerializeField]
	private BaronessLevelWaffle wafflePrefab;

	[SerializeField]
	private BaronessLevelGumball gumballPrefab;

	[SerializeField]
	private BaronessLevelJawbreaker jawBreakerPrefab;

	[SerializeField]
	private BaronessLevelCandyCorn candyCornPrefab;

	[SerializeField]
	private BaronessLevelJellybeans greenJellyPrefab;

	[SerializeField]
	private BaronessLevelJellybeans pinkJellyPrefab;

	[SerializeField]
	private Transform emergePoint;

	[SerializeField]
	private Transform pivotPoint;

	[SerializeField]
	private GameObject castleCollidePhase2;

	[SerializeField]
	private SpriteRenderer castleWallFix;

	private int bossIndex;

	private int timeIndex;

	private int transitionCounter;

	private int blinkCounter;

	private int blinkCounterMax;

	private float setWaitTime;

	private float distToGround;

	private bool maxMiniBosses;

	private bool castleOpen;

	private bool baronessPoppedUp;

	private bool continueTransition;

	private bool inAnimationLoop;

	private bool jellyIsPink;

	private bool jellyChangeDelay;

	private bool openTeeth;

	private bool activateForce = true;

	private Vector3 originalEmergePos;

	private Vector3 originalBaronessPoint;

	private AbstractPlayerController player;

	private LevelPlayerMotor.VelocityManager.Force scrollForce;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	private BossPossibility[] bossPattern = new BossPossibility[5]
	{
		BossPossibility.Gumball,
		BossPossibility.Waffle,
		BossPossibility.CandyCorn,
		BossPossibility.Cupcake,
		BossPossibility.Jawbreaker
	};

	public static BaronessLevelMiniBossBase CURRENT_MINI_BOSS { get; private set; }

	public State state { get; private set; }

	public TeethState teethState { get; private set; }

	public event Action OnDeathEvent;

	protected override void Awake()
	{
		base.Awake();
		maxMiniBosses = false;
		continueTransition = false;
		originalEmergePos = emergePoint.position;
		originalBaronessPoint = baronessPhase1.transform.position;
		teethState = TeethState.Unspawned;
		baronessPhase2.gameObject.SetActive(false);
		blink.enabled = false;
		blinkCounterMax = UnityEngine.Random.Range(4, 7);
		damageReceiver = baronessPhase2.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageDealer = new DamageDealer(1f, 1f);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (base.properties.CurrentHealth <= 0f && state != State.Dead)
		{
			state = State.Dead;
			StartDeath();
		}
	}

	protected override void Update()
	{
		base.Update();
		damageDealer.Update();
		player = PlayerManager.GetNext();
		distToGround = player.transform.position.y - -360f;
		if (state != State.Idle)
		{
			return;
		}
		if (CURRENT_MINI_BOSS == null)
		{
			if (!maxMiniBosses)
			{
				jellyChangeDelay = true;
				StartOpen();
			}
			else if (Level.Current.mode != Level.Mode.Easy)
			{
				state = State.ChaseIntro;
				StartChase();
			}
		}
		else if (Level.Current.mode == Level.Mode.Easy && state != State.EasyFinal && CURRENT_MINI_BOSS.isDying && maxMiniBosses)
		{
			state = State.EasyFinal;
			StartCoroutine(shoot_easy_cr());
		}
	}

	public override void LevelInit(LevelProperties.Baroness properties)
	{
		base.LevelInit(properties);
		baronessPhase1.getProperties(properties, properties.CurrentState.baronessVonBonbon.HP, this);
		platform.getProperties(properties.CurrentState.platform);
	}

	public void StartIntro()
	{
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.6f);
		baronessPhase1.animator.SetTrigger("Continue");
		yield return CupheadTime.WaitForSeconds(this, 2f);
		base.animator.Play("Castle_Open");
		AudioManager.Play("level_baroness_castle_gate_open");
		yield return base.animator.WaitForAnimationToEnd(this, "Castle_Open");
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		state = State.Idle;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void StartOpen()
	{
		state = State.Open;
		StartCoroutine(open_cr());
	}

	private void SetEyes()
	{
		base.animator.SetBool("ToCastleLoop", true);
	}

	private IEnumerator open_cr()
	{
		LevelProperties.Baroness.Open p = base.properties.CurrentState.open;
		string[] pattern = p.miniBossString.GetRandom().Split(',');
		castleOpen = true;
		if (baronessPoppedUp)
		{
			baronessPoppedUp = false;
			yield return baronessPhase1.animator.WaitForAnimationToEnd(this, "Baroness_Leave");
		}
		if (bossIndex != 0)
		{
			AudioManager.Play("level_baroness_castle_gate_open");
			base.animator.Play("Castle_Open");
			yield return base.animator.WaitForAnimationToEnd(this, "Castle_Open");
			baronessPhase1.animator.Play("Baroness_Mad_Start");
			AudioManager.Play("level_baroness_stick_head_pop");
			while (baronessPhase1.popUpCounter < 4)
			{
				yield return null;
			}
			baronessPhase1.animator.SetTrigger("PopIn");
			AudioManager.Play("level_baroness_stick_head_pop");
			baronessPhase1.popUpCounter = 0;
			yield return CupheadTime.WaitForSeconds(this, 1.5f);
		}
		switch ((BossPossibility)Enum.Parse(typeof(BossPossibility), BaronessLevel.PICKED_BOSSES[bossIndex]))
		{
		case BossPossibility.Gumball:
			SpawnGumball();
			break;
		case BossPossibility.Waffle:
			SpawnWaffle();
			break;
		case BossPossibility.CandyCorn:
			SpawnCandyCorn();
			break;
		case BossPossibility.Cupcake:
			SpawnCupcake();
			break;
		case BossPossibility.Jawbreaker:
			SpawnJawbreaker();
			break;
		default:
			Debug.Log("Invalid number");
			break;
		}
		yield return CupheadTime.WaitForSeconds(this, setWaitTime);
		base.animator.SetBool("ToCastleLoop", false);
		if (bossIndex < p.miniBossAmount)
		{
			bossIndex++;
		}
		if (base.properties.CurrentState.jellybeans.startingPoint == (float)bossIndex)
		{
			StartJellybeans();
		}
		if (bossIndex == p.miniBossAmount)
		{
			maxMiniBosses = true;
		}
		AudioManager.Play("level_baroness_castle_gate_close");
		yield return base.animator.WaitForAnimationToEnd(this, "Castle_Close");
		castleOpen = false;
		if (base.properties.CurrentState.baronessVonBonbon.miniBossStart == (float)bossIndex)
		{
			StartBaronessShoot();
		}
		state = State.Idle;
		yield return null;
	}

	private void Blink()
	{
		if (blinkCounter < blinkCounterMax)
		{
			blink.enabled = false;
			blinkCounter++;
		}
		else
		{
			blink.enabled = true;
			blinkCounter = 0;
			blinkCounterMax = UnityEngine.Random.Range(4, 7);
		}
	}

	private void SpawnGumball()
	{
		Vector3 position = emergePoint.position;
		position.y = emergePoint.position.y + 100f;
		emergePoint.position = position;
		AbstractPlayerController next = PlayerManager.GetNext();
		BaronessLevelGumball baronessLevelGumball = UnityEngine.Object.Instantiate(gumballPrefab);
		LevelProperties.Baroness.Gumball gumball = base.properties.CurrentState.gumball;
		baronessLevelGumball.Init(gumball, next, emergePoint.position, gumball.gumballMovementSpeed, gumball.HP);
		CURRENT_MINI_BOSS = baronessLevelGumball;
		CURRENT_MINI_BOSS.bossId = BossPossibility.Gumball;
		setWaitTime = 1f;
		emergePoint.position = originalEmergePos;
	}

	private void SpawnWaffle()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		BaronessLevelWaffle baronessLevelWaffle = UnityEngine.Object.Instantiate(wafflePrefab);
		LevelProperties.Baroness.Waffle waffle = base.properties.CurrentState.waffle;
		baronessLevelWaffle.Init(waffle, next, emergePoint.position, pivotPoint, waffle.movementSpeed, waffle.HP);
		CURRENT_MINI_BOSS = baronessLevelWaffle;
		CURRENT_MINI_BOSS.bossId = BossPossibility.Waffle;
		setWaitTime = 1f;
	}

	private void SpawnCandyCorn()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		BaronessLevelCandyCorn baronessLevelCandyCorn = UnityEngine.Object.Instantiate(candyCornPrefab);
		LevelProperties.Baroness.CandyCorn candyCorn = base.properties.CurrentState.candyCorn;
		baronessLevelCandyCorn.Init(candyCorn, next, new Vector3(emergePoint.position.x, emergePoint.position.y + 40f), candyCorn.movementSpeed, candyCorn.HP);
		CURRENT_MINI_BOSS = baronessLevelCandyCorn;
		CURRENT_MINI_BOSS.bossId = BossPossibility.CandyCorn;
		setWaitTime = 1f;
	}

	private void SpawnCupcake()
	{
		Vector3 position = emergePoint.position;
		position.y = emergePoint.position.y;
		position.x = emergePoint.position.x + 200f;
		emergePoint.position = position;
		BaronessLevelCupcake baronessLevelCupcake = UnityEngine.Object.Instantiate(cupcakePrefab);
		LevelProperties.Baroness.Cupcake cupcake = base.properties.CurrentState.cupcake;
		baronessLevelCupcake.Init(cupcake, emergePoint.position, cupcake.HP);
		CURRENT_MINI_BOSS = baronessLevelCupcake;
		CURRENT_MINI_BOSS.bossId = BossPossibility.Cupcake;
		setWaitTime = 1f;
		emergePoint.position = originalEmergePos;
	}

	private void SpawnJawbreaker()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		BaronessLevelJawbreaker baronessLevelJawbreaker = UnityEngine.Object.Instantiate(jawBreakerPrefab);
		LevelProperties.Baroness.Jawbreaker jawbreaker = base.properties.CurrentState.jawbreaker;
		baronessLevelJawbreaker.Init(jawbreaker, next, new Vector3(emergePoint.position.x, emergePoint.position.y + 10f), jawbreaker.jawbreakerHomingSpeed, jawbreaker.jawbreakerHomingRotation, jawbreaker.jawbreakerHomingHP);
		CURRENT_MINI_BOSS = baronessLevelJawbreaker;
		CURRENT_MINI_BOSS.bossId = BossPossibility.Jawbreaker;
		setWaitTime = 3f;
		for (int i = 0; i < jawbreaker.jawbreakerMinis; i++)
		{
			setWaitTime += 0.5f;
		}
	}

	public void StartBaronessShoot()
	{
		StartCoroutine(shoot_cr());
	}

	private IEnumerator shoot_cr()
	{
		state = State.Idle;
		LevelProperties.Baroness.BaronessVonBonbon p = base.properties.CurrentState.baronessVonBonbon;
		string[] pattern = p.timeString.GetRandom().Split(',');
		timeIndex = UnityEngine.Random.Range(0, pattern.Length);
		Collider2D collider = baronessPhase1.shootPoint.GetComponent<Collider2D>();
		while (true)
		{
			float timeShoot;
			float.TryParse(pattern[timeIndex], out timeShoot);
			yield return CupheadTime.WaitForSeconds(this, timeShoot);
			baronessPhase1.shotEnough = false;
			if (castleOpen)
			{
				yield return base.animator.WaitForAnimationToEnd(this, "Castle_Close");
			}
			baronessPoppedUp = true;
			AudioManager.Play("level_baroness_stick_head_open");
			baronessPhase1.animator.Play("Baroness_Pop_Up");
			while (baronessPoppedUp)
			{
				collider.enabled = true;
				if ((float)baronessPhase1.shootCounter < p.attackCount.RandomFloat() && !baronessPhase1.shotEnough)
				{
					baronessPhase1.animator.SetTrigger("ToShoot");
					yield return CupheadTime.WaitForSeconds(this, p.attackDelay);
					yield return null;
					continue;
				}
				break;
			}
			baronessPoppedUp = false;
			collider.enabled = false;
			baronessPhase1.shootCounter = 0;
			AudioManager.Play("level_baroness_stick_head_closed");
			baronessPhase1.animator.SetTrigger("Leave");
			if (timeIndex < pattern.Length - 1)
			{
				timeIndex++;
			}
			else
			{
				timeIndex = 0;
			}
			yield return null;
		}
	}

	private IEnumerator shoot_easy_cr()
	{
		float t = 0f;
		baronessPhase1.isEasyFinal = true;
		LevelProperties.Baroness.BaronessVonBonbon p = base.properties.CurrentState.baronessVonBonbon;
		Collider2D collider = baronessPhase1.shootPoint.GetComponent<Collider2D>();
		baronessPoppedUp = true;
		AudioManager.Play("level_baroness_stick_head_open");
		baronessPhase1.animator.Play("Baroness_Pop_Up");
		collider.enabled = true;
		if (castleOpen)
		{
			yield return base.animator.WaitForAnimationToEnd(this, "Castle_Close");
		}
		while (baronessPhase1.isEasyFinal)
		{
			baronessPhase1.animator.SetTrigger("ToShoot");
			while (t < (float)p.attackDelay && baronessPhase1.isEasyFinal)
			{
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			t = 0f;
		}
		StartDeathEasy();
	}

	public void StartJellybeans()
	{
		StartCoroutine(spawnJellybeans_cr());
	}

	private void SpawnJellyBeans(BaronessLevelJellybeans prefab)
	{
		Vector3 position = emergePoint.position;
		emergePoint.position = position;
		position.y = emergePoint.position.y - 20f;
		AbstractPlayerController next = PlayerManager.GetNext();
		LevelProperties.Baroness.Jellybeans jellybeans = base.properties.CurrentState.jellybeans;
		prefab.Create(base.properties.CurrentState.jellybeans, this, next, position, jellybeans.movementSpeed, jellybeans.HP);
		emergePoint.position = originalEmergePos;
	}

	private IEnumerator spawnJellybeans_cr()
	{
		LevelProperties.Baroness.Jellybeans p = base.properties.CurrentState.jellybeans;
		string[] typePattern = p.typeArray.GetRandom().Split(',');
		float change = 0f;
		while (state != State.ChaseIntro)
		{
			for (int i = 0; i < typePattern.Length; i++)
			{
				BaronessLevelJellybeans toSpawn = null;
				float beanSpawnDelay = p.spawnDelay.RandomFloat();
				if (typePattern[i][0] == 'R')
				{
					toSpawn = greenJellyPrefab;
					jellyIsPink = false;
				}
				else if (typePattern[i][0] == 'P')
				{
					toSpawn = pinkJellyPrefab;
					jellyIsPink = true;
				}
				if ((CURRENT_MINI_BOSS != null && state == State.Idle) || state == State.EasyFinal)
				{
					SpawnJellyBeans(toSpawn);
					yield return CupheadTime.WaitForSeconds(this, beanSpawnDelay - change);
				}
				else
				{
					yield return null;
				}
				if (jellyChangeDelay)
				{
					change += beanSpawnDelay - beanSpawnDelay * (1f - p.spawnDelayChangePercentage / 100f);
					jellyChangeDelay = false;
				}
			}
		}
	}

	private void StartDeathEasy()
	{
		StopAllCoroutines();
		state = State.Dead;
		StartCoroutine(death_easy_cr());
	}

	private IEnumerator death_easy_cr()
	{
		float offset = 100f;
		float speed = 400f;
		if (!baronessPoppedUp)
		{
			Vector3 position = baronessPhase1.transform.position;
			position.y -= offset;
			baronessPhase1.transform.position = position;
		}
		baronessPhase1.animator.SetTrigger("Death");
		base.animator.SetTrigger("DeathEasy");
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
		base.properties.WinInstantly();
		if (!baronessPoppedUp)
		{
			while (baronessPhase1.transform.position != originalBaronessPoint)
			{
				baronessPhase1.transform.position = Vector3.MoveTowards(baronessPhase1.transform.position, originalBaronessPoint, speed * (float)CupheadTime.Delta);
				yield return null;
			}
		}
		yield return null;
	}

	private void StartDeath()
	{
		StopAllCoroutines();
		StartCoroutine(death_cr());
	}

	private IEnumerator death_cr()
	{
		pauseScrolling = true;
		teethState = TeethState.Off;
		base.animator.SetTrigger("Death");
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
		LevelPlayerMotor[] array = UnityEngine.Object.FindObjectsOfType<LevelPlayerMotor>();
		foreach (LevelPlayerMotor levelPlayerMotor in array)
		{
			levelPlayerMotor.RemoveForce(scrollForce);
		}
		blackCastleHole.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		base.animator.Play("Castle_Death");
		yield return null;
	}

	public void StartChase()
	{
		StopAllCoroutines();
		StartCoroutine(chase_intro_cr());
	}

	private IEnumerator chase_intro_cr()
	{
		baronessPhase1.transformCounter = 0;
		if (baronessPoppedUp)
		{
			baronessPhase1.animator.SetTrigger("Leave");
			baronessPhase1.animator.WaitForAnimationToEnd(this, "Baroness_Leave");
			baronessPoppedUp = false;
			yield return CupheadTime.WaitForSeconds(this, 1f);
		}
		baronessPhase2.gameObject.SetActive(true);
		baronessPhase1.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
		baronessPhase1.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
		baronessPhase1.animator.Play("Baroness_To_Idle_1");
		while (baronessPhase1.transformCounter <= 2)
		{
			yield return null;
		}
		baronessPhase1.animator.SetTrigger("Continue");
		yield return baronessPhase1.animator.WaitForAnimationToEnd(baronessPhase1, "Baroness_Transition_1");
		baronessPhase1.transformCounter = 0;
		while (baronessPhase1.transformCounter <= 2 && continueTransition)
		{
			inAnimationLoop = true;
			yield return null;
		}
		baronessPhase1.animator.SetTrigger("Continue");
		yield return baronessPhase1.animator.WaitForAnimationToEnd(baronessPhase1, "Baroness_Transition_2_Loop");
		baronessPhase1.animator.SetTrigger("OnCandyCaneExit");
		yield return baronessPhase1.animator.WaitForAnimationToEnd(baronessPhase1, "Baroness_Transition_3");
		base.animator.SetTrigger("StartPhase2");
		baronessPhase1.transformCounter = 0;
		AudioManager.Play("level_baroness_grab_castle");
		while (transitionCounter <= 4)
		{
			yield return null;
		}
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Castle_Transition_11");
		base.animator.SetTrigger("LoopArms");
		castleWallFix.sortingLayerName = "Default";
		baronessPhase2.gameObject.SetActive(true);
		castleCollidePhase2.SetActive(true);
		state = State.Chase;
		StartCoroutine(handle_scroll_cr());
		StartCoroutine(peppermint_cr());
		StartCoroutine(final_shoot_cr());
		yield return null;
	}

	private void PauseScroll()
	{
		pauseScrolling = !pauseScrolling;
		activateForce = !activateForce;
	}

	private void ActivateTeeth()
	{
		base.animator.Play("Castle_Chase_Arms", 2);
		teethState = TeethState.Idle;
		FrameDelayedCallback(StartBaronessAnimation, 2);
	}

	private void StartBaronessAnimation()
	{
		base.animator.Play("Baroness_Chase");
	}

	private void HitCastleFrame()
	{
		if (inAnimationLoop)
		{
			continueTransition = true;
		}
	}

	private void TransitionCounter()
	{
		transitionCounter++;
	}

	private void SwitchLayersToDefault()
	{
		baronessPhase2.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
		baronessPhase2.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
		blackCastleHole.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
		blackCastleHole.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
		base.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
		base.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
		castlePhase2TopLayer.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
		castlePhase2TopLayer.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
	}

	private IEnumerator handle_scroll_cr()
	{
		scrollForce = new LevelPlayerMotor.VelocityManager.Force(LevelPlayerMotor.VelocityManager.Force.Type.Ground, 190f);
		while (true)
		{
			LevelPlayerMotor[] array = UnityEngine.Object.FindObjectsOfType<LevelPlayerMotor>();
			foreach (LevelPlayerMotor levelPlayerMotor in array)
			{
				if (distToGround < 200f && activateForce)
				{
					levelPlayerMotor.AddForce(scrollForce);
				}
				else
				{
					levelPlayerMotor.RemoveForce(scrollForce);
				}
			}
			yield return null;
		}
	}

	private void HandsSound()
	{
		AudioManager.Play("level_baroness_castle_hands");
	}

	private void CastleRoar()
	{
		AudioManager.Play("level_baroness_castle_roar");
	}

	private void PointSound()
	{
		AudioManager.Play("level_baroness_go_castle");
	}

	private IEnumerator peppermint_cr()
	{
		while (true)
		{
			float seconds = base.properties.CurrentState.peppermint.peppermintSpawnDurationRange.RandomFloat();
			yield return CupheadTime.WaitForSeconds(this, seconds);
			teethState = TeethState.StartOpen;
			yield return null;
		}
	}

	private void OpenTeeth()
	{
		StartCoroutine(open_teeth_cr());
	}

	private IEnumerator open_teeth_cr()
	{
		if (teethState == TeethState.StartOpen)
		{
			teethState = TeethState.Open;
			base.animator.SetBool("TeethOpen", true);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			BaronessLevelPeppermint peppermint = UnityEngine.Object.Instantiate(peppermintPrefab);
			LevelProperties.Baroness.Peppermint p = base.properties.CurrentState.peppermint;
			peppermint.Init(p, emergePoint.position, p.peppermintSpeed);
			yield return CupheadTime.WaitForSeconds(this, 0.5f);
			teethState = TeethState.Off;
		}
	}

	private void CloseTeeth()
	{
		if (teethState == TeethState.Off)
		{
			base.animator.SetBool("TeethOpen", false);
			teethState = TeethState.Idle;
		}
	}

	private void HideTeeth()
	{
		teeth.enabled = false;
	}

	private void ShowTeeth()
	{
		teeth.enabled = true;
	}

	private IEnumerator final_shoot_cr()
	{
		LevelProperties.Baroness.BaronessVonBonbon p = base.properties.CurrentState.baronessVonBonbon;
		string[] headString = p.finalProjectileHeadToss.Split(',');
		int headIndex = UnityEngine.Random.Range(0, headString.Length);
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.baronessVonBonbon.finalProjectileInitialDelay);
		while (true)
		{
			if (headString[headIndex][0] == 'H')
			{
				base.animator.SetBool("Toss", true);
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.baronessVonBonbon.finalProjectileAttackDelayRange.RandomFloat());
			}
			else
			{
				string[] delayString = headString[headIndex].Split(':');
				yield return CupheadTime.WaitForSeconds(this, float.Parse(delayString[1]));
			}
			headIndex = (headIndex + 1) % headString.Length;
			yield return null;
		}
	}

	private void FireHead()
	{
		baronessPhase1.FireFinalProjectile();
		base.animator.SetBool("Toss", false);
	}
}
