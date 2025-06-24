using System.Collections;
using UnityEngine;

public class AirshipCrabLevel : Level
{
	[SerializeField]
	private AirshipCrabLevelCrab crab;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	private LevelProperties.AirshipCrab properties;

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

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.AirshipCrab;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_airship_crab;
		}
	}

	protected override void Start()
	{
		base.Start();
		crab.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(airshipcrabPattern_cr());
	}

	private IEnumerator airshipcrabPattern_cr()
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
		properties = LevelProperties.AirshipCrab.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
