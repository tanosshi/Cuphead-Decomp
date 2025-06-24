using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MausoleumLevel : Level
{
	public static int SPAWNCOUNTER;

	[SerializeField]
	private GameObject[] WorldBackgrounds;

	[SerializeField]
	private MausoleumLevelCircleGhost circleGhost;

	[SerializeField]
	private MausoleumLevelRegularGhost regularGhost;

	[SerializeField]
	private MausoleumLevelBigGhost bigGhost;

	[SerializeField]
	private MausoleumLevelDelayGhost delayGhost;

	[SerializeField]
	private MausoleumLevelSineGhost sineGhost;

	[SerializeField]
	private Transform[] positions;

	[SerializeField]
	private MausoleumLevelUrn urn;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitEasy;

	[SerializeField]
	private Sprite _bossPortraitNormal;

	[SerializeField]
	private Sprite _bossPortraitHard;

	[SerializeField]
	private string _bossQuoteEasy;

	[SerializeField]
	private string _bossQuoteNormal;

	[SerializeField]
	private string _bossQuoteHard;

	[SerializeField]
	private Animator helpSignAnimator;

	[SerializeField]
	private Animator[] urnsAnimator;

	[SerializeField]
	private Animator[] chaliceCharacterAnimators;

	private Animator currentUrnAnimator;

	private Animator currentChaliceAnimator;

	[SerializeField]
	private MausoleumDialogueInteraction dialogue;

	private bool isLevelOver;

	private bool PowerUpSFXActive;

	private int maxCounter;

	private bool isActive = true;

	public Action WinGame;

	public Action LoseGame;

	private Mode originalMode;

	private LevelProperties.Mausoleum properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (base.mode)
			{
			case Mode.Easy:
				return _bossPortraitEasy;
			case Mode.Normal:
				return _bossPortraitNormal;
			case Mode.Hard:
				return _bossPortraitHard;
			default:
				Debug.LogError(string.Concat("Couldn't find portrait for state ", base.mode, ". Using Main."));
				return _bossPortraitEasy;
			}
		}
	}

	public override string BossQuote
	{
		get
		{
			switch (base.mode)
			{
			case Mode.Easy:
				return _bossQuoteEasy;
			case Mode.Normal:
				return _bossQuoteNormal;
			case Mode.Hard:
				return _bossQuoteHard;
			default:
				Debug.LogError(string.Concat("Couldn't find quote for state ", base.mode, ". Using Main."));
				return _bossQuoteEasy;
			}
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.Mausoleum;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_mausoleum;
		}
	}

	protected override void Awake()
	{
		Level.OverrideDifficulty = true;
		LoseGame = (Action)Delegate.Combine(LoseGame, new Action(Failure));
		switch (PlayerData.Data.CurrentMap)
		{
		case Scenes.scene_map_world_1:
			base.mode = Mode.Easy;
			break;
		case Scenes.scene_map_world_2:
			base.mode = Mode.Normal;
			break;
		case Scenes.scene_map_world_3:
			base.mode = Mode.Hard;
			break;
		}
		originalMode = Level.CurrentMode;
		Level.SetCurrentMode(base.mode);
		GameObject[] worldBackgrounds = WorldBackgrounds;
		foreach (GameObject gameObject in worldBackgrounds)
		{
			gameObject.SetActive(false);
		}
		WorldBackgrounds[(int)base.mode].SetActive(true);
		currentUrnAnimator = urnsAnimator[(int)base.mode];
		currentChaliceAnimator = chaliceCharacterAnimators[(int)base.mode];
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		isMausoleum = true;
		Dialoguer.events.onMessageEvent += OnMessageEvent;
		dialogue.chaliceAnimator = currentChaliceAnimator;
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(main_pattern_cr());
		StartCoroutine(helpsignDisappear_cr());
		StartCoroutine(urnrandomanimation_cr());
	}

	protected override void OnStateChanged()
	{
		Debug.Log("WAVE CHANGED");
		base.OnStateChanged();
	}

	public void OnMessageEvent(string message, string metaData)
	{
		if (!(message == "PowerUpGiven"))
		{
			return;
		}
		AbstractPlayerController[] array = players;
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			if (!(abstractPlayerController == null))
			{
				if (!PowerUpSFXActive)
				{
					AudioManager.Play("player_power_up");
					emitAudioFromObject.Add("player_power_up");
					PowerUpSFXActive = true;
				}
				abstractPlayerController.animator.Play("Power_Up");
			}
		}
	}

	private IEnumerator mausoleumPattern_cr()
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

	private IEnumerator helpsignDisappear_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		helpSignAnimator.SetTrigger("Outro");
	}

	private IEnumerator urnrandomanimation_cr()
	{
		while (!isLevelOver)
		{
			yield return CupheadTime.WaitForSeconds(this, 2f);
			AudioManager.Play("mausoleum_ghost_jar_shake");
			emitAudioFromObject.Add("mausoleum_ghost_jar_shake");
			switch (UnityEngine.Random.Range(1, 4))
			{
			case 1:
				currentUrnAnimator.SetTrigger("Shake");
				break;
			case 2:
				currentUrnAnimator.SetTrigger("SmallVibrate");
				break;
			case 3:
				currentUrnAnimator.SetTrigger("BigVibrate");
				break;
			}
		}
	}

	private IEnumerator legendarychaliceappear_cr()
	{
		AudioManager.StopBGM();
		AudioManager.PlayBGMPlaylistManually(false);
		currentUrnAnimator.SetTrigger("Sparkle");
		yield return CupheadTime.WaitForSeconds(this, 2.5f);
		currentUrnAnimator.SetTrigger("Pop");
		AudioManager.Play("mausoleum_lid_pop");
		yield return CupheadTime.WaitForSeconds(this, 2f);
		float t = 0f;
		float TIME = 1.5f;
		float arcHeight = 150f;
		float arcHeightAdd = 0f;
		Vector3 startPosition = currentChaliceAnimator.gameObject.transform.localPosition;
		Vector3 endPosition = currentChaliceAnimator.gameObject.transform.localPosition + new Vector3(400f, 0f, 0f);
		AudioManager.Play("mausoleum_ghost_jar_travel");
		emitAudioFromObject.Add("mausoleum_ghost_jar_travel");
		while (t < TIME)
		{
			float val = t / TIME;
			Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, val));
			arcHeightAdd = ((!(t < TIME / 2f)) ? Mathf.Lerp(arcHeight, 0f, EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, val * 2f - 1f)) : Mathf.Lerp(arcHeight, 0f, EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, 1f - val * 2f)));
			newPosition.y += arcHeightAdd;
			currentChaliceAnimator.gameObject.transform.localPosition = newPosition;
			t += Time.deltaTime;
			yield return null;
		}
		AudioManager.Play("mausoleum_ghost_jar_queen_ghost_appear");
		emitAudioFromObject.Add("mausoleum_ghost_jar_queen_ghost_appear");
		currentChaliceAnimator.SetTrigger("Transition");
		dialogue.BeginDialogue();
		yield return CupheadTime.WaitForSeconds(this, 1f);
		float currentDialogueFloat = Dialoguer.GetGlobalFloat(14);
		Dialoguer.SetGlobalFloat(14, currentDialogueFloat + 1f);
		PlayerData.SaveCurrentFile();
	}

	private void SetupTimeline()
	{
		base.timeline = new Timeline();
		base.timeline.health = 0f;
		List<float> list = new List<float>();
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			base.timeline.health += properties.CurrentState.main.ghostCount;
			list.Add(properties.CurrentState.main.ghostCount);
		}
		for (int j = 0; j < num - 1; j++)
		{
			base.timeline.AddEventAtHealth(j.ToString(), base.timeline.GetHealthOfLastEvent() + (int)list[j]);
		}
	}

	private IEnumerator main_pattern_cr()
	{
		SetupTimeline();
		yield return null;
		while (true)
		{
			LevelProperties.Mausoleum.Main p = properties.CurrentState.main;
			int delayMainIndex = UnityEngine.Random.Range(0, p.delayString.Length);
			string[] delayString = p.delayString[delayMainIndex].Split(',');
			int delayIndex = UnityEngine.Random.Range(0, delayString.Length);
			int spawnMainIndex = UnityEngine.Random.Range(0, p.spawnString.Length);
			string[] spawnString = p.spawnString[spawnMainIndex].Split(',');
			int spawnIndex = UnityEngine.Random.Range(0, spawnString.Length);
			int ghostTypeIndex = UnityEngine.Random.Range(0, p.ghostTypeString.Length);
			string[] ghostString = p.ghostTypeString[ghostTypeIndex].Split(',');
			int ghostIndex = UnityEngine.Random.Range(0, ghostString.Length);
			float delay = 0f;
			int spawnPos = 0;
			maxCounter = p.ghostCount;
			SPAWNCOUNTER = 0;
			while (SPAWNCOUNTER < maxCounter)
			{
				delayString = p.delayString[delayMainIndex].Split(',');
				ghostString = p.ghostTypeString[ghostTypeIndex].Split(',');
				string[] ghostSplit = ghostString[ghostIndex].Split('-');
				float extraDelay = 0f;
				int splitcount = 0;
				string[] array = ghostSplit;
				foreach (string split in array)
				{
					spawnString = p.spawnString[spawnMainIndex].Split(',');
					spawnPos = ((int.Parse(spawnString[spawnIndex]) < 6) ? (int.Parse(spawnString[spawnIndex]) - 1) : (int.Parse(spawnString[spawnIndex]) - 2));
					Vector3 direction = urn.transform.position - positions[spawnPos].transform.position;
					float angle = MathUtils.DirectionToAngle(direction);
					int repeatCount = 0;
					int.TryParse(ghostString[ghostIndex].Substring(1), out repeatCount);
					if (int.Parse(spawnString[spawnIndex]) == 2 || int.Parse(spawnString[spawnIndex]) == 8)
					{
						MausoleumLevelDelayGhost mausoleumLevelDelayGhost = delayGhost.Create(positions[spawnPos].transform.position, angle, 0f, properties.CurrentState.delayGhost);
						mausoleumLevelDelayGhost.GetParent(this);
					}
					else
					{
						switch (ghostString[ghostIndex][0])
						{
						case 'R':
							if (repeatCount != 0)
							{
								for (int k = 0; k < repeatCount; k++)
								{
									MausoleumLevelRegularGhost g = regularGhost.Create(positions[spawnPos].transform.position, angle, properties.CurrentState.regularGhost.speed) as MausoleumLevelRegularGhost;
									g.Counts = splitcount == 0;
									g.GetParent(this);
									yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.regularGhost.multiDelay);
								}
								extraDelay += properties.CurrentState.regularGhost.mainAddDelay;
							}
							else
							{
								MausoleumLevelRegularGhost mausoleumLevelRegularGhost = regularGhost.Create(positions[spawnPos].transform.position, angle, properties.CurrentState.regularGhost.speed) as MausoleumLevelRegularGhost;
								mausoleumLevelRegularGhost.Counts = splitcount == 0;
								mausoleumLevelRegularGhost.GetParent(this);
								extraDelay += properties.CurrentState.regularGhost.mainAddDelay;
							}
							break;
						case 'D':
						{
							MausoleumLevelDelayGhost d = delayGhost.Create(positions[spawnPos].transform.position, angle, 0f, properties.CurrentState.delayGhost);
							d.Counts = splitcount == 0;
							d.GetParent(this);
							break;
						}
						case 'B':
							if (repeatCount != 0)
							{
								for (int l = 0; l < repeatCount; l++)
								{
									MausoleumLevelBigGhost b = bigGhost.Create(positions[spawnPos].transform.position, angle, properties.CurrentState.bigGhost.speed, properties.CurrentState.bigGhost, urn.gameObject);
									b.Counts = splitcount == 0;
									b.GetParent(this);
									yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.bigGhost.multiDelay);
								}
								extraDelay += properties.CurrentState.bigGhost.mainAddDelay;
							}
							else
							{
								MausoleumLevelBigGhost mausoleumLevelBigGhost = bigGhost.Create(positions[spawnPos].transform.position, angle, properties.CurrentState.bigGhost.speed, properties.CurrentState.bigGhost, urn.gameObject);
								mausoleumLevelBigGhost.Counts = splitcount == 0;
								mausoleumLevelBigGhost.GetParent(this);
								extraDelay += properties.CurrentState.bigGhost.mainAddDelay;
							}
							break;
						case 'C':
						{
							MausoleumLevelCircleGhost c = circleGhost.Create(positions[spawnPos].transform.position, urn.transform.position, angle, properties.CurrentState.circleGhost.circleSpeed, properties.CurrentState.circleGhost.circleRate) as MausoleumLevelCircleGhost;
							c.Counts = splitcount == 0;
							c.GetParent(this);
							break;
						}
						case 'S':
							if (repeatCount != 0)
							{
								for (int j = 0; j < repeatCount; j++)
								{
									MausoleumLevelSineGhost s = sineGhost.Create(positions[spawnPos].transform.position, angle, properties.CurrentState.sineGhost.ghostSpeed, properties.CurrentState.sineGhost);
									s.Counts = splitcount == 0;
									s.GetParent(this);
									yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.sineGhost.multiDelay);
								}
								extraDelay = properties.CurrentState.sineGhost.mainAddDelay;
							}
							else
							{
								MausoleumLevelSineGhost mausoleumLevelSineGhost = sineGhost.Create(positions[spawnPos].transform.position, angle, properties.CurrentState.sineGhost.ghostSpeed, properties.CurrentState.sineGhost);
								mausoleumLevelSineGhost.Counts = splitcount == 0;
								mausoleumLevelSineGhost.GetParent(this);
								extraDelay = properties.CurrentState.sineGhost.mainAddDelay;
							}
							break;
						default:
							Debug.Log("ERROR: INVALID TYPE STRING");
							break;
						}
					}
					splitcount++;
					if (spawnIndex < spawnString.Length - 1)
					{
						spawnIndex++;
						continue;
					}
					spawnMainIndex = (spawnMainIndex + 1) % p.spawnString.Length;
					spawnIndex = 0;
				}
				base.timeline.DealDamage(1f);
				yield return null;
				delay = float.Parse(delayString[delayIndex]) + extraDelay;
				yield return CupheadTime.WaitForSeconds(this, delay);
				extraDelay = 0f;
				if (delayIndex < delayString.Length - 1)
				{
					delayIndex++;
				}
				else
				{
					delayMainIndex = (delayMainIndex + 1) % p.delayString.Length;
					delayIndex = 0;
				}
				if (ghostIndex < ghostString.Length - 1)
				{
					ghostIndex++;
				}
				else
				{
					ghostTypeIndex = (ghostTypeIndex + 1) % p.ghostTypeString.Length;
					ghostIndex = 0;
				}
				yield return null;
			}
			MausoleumLevelGhostBase[] ghosts = UnityEngine.Object.FindObjectsOfType(typeof(MausoleumLevelGhostBase)) as MausoleumLevelGhostBase[];
			bool ghostsAlive = true;
			int ghostCounter = 0;
			while (ghostsAlive)
			{
				ghosts = UnityEngine.Object.FindObjectsOfType(typeof(MausoleumLevelGhostBase)) as MausoleumLevelGhostBase[];
				for (int m = 0; m < ghosts.Length; m++)
				{
					if (ghosts[m].isDead)
					{
						ghostCounter++;
						if (ghostCounter >= ghosts.Length)
						{
							ghostsAlive = false;
							break;
						}
					}
				}
				ghostCounter = 0;
				if (ghostsAlive)
				{
					yield return CupheadTime.WaitForSeconds(this, 0.25f);
					yield return null;
					continue;
				}
				break;
			}
			properties.DealDamageToNextNamedState();
			yield return null;
		}
	}

	private void Failure()
	{
		_OnLose();
		AudioManager.Play("mausoleum_ghost_jar_burst");
		emitAudioFromObject.Add("mausoleum_ghost_jar_burst");
		PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerAnimationController>().ScaredSprite(FacingLeft(PlayerManager.GetPlayer(PlayerId.PlayerOne)));
		if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
		{
			PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerAnimationController>().ScaredSprite(PlayerManager.GetPlayer(PlayerId.PlayerTwo));
		}
		base.timeline.OnPlayerDeath(PlayerId.PlayerOne);
		if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
		{
			base.timeline.OnPlayerDeath(PlayerId.PlayerTwo);
		}
	}

	private bool FacingLeft(AbstractPlayerController player)
	{
		if (player.transform.position.x > urn.transform.position.x)
		{
			if (player.transform.localScale.x == 1f)
			{
				return true;
			}
			return false;
		}
		if (player.transform.localScale.x == -1f)
		{
			return true;
		}
		return false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Level.SetCurrentMode(originalMode);
		StopCoroutine(urnrandomanimation_cr());
		Dialoguer.events.onMessageEvent -= OnMessageEvent;
	}

	protected override void OnPreWin()
	{
		isLevelOver = true;
		StopCoroutine(urnrandomanimation_cr());
		StartCoroutine(legendarychaliceappear_cr());
	}

	protected override void OnWin()
	{
		base.OnWin();
		Super value = Super.level_super_beam;
		switch (PlayerData.Data.CurrentMap)
		{
		case Scenes.scene_map_world_1:
			value = Super.level_super_beam;
			break;
		case Scenes.scene_map_world_2:
			value = Super.level_super_invincible;
			break;
		case Scenes.scene_map_world_3:
			value = Super.level_super_ghost;
			break;
		}
		if (!PlayerData.Data.IsUnlocked(PlayerId.PlayerOne, value))
		{
			PlayerData.Data.Buy(PlayerId.PlayerOne, value);
			PlayerData.Data.Buy(PlayerId.PlayerTwo, value);
			Level.SuperUnlocked = true;
		}
		if (PlayerData.Data.NumSupers(PlayerId.PlayerOne) >= 3)
		{
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "UnlockedAllSupers");
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Mausoleum.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
