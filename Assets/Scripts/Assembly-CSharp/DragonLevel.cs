using System;
using System.Collections;
using UnityEngine;

public class DragonLevel : Level
{
	private enum LightningState
	{
		Off = 0,
		FirstFlash = 1,
		SecondFlash = 2
	}

	[Serializable]
	public class Prefabs
	{
	}

	private const float Flash1Probability = 0.25f;

	private const float Flash2Probability = 0.25f;

	[SerializeField]
	private DragonLevelBackgroundFlash[] lightningFlashes;

	[SerializeField]
	private DragonLevelCloudPlatform[] platforms;

	[SerializeField]
	private SpriteRenderer spire;

	[SerializeField]
	private SpriteRenderer darkSpire;

	[SerializeField]
	private DragonLevelDragon dragon;

	[SerializeField]
	private DragonLevelLeftSideDragon leftSideDragon;

	[SerializeField]
	private DragonLevelTail tail;

	[SerializeField]
	private DragonLevelPlatformManager manager;

	[SerializeField]
	private DragonLevelLightning lightningStrikes;

	[SerializeField]
	private Material dragonFlashMaterial;

	private LightningState lightningState;

	[SerializeField]
	private SpriteRenderer[] backgroundClouds;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitFireMarchers;

	[SerializeField]
	private Sprite _bossPortraitThreeHeads;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuoteFireMarchers;

	[SerializeField]
	private string _bossQuoteThreeHeads;

	private bool cloudsSameDir;

	private Material dragonMaterial;

	private LevelProperties.Dragon properties;

