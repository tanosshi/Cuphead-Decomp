using System.Collections;
using UnityEngine;

public class DicePalaceMainLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private DicePalaceMainLevelGameManager gameManager;

	[SerializeField]
	private DicePalaceMainLevelKingDice kingDice;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceMain properties;

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

	public DicePalaceMainLevelGameManager GameManager
	{
		get
		{
			return gameManager;
		}
	}

	public override DicePalaceLevels CurrentDicePalaceLevel
	{
		get
		{
			return DicePalaceLevels.DicePalaceMain;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceMain;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_main;
		}
	}

	protected override void Start()
	{
		base.Start();
		gameManager.LevelInit(properties);
		kingDice.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalacemainPattern_cr());
	}

	protected override void CheckIfDicePalace()
	{
		base.CheckIfDicePalace();
		Level.IsDicePalaceMain = true;
	}

	private IEnumerator dicepalacemainPattern_cr()
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
		LevelProperties.DicePalaceMain.Pattern p = properties.CurrentState.NextPattern;
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.DicePalaceMain.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
