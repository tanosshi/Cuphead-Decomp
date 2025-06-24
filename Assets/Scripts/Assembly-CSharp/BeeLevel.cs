using System;
using System.Collections;
using UnityEngine;

public class BeeLevel : Level
{
	[Serializable]
	public class Prefabs
	{
		public BeeLevelGrunt grunt;

		public BeeLevelHoneyDrip drip;
	}

	private const float SPEED_TIME = 0.5f;

	[SerializeField]
	private BeeLevelAirplane airplane;

	[Space(10f)]
	[SerializeField]
	private BeeLevelQueen queen;

	[SerializeField]
	private BeeLevelSecurityGuard guard;

	[Space(10f)]
	[SerializeField]
	private Transform[] gruntRoots;

	[Space(10f)]
	[SerializeField]
	private BeeLevelBackground background;

	[Space(10f)]
	[SerializeField]
	private Prefabs prefabs;

	private bool honeyDripping = true;

	private float speed;

	private float targetSpeed;

	private int missingPlatformCount;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitGuard;

	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitAirplane;

	[SerializeField]
	private string _bossQuoteGuard;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteAirplane;

	private Coroutine gruntCoroutine;

	private LevelProperties.Bee properties;

	public float Speed
	{
		get
		{
			return speed * CupheadTime.GlobalSpeed;
		}
	}

	public int MissingPlatformCount
	{
		get
		{
			return missingPlatformCount;
		}
	}

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Bee.States.Main:
				return _bossPortraitGuard;
			case LevelProperties.Bee.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Bee.States.Airplane:
				return _bossPortraitAirplane;
			default:
				Debug.LogError(string.Concat("Couldn't find portrait for state ", properties.CurrentState.stateName, ". Using Main."));
				return _bossPortraitMain;
			}
		}
	}

	public override string BossQuote
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Bee.States.Main:
				return _bossQuoteGuard;
			case LevelProperties.Bee.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Bee.States.Airplane:
				return _bossQuoteAirplane;
			default:
				Debug.LogError(string.Concat("Couldn't find quote for state ", properties.CurrentState.stateName, ". Using Main."));
				return _bossQuoteMain;
			}
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.Bee;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_bee;
		}
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(drip_cr());
		queen.LevelInit(properties);
		guard.LevelInit(properties);
		background.LevelInit(properties);
		airplane.LevelInit(properties);
	}

	protected override void Update()
	{
		base.Update();
		UpdateSpeed();
	}

	protected override void OnLevelStart()
	{
		missingPlatformCount = properties.CurrentState.movement.missingPlatforms;
		targetSpeed = 0f - properties.CurrentState.movement.speed;
		StartCoroutine(beePattern_cr());
		CheckGrunts();
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		if (properties.CurrentState.stateName == LevelProperties.Bee.States.Airplane)
		{
			StartCoroutine(airplane_cr());
		}
	}

	private void UpdateSpeed()
	{
		speed = Mathf.Lerp(speed, targetSpeed, 0.5f * (float)CupheadTime.Delta);
	}

	private void CheckGrunts()
	{
		if (gruntCoroutine != null)
		{
			StopCoroutine(gruntCoroutine);
		}
		if (properties.CurrentState.grunts.active)
		{
			gruntCoroutine = StartCoroutine(grunts_cr());
		}
	}

	private IEnumerator beePattern_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			yield return StartCoroutine(nextPattern_cr());
			yield return null;
		}
	}

	private IEnumerator nextPattern_cr()
	{
		switch (properties.CurrentState.NextPattern)
		{
		default:
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		case LevelProperties.Bee.Pattern.SecurityGuard:
			yield return StartCoroutine(security_cr());
			break;
		case LevelProperties.Bee.Pattern.Chain:
			yield return StartCoroutine(chain_cr());
			break;
		case LevelProperties.Bee.Pattern.BlackHole:
			yield return StartCoroutine(blackHole_cr());
			break;
		case LevelProperties.Bee.Pattern.Triangle:
			yield return StartCoroutine(triangle_cr());
			break;
		case LevelProperties.Bee.Pattern.Follower:
			yield return StartCoroutine(follower_cr());
			break;
		case LevelProperties.Bee.Pattern.Turbine:
			yield return StartCoroutine(turbine_cr());
			break;
		case LevelProperties.Bee.Pattern.Wing:
			yield return StartCoroutine(wing_cr());
			break;
		}
	}

	private IEnumerator airplane_cr()
	{
		while (queen.state != BeeLevelQueen.State.Idle)
		{
			yield return null;
		}
		queen.StartMorph();
		honeyDripping = false;
		targetSpeed = 0f - properties.CurrentState.general.screenScrollSpeed;
		while (queen.state != BeeLevelQueen.State.Idle)
		{
			yield return null;
		}
		yield return null;
	}

	private IEnumerator turbine_cr()
	{
		while (airplane.state != BeeLevelAirplane.State.Idle)
		{
			yield return null;
		}
		airplane.StartTurbine();
		while (airplane.state != BeeLevelAirplane.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator wing_cr()
	{
		while (airplane.state != BeeLevelAirplane.State.Idle)
		{
			yield return null;
		}
		airplane.StartWing();
		while (airplane.state != BeeLevelAirplane.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator security_cr()
	{
		guard.StartSecurityGuard();
		while (guard.state != BeeLevelSecurityGuard.State.Ready)
		{
			yield return null;
		}
	}

	private IEnumerator blackHole_cr()
	{
		queen.StartBlackHole();
		while (queen.state != BeeLevelQueen.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator triangle_cr()
	{
		queen.StartTriangle();
		while (queen.state != BeeLevelQueen.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator follower_cr()
	{
		queen.StartFollower();
		while (queen.state != BeeLevelQueen.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator chain_cr()
	{
		queen.StartChain();
		while (queen.state != BeeLevelQueen.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator drip_cr()
	{
		while (honeyDripping)
		{
			yield return CupheadTime.WaitForSeconds(this, UnityEngine.Random.Range(1, 3));
			prefabs.drip.Create();
		}
	}

	private IEnumerator grunts_cr()
	{
		string[] strings = properties.CurrentState.grunts.entrancePoints[UnityEngine.Random.Range(0, properties.CurrentState.grunts.entrancePoints.Length)].Split(',');
		int[] positions = new int[strings.Length];
		for (int i = 0; i < strings.Length; i++)
		{
			int.TryParse(strings[i], out positions[i]);
			positions[i] = Mathf.Clamp(positions[i], 0, gruntRoots.Length);
		}
		int index = UnityEngine.Random.Range(0, positions.Length);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.grunts.delay);
			int scale = ((!(PlayerManager.Center.x > 0f)) ? 1 : (-1));
			if (PlayerManager.Center.x > 0f)
			{
				scale = -1;
			}
			prefabs.grunt.Create(gruntRoots[positions[index]].position + new Vector3(840 * scale, 0f, 0f), scale, properties.CurrentState.grunts.health, properties.CurrentState.grunts.speed);
			index = (int)Mathf.Repeat(index + 1, positions.Length);
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Bee.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
