using System;
using System.Collections;
using UnityEngine;

public class SallyStagePlayLevel : Level
{
	[SerializeField]
	private SallyStagePlayLevelBackgroundHandler backgroundHandler;

	[SerializeField]
	private SallyStagePlayLevelAngel angel;

	[SerializeField]
	private SallyStagePlayLevelSally sally;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitHouse;

	[SerializeField]
	private Sprite _bossPortraitAngel;

	[SerializeField]
	private Sprite _bossPortraitFinal;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteHouse;

	[SerializeField]
	private string _bossQuoteAngel;

	[SerializeField]
	private string _bossQuoteFinal;

	private LevelProperties.SallyStagePlay properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.SallyStagePlay.States.Main:
			case LevelProperties.SallyStagePlay.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.SallyStagePlay.States.House:
				return _bossPortraitHouse;
			case LevelProperties.SallyStagePlay.States.Angel:
				return _bossPortraitAngel;
			case LevelProperties.SallyStagePlay.States.Final:
				return _bossPortraitFinal;
			default:
				Debug.LogError(string.Concat("Couldn't find portrait for state ", properties.CurrentState.stateName, ". Using Main."));
				return _bossPortraitMain;
			}
		}
	}

	public override string BossQuote
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.SallyStagePlay.States.Main:
			case LevelProperties.SallyStagePlay.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.SallyStagePlay.States.House:
				return _bossQuoteHouse;
			case LevelProperties.SallyStagePlay.States.Angel:
				return _bossQuoteAngel;
			case LevelProperties.SallyStagePlay.States.Final:
				return _bossQuoteFinal;
			default:
				Debug.LogError(string.Concat("Couldn't find quote for state ", properties.CurrentState.stateName, ". Using Main."));
				return _bossQuoteMain;
			}
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.SallyStagePlay;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_sally_stage_play;
		}
	}

	public event Action OnPhase3;

	public event Action OnPhase2;

	public event Action OnPhase4;

	protected override void Start()
	{
		base.Start();
		sally.LevelInit(properties);
		sally.GetParent(this);
		angel.LevelInit(properties);
		backgroundHandler.GetProperties(properties, this);
		StartCoroutine(intro_cr());
	}

	protected override void OnStateChanged()
	{
		Debug.Log("State: " + properties.CurrentState.stateName);
		base.OnStateChanged();
		if (properties.CurrentState.stateName == LevelProperties.SallyStagePlay.States.House)
		{
			StartCoroutine(residence_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.SallyStagePlay.States.Angel)
		{
			StopAllCoroutines();
			StartCoroutine(angel_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.SallyStagePlay.States.Final)
		{
			StopAllCoroutines();
			StartCoroutine(final_cr());
		}
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(sallystageplayPattern_cr());
	}

	private IEnumerator sallystageplayPattern_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			if (sally.state != SallyStagePlayLevelSally.State.Transition)
			{
				yield return StartCoroutine(nextPattern_cr());
			}
			yield return null;
		}
	}

	private IEnumerator nextPattern_cr()
	{
		LevelProperties.SallyStagePlay.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		case LevelProperties.SallyStagePlay.Pattern.Jump:
			StartCoroutine(jump_cr());
			break;
		case LevelProperties.SallyStagePlay.Pattern.Umbrella:
			StartCoroutine(umbrella_cr());
			break;
		case LevelProperties.SallyStagePlay.Pattern.Kiss:
			StartCoroutine(kiss_cr());
			break;
		case LevelProperties.SallyStagePlay.Pattern.Teleport:
			StartCoroutine(teleport_cr());
			break;
		default:
			Debug.LogWarning("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		}
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2.2f);
		backgroundHandler.OpenCurtain(SallyStagePlayLevelBackgroundHandler.Backgrounds.Church);
		yield return null;
	}

	private IEnumerator jump_cr()
	{
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
		sally.OnJumpAttack();
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator umbrella_cr()
	{
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
		sally.OnUmbrellaAttack();
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator kiss_cr()
	{
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
		sally.OnKissAttack();
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator teleport_cr()
	{
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
		sally.OnTeleportAttack();
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator residence_cr()
	{
		backgroundHandler.RollUpCupids();
		sally.PrePhase2();
		while (sally.state != SallyStagePlayLevelSally.State.Idle)
		{
			yield return null;
		}
		if (this.OnPhase2 != null)
		{
			this.OnPhase2();
			StopAllCoroutines();
			StartCoroutine(sallystageplayPattern_cr());
		}
		yield return null;
	}

	private IEnumerator angel_cr()
	{
		if (this.OnPhase3 != null)
		{
			this.OnPhase3();
		}
		sally.OnPhase3(SallyStagePlayLevelBackgroundHandler.HUSBAND_GONE);
		yield return null;
	}

	private IEnumerator final_cr()
	{
		if (this.OnPhase4 != null)
		{
			this.OnPhase4();
		}
		angel.OnPhase4();
		yield return null;
		AudioManager.PlayLoop("sally_audience_applause_ph4_loop");
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.SallyStagePlay.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
