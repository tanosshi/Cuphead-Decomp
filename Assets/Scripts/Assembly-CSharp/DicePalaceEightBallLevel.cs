using System.Collections;
using UnityEngine;

public class DicePalaceEightBallLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private DicePalaceEightBallLevelEightBall eightBall;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceEightBall properties;

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
			return DicePalaceLevels.DicePalaceEightBall;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceEightBall;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_eight_ball;
		}
	}

	protected override void Start()
	{
		base.Start();
		eightBall.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalaceeightballPattern_cr());
	}

	private IEnumerator dicepalaceeightballPattern_cr()
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
		properties = LevelProperties.DicePalaceEightBall.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
