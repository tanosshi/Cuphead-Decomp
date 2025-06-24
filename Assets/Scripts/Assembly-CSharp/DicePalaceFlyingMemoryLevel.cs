using System.Collections;
using UnityEngine;

public class DicePalaceFlyingMemoryLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private DicePalaceFlyingMemoryLevelStuffedToy stuffedToy;

	[SerializeField]
	private DicePalaceFlyingMemoryLevelGameManager gameManager;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceFlyingMemory properties;

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
			return DicePalaceLevels.DicePalaceFlyingMemory;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceFlyingMemory;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_flying_memory;
		}
	}

	protected override void Start()
	{
		base.Start();
		gameManager.LevelInit(properties);
		stuffedToy.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalaceflyingmemoryPattern_cr());
	}

	private IEnumerator dicepalaceflyingmemoryPattern_cr()
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
		properties = LevelProperties.DicePalaceFlyingMemory.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
