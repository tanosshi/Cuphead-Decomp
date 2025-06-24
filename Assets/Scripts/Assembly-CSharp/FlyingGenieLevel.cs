using System.Collections;
using UnityEngine;

public class FlyingGenieLevel : Level
{
	public const float SHADE_PERIOD = 12f;

	private const float SHADE_START_TIME = 9.25f;

	public static float mainTimer;

	[SerializeField]
	private FlyingGenieLevelGoop goop;

	[SerializeField]
	private FlyingGenieLevelGenie genie;

	[SerializeField]
	private FlyingGenieLevelGenieTransform genieTransformed;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitDisappear;

	[SerializeField]
	private Sprite _bossPortraitCoffin;

	[SerializeField]
	private Sprite _bossPortraitMarionette;

	[SerializeField]
	private Sprite _bossPortraitGiant;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteDisappear;

	[SerializeField]
	private string _bossQuoteCoffin;

	[SerializeField]
	private string _bossQuoteMarionette;

	[SerializeField]
	private string _bossQuoteGiant;

	private LevelProperties.FlyingGenie properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.FlyingGenie.States.Main:
			case LevelProperties.FlyingGenie.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.FlyingGenie.States.Disappear:
				return (genie.state != FlyingGenieLevelGenie.State.Disappear) ? _bossPortraitCoffin : _bossPortraitDisappear;
			case LevelProperties.FlyingGenie.States.Marionette:
				return _bossPortraitMarionette;
			case LevelProperties.FlyingGenie.States.Giant:
				return _bossPortraitGiant;
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
			case LevelProperties.FlyingGenie.States.Main:
			case LevelProperties.FlyingGenie.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.FlyingGenie.States.Disappear:
				return (genie.state != FlyingGenieLevelGenie.State.Disappear) ? _bossQuoteCoffin : _bossQuoteDisappear;
			case LevelProperties.FlyingGenie.States.Marionette:
				return _bossQuoteMarionette;
			case LevelProperties.FlyingGenie.States.Giant:
				return _bossQuoteGiant;
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
			return Levels.FlyingGenie;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_flying_genie;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(timer_cr());
	}

	protected override void Start()
	{
		base.Start();
		genie.LevelInit(properties);
		genieTransformed.LevelInit(properties);
		goop.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		Debug.Log("State changed: " + properties.CurrentState.stateName);
		if (properties.CurrentState.stateName == LevelProperties.FlyingGenie.States.Marionette)
		{
			StartCoroutine(phase2_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.FlyingGenie.States.Giant)
		{
			StartCoroutine(phase3_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.FlyingGenie.States.Disappear)
		{
			StartCoroutine(pillar_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.FlyingGenie.States.Generic)
		{
			StartCoroutine(treasure_cr());
		}
	}

	private IEnumerator flyinggeniePattern_cr()
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

	private IEnumerator phase2_cr()
	{
		genie.HitTrigger();
		while (genie.state != FlyingGenieLevelGenie.State.Idle)
		{
			yield return null;
		}
		genie.animator.SetTrigger("ToPhase2");
	}

	private IEnumerator phase3_cr()
	{
		genieTransformed.EndMarionette();
		yield return null;
	}

	private IEnumerator pillar_cr()
	{
		genie.HitTrigger();
		while (genie.state != FlyingGenieLevelGenie.State.Idle)
		{
			yield return null;
		}
		genie.StartObelisk();
		while (genie.state != FlyingGenieLevelGenie.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator treasure_cr()
	{
		genie.HitTrigger();
		while (genie.state != FlyingGenieLevelGenie.State.Idle)
		{
			yield return null;
		}
		genie.StartTreasure();
		while (genie.state != FlyingGenieLevelGenie.State.Idle)
		{
			yield return null;
		}
	}

	private static IEnumerator timer_cr()
	{
		mainTimer = 9.25f;
		while (true)
		{
			mainTimer += CupheadTime.Delta;
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.FlyingGenie.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
