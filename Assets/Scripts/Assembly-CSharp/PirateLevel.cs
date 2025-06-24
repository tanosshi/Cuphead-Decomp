using System;
using System.Collections;
using UnityEngine;

public class PirateLevel : Level
{
	public enum Creature
	{
		Squid = 0,
		Shark = 1,
		DogFish = 2
	}

	public delegate void WhistleDelegate(Creature creature);

	[Serializable]
	public class Prefabs
	{
		public PirateLevelSquid squid;

		public PirateLevelShark shark;

		public PirateLevelDogFish dogFish;

		[Space(10f)]
		public PirateLevelSquidInkOverlay inkOverlay;
	}

	private const float WHISTLE_ANIM_TIME = 1.5f;

	[Space(10f)]
	public PirateLevelPirate pirate;

	public PirateLevelPirateDead deadPirate;

	public PirateLevelBoat boat;

	public PirateLevelBarrel barrel;

	public PirateLevelDogFishScope scope;

	public Transform[] boatParts;

	[Space(10f)]
	[SerializeField]
	private Prefabs prefabs;

	private PirateLevelSquidInkOverlay inkOverlay;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitBoat;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteBoat;

	private LevelProperties.Pirate properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Pirate.States.Main:
			case LevelProperties.Pirate.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Pirate.States.Boat:
				return _bossPortraitBoat;
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
			case LevelProperties.Pirate.States.Main:
			case LevelProperties.Pirate.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Pirate.States.Boat:
				return _bossQuoteBoat;
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
			return Levels.Pirate;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_pirate;
		}
	}

	public event WhistleDelegate OnWhistleEvent;

	protected override void Awake()
	{
		base.Awake();
		inkOverlay = prefabs.inkOverlay.InstantiatePrefab<PirateLevelSquidInkOverlay>();
	}

	protected override void Start()
	{
		base.Start();
		barrel.LevelInit(properties);
		inkOverlay.LevelInit(properties);
		pirate.LevelInit(properties);
		boat.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(piratePattern_cr());
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		Debug.Log("New State: " + properties.CurrentState.stateName);
	}

	private void StartBoat()
	{
		StopAllCoroutines();
		boat.OnLaunchPirate += OnBoatLaunchPirate;
		boat.StartTransformation();
	}

	private void OnBoatLaunchPirate()
	{
		StartCoroutine(launchPirate_cr());
	}

	private void Whistle(Creature creature)
	{
		if (this.OnWhistleEvent != null)
		{
			this.OnWhistleEvent(creature);
		}
	}

	private IEnumerator piratePattern_cr()
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
		LevelProperties.Pirate.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		case LevelProperties.Pirate.Pattern.Squid:
			yield return StartCoroutine(squid_cr());
			break;
		case LevelProperties.Pirate.Pattern.Shark:
			yield return StartCoroutine(shark_cr());
			break;
		case LevelProperties.Pirate.Pattern.DogFish:
			yield return StartCoroutine(dogFish_cr());
			break;
		case LevelProperties.Pirate.Pattern.Peashot:
			yield return StartCoroutine(peashot_cr());
			break;
		case LevelProperties.Pirate.Pattern.Boat:
			StartBoat();
			break;
		default:
			Debug.Log("No pattern programmed for " + p);
			yield return new WaitForSeconds(1f);
			break;
		}
	}

	private IEnumerator squid_cr()
	{
		Whistle(Creature.Squid);
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		Debug.Log(properties.CurrentState.squid.startDelay);
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.squid.startDelay);
		PirateLevelSquid squid = prefabs.squid.InstantiatePrefab<PirateLevelSquid>();
		squid.LevelInit(properties);
		while (squid.state != PirateLevelSquid.State.Exit && squid.state != PirateLevelSquid.State.Die)
		{
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.squid.endDelay);
	}

	private IEnumerator shark_cr()
	{
		Whistle(Creature.Shark);
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.shark.startDelay);
		PirateLevelShark shark = prefabs.shark.InstantiatePrefab<PirateLevelShark>();
		shark.LevelInitWithGroup(properties.CurrentState.shark);
		while (shark.state != PirateLevelShark.State.Complete)
		{
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.shark.endDelay);
	}

	private IEnumerator dogFish_cr()
	{
		bool secretHitBox = false;
		Whistle(Creature.DogFish);
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		scope.In();
		yield return CupheadTime.WaitForSeconds(this, this.properties.CurrentState.dogFish.startDelay);
		LevelProperties.Pirate.DogFish properties = this.properties.CurrentState.dogFish;
		for (int i = 0; i < properties.count; i++)
		{
			Debug.Log(i);
			secretHitBox = i == 3;
			PirateLevelDogFish dogFish = prefabs.dogFish.InstantiatePrefab<PirateLevelDogFish>();
			dogFish.transform.SetPosition(0f, -210f, 0f);
			dogFish.Init(this.properties, secretHitBox);
			yield return CupheadTime.WaitForSeconds(this, properties.nextFishDelay);
		}
		yield return CupheadTime.WaitForSeconds(this, properties.endDelay);
	}

	private IEnumerator peashot_cr()
	{
		LevelProperties.Pirate.Peashot properties = this.properties.CurrentState.peashot;
		KeyValue[] pattern = KeyValue.ListFromString(properties.patterns[UnityEngine.Random.Range(0, properties.patterns.Length)], new char[2] { 'P', 'D' });
		pirate.StartGun();
		yield return CupheadTime.WaitForSeconds(this, properties.startDelay);
		for (int i = 0; i < pattern.Length; i++)
		{
			if (pattern[i].key == "P")
			{
				for (int p = 0; (float)p < pattern[i].value; p++)
				{
					yield return CupheadTime.WaitForSeconds(this, properties.shotDelay);
					pirate.FireGun(properties);
				}
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, pattern[i].value);
			}
			yield return null;
		}
		pirate.EndGun();
		yield return CupheadTime.WaitForSeconds(this, properties.endDelay);
	}

	private IEnumerator launchPirate_cr()
	{
		LevelProperties.Pirate.Boat p = properties.CurrentState.boat;
		deadPirate.Go(p.pirateFallDelay, p.pirateFallTime);
		float t = 0f;
		float time = 1f;
		float speed = 1200f;
		while (t < time)
		{
			float y = speed * (float)CupheadTime.Delta;
			Transform[] array = boatParts;
			foreach (Transform transform in array)
			{
				transform.AddPosition(0f, y);
			}
			pirate.transform.AddPosition(0f, y);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		Transform[] array2 = boatParts;
		foreach (Transform transform2 in array2)
		{
			UnityEngine.Object.Destroy(transform2.gameObject);
		}
		pirate.CleanUp();
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Pirate.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
