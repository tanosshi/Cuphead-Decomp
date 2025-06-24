using System.Collections;
using UnityEngine;

public class RetroArcadeLevel : Level
{
	public static float TOTAL_POINTS;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	[SerializeField]
	private RetroArcadeAlienManager alienManager;

	[SerializeField]
	private RetroArcadeCaterpillarManager caterpillarManager;

	[SerializeField]
	private RetroArcadeRobotManager robotManager;

	[SerializeField]
	private RetroArcadePaddleShip paddleShip;

	[SerializeField]
	private RetroArcadeQShip qShip;

	[SerializeField]
	private RetroArcadeUFO ufo;

	[SerializeField]
	private RetroArcadeToadManager toadManager;

	[SerializeField]
	private RetroArcadeWorm worm;

	[SerializeField]
	private RetroArcadeBigPlayer bigCuphead;

	[SerializeField]
	private RetroArcadeBigPlayer bigMugman;

	public ArcadePlayerController playerPrefab;

	private LevelProperties.RetroArcade properties;

	public static float ACCURACY_BONUS { get; private set; }

	public override Sprite BossPortrait
	{
		get
		{
			return _bossPortrait;
		}
	}

	public override string BossQuote
	{
		get
		{
			return _bossQuote;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.RetroArcade;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_retro_arcade;
		}
	}

	protected override void Start()
	{
		base.Start();
		alienManager.LevelInit(properties);
		caterpillarManager.LevelInit(properties);
		robotManager.LevelInit(properties);
		paddleShip.LevelInit(properties);
		qShip.LevelInit(properties);
		ufo.LevelInit(properties);
		toadManager.LevelInit(properties);
		worm.LevelInit(properties);
		ACCURACY_BONUS = properties.CurrentState.general.accuracyBonus;
		bigCuphead.Init(PlayerManager.GetPlayer(PlayerId.PlayerOne) as ArcadePlayerController);
		bigMugman.Init(PlayerManager.GetPlayer(PlayerId.PlayerTwo) as ArcadePlayerController);
	}

	protected override void CreatePlayers()
	{
		base.CreatePlayers();
	}

	protected override void Update()
	{
		base.Update();
		Debug.Log("CURRENT SCORE: " + TOTAL_POINTS);
	}

	protected override void OnLevelStart()
	{
		bigCuphead.LevelStart();
		bigMugman.LevelStart();
		StartStateCoroutine();
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		bigCuphead.OnVictory();
		bigMugman.OnVictory();
		StartStateCoroutine();
	}

	private void StartStateCoroutine()
	{
		switch (properties.CurrentState.stateName)
		{
		case LevelProperties.RetroArcade.States.Main:
		case LevelProperties.RetroArcade.States.Aliens:
			StartCoroutine(startAliens_cr());
			break;
		case LevelProperties.RetroArcade.States.Caterpillar:
			StartCoroutine(startCaterpillars_cr());
			break;
		case LevelProperties.RetroArcade.States.Robots:
			StartCoroutine(startRobots_cr());
			break;
		case LevelProperties.RetroArcade.States.PaddleShip:
			StartCoroutine(startPaddleShip_cr());
			break;
		case LevelProperties.RetroArcade.States.QShip:
			StartCoroutine(startQShip_cr());
			break;
		case LevelProperties.RetroArcade.States.UFO:
			StartCoroutine(startUFO_cr());
			break;
		case LevelProperties.RetroArcade.States.Toad:
			StartCoroutine(startToad_cr());
			break;
		case LevelProperties.RetroArcade.States.Worm:
			StartCoroutine(startWorm_cr());
			break;
		case LevelProperties.RetroArcade.States.Generic:
			break;
		}
	}

	public override void OnLevelEnd()
	{
		base.OnLevelEnd();
		bigCuphead.OnVictory();
		bigMugman.OnVictory();
	}

	private IEnumerator startAliens_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		alienManager.StartAliens();
	}

	private IEnumerator startCaterpillars_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		caterpillarManager.StartCaterpillar();
	}

	private IEnumerator startRobots_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		robotManager.StartRobots();
	}

	private IEnumerator startPaddleShip_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		paddleShip.StartPaddleShip();
	}

	private IEnumerator startQShip_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		qShip.StartQShip();
	}

	private IEnumerator startUFO_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		ufo.StartUFO();
	}

	private IEnumerator startToad_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		toadManager.StartToad();
	}

	private IEnumerator startWorm_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		worm.StartWorm();
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.RetroArcade.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
