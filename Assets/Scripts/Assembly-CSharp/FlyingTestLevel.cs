using System;
using System.Collections;
using UnityEngine;

public class FlyingTestLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	private LevelProperties.FlyingTest properties;

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
			return Levels.FlyingTest;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_flying_test;
		}
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(flyingtestPattern_cr());
	}

	private IEnumerator flyingtestPattern_cr()
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
		LevelProperties.FlyingTest.Pattern p = properties.CurrentState.NextPattern;
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.FlyingTest.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
