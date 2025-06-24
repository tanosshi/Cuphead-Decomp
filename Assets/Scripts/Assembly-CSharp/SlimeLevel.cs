using System.Collections;
using UnityEngine;

public class SlimeLevel : Level
{
	[SerializeField]
	private SlimeLevelSlime smallSlime;

	[SerializeField]
	private SlimeLevelSlime bigSlime;

	[SerializeField]
	private SlimeLevelTombstone tombStone;

	private bool reachedBigSlimeState;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitBigSlime;

	[SerializeField]
	private Sprite _bossPortraitTombstone;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteBigSlime;

	[SerializeField]
	private string _bossQuoteTombstone;

	private LevelProperties.Slime properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Slime.States.Main:
			case LevelProperties.Slime.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Slime.States.BigSlime:
				return _bossPortraitBigSlime;
			case LevelProperties.Slime.States.Tombstone:
				return _bossPortraitTombstone;
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
			case LevelProperties.Slime.States.Main:
			case LevelProperties.Slime.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Slime.States.BigSlime:
				return _bossQuoteBigSlime;
			case LevelProperties.Slime.States.Tombstone:
				return _bossQuoteTombstone;
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
			return Levels.Slime;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_slime;
		}
	}

	protected override void Start()
	{
		base.Start();
		smallSlime.LevelInit(properties);
		bigSlime.LevelInit(properties);
		tombStone.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		smallSlime.IntroContinue();
		StartCoroutine(slimePattern_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		Debug.Log("STATE: " + properties.CurrentState.stateName);
		if (properties.CurrentState.stateName == LevelProperties.Slime.States.BigSlime)
		{
			reachedBigSlimeState = true;
			StopAllCoroutines();
			smallSlime.Transform();
		}
		else if (properties.CurrentState.stateName == LevelProperties.Slime.States.Tombstone)
		{
			StopAllCoroutines();
			bigSlime.DeathTransform();
		}
		if (!reachedBigSlimeState)
		{
			smallSlime.CurrentPropertyState = properties.CurrentState;
		}
		bigSlime.CurrentPropertyState = properties.CurrentState;
	}

	private IEnumerator slimePattern_cr()
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

	protected override void PartialInit()
	{
		properties = LevelProperties.Slime.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
