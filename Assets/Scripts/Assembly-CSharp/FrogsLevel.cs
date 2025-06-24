using System;
using System.Collections;
using UnityEngine;

public class FrogsLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[SerializeField]
	private FrogsLevelTall tall;

	[SerializeField]
	private FrogsLevelShort small;

	[SerializeField]
	private FrogsLevelMorphed morphed;

	[SerializeField]
	private FrogsLevelDemonTrigger demonTrigger;

	private bool wantsToRoll;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitRoll;

	[SerializeField]
	private Sprite _bossPortraitMorph;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteRoll;

	[SerializeField]
	private string _bossQuoteMorph;

	private Coroutine checkCoroutine;

	private Coroutine stateCoroutine;

	private Coroutine fanCoroutine;

	private LevelProperties.Frogs properties;

	public static bool FINAL_FORM { get; private set; }

	public static bool DEMON_TRIGGERED { get; private set; }

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Frogs.States.Main:
			case LevelProperties.Frogs.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Frogs.States.Roll:
				return _bossPortraitRoll;
			case LevelProperties.Frogs.States.Morph:
				return _bossPortraitMorph;
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
			case LevelProperties.Frogs.States.Main:
			case LevelProperties.Frogs.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Frogs.States.Roll:
				return _bossQuoteRoll;
			case LevelProperties.Frogs.States.Morph:
				return _bossQuoteMorph;
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
			return Levels.Frogs;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_frogs;
		}
	}

	protected override void Start()
	{
		base.Start();
		tall.LevelInit(properties);
		small.LevelInit(properties);
		morphed.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		FINAL_FORM = false;
		StartState(LevelProperties.Frogs.States.Main);
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		LevelProperties.Frogs.States stateName = properties.CurrentState.stateName;
		Debug.Log("STATE CHANGED: " + stateName);
		if (stateName == LevelProperties.Frogs.States.Morph)
		{
			FINAL_FORM = true;
		}
		StartState(stateName);
	}

	protected override void CreatePlayers()
	{
		base.CreatePlayers();
		if (PlayerManager.Multiplayer && allowMultiplayer)
		{
			tall.AddFanForce(players[0]);
			tall.AddFanForce(players[1]);
		}
		else
		{
			tall.AddFanForce(players[0]);
		}
	}

	protected override void CreatePlayerTwoOnJoin()
	{
		base.CreatePlayerTwoOnJoin();
		if (PlayerManager.Multiplayer && allowMultiplayer)
		{
			tall.AddFanForce(players[1]);
		}
	}

	private void StartState(LevelProperties.Frogs.States state)
	{
		if (state != LevelProperties.Frogs.States.Generic)
		{
			if (checkCoroutine != null)
			{
				StopCoroutine(checkCoroutine);
			}
			checkCoroutine = null;
			if (stateCoroutine != null)
			{
				StopCoroutine(stateCoroutine);
			}
			stateCoroutine = null;
			if (fanCoroutine != null)
			{
				StopCoroutine(fanCoroutine);
			}
			fanCoroutine = null;
		}
		checkCoroutine = StartCoroutine(startState_cr(state));
	}

	private IEnumerator startState_cr(LevelProperties.Frogs.States state)
	{
		switch (state)
		{
		default:
			stateCoroutine = StartCoroutine(mainState_cr());
			break;
		case LevelProperties.Frogs.States.Roll:
			wantsToRoll = true;
			yield return StartCoroutine(waitForFrogs_cr());
			small.StartRoll();
			yield return StartCoroutine(waitForShort_cr());
			stateCoroutine = StartCoroutine(rollState_cr());
			break;
		case LevelProperties.Frogs.States.Morph:
			yield return StartCoroutine(waitForFrogs_cr());
			DEMON_TRIGGERED = demonTrigger.getTrigger();
			tall.StartMorph();
			small.StartMorph();
			break;
		}
	}

	private IEnumerator mainState_cr()
	{
		while (true)
		{
			LevelProperties.Frogs.Pattern p = properties.CurrentState.NextPattern;
			switch (p)
			{
			case LevelProperties.Frogs.Pattern.ShortClap:
				yield return StartCoroutine(shortClap_cr());
				break;
			case LevelProperties.Frogs.Pattern.ShortRage:
				yield return StartCoroutine(shortRage_cr());
				break;
			case LevelProperties.Frogs.Pattern.TallFan:
				yield return StartCoroutine(tallFan_cr());
				break;
			case LevelProperties.Frogs.Pattern.TallFireflies:
				yield return StartCoroutine(tallFireflies_cr());
				break;
			case LevelProperties.Frogs.Pattern.RagePlusFireflies:
				yield return StartCoroutine(ragePlusFireflies_cr());
				break;
			default:
				Debug.Log("No pattern programmed for " + p);
				yield return new WaitForSeconds(1f);
				break;
			}
		}
	}

	private IEnumerator rollState_cr()
	{
		if (fanCoroutine != null)
		{
			StopCoroutine(fanCoroutine);
		}
		fanCoroutine = StartCoroutine(rollFan_cr());
		while (true)
		{
			LevelProperties.Frogs.Pattern p = properties.CurrentState.NextPattern;
			switch (p)
			{
			case LevelProperties.Frogs.Pattern.ShortClap:
				yield return StartCoroutine(shortClap_cr());
				break;
			case LevelProperties.Frogs.Pattern.ShortRage:
				yield return StartCoroutine(shortRage_cr());
				break;
			default:
				Debug.Log("No pattern programmed for " + p);
				yield return new WaitForSeconds(1f);
				break;
			}
		}
	}

	private IEnumerator rollFan_cr()
	{
		float hesitate = properties.CurrentState.tallFan.hesitate;
		yield return StartCoroutine(waitForShort_cr());
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			tall.StartFan();
			yield return StartCoroutine(waitForTall_cr());
			yield return CupheadTime.WaitForSeconds(this, hesitate);
		}
	}

	private IEnumerator waitForFrogs_cr()
	{
		while ((tall.state != FrogsLevelTall.State.Complete && tall.state != FrogsLevelTall.State.Morphed && tall.state != FrogsLevelTall.State.Idle) || (small.state != FrogsLevelShort.State.Complete && small.state != FrogsLevelShort.State.Morphed && small.state != FrogsLevelShort.State.Idle))
		{
			yield return null;
		}
	}

	private IEnumerator waitForTall_cr()
	{
		while (tall.state != FrogsLevelTall.State.Complete && tall.state != FrogsLevelTall.State.Morphed)
		{
			yield return null;
		}
	}

	private IEnumerator waitForShort_cr()
	{
		while (small.state != FrogsLevelShort.State.Complete && small.state != FrogsLevelShort.State.Morphed)
		{
			yield return null;
		}
	}

	private IEnumerator tallFan_cr()
	{
		tall.StartFan();
		yield return StartCoroutine(waitForTall_cr());
	}

	private IEnumerator tallFireflies_cr()
	{
		tall.StartFireflies();
		yield return StartCoroutine(waitForTall_cr());
	}

	private IEnumerator shortRage_cr()
	{
		small.StartRage();
		yield return StartCoroutine(waitForShort_cr());
	}

	private IEnumerator ragePlusFireflies_cr()
	{
		tall.StartFireflies();
		small.StartRage();
		while (!wantsToRoll)
		{
			if (tall.state == FrogsLevelTall.State.Complete)
			{
				tall.StartFireflies();
			}
			if (small.state == FrogsLevelShort.State.Complete)
			{
				small.StartRage();
			}
			yield return null;
		}
	}

	private IEnumerator shortClap_cr()
	{
		small.StartClap();
		yield return StartCoroutine(waitForShort_cr());
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Frogs.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
