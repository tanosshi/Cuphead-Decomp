using System.Collections;
using UnityEngine;

public class DicePalaceDominoLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private DicePalaceDominoLevelDomino domino;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceDomino properties;

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
			return DicePalaceLevels.DicePalaceDomino;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceDomino;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_domino;
		}
	}

	protected override void Start()
	{
		base.Start();
		domino.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalacedominoPattern_cr());
	}

	private IEnumerator dicepalacedominoPattern_cr()
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
		LevelProperties.DicePalaceDomino.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		case LevelProperties.DicePalaceDomino.Pattern.Boomerang:
			yield return StartCoroutine(boomerang_cr());
			break;
		case LevelProperties.DicePalaceDomino.Pattern.BouncyBall:
			yield return StartCoroutine(bouncyball_cr());
			break;
		default:
			Debug.LogWarning("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		}
	}

	private IEnumerator boomerang_cr()
	{
		while (domino.state != DicePalaceDominoLevelDomino.State.Idle)
		{
			yield return null;
		}
		domino.OnBoomerang();
		while (domino.state != DicePalaceDominoLevelDomino.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator bouncyball_cr()
	{
		while (domino.state != DicePalaceDominoLevelDomino.State.Idle)
		{
			yield return null;
		}
		domino.OnBouncyBall();
		while (domino.state != DicePalaceDominoLevelDomino.State.Idle)
		{
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.DicePalaceDomino.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
