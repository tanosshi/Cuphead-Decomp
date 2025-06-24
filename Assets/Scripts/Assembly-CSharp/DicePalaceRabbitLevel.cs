using System.Collections;
using UnityEngine;

public class DicePalaceRabbitLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private DicePalaceRabbitLevelRabbit rabbit;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceRabbit properties;

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
			return DicePalaceLevels.DicePalaceRabbit;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceRabbit;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_rabbit;
		}
	}

	protected override void Start()
	{
		base.Start();
		rabbit.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalacerabbitPattern_cr());
	}

	private IEnumerator dicepalacerabbitPattern_cr()
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
		LevelProperties.DicePalaceRabbit.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		case LevelProperties.DicePalaceRabbit.Pattern.MagicWand:
			yield return StartCoroutine(magicwand_cr());
			break;
		case LevelProperties.DicePalaceRabbit.Pattern.MagicParry:
			yield return StartCoroutine(magicparry_cr());
			break;
		default:
			Debug.LogWarning("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		}
	}

	private IEnumerator magicwand_cr()
	{
		while (rabbit.state != DicePalaceRabbitLevelRabbit.State.Idle)
		{
			yield return null;
		}
		rabbit.OnMagicWand();
		while (rabbit.state != DicePalaceRabbitLevelRabbit.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator magicparry_cr()
	{
		while (rabbit.state != DicePalaceRabbitLevelRabbit.State.Idle)
		{
			yield return null;
		}
		rabbit.OnMagicParry();
		while (rabbit.state != DicePalaceRabbitLevelRabbit.State.Idle)
		{
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.DicePalaceRabbit.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
