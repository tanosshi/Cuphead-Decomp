using System;
using System.Collections;
using UnityEngine;

public class BatLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[Space(10f)]
	[SerializeField]
	private BatLevelBat bat;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	private LevelProperties.Bat properties;

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
			return Levels.Bat;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_bat;
		}
	}

	protected override void Start()
	{
		base.Start();
		bat.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		base.OnLevelStart();
		StartCoroutine(batPattern_cr());
		StartCoroutine(goblins_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		if (properties.CurrentState.stateName == LevelProperties.Bat.States.Coffin)
		{
			StopAllCoroutines();
			StartCoroutine(phase_2_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.Bat.States.Wolf)
		{
			StopAllCoroutines();
			StartCoroutine(phase_3_cr());
		}
	}

	private IEnumerator batPattern_cr()
	{
		yield return new WaitForSeconds(1f);
		while (true)
		{
			yield return StartCoroutine(nextPattern_cr());
			yield return null;
		}
	}

	private IEnumerator nextPattern_cr()
	{
		LevelProperties.Bat.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		default:
			Debug.Log("No pattern programmed for " + p);
			yield return new WaitForSeconds(1f);
			break;
		case LevelProperties.Bat.Pattern.Bouncer:
			yield return StartCoroutine(bouncer_cr());
			break;
		case LevelProperties.Bat.Pattern.Lightning:
			yield return StartCoroutine(lightning_cr());
			break;
		}
	}

	private IEnumerator bouncer_cr()
	{
		while (bat.state != BatLevelBat.State.Idle)
		{
			yield return null;
		}
		bat.StartBouncer();
		while (bat.state != BatLevelBat.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator lightning_cr()
	{
		while (bat.state != BatLevelBat.State.Idle)
		{
			yield return null;
		}
		bat.StartLightning();
		while (bat.state != BatLevelBat.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator phase_2_cr()
	{
		bat.StartPhase2();
		yield return null;
	}

	private IEnumerator phase_3_cr()
	{
		bat.StartPhase3();
		yield return null;
	}

	private IEnumerator goblins_cr()
	{
		if (!properties.CurrentState.goblins.Enabled)
		{
			yield return null;
		}
		else
		{
			bat.StartGoblin();
		}
		yield return null;
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Bat.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
