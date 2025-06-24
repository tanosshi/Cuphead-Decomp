using System;
using System.Collections;
using UnityEngine;

public class MouseLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[SerializeField]
	private MouseLevelCanMouse mouseCan;

	[SerializeField]
	private MouseLevelBrokenCanMouse mouseBrokenCan;

	[SerializeField]
	private MouseLevelCat cat;

	[SerializeField]
	private Animator wallAnimator;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitBrokenCan;

	[SerializeField]
	private Sprite _bossPortraitCat;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteBrokenCan;

	[SerializeField]
	private string _bossQuoteCat;

	private bool hasMoved;

	private LevelProperties.Mouse properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Mouse.States.Main:
			case LevelProperties.Mouse.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Mouse.States.BrokenCan:
				return _bossPortraitBrokenCan;
			case LevelProperties.Mouse.States.Cat:
				return _bossPortraitCat;
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
			case LevelProperties.Mouse.States.Main:
			case LevelProperties.Mouse.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Mouse.States.BrokenCan:
				return _bossQuoteBrokenCan;
			case LevelProperties.Mouse.States.Cat:
				return _bossQuoteCat;
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
			return Levels.Mouse;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_mouse;
		}
	}

	protected override void Start()
	{
		base.Start();
		mouseCan.LevelInit(properties);
		mouseBrokenCan.LevelInit(properties);
		cat.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(mousePattern_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		Debug.Log("STATE: " + properties.CurrentState.stateName);
		if (properties.CurrentState.stateName == LevelProperties.Mouse.States.BrokenCan)
		{
			StopAllCoroutines();
			mouseCan.Explode(StartMouseCanPlatform, OnMouseCanTransitionComplete);
		}
		else if (properties.CurrentState.stateName == LevelProperties.Mouse.States.Cat)
		{
			StopAllCoroutines();
			StartCoroutine(catIntro_cr());
		}
	}

	private void StartMouseCanPlatform()
	{
		wallAnimator.SetTrigger("OnContinue");
		AudioManager.Play("level_mouse_phase2_background_shelf_drop");
	}

	private void OnMouseCanTransitionComplete()
	{
		StartCoroutine(mousePattern_cr());
	}

	private IEnumerator mousePattern_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.canMove.initialHesitate);
		while (true)
		{
			yield return StartCoroutine(nextPattern_cr());
			yield return null;
		}
	}

	private IEnumerator nextPattern_cr()
	{
		LevelProperties.Mouse.Pattern p = properties.CurrentState.NextPattern;
		if (p != LevelProperties.Mouse.Pattern.Dash || hasMoved)
		{
			hasMoved = true;
			switch (p)
			{
			default:
				yield return CupheadTime.WaitForSeconds(this, 1f);
				break;
			case LevelProperties.Mouse.Pattern.Dash:
				yield return StartCoroutine(canDash_cr());
				break;
			case LevelProperties.Mouse.Pattern.CherryBomb:
				yield return StartCoroutine(cherryBomb_cr());
				break;
			case LevelProperties.Mouse.Pattern.Catapult:
				yield return StartCoroutine(catapult_cr());
				break;
			case LevelProperties.Mouse.Pattern.RomanCandle:
				yield return StartCoroutine(romanCandle_cr());
				break;
			case LevelProperties.Mouse.Pattern.LeftClaw:
				yield return StartCoroutine(claw_cr(true));
				break;
			case LevelProperties.Mouse.Pattern.RightClaw:
				yield return StartCoroutine(claw_cr(false));
				break;
			case LevelProperties.Mouse.Pattern.GhostMouse:
				yield return StartCoroutine(ghostMouse_cr());
				break;
			}
		}
	}

	private IEnumerator canDash_cr()
	{
		while (mouseCan.state != MouseLevelCanMouse.State.Idle)
		{
			yield return null;
		}
		mouseCan.StartDash();
		while (mouseCan.state != MouseLevelCanMouse.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator cherryBomb_cr()
	{
		while (mouseCan.state != MouseLevelCanMouse.State.Idle)
		{
			yield return null;
		}
		mouseCan.StartCherryBomb();
		while (mouseCan.state != MouseLevelCanMouse.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator catapult_cr()
	{
		while (mouseCan.state != MouseLevelCanMouse.State.Idle)
		{
			yield return null;
		}
		mouseCan.StartCatapult();
		while (mouseCan.state != MouseLevelCanMouse.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator romanCandle_cr()
	{
		while (mouseCan.state != MouseLevelCanMouse.State.Idle)
		{
			yield return null;
		}
		mouseCan.StartRomanCandle();
		while (mouseCan.state != MouseLevelCanMouse.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator claw_cr(bool left)
	{
		while (cat.state != MouseLevelCat.State.Idle)
		{
			yield return null;
		}
		cat.StartClaw(left);
		while (cat.state != MouseLevelCat.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator ghostMouse_cr()
	{
		while (cat.state != MouseLevelCat.State.Idle)
		{
			yield return null;
		}
		cat.StartGhostMouse();
		while (cat.state != MouseLevelCat.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator catIntro_cr()
	{
		mouseBrokenCan.Transform();
		while (cat.state != MouseLevelCat.State.Idle)
		{
			yield return null;
		}
		StartCoroutine(mousePattern_cr());
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Mouse.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
