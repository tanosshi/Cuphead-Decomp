using System;
using System.Collections;
using UnityEngine;

public class AirshipJellyLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[SerializeField]
	private AirshipJellyLevelJelly jelly;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	private LevelProperties.AirshipJelly properties;

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
			return Levels.AirshipJelly;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_airship_jelly;
		}
	}

	protected override void Start()
	{
		base.Start();
		jelly.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
	}

	private IEnumerator airshipPattern_cr()
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
		Debug.Log("No pattern programmed for " + properties.CurrentState.NextPattern);
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.AirshipJelly.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