	public static float SPEED { get; private set; }

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Dragon.States.Main:
			case LevelProperties.Dragon.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Dragon.States.FireMarchers:
				return _bossPortraitFireMarchers;
			case LevelProperties.Dragon.States.ThreeHeads:
				return _bossPortraitThreeHeads;
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
			case LevelProperties.Dragon.States.Main:
			case LevelProperties.Dragon.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Dragon.States.FireMarchers:
				return _bossQuoteFireMarchers;
			case LevelProperties.Dragon.States.ThreeHeads:
				return _bossQuoteThreeHeads;
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
			return Levels.Dragon;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dragon;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		SPEED = 0f;
	}

	protected override void Start()
	{
		base.Start();
		dragon.LevelInit(properties);
		tail.LevelInit(properties);
		leftSideDragon.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(speed_cr());
		StartCoroutine(tail_cr());
		StartCoroutine(dragonPattern_cr());
		manager.Init(properties.CurrentState.clouds);
		SetPlatformVariables(true);
		cloudsSameDir = properties.CurrentState.clouds.movingRight;
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		manager.UpdateProperties(properties.CurrentState.clouds);
		SetPlatformVariables(false);
		if (properties.CurrentState.clouds.movingRight != cloudsSameDir)
		{
			StartCoroutine(speed_cr());
			cloudsSameDir = properties.CurrentState.clouds.movingRight;
		}
		Debug.Log(string.Concat("State Changed: ", properties.CurrentState.stateName, " (", properties.CurrentState.healthTrigger, ")"));
		if (properties.CurrentState.stateName == LevelProperties.Dragon.States.FireMarchers)
		{
			StopAllCoroutines();
			dragon.Leave();
		}
		else if (properties.CurrentState.stateName == LevelProperties.Dragon.States.ThreeHeads)
		{
			StopAllCoroutines();
			leftSideDragon.StartThreeHeads();
			StartCoroutine(phase3ColorTransition());
		}
	}

	private void SetPlatformVariables(bool firstTime)
	{
		DragonLevelCloudPlatform[] array = platforms;
		foreach (DragonLevelCloudPlatform dragonLevelCloudPlatform in array)
		{
			dragonLevelCloudPlatform.GetProperties(properties.CurrentState.clouds, firstTime);
		}
	}

	private IEnumerator dragonPattern_cr()
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
		switch (properties.CurrentState.NextPattern)
		{
		default:
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		case LevelProperties.Dragon.Pattern.Meteor:
			yield return StartCoroutine(meteor_cr());
			break;
		case LevelProperties.Dragon.Pattern.Peashot:
			yield return StartCoroutine(peashot_cr());
			break;
		}
	}

	private IEnumerator meteor_cr()
	{
		while (dragon.state != DragonLevelDragon.State.Idle)
		{
			yield return null;
		}
		dragon.StartMeteor();
		while (dragon.state != DragonLevelDragon.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator peashot_cr()
	{
		while (dragon.state != DragonLevelDragon.State.Idle)
		{
			yield return null;
		}
		dragon.StartPeashot();
		while (dragon.state != DragonLevelDragon.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator speed_cr()
	{
		float t = 0f;
		while (t < 3f)
		{
			SPEED = t / 3f;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		SPEED = 1f;
	}

	private IEnumerator tail_cr()
	{
		while (dragon.state != DragonLevelDragon.State.Idle)
		{
			yield return null;
		}
		while (true)
		{
			if (!properties.CurrentState.tail.active)
			{
				yield return null;
				continue;
			}
			LevelProperties.Dragon.Tail tailProperties = properties.CurrentState.tail;
			tail.TailStart(tailProperties.warningTime, tailProperties.inTime, tailProperties.holdTime, tailProperties.outTime);
			while (tail.state != DragonLevelTail.State.Idle)
			{
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, tailProperties.attackDelay.RandomFloat());
		}
	}

	private IEnumerator phase3ColorTransition()
	{
		while (leftSideDragon.state != DragonLevelLeftSideDragon.State.ThreeHeads)
		{
			yield return null;
		}
		StartCoroutine(lightning_cr());
		float t = 0f;
		float fadeTime = 6f;
		LightningState lastLightningState = lightningState;
		HitFlash dragonHitFlash = leftSideDragon.GetComponentInChildren<HitFlash>();
		dragonMaterial = leftSideDragon.GetComponent<SpriteRenderer>().material;
		while (true)
		{
			LevelPlayerController playerOne = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerOne);
			LevelPlayerController playerTwo = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerTwo);
			float ratio = Mathf.Min(1f, t / fadeTime);
			Color playerColor;
			Color projectileColor;
			Color dragonColor;
			Color darkSpireColor;
			Color platformColor;
			if (lightningState == LightningState.FirstFlash)
			{
				playerColor = ColorUtils.HexToColor("333333");
				projectileColor = ColorUtils.HexToColor("333333");
				dragonColor = ColorUtils.HexToColor("333333");
				darkSpireColor = new Color(0.2f, 0.2f, 0.2f, darkSpire.color.a);
				platformColor = ColorUtils.HexToColor("191919");
			}
			else if (lightningState == LightningState.SecondFlash)
			{
				playerColor = ColorUtils.HexToColor("191919");
				projectileColor = ColorUtils.HexToColor("191919");
				dragonColor = ColorUtils.HexToColor("191919");
				darkSpireColor = new Color(0.1f, 0.1f, 0.1f, darkSpire.color.a);
				platformColor = ColorUtils.HexToColor("0c0c0c");
			}
			else
			{
				playerColor = Color.Lerp(Color.white, ColorUtils.HexToColor("d8d8d8"), ratio);
				projectileColor = Color.Lerp(Color.white, ColorUtils.HexToColor("d8d8d8"), ratio);
				dragonColor = Color.black;
				darkSpireColor = new Color(1f, 1f, 1f, darkSpire.color.a);
				platformColor = Color.Lerp(Color.white, ColorUtils.HexToColor("9c9da63"), ratio);
			}
			if (playerOne != null)
			{
				playerOne.animationController.SetColor(playerColor);
			}
			if (playerTwo != null)
			{
				playerTwo.animationController.SetColor(playerColor);
			}
			darkSpire.color = darkSpireColor;
			if (lightningState != lastLightningState)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(Tags.PlayerProjectile.ToString());
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.color = projectileColor;
					}
				}
			}
			if (!dragonHitFlash.flashing)
			{
				SpriteRenderer[] componentsInChildren = leftSideDragon.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer in componentsInChildren)
				{
					spriteRenderer.material = ((lightningState != LightningState.Off) ? dragonFlashMaterial : dragonMaterial);
					spriteRenderer.color = dragonColor;
				}
			}
			foreach (DragonLevelCloudPlatform platform in manager.platforms)
			{
				platform.GetComponent<SpriteRenderer>().color = platformColor;
				platform.top.color = platformColor;
			}
			for (int k = 0; k < lightningFlashes.Length; k++)
			{
				if (lightningState == LightningState.FirstFlash)
				{
					lightningFlashes[k].SetFlash1();
				}
				else if (lightningState == LightningState.SecondFlash)
				{
					lightningFlashes[k].SetFlash2();
				}
				else if (lightningState == LightningState.Off)
				{
					lightningFlashes[k].SetNormal();
				}
			}
			t += (float)CupheadTime.Delta;
			lastLightningState = lightningState;
			yield return null;
		}
	}

	private IEnumerator lightning_cr()
	{
		while (true)
		{
			lightningState = LightningState.Off;
			yield return CupheadTime.WaitForSeconds(this, MathUtils.ExpRandom(2f) + 1f);
			lightningStrikes.PlayLightning();
			float rand = UnityEngine.Random.value;
			if (rand < 0.25f)
			{
				lightningState = LightningState.FirstFlash;
				yield return CupheadTime.WaitForSeconds(this, 0.041f);
				continue;
			}
			if (rand < 0.5f)
			{
				lightningState = LightningState.SecondFlash;
				yield return CupheadTime.WaitForSeconds(this, 0.041f);
				continue;
			}
			lightningState = LightningState.FirstFlash;
			yield return CupheadTime.WaitForSeconds(this, 0.041f);
			lightningState = LightningState.Off;
			yield return CupheadTime.WaitForSeconds(this, 0.041f);
			lightningState = LightningState.SecondFlash;
			yield return CupheadTime.WaitForSeconds(this, 0.041f);
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Dragon.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
