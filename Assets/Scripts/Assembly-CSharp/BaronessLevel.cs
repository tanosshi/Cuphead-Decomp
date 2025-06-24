using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronessLevel : Level
{
	[SerializeField]
	private BaronessLevelCastle castle;

	private BaronessLevelMiniBossBase currentMiniBoss;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitGumball;

	[SerializeField]
	private Sprite _bossPortraitWaffle;

	[SerializeField]
	private Sprite _bossPortraitCandyCorn;

	[SerializeField]
	private Sprite _bossPortraitCupcake;

	[SerializeField]
	private Sprite _bossPortraitJawbreaker;

	[SerializeField]
	private Sprite _bossPortraitChase;

	[SerializeField]
	private string _bossQuoteGumball;

	[SerializeField]
	private string _bossQuoteWaffle;

	[SerializeField]
	private string _bossQuoteCandyCorn;

	[SerializeField]
	private string _bossQuoteCupcake;

	[SerializeField]
	private string _bossQuoteJawbreaker;

	[SerializeField]
	private string _bossQuoteChase;

	private LevelProperties.Baroness properties;

	public static List<string> PICKED_BOSSES { get; private set; }

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Baroness.States.Main:
			case LevelProperties.Baroness.States.Generic:
				if (currentMiniBoss == null)
				{
					return _bossPortraitChase;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.Gumball)
				{
					return _bossPortraitGumball;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.Waffle)
				{
					return _bossPortraitWaffle;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.CandyCorn)
				{
					return _bossPortraitCandyCorn;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.Cupcake)
				{
					return _bossPortraitCupcake;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.Jawbreaker)
				{
					return _bossPortraitJawbreaker;
				}
				return _bossPortraitChase;
			case LevelProperties.Baroness.States.Chase:
				return _bossPortraitChase;
			default:
				Debug.LogError(string.Concat("Couldn't find portrait for state ", properties.CurrentState.stateName, ". Using Main."));
				return _bossPortraitChase;
			}
		}
	}

	public override string BossQuote
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Baroness.States.Main:
			case LevelProperties.Baroness.States.Generic:
				if (currentMiniBoss == null)
				{
					return _bossQuoteChase;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.Gumball)
				{
					return _bossQuoteGumball;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.Waffle)
				{
					return _bossQuoteWaffle;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.CandyCorn)
				{
					return _bossQuoteCandyCorn;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.Cupcake)
				{
					return _bossQuoteCupcake;
				}
				if (currentMiniBoss.bossId == BaronessLevelCastle.BossPossibility.Jawbreaker)
				{
					return _bossQuoteJawbreaker;
				}
				return _bossQuoteChase;
			case LevelProperties.Baroness.States.Chase:
				return _bossQuoteChase;
			default:
				Debug.LogError(string.Concat("Couldn't find quote for state ", properties.CurrentState.stateName, ". Using Main."));
				return _bossQuoteChase;
			}
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.Baroness;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_baroness;
		}
	}

	protected override void Start()
	{
		base.Start();
		castle.LevelInit(properties);
		PickMiniBosses();
	}

	public void PickMiniBosses()
	{
		StartCoroutine(pickminibosses_cr());
	}

	private IEnumerator update_current_boss_cr()
	{
		while (properties.CurrentState.stateName != LevelProperties.Baroness.States.Chase)
		{
			while (BaronessLevelCastle.CURRENT_MINI_BOSS == currentMiniBoss && BaronessLevelCastle.CURRENT_MINI_BOSS != null)
			{
				yield return null;
			}
			currentMiniBoss = BaronessLevelCastle.CURRENT_MINI_BOSS;
			if (currentMiniBoss != null)
			{
				currentMiniBoss.OnDamageTakenEvent += base.timeline.DealDamage;
			}
			yield return null;
		}
	}

	private IEnumerator pickminibosses_cr()
	{
		LevelProperties.Baroness.Open p = properties.CurrentState.open;
		string[] pattern = p.miniBossString.GetRandom().Split(',');
		int randIndex = 0;
		List<string> tempList = new List<string>(pattern);
		PICKED_BOSSES = new List<string>();
		for (int i = 0; i < p.miniBossAmount; i++)
		{
			randIndex = Random.Range(0, tempList.ToArray().Length);
			PICKED_BOSSES.Add(tempList[randIndex]);
			tempList.Remove(tempList[randIndex]);
		}
		SetUpTimeline();
		yield return null;
	}

	private void SetUpTimeline()
	{
		base.timeline = new Timeline();
		base.timeline.health = 0f;
		List<float> list = new List<float>();
		for (int i = 0; i < PICKED_BOSSES.Count; i++)
		{
			switch (PICKED_BOSSES[i])
			{
			case "1":
				base.timeline.health += properties.CurrentState.gumball.HP;
				list.Add(properties.CurrentState.gumball.HP);
				break;
			case "2":
				base.timeline.health += properties.CurrentState.waffle.HP;
				list.Add(properties.CurrentState.waffle.HP);
				break;
			case "3":
				base.timeline.health += properties.CurrentState.candyCorn.HP;
				list.Add(properties.CurrentState.candyCorn.HP);
				break;
			case "4":
				base.timeline.health += properties.CurrentState.cupcake.HP;
				list.Add(properties.CurrentState.cupcake.HP);
				break;
			case "5":
				base.timeline.health += properties.CurrentState.jawbreaker.jawbreakerHomingHP;
				list.Add(properties.CurrentState.jawbreaker.jawbreakerHomingHP);
				break;
			}
		}
		if (base.mode != Mode.Easy)
		{
			base.timeline.health += properties.CurrentHealth;
		}
		for (int j = 0; j < PICKED_BOSSES.Count; j++)
		{
			base.timeline.AddEventAtHealth(PICKED_BOSSES[j], base.timeline.GetHealthOfLastEvent() + (int)list[j]);
		}
		if (base.mode != Mode.Easy)
		{
			base.timeline.AddEventAtHealth("Chasing", base.timeline.GetHealthOfLastEvent() + (int)properties.CurrentHealth);
		}
		castle.StartIntro();
		StartCoroutine(update_current_boss_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		if (properties.CurrentState.stateName == LevelProperties.Baroness.States.Chase)
		{
			StopAllCoroutines();
			StartCoroutine(chase_cr());
		}
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(baronessPattern_cr());
	}

	private IEnumerator baronessPattern_cr()
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
		Debug.LogWarning("No pattern programmed for " + properties.CurrentState.NextPattern);
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	private IEnumerator chase_cr()
	{
		castle.StartChase();
		yield return null;
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Baroness.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
