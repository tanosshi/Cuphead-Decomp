using System.Collections;
using UnityEngine;

public class DicePalaceRouletteLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private DicePalaceRouletteLevelRoulette roulette;

	[SerializeField]
	private DicePalaceRouletteLevelPlatform[] platforms;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceRoulette properties;

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
			return DicePalaceLevels.DicePalaceRoulette;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceRoulette;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_roulette;
		}
	}

	protected override void Start()
	{
		base.Start();
		roulette.LevelInit(properties);
		DicePalaceRouletteLevelPlatform[] array = platforms;
		foreach (DicePalaceRouletteLevelPlatform dicePalaceRouletteLevelPlatform in array)
		{
			dicePalaceRouletteLevelPlatform.Init(properties.CurrentState.platform);
		}
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalaceroulettePattern_cr());
	}

	private IEnumerator dicepalaceroulettePattern_cr()
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
		LevelProperties.DicePalaceRoulette.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		case LevelProperties.DicePalaceRoulette.Pattern.Twirl:
			yield return StartCoroutine(twirl_cr());
			break;
		case LevelProperties.DicePalaceRoulette.Pattern.Marble:
			yield return StartCoroutine(marble_cr());
			break;
		default:
			Debug.LogWarning("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		}
	}

	private IEnumerator twirl_cr()
	{
		while (roulette.state != DicePalaceRouletteLevelRoulette.State.Idle)
		{
			yield return null;
		}
		roulette.StartTwirl();
		while (roulette.state != DicePalaceRouletteLevelRoulette.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator marble_cr()
	{
		while (roulette.state != DicePalaceRouletteLevelRoulette.State.Idle)
		{
			yield return null;
		}
		roulette.StartMarbleDrop();
		while (roulette.state != DicePalaceRouletteLevelRoulette.State.Idle)
		{
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.DicePalaceRoulette.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
