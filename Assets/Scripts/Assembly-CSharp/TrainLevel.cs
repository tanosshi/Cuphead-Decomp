using System;
using System.Collections;
using UnityEngine;

public class TrainLevel : Level
{
	[Serializable]
	public class Prefabs
	{
	}

	[SerializeField]
	private TrainLevelTrain train;

	[Space(10f)]
	[SerializeField]
	private TrainLevelPumpkin pumpkinPrefab;

	[SerializeField]
	private Transform leftValve;

	[SerializeField]
	private Transform rightValve;

	[Space(10f)]
	[SerializeField]
	private TrainLevelBlindSpecter blindSpecter;

	[SerializeField]
	private TrainLevelSkeleton skeleton;

	[SerializeField]
	private TrainLevelLollipopGhoulsManager ghouls;

	[SerializeField]
	private TrainLevelEngineBoss engine;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitSpecter;

	[SerializeField]
	private Sprite _bossPortraitSkeleton;

	[SerializeField]
	private Sprite _bossPortraitLollipop;

	[SerializeField]
	private Sprite _bossPortraitEngine;

	[SerializeField]
	private string _bossQuoteSpecter;

	[SerializeField]
	private string _bossQuoteSkeleton;

	[SerializeField]
	private string _bossQuoteLollipop;

	[SerializeField]
	private string _bossQuoteEngine;

	private bool pumpkinsEnabled;

	private int currentPhase;

	private LevelProperties.Train properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (currentPhase)
			{
			case 1:
				return _bossPortraitSpecter;
			case 2:
				return _bossPortraitSkeleton;
			case 3:
				return _bossPortraitLollipop;
			case 4:
				return _bossPortraitEngine;
			default:
				Debug.LogError("Couldn't find portrait for phase " + currentPhase + ". Using Main.");
				return _bossPortraitEngine;
			}
		}
	}

	public override string BossQuote
	{
		get
		{
			switch (currentPhase)
			{
			case 1:
				return _bossQuoteSpecter;
			case 2:
				return _bossQuoteSkeleton;
			case 3:
				return _bossQuoteLollipop;
			case 4:
				return _bossQuoteEngine;
			default:
				Debug.LogError("Couldn't find portrait for phase " + currentPhase + ". Using Main.");
				return _bossQuoteEngine;
			}
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.Train;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_train;
		}
	}

	protected override void Start()
	{
		base.Start();
		properties.OnBossDamaged -= base.timeline.DealDamage;
		base.timeline = new Timeline();
		base.timeline.health = 0f;
		base.timeline.health += properties.CurrentState.blindSpecter.health;
		base.timeline.health += properties.CurrentState.skeleton.health;
		base.timeline.health += properties.CurrentState.lollipopGhouls.health * 2f;
		base.timeline.health += properties.CurrentState.engine.health;
		base.timeline.AddEvent(new Timeline.Event("Skeleton", 1f - (float)properties.CurrentState.blindSpecter.health / base.timeline.health));
		base.timeline.AddEvent(new Timeline.Event("Lollipop Ghouls", 1f - ((float)properties.CurrentState.blindSpecter.health + properties.CurrentState.skeleton.health) / base.timeline.health));
		base.timeline.AddEvent(new Timeline.Event("Engine", 1f - ((float)properties.CurrentState.blindSpecter.health + properties.CurrentState.skeleton.health + properties.CurrentState.lollipopGhouls.health * 2f) / base.timeline.health));
		train.LevelInit(properties);
		blindSpecter.LevelInit(properties);
		skeleton.LevelInit(properties);
		ghouls.LevelInit(properties);
		engine.LevelInit(properties);
		blindSpecter.OnDeathEvent += OnBlindSpecterDeath;
		skeleton.OnDeathEvent += OnSkeletonDeath;
		ghouls.OnDeathEvent += OnLollipopsDeath;
		engine.OnDeathEvent += OnEngineDeath;
		blindSpecter.OnDamageTakenEvent += base.timeline.DealDamage;
		skeleton.OnDamageTakenEvent += base.timeline.DealDamage;
		ghouls.OnDamageTakenEvent += base.timeline.DealDamage;
		engine.OnDamageTakenEvent += base.timeline.DealDamage;
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(pumpkins_cr());
		StartCoroutine(trainPattern_cr());
		setPhase(1);
	}

	private void OnBlindSpecterDeath()
	{
		train.OnBlindSpectreDeath();
		setPhase(2);
	}

	private void OnSkeletonDeath()
	{
		train.OnSkeletonDeath();
		setPhase(3);
	}

	private void OnLollipopsDeath()
	{
		if (Level.Current.mode == Mode.Easy)
		{
			properties.WinInstantly();
			return;
		}
		train.OnLollipopsDeath();
		setPhase(4);
	}

	private void OnEngineDeath()
	{
		properties.WinInstantly();
	}

	private void setPhase(int phase)
	{
		currentPhase = phase;
		string[] array = properties.CurrentState.pumpkins.bossPhaseOn.Split(',');
		foreach (string s in array)
		{
			int result = 0;
			int.TryParse(s, out result);
			if (result == phase)
			{
				pumpkinsEnabled = true;
				return;
			}
		}
		pumpkinsEnabled = false;
	}

	private IEnumerator trainPattern_cr()
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
		LevelProperties.Train.Pattern p = properties.CurrentState.NextPattern;
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator pumpkins_cr()
	{
		int dir = (Rand.Bool() ? 1 : (-1));
		Transform target = rightValve;
		LevelProperties.Train.Pumpkins p = properties.CurrentState.pumpkins;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, p.delay);
			if (pumpkinsEnabled)
			{
				pumpkinPrefab.Create(new Vector2(840 * -dir, 280f), dir, p.speed, p.health, p.fallTime, target);
				dir *= -1;
				if (train.state != TrainLevelTrain.State.BlindSpecter)
				{
					target = ((dir >= 0) ? leftValve : rightValve);
				}
			}
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Train.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
