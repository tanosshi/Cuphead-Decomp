using System.Collections;
using UnityEngine;

public class DevilLevel : Level
{
	private const float Phase2FadeInTime = 2f;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitMain;

	[SerializeField]
	private Sprite _bossPortraitPhaseTwo;

	[SerializeField]
	private Sprite _bossPortraitPhaseThree;

	[SerializeField]
	private string _bossQuoteMain;

	[SerializeField]
	private string _bossQuotePhaseTwo;

	[SerializeField]
	private string _bossQuotePhaseThree;

	[SerializeField]
	private GameObject groundHandler;

	[SerializeField]
	private ParallaxLayer[] parallax;

	[SerializeField]
	private GameObject pit;

	[SerializeField]
	private GameObject middlePiece;

	[SerializeField]
	private Transform phase1Scroll;

	[SerializeField]
	private SpriteRenderer phase1Foreground;

	[SerializeField]
	private GameObject phase2Background;

	[SerializeField]
	private GameObject phase3Platforms;

	[SerializeField]
	private SpriteRenderer phase1Fade;

	[SerializeField]
	private DevilLevelSittingDevil sittingDevil;

	[SerializeField]
	private DevilLevelSplitDevil[] splitDevils;

	[SerializeField]
	private DevilLevelGiantHead giantHead;

	[SerializeField]
	private DevilLevelEffectSpawner[] smokeSpawners;

	[SerializeField]
	private Transform Phase2P1spawn;

	[SerializeField]
	private Transform Phase2P2spawn;

	private LevelProperties.Devil properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (properties.CurrentState.stateName)
			{
			case LevelProperties.Devil.States.Main:
			case LevelProperties.Devil.States.Generic:
				return _bossPortraitMain;
			case LevelProperties.Devil.States.GiantHead:
				return _bossPortraitPhaseTwo;
			case LevelProperties.Devil.States.Hands:
			case LevelProperties.Devil.States.Tears:
				return _bossPortraitPhaseThree;
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
			case LevelProperties.Devil.States.Main:
			case LevelProperties.Devil.States.Generic:
				return _bossQuoteMain;
			case LevelProperties.Devil.States.GiantHead:
				return _bossQuotePhaseTwo;
			case LevelProperties.Devil.States.Hands:
			case LevelProperties.Devil.States.Tears:
				return _bossQuotePhaseThree;
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
			return Levels.Devil;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_devil;
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		isDevil = true;
		DevilLevelSplitDevil[] array = splitDevils;
		foreach (DevilLevelSplitDevil devilLevelSplitDevil in array)
		{
			devilLevelSplitDevil.LevelInit(properties);
		}
		sittingDevil.LevelInit(properties);
		giantHead.LevelInit(properties);
		StartCoroutine(DelayedStart());
	}

	private IEnumerator DelayedStart()
	{
		yield return null;
		phase2Background.SetActive(false);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(devilPattern_cr());
		sittingDevil.StartDemons();
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		Debug.Log("State Changed: " + properties.CurrentState.stateName);
		if (properties.CurrentState.stateName == LevelProperties.Devil.States.Split)
		{
			StopAllCoroutines();
		}
		else if (properties.CurrentState.stateName == LevelProperties.Devil.States.GiantHead)
		{
			StopAllCoroutines();
			sittingDevil.StartTransform();
			StartCoroutine(phase_1_end_trans());
			StartCoroutine(devilPattern_cr());
		}
		else if (properties.CurrentState.stateName == LevelProperties.Devil.States.Hands)
		{
			StopAllCoroutines();
			giantHead.StartHands();
		}
		else if (properties.CurrentState.stateName == LevelProperties.Devil.States.Tears)
		{
			StopAllCoroutines();
			giantHead.StartTears();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	private IEnumerator phase_1_end_trans()
	{
		DevilLevelEffectSpawner[] array = smokeSpawners;
		foreach (DevilLevelEffectSpawner devilLevelEffectSpawner in array)
		{
			devilLevelEffectSpawner.KillSmoke();
		}
		while (!DevilLevelHole.PHASE_1_COMPLETE)
		{
			yield return null;
		}
		groundHandler.SetActive(false);
		bool startZoomout = false;
		float t = 0f;
		float cameraSlideUpTime = 1f;
		float time = 3.3f;
		float endCameraTime = 2f;
		Vector3 phase1Start = phase1Scroll.transform.position;
		Vector3 phase1End = Vector3.zero;
		Vector3 cameraStart = CupheadLevelCamera.Current.transform.position;
		Vector3 cameraEffectEnd = new Vector3(CupheadLevelCamera.Current.transform.position.x, 50f);
		Vector3 cameraOffsetEnd = new Vector3(CupheadLevelCamera.Current.transform.position.x, 600f);
		ParallaxLayer[] array2 = parallax;
		foreach (ParallaxLayer parallaxLayer in array2)
		{
			parallaxLayer.enabled = false;
		}
		sittingDevil.RemoveFire();
		yield return StartCoroutine(CupheadLevelCamera.Current.slide_camera_cr(cameraEffectEnd, cameraSlideUpTime));
		StartCoroutine(CupheadLevelCamera.Current.slide_camera_cr(cameraOffsetEnd, time));
		while (t < time)
		{
			if (t >= 2f && !startZoomout)
			{
				ZoomOut(cameraStart, endCameraTime);
				startZoomout = true;
			}
			t += (float)CupheadTime.Delta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			phase1Scroll.transform.position = Vector3.Lerp(phase1Start, phase1End, val);
			Color c = phase1Foreground.color;
			c.a = Mathf.Clamp(1f - t * 2f, 0f, 1f);
			phase1Foreground.color = c;
			yield return null;
		}
		phase1Scroll.transform.position = phase1End;
		giantHead.transform.parent = null;
		giantHead.StartIntroTransform();
		StartCoroutine(phase2BackgroundFade_cr(2f));
		AudioManager.FadeBGMVolume(0f, 0.5f, true);
		AudioManager.PlayBGMPlaylistManually(false);
		AudioManager.Play("transition_sting");
		yield return CupheadTime.WaitForSeconds(this, endCameraTime);
		phase1Scroll.gameObject.SetActive(false);
		Object.Destroy(sittingDevil.gameObject);
		yield return null;
	}

	private IEnumerator phase2BackgroundFade_cr(float time)
	{
		SpriteRenderer[] sprites = phase2Background.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < sprites.Length; i++)
		{
			Color color = sprites[i].color;
			color.a = 0f;
			sprites[i].color = color;
		}
		phase2Background.SetActive(true);
		float t = 0f;
		while (t < time)
		{
			for (int j = 0; j < sprites.Length; j++)
			{
				Color color2 = sprites[j].color;
				color2.a = t / time;
				sprites[j].color = color2;
			}
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		for (int k = 0; k < sprites.Length; k++)
		{
			Color color3 = sprites[k].color;
			color3.a = 1f;
			sprites[k].color = color3;
		}
	}

	private void ZoomOut(Vector3 cameraStart, float endCameraTime)
	{
		AudioManager.FadeBGMVolume(0f, 1f, true);
		Level.Current.SetBounds(932, 932, 460, 306);
		StartCoroutine(CupheadLevelCamera.Current.change_zoom_cr(0.811f, 10f));
		StartCoroutine(CupheadLevelCamera.Current.slide_camera_cr(cameraStart, endCameraTime));
		phase3Platforms.SetActive(true);
		PlayerManager.GetPlayer(PlayerId.PlayerOne).transform.SetScale(1f);
		PlayerManager.GetPlayer(PlayerId.PlayerOne).transform.position = Phase2P1spawn.position;
		PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerAnimationController>().PlayIntro();
		if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
		{
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).transform.SetScale(1f);
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).transform.position = Phase2P2spawn.position;
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerAnimationController>().PlayIntro();
		}
		StartCoroutine(disable_input_cr());
		pit.SetActive(true);
	}

