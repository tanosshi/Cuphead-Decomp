using System.Collections;
using UnityEngine;

public class DicePalaceCigarLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private DicePalaceCigarLevelCigar cigar;

	[SerializeField]
	private Animator fire;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceCigar properties;

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
			return DicePalaceLevels.DicePalaceCigar;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceCigar;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_cigar;
		}
	}

	protected override void Start()
	{
		base.Start();
		cigar.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalacecigarPattern_cr());
	}

	private IEnumerator dicepalacecigarPattern_cr()
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
		properties = LevelProperties.DicePalaceCigar.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
