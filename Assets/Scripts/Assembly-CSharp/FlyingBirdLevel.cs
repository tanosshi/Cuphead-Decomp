using System;
using System.Collections;
using UnityEngine;

public class FlyingBirdLevel : Level
{
	[Serializable]
	public class Prefabs
	{
		public FlyingBirdLevelEnemy formationBird;

		public FlyingBirdLevelTurret turretBird;
	}

	[SerializeField]
	private FlyingBirdLevelBird bird;

	[SerializeField]
	private FlyingBirdLevelSmallBird smallBird;

	[Space(10f)]
	[SerializeField]
	private Transform enemyRoot;

	[Space(10f)]
	[SerializeField]
	private Transform turretRootTop;

	[SerializeField]
	private Transform turretRootBottom;

	[Space(10f)]
	[SerializeField]
	private Prefabs prefabs;

	private IEnumerator skybirdPattern;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitHouseDeath;

	[SerializeField]
	private Sprite _bossPortraitBirdRevival;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteHouseDeath;

	[SerializeField]
	private string _bossQuoteBirdRevival;

	private LevelProperties.FlyingBird properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.FlyingBird.States.Main:
			case LevelProperties.FlyingBird.States.Generic:
			case LevelProperties.FlyingBird.States.Whistle:
				return _bossPortraitMain;
			case LevelProperties.FlyingBird.States.HouseDeath:
				return _bossPortraitHouseDeath;
			case LevelProperties.FlyingBird.States.BirdRevival:
				return _bossPortraitBirdRevival;
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
			case LevelProperties.FlyingBird.States.Main:
			case LevelProperties.FlyingBird.States.Generic:
			case LevelProperties.FlyingBird.States.Whistle:
				return _bossQuoteMain;
			case LevelProperties.FlyingBird.States.HouseDeath:
				return _bossQuoteHouseDeath;
			case LevelProperties.FlyingBird.States.BirdRevival:
				return _bossQuoteBirdRevival;
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
			return Levels.FlyingBird;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_flying_bird;
		}
	}

	protected override void Start()
	{
		base.Start();
		bird.LevelInit(properties);
		smallBird.LevelInit(properties);
		skybirdPattern = skybirdPattern_cr();
	}

	protected override void OnLevelStart()
	{
		bird.IntroContinue();
		StartCoroutine(skybirdPattern_cr());
		StartCoroutine(enemies_cr());
		StartCoroutine(turrets_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		Debug.Log("State Changed: " + properties.CurrentState.stateName);
		if (properties.CurrentState.stateName == LevelProperties.FlyingBird.States.HouseDeath)
		{
			StopCoroutine(skybirdPattern);
			StartCoroutine(houseDie_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.FlyingBird.States.BirdRevival)
		{
			StopCoroutine(skybirdPattern);
			StartCoroutine(birdHouseRevival_cr());
		}
	}

	private IEnumerator skybirdPattern_cr()
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
		LevelProperties.FlyingBird.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		default:
			Debug.Log("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		case LevelProperties.FlyingBird.Pattern.Feathers:
			yield return StartCoroutine(feathers_cr());
			break;
		case LevelProperties.FlyingBird.Pattern.Eggs:
			yield return StartCoroutine(eggs_cr());
			break;
		case LevelProperties.FlyingBird.Pattern.Lasers:
			yield return StartCoroutine(lasers_cr());
			break;
		case LevelProperties.FlyingBird.Pattern.Garbage:
			yield return StartCoroutine(garbage_cr());
			break;
		case LevelProperties.FlyingBird.Pattern.Heart:
			yield return StartCoroutine(heartAttack_cr());
			break;
		}
	}

	private IEnumerator feathers_cr()
	{
		while (bird.state != FlyingBirdLevelBird.State.Idle)
		{
			yield return null;
		}
		bird.StartFeathers();
		while (bird.state != FlyingBirdLevelBird.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator eggs_cr()
	{
		while (bird.state != FlyingBirdLevelBird.State.Idle)
		{
			yield return null;
		}
		bird.StartEggs();
		while (bird.state != FlyingBirdLevelBird.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator lasers_cr()
	{
		while (bird.state != FlyingBirdLevelBird.State.Idle)
		{
			yield return null;
		}
		bird.StartLasers();
		while (bird.state != FlyingBirdLevelBird.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator houseDie_cr()
	{
		bird.BirdFall();
		yield return null;
	}

	private IEnumerator enemies_cr()
	{
		bool firstTime = true;
		AbstractPlayerController target = PlayerManager.GetNext();
		int r = 1;
		while (true)
		{
			if (!this.properties.CurrentState.enemies.active)
			{
				firstTime = true;
				while (!this.properties.CurrentState.enemies.active)
				{
					yield return null;
				}
			}
			LevelProperties.FlyingBird.Enemies properties = this.properties.CurrentState.enemies;
			int i = 0;
			FlyingBirdLevelEnemy.Properties p = new FlyingBirdLevelEnemy.Properties(properties.health, properties.speed, properties.floatRange, properties.floatTime, properties.projectileHeight, properties.projectileFallTime, properties.projectileDelay);
			target = PlayerManager.GetNext();
			Vector2 pos = enemyRoot.position;
			if (!this.properties.CurrentState.enemies.aim)
			{
				pos.y *= r;
				r *= -1;
			}
			else
			{
				pos.y = target.center.y;
			}
			for (; i < properties.count; i++)
			{
				yield return CupheadTime.WaitForSeconds(this, properties.delay);
				bool parryable = i == properties.count - 1;
				prefabs.formationBird.Create(pos, p).SetParryable(parryable);
			}
			yield return CupheadTime.WaitForSeconds(this, firstTime ? properties.initalGroupDelay : properties.groupDelay);
			firstTime = false;
		}
	}

	private IEnumerator turrets_cr()
	{
		FlyingBirdLevelTurret top = null;
		FlyingBirdLevelTurret bottom = null;
		while (true)
		{
			if (!properties.CurrentState.turrets.active)
			{
				while (!properties.CurrentState.turrets.active)
				{
					yield return null;
				}
			}
			if (top == null || top.transform == null || top.state == FlyingBirdLevelTurret.State.Respawn)
			{
				top = CreateTurret(turretRootTop.position);
			}
			if (bottom == null || bottom.transform == null || bottom.state == FlyingBirdLevelTurret.State.Respawn)
			{
				bottom = CreateTurret(turretRootBottom.position);
			}
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.turrets.respawnDelay);
		}
	}

	private IEnumerator birdHouseRevival_cr()
	{
		while (smallBird.isActiveAndEnabled)
		{
			yield return null;
		}
		bird.OnBossRevival();
		while (bird.state == FlyingBirdLevelBird.State.Reviving)
		{
			yield return null;
		}
		StartCoroutine(skybirdPattern_cr());
	}

	private IEnumerator garbage_cr()
	{
		while (bird.state != FlyingBirdLevelBird.State.Revived)
		{
			yield return null;
		}
		bird.StartGarbageOne();
		while (bird.state != FlyingBirdLevelBird.State.Revived)
		{
			yield return null;
		}
	}

	private IEnumerator heartAttack_cr()
	{
		while (bird.state != FlyingBirdLevelBird.State.Revived)
		{
			yield return null;
		}
		bird.StartHeartAttack();
		while (bird.state != FlyingBirdLevelBird.State.Revived)
		{
			yield return null;
		}
	}

	private FlyingBirdLevelTurret CreateTurret(Vector2 pos)
	{
		FlyingBirdLevelTurret.Properties properties = new FlyingBirdLevelTurret.Properties(this.properties.CurrentState.turrets.health, this.properties.CurrentState.turrets.inTime, pos.x, this.properties.CurrentState.turrets.bulletSpeed, this.properties.CurrentState.turrets.bulletDelay, this.properties.CurrentState.turrets.floatRange, this.properties.CurrentState.turrets.floatTime);
		return prefabs.turretBird.Create(new Vector2(690f, pos.y), properties);
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.FlyingBird.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
