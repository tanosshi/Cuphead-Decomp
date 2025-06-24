using System.Collections;
using UnityEngine;

public class HouseLevel : Level
{
	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	[SerializeField]
	private PlayerDeathEffect[] playerTutorialEffects;

	[SerializeField]
	private HouseElderKettle elderDialoguePoint;

	[SerializeField]
	private GameObject tutorialGameObject;

	[SerializeField]
	private int dialoguerVariableID;

	private LevelProperties.House properties;

	public override Sprite BossPortrait
	{
		get
		{
			return _bossPortrait;
		}
	}

	public override string BossQuote
	{
		get
		{
			return _bossQuote;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.House;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_house_elder_kettle;
		}
	}

	protected override void Start()
	{
		base.Start();
		if (PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Levels.Devil }, Mode.Hard))
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 8f);
		}
		else if (PlayerData.Data.CountLevelsHaveMinDifficulty(Level.world1BossLevels, Mode.Hard) + PlayerData.Data.CountLevelsHaveMinDifficulty(Level.world2BossLevels, Mode.Hard) + PlayerData.Data.CountLevelsHaveMinDifficulty(Level.world3BossLevels, Mode.Hard) + PlayerData.Data.CountLevelsHaveMinDifficulty(Level.world4BossLevels, Mode.Hard) > 0)
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 7f);
		}
		else if (PlayerData.Data.IsHardModeAvailable)
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 6f);
		}
		else if (PlayerData.Data.CheckLevelsCompleted(Level.world2BossLevels))
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 5f);
		}
		else if (PlayerData.Data.CheckLevelsCompleted(Level.world1BossLevels))
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 4f);
		}
		else if (PlayerData.Data.CountLevelsCompleted(Level.world1BossLevels) > 1)
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 3f);
		}
		else if (PlayerData.Data.IsTutorialCompleted)
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 2f);
		}
		else if (Dialoguer.GetGlobalFloat(dialoguerVariableID) == 0f)
		{
			tutorialGameObject.SetActive(false);
		}
		SceneLoader.OnLoaderCompleteEvent += SelectMusic;
		AddDialoguerEvents();
	}

	private void SelectMusic()
	{
		if (PlayerData.Data.pianoAudioEnabled)
		{
			AudioManager.PlayBGMPlaylistManually(false);
		}
		else
		{
			AudioManager.PlayBGM();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SceneLoader.OnLoaderCompleteEvent -= SelectMusic;
		RemoveDialoguerEvents();
	}

	public void AddDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
	}

	public void RemoveDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
	}

	public void StartTutorial()
	{
		AbstractPlayerController player = PlayerManager.GetPlayer(PlayerId.PlayerOne);
		playerTutorialEffects[0].gameObject.SetActive(true);
		playerTutorialEffects[0].transform.position = player.transform.position;
		player.gameObject.SetActive(false);
		playerTutorialEffects[0].animator.SetTrigger("OnStartTutorial");
		player = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
		if (player != null)
		{
			playerTutorialEffects[1].gameObject.SetActive(true);
			playerTutorialEffects[1].transform.position = player.transform.position;
			player.gameObject.SetActive(false);
			playerTutorialEffects[1].animator.SetTrigger("OnStartTutorial");
		}
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (message == "ElderKettleFirstWeapon")
		{
			tutorialGameObject.SetActive(true);
			StartCoroutine(power_up_cr());
		}
		if (message == "EndJoy")
		{
		}
		if (!(message == "Sleep"))
		{
		}
	}

	private IEnumerator power_up_cr()
	{
		yield return new WaitForSeconds(0.15f);
		AudioManager.Play("sfx_potion_poof");
		AbstractPlayerController[] array = players;
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			if (!(abstractPlayerController == null))
			{
				abstractPlayerController.animator.Play("Power_Up");
			}
		}
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(housePattern_cr());
	}

	private IEnumerator housePattern_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		if (Dialoguer.GetGlobalFloat(dialoguerVariableID) == 0f)
		{
			elderDialoguePoint.BeginDialogue();
		}
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
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

	protected override void PartialInit()
	{
		properties = LevelProperties.House.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