	private IEnumerator disable_input_cr()
	{
		PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerMotor>().DisableInput();
		PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerWeaponManager>().DisableInput();
		if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
		{
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerMotor>().DisableInput();
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerWeaponManager>().DisableInput();
		}
		yield return PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerMotor>().animator.WaitForAnimationToEnd(this, "Intro_Scared");
		PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerMotor>().ClearBufferedInput();
		PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerMotor>().EnableInput();
		PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerWeaponManager>().EnableInput();
		if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
		{
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerMotor>().ClearBufferedInput();
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerMotor>().EnableInput();
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerWeaponManager>().EnableInput();
		}
		yield return null;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(Phase2P1spawn.position, 30f);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(Phase2P2spawn.position, 30f);
	}

	private IEnumerator devilPattern_cr()
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
		LevelProperties.Devil.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		case LevelProperties.Devil.Pattern.Clap:
			yield return StartCoroutine(clap_cr());
			break;
		case LevelProperties.Devil.Pattern.Head:
			yield return StartCoroutine(head_cr());
			break;
		case LevelProperties.Devil.Pattern.Pitchfork:
			yield return StartCoroutine(pitchfork_cr());
			break;
		case LevelProperties.Devil.Pattern.BombEye:
			yield return StartCoroutine(bombEye_cr());
			break;
		case LevelProperties.Devil.Pattern.SkullEye:
			yield return StartCoroutine(skullEye_cr());
			break;
		default:
			Debug.LogWarning("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		}
	}

	private IEnumerator clap_cr()
	{
		while (sittingDevil.state != DevilLevelSittingDevil.State.Idle)
		{
			yield return null;
		}
		sittingDevil.StartClap();
		while (sittingDevil.state != DevilLevelSittingDevil.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator head_cr()
	{
		while (sittingDevil.state != DevilLevelSittingDevil.State.Idle)
		{
			yield return null;
		}
		sittingDevil.StartHead();
		while (sittingDevil.state != DevilLevelSittingDevil.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator pitchfork_cr()
	{
		while (sittingDevil.state != DevilLevelSittingDevil.State.Idle)
		{
			yield return null;
		}
		sittingDevil.StartPitchfork();
		while (sittingDevil.state != DevilLevelSittingDevil.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator bombEye_cr()
	{
		while (giantHead.state != DevilLevelGiantHead.State.Idle)
		{
			yield return null;
		}
		giantHead.StartBombEye();
		while (giantHead.state != DevilLevelGiantHead.State.Idle)
		{
			yield return null;
		}
	}

	private IEnumerator skullEye_cr()
	{
		while (giantHead.state != DevilLevelGiantHead.State.Idle)
		{
			yield return null;
		}
		giantHead.StartSkullEye();
		while (giantHead.state != DevilLevelGiantHead.State.Idle)
		{
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Devil.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
