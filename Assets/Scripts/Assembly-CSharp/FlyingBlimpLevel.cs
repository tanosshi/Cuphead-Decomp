using System.Collections;
using UnityEngine;

public class FlyingBlimpLevel : Level
{
	[Space(10f)]
	[SerializeField]
	private FlyingBlimpLevelBlimpLady blimpLady;

	[SerializeField]
	private FlyingBlimpLevelMoonLady moonLady;

	public bool changingStates;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitMagical;

	[SerializeField]
	private Sprite _bossPortraitMoon;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteMagical;

	[SerializeField]
	private string _bossQuoteMoon;

	private LevelProperties.FlyingBlimp properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.FlyingBlimp.States.Main:
			case LevelProperties.FlyingBlimp.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.FlyingBlimp.States.Sagittarius:
			case LevelProperties.FlyingBlimp.States.Taurus:
			case LevelProperties.FlyingBlimp.States.Gemini:
			case LevelProperties.FlyingBlimp.States.SagOrGem:
				return _bossPortraitMagical;
			case LevelProperties.FlyingBlimp.States.Moon:
				return _bossPortraitMoon;
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
			case LevelProperties.FlyingBlimp.States.Main:
			case LevelProperties.FlyingBlimp.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.FlyingBlimp.States.Sagittarius:
			case LevelProperties.FlyingBlimp.States.Taurus:
			case LevelProperties.FlyingBlimp.States.Gemini:
			case LevelProperties.FlyingBlimp.States.SagOrGem:
				return _bossQuoteMagical;
			case LevelProperties.FlyingBlimp.States.Moon:
				return _bossQuoteMoon;
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
			return Levels.FlyingBlimp;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_flying_blimp;
		}
	}

	protected override void Start()
	{
		base.Start();
		blimpLady.LevelInit(properties);
		moonLady.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(flyingblimpPattern_cr());
		StartCoroutine(enemies_cr());
	}

	protected override void OnStateChanged()
	{
		Debug.Log("STATE CHANGED: " + properties.CurrentState.stateName);
		base.OnStateChanged();
		changingStates = true;
		StopAllCoroutines();
		StopCoroutine(blimpLady.spawnEnemy_cr());
		StartCoroutine(enemies_cr());
		if (properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.Moon)
		{
			StartCoroutine(morph_to_moon_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.Sagittarius)
		{
			StartCoroutine(sagittarius_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.Taurus)
		{
			StartCoroutine(taurus_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.Gemini)
		{
			StartCoroutine(gemini_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.FlyingBlimp.States.SagOrGem)
		{
			Coroutine coroutine = ((!Rand.Bool()) ? StartCoroutine(gemini_cr()) : StartCoroutine(sagittarius_cr()));
		}
		else
		{
			StartCoroutine(flyingblimpPattern_cr());
		}
	}

	private IEnumerator flyingblimpPattern_cr()
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
		LevelProperties.FlyingBlimp.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		default:
			Debug.Log("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		case LevelProperties.FlyingBlimp.Pattern.Tornado:
			yield return StartCoroutine(tornado_cr());
			break;
		case LevelProperties.FlyingBlimp.Pattern.Shoot:
			yield return StartCoroutine(shoot_cr());
			break;
		}
	}

	private IEnumerator tornado_cr()
	{
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
		blimpLady.StartTornado();
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator sagittarius_cr()
	{
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
		blimpLady.StartSagittarius();
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator taurus_cr()
	{
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
		blimpLady.StartTaurus();
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator gemini_cr()
	{
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
		blimpLady.StartGemini();
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator shoot_cr()
	{
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
		blimpLady.StartShoot();
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator enemies_cr()
	{
		if (!properties.CurrentState.enemy.active)
		{
			yield return null;
		}
		else
		{
			StartCoroutine(blimpLady.spawnEnemy_cr());
		}
		yield return null;
	}

	private IEnumerator morph_to_moon_cr()
	{
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
		blimpLady.SpawnMoonLady();
		while (blimpLady.state != FlyingBlimpLevelBlimpLady.State.Idle)
		{
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.FlyingBlimp.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
