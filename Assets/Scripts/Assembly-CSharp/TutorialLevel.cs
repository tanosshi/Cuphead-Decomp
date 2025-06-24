using System;
using System.Collections;
using UnityEngine;

public class TutorialLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[SerializeField]
	private Transform background;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	private LevelProperties.Tutorial properties;

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
			return Levels.Tutorial;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_tutorial;
		}
	}

	protected override void Start()
	{
		base.Start();
		background.SetParent(UnityEngine.Camera.main.transform);
		background.ResetLocalTransforms();
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(tutorialPattern_cr());
	}

	private IEnumerator tutorialPattern_cr()
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
		LevelProperties.Tutorial.Pattern p = properties.CurrentState.NextPattern;
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Tutorial.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
