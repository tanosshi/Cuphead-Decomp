using System.Collections;
using UnityEngine;

public class RobotLevel : Level
{
	[SerializeField]
	private RobotLevelRobot robot;

	[SerializeField]
	private RobotLevelHelihead heliHead;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitHeliHead;

	[SerializeField]
	private Sprite _bossPortraitInventor;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteHeliHead;

	[SerializeField]
	private string _bossQuoteInventor;

	private LevelProperties.Robot properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Robot.States.Main:
			case LevelProperties.Robot.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Robot.States.HeliHead:
				return _bossPortraitHeliHead;
			case LevelProperties.Robot.States.Inventor:
				return _bossPortraitInventor;
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
			case LevelProperties.Robot.States.Main:
			case LevelProperties.Robot.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Robot.States.HeliHead:
				return _bossQuoteHeliHead;
			case LevelProperties.Robot.States.Inventor:
				return _bossQuoteInventor;
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
			return Levels.Robot;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_robot;
		}
	}

	protected override void Start()
	{
		base.Start();
		robot.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(robotPattern_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		switch (properties.CurrentState.stateName)
		{
		case LevelProperties.Robot.States.HeliHead:
			StopAllCoroutines();
			robot.TriggerPhaseTwo(OnHeliheadSpawn);
			break;
		case LevelProperties.Robot.States.Inventor:
			heliHead.ChangeState();
			break;
		}
	}

	private IEnumerator robotPattern_cr()
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
		Debug.LogWarning("No pattern programmed for " + properties.CurrentState.NextPattern);
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	private void OnHeliheadSpawn()
	{
		StartCoroutine(spawnHeliHead_cr());
	}

	private IEnumerator spawnHeliHead_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2.5f);
		robot.animator.SetTrigger("Phase2Transition");
		yield return robot.animator.WaitForAnimationToEnd(this, "Death Dance", true);
		heliHead.GetComponent<RobotLevelHelihead>().InitHeliHead(properties);
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Robot.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
