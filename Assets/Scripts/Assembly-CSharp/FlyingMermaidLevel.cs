using System;
using System.Collections;
using UnityEngine;

public class FlyingMermaidLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[SerializeField]
	private FlyingMermaidLevelMermaid mermaid;

	[Header("FlyingMermaidLevel")]
	[SerializeField]
	private Prefabs prefabs;

	[SerializeField]
	private FlyingMermaidLevelMerdusa merdusa;

	[SerializeField]
	private FlyingMermaidLevelMerdusaHead merdusaHead;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitMerdusa;

	[SerializeField]
	private Sprite _bossPortraitHead;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteMerdusa;

	[SerializeField]
	private string _bossQuoteHead;

	private LevelProperties.FlyingMermaid properties;

	public bool MerdusaTransformStarted { get; set; }

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.FlyingMermaid.States.Main:
			case LevelProperties.FlyingMermaid.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.FlyingMermaid.States.Merdusa:
				return _bossPortraitMerdusa;
			case LevelProperties.FlyingMermaid.States.Head:
				return _bossPortraitHead;
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
			case LevelProperties.FlyingMermaid.States.Main:
			case LevelProperties.FlyingMermaid.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.FlyingMermaid.States.Merdusa:
				return _bossQuoteMerdusa;
			case LevelProperties.FlyingMermaid.States.Head:
				return _bossQuoteHead;
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
			return Levels.FlyingMermaid;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_flying_mermaid;
		}
	}

	protected override void Start()
	{
		base.Start();
		mermaid.LevelInit(properties);
		merdusa.LevelInit(properties);
		merdusaHead.LevelInit(properties);
		MerdusaTransformStarted = false;
	}

	protected override void OnLevelStart()
	{
		mermaid.IntroContinue();
		StartCoroutine(mermaidPattern_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		Debug.Log("Changed State: " + properties.CurrentState.stateName);
		if (properties.CurrentState.stateName == LevelProperties.FlyingMermaid.States.Merdusa)
		{
			StopAllCoroutines();
			StartCoroutine(transform_to_merdusa_cr());
		}
		if (properties.CurrentState.stateName == LevelProperties.FlyingMermaid.States.Head)
		{
			StopAllCoroutines();
			StartCoroutine(mermaidPattern_cr());
			StartCoroutine(transform_to_head_cr());
		}
	}

	private IEnumerator mermaidPattern_cr()
	{
		while (true)
		{
			yield return StartCoroutine(nextPattern_cr());
			yield return null;
		}
	}

	private IEnumerator nextPattern_cr()
	{
		switch (properties.CurrentState.NextPattern)
		{
		default:
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		case LevelProperties.FlyingMermaid.Pattern.Yell:
			yield return StartCoroutine(yell_cr());
			break;
		case LevelProperties.FlyingMermaid.Pattern.Summon:
			yield return StartCoroutine(summon_cr());
			break;
		case LevelProperties.FlyingMermaid.Pattern.Fish:
			yield return StartCoroutine(fish_cr());
			break;
		case LevelProperties.FlyingMermaid.Pattern.Zap:
			yield return StartCoroutine(zap_cr());
			break;
		case LevelProperties.FlyingMermaid.Pattern.Bubble:
			yield return StartCoroutine(bubble_cr());
			break;
		case LevelProperties.FlyingMermaid.Pattern.HeadBlast:
			yield return StartCoroutine(head_blast_cr());
			break;
		case LevelProperties.FlyingMermaid.Pattern.BubbleHeadBlast:
			yield return StartCoroutine(bubble_head_blast_cr());
			break;
		}
	}

	private IEnumerator yell_cr()
	{
		while (mermaid.state != FlyingMermaidLevelMermaid.State.Idle)
		{
			yield return null;
		}
		mermaid.StartYell();
		while (mermaid.state != FlyingMermaidLevelMermaid.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator transform_to_merdusa_cr()
	{
		mermaid.StartTransform();
		while (merdusa.state != FlyingMermaidLevelMerdusa.State.Idle)
		{
			yield return null;
		}
		StartCoroutine(mermaidPattern_cr());
	}

	private IEnumerator transform_to_head_cr()
	{
		merdusa.StartTransform();
		while (merdusaHead.state != FlyingMermaidLevelMerdusaHead.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator summon_cr()
	{
		while (mermaid.state != FlyingMermaidLevelMermaid.State.Idle)
		{
			yield return null;
		}
		mermaid.StartSummon();
		while (mermaid.state != FlyingMermaidLevelMermaid.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator fish_cr()
	{
		while (mermaid.state != FlyingMermaidLevelMermaid.State.Idle)
		{
			yield return null;
		}
		mermaid.StartFish();
		while (mermaid.state != FlyingMermaidLevelMermaid.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator zap_cr()
	{
		while (merdusa.state != FlyingMermaidLevelMerdusa.State.Idle)
		{
			yield return null;
		}
		merdusa.StartZap();
		while (merdusa.state != FlyingMermaidLevelMerdusa.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator bubble_cr()
	{
		while (merdusaHead.state != FlyingMermaidLevelMerdusaHead.State.Idle)
		{
			yield return null;
		}
		merdusaHead.StartBubble();
		while (merdusaHead.state != FlyingMermaidLevelMerdusaHead.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator head_blast_cr()
	{
		while (merdusaHead.state != FlyingMermaidLevelMerdusaHead.State.Idle)
		{
			yield return null;
		}
		merdusaHead.StartHeadBlast();
		while (merdusaHead.state != FlyingMermaidLevelMerdusaHead.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator bubble_head_blast_cr()
	{
		while (merdusaHead.state != FlyingMermaidLevelMerdusaHead.State.Idle)
		{
			yield return null;
		}
		merdusaHead.StartHeadBubble();
		while (merdusaHead.state != FlyingMermaidLevelMerdusaHead.State.Idle)
		{
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.FlyingMermaid.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
