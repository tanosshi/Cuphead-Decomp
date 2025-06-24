using System.Collections;
using UnityEngine;

public class DicePalaceCardLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private DicePalaceCardGameManager gameManager;

	[SerializeField]
	private DicePalaceCardLevelCard card;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceCard properties;

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

	public override DicePalaceLevels CurrentDicePalaceLevel
	{
		get
		{
			return DicePalaceLevels.DicePalaceCard;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceCard;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_card;
		}
	}

	protected override void Start()
	{
		base.Start();
		card.LevelInit(properties);
		gameManager.GameSetup(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalacecardPattern_cr());
		StartCoroutine(gameManager.start_game_cr());
	}

	private IEnumerator dicepalacecardPattern_cr()
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
		properties = LevelProperties.DicePalaceCard.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
