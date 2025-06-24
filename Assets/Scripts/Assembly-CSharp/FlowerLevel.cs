using System;
using System.Collections;
using UnityEngine;

public class FlowerLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[SerializeField]
	public FlowerLevelFlower flower;

	private bool attacking;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitPhaseTwo;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuotePhaseTwo;

	private LevelProperties.Flower properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Flower.States.Main:
			case LevelProperties.Flower.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Flower.States.PhaseTwo:
				return _bossPortraitPhaseTwo;
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
			case LevelProperties.Flower.States.Main:
			case LevelProperties.Flower.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Flower.States.PhaseTwo:
				return _bossQuotePhaseTwo;
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
			return Levels.Flower;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_flower;
		}
	}

	protected override void Start()
	{
		base.Start();
		flower.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(flowerPattern_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		Debug.Log(string.Concat("State Changed: ", properties.CurrentState.stateName, " (", properties.CurrentState.healthTrigger, ")"));
		if (properties.CurrentState.stateName == LevelProperties.Flower.States.PhaseTwo)
		{
			if (Level.Current.mode == Mode.Easy)
			{
				properties.WinInstantly();
				AudioManager.PlayLoop("flower_phase1_death_loop");
				AudioManager.Play("flower_phase1_death_scream");
			}
			else
			{
				StopAllCoroutines();
				flower.PhaseTwoTrigger();
			}
		}
	}

	private IEnumerator flowerPattern_cr()
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
		LevelProperties.Flower.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		case LevelProperties.Flower.Pattern.Laser:
			yield return StartCoroutine(laserAttack_cr());
			break;
		case LevelProperties.Flower.Pattern.PodHands:
			yield return StartCoroutine(potHands_cr());
			break;
		case LevelProperties.Flower.Pattern.GattlingGun:
			yield return StartCoroutine(gattlingGun_cr());
			break;
		default:
			Debug.LogWarning("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		}
	}

	private IEnumerator laserAttack_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.laser.hesitateAfterAttack);
		attacking = true;
		flower.StartLaser(OnLaserAttackComplete);
		while (attacking)
		{
			yield return null;
		}
	}

	private void OnLaserAttackComplete()
	{
		attacking = false;
	}

	private IEnumerator potHands_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.podHands.hesitateAfterAttack);
		attacking = true;
		flower.StartPotHands(OnPotHandsAttackComplete);
		while (attacking)
		{
			yield return null;
		}
	}

	private void OnPotHandsAttackComplete()
	{
		attacking = false;
	}

	private IEnumerator gattlingGun_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.gattlingGun.hesitateAfterAttack);
		attacking = true;
		flower.StartGattlingGun(OnGattlingGunComplete);
		while (attacking)
		{
			yield return null;
		}
	}

	private void OnGattlingGunComplete()
	{
		attacking = false;
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Flower.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
