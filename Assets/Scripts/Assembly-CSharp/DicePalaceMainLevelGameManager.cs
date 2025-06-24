using System.Collections;
using UnityEngine;

public class DicePalaceMainLevelGameManager : LevelProperties.DicePalaceMain.Entity
{
	public enum BoardSpaces
	{
		Booze = 0,
		Chips = 1,
		Cigar = 2,
		Domino = 3,
		EightBall = 4,
		FlyingHorse = 5,
		FlyingMemory = 6,
		Pachinko = 7,
		Rabbit = 8,
		Roulette = 9,
		FreeSpace = 10,
		StartOver = 11
	}

	private const float MarkerMovementTime = 0.3f;

	private const float MarkerFastMove = 1f / 12f;

	[SerializeField]
	private BoardSpaces[] allBoardSpaces;

	[SerializeField]
	private DicePalaceMainLevelBoardSpace[] boardSpacesObj;

	[SerializeField]
	private DicePalaceMainLevelBoardSpace startSpaceObj;

	[SerializeField]
	private DicePalaceMainLevelBoardSpace endSpaceObj;

	[SerializeField]
	private DicePalaceMainLevelKingDice kingDice;

	[SerializeField]
	private DicePalaceMainLevelDice dicePrefab;

	[SerializeField]
	private Transform pivotPoint1;

	[SerializeField]
	private Transform marker;

	[SerializeField]
	private Animator markerAnimator;

	[SerializeField]
	private float endSpaceFlashRate;

	[SerializeField]
	private GameObject heart;

	private Animator kingDiceAni;

	private DicePalaceMainLevelDice dice;

	private DicePalaceMainLevelGameInfo gameInfo;

	private int previousSpace;

	private int maxSpaces;

	protected override void Start()
	{
		base.Start();
	}

	public override void LevelInit(LevelProperties.DicePalaceMain properties)
	{
		base.LevelInit(properties);
		Level.Current.OnIntroEvent += StartDice;
		gameInfo = DicePalaceMainLevelGameInfo.GameInfo;
		kingDiceAni = kingDice.GetComponent<Animator>();
		maxSpaces = allBoardSpaces.Length;
		GameSetup();
		marker.position = boardSpacesObj[DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED].Pivot.position;
		marker.rotation = boardSpacesObj[DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED].Pivot.rotation;
		if (!DicePalaceMainLevelGameInfo.PLAYED_INTRO_SFX)
		{
			AudioManager.Play("vox_intro");
			emitAudioFromObject.Add("vox_intro");
			DicePalaceMainLevelGameInfo.PLAYED_INTRO_SFX = true;
		}
	}

	public void GameSetup()
	{
		LevelProperties.DicePalaceMain.Dice dice = base.properties.CurrentState.dice;
		this.dice = Object.Instantiate(dicePrefab);
		this.dice.Init(Vector2.zero, dice, pivotPoint1);
		pivotPoint1.position = this.dice.transform.position;
		CheckSafeSpaces();
		CheckHearts();
	}

	public void CheckSafeSpaces()
	{
		for (int i = 0; i < DicePalaceMainLevelGameInfo.SAFE_INDEXES.Count; i++)
		{
			allBoardSpaces[DicePalaceMainLevelGameInfo.SAFE_INDEXES[i]] = BoardSpaces.FreeSpace;
			boardSpacesObj[DicePalaceMainLevelGameInfo.SAFE_INDEXES[i] + 1].Clear = true;
		}
	}

	private void CheckHearts()
	{
		for (int i = 0; i < DicePalaceMainLevelGameInfo.HEART_INDEXES.Length; i++)
		{
			boardSpacesObj[DicePalaceMainLevelGameInfo.HEART_INDEXES[i] + 1].HasHeart = true;
		}
	}

	private void StartDice()
	{
		StartCoroutine(check_for_rolled_cr());
	}

	public void RevealDice()
	{
		dice.StartRoll();
	}

	private IEnumerator check_for_rolled_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.dice.revealDelay);
		kingDiceAni.SetTrigger("OnReveal");
		yield return kingDiceAni.WaitForAnimationToEnd(kingDice, "Dice_Reveal");
		LevelProperties.DicePalaceMain.Dice p = base.properties.CurrentState.dice;
		int spacesToMove = 0;
		bool playingGame = true;
		while (playingGame)
		{
			while (dice.waitingToRoll)
			{
				yield return null;
			}
			spacesToMove = 0;
			switch (dice.roll)
			{
			case DicePalaceMainLevelDice.Roll.One:
				spacesToMove = 1;
				break;
			case DicePalaceMainLevelDice.Roll.Two:
				spacesToMove = 2;
				break;
			case DicePalaceMainLevelDice.Roll.Three:
				spacesToMove = 3;
				break;
			}
			int spaces = ((DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED + spacesToMove > 1) ? (DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED + spacesToMove - 1) : 0);
			if (spaces < maxSpaces && allBoardSpaces[spaces] != BoardSpaces.FreeSpace && allBoardSpaces[spaces] != BoardSpaces.StartOver && spaces + 1 < boardSpacesObj.Length && !boardSpacesObj[spaces + 1].HasHeart)
			{
				AudioManager.Stop("vox_curious");
				AudioManager.Play("vox_laugh");
				emitAudioFromObject.Add("vox_laugh");
			}
			yield return StartCoroutine(MoveMarker(spacesToMove, false));
			DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED += spacesToMove;
			if (DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED > maxSpaces)
			{
				DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED = maxSpaces;
				playingGame = false;
				kingDiceAni.SetBool("IsSafe", true);
				break;
			}
			BoardSpaces space = allBoardSpaces[spaces];
			Debug.Log(space);
			kingDiceAni.SetTrigger("OnEager");
			if (playingGame)
			{
				if (space == BoardSpaces.FreeSpace || DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED == previousSpace)
				{
					AudioManager.Play("vox_curious");
					emitAudioFromObject.Add("vox_curious");
					kingDiceAni.SetBool("IsSafe", true);
					Debug.Log("On Free Space");
				}
				else if (space == BoardSpaces.StartOver)
				{
					AudioManager.Play("vox_startover");
					emitAudioFromObject.Add("vox_startover");
					AudioManager.Stop("vox_curious");
					Debug.Log("BACK TO THE BEGINNING");
					DicePalaceMainLevelGameInfo.SAFE_INDEXES.Add(spaces);
					boardSpacesObj[spaces + 1].Clear = true;
					CheckSafeSpaces();
					yield return StartCoroutine(MoveMarker(-maxSpaces, true));
					kingDiceAni.SetBool("IsSafe", true);
					DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED = 0;
					yield return CupheadTime.WaitForSeconds(this, p.pauseWhenRolled);
				}
				else
				{
					Debug.Log("Player is on space #" + DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED);
					previousSpace = DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED;
					kingDiceAni.SetBool("IsSafe", false);
					DicePalaceMainLevelGameInfo.SAFE_INDEXES.Add(spaces);
					for (int i = 0; i < DicePalaceMainLevelGameInfo.HEART_INDEXES.Length; i++)
					{
						if (DicePalaceMainLevelGameInfo.HEART_INDEXES[i] != spaces)
						{
							continue;
						}
						if (DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS == null)
						{
							DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS = new DicePalaceMainLevelGameInfo.PlayerStats();
						}
						PlayerStatsManager playerStats = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats;
						if (playerStats.Health > 0)
						{
							DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.BonusHP++;
							playerStats.SetHealth(playerStats.Health + 1);
						}
						if (PlayerManager.Multiplayer)
						{
							if (DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS == null)
							{
								DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS = new DicePalaceMainLevelGameInfo.PlayerStats();
							}
							PlayerStatsManager stats = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats;
							if (stats.Health > 0)
							{
								DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.BonusHP++;
								stats.SetHealth(stats.Health + 1);
							}
						}
						boardSpacesObj[DicePalaceMainLevelGameInfo.HEART_INDEXES[i] + 1].HasHeart = false;
						heart.transform.position = boardSpacesObj[DicePalaceMainLevelGameInfo.HEART_INDEXES[i] + 1].HeartSpacePosition;
						heart.SetActive(true);
						AudioManager.Play("pop_up");
						emitAudioFromObject.Add("pop_up");
						yield return CupheadTime.WaitForSeconds(this, 1.5f);
						heart.SetActive(false);
						DicePalaceMainLevelGameInfo.HEART_INDEXES[i] = -1;
						break;
					}
					yield return StartCoroutine(start_mini_boss_cr(SelectLevel(space)));
				}
				dice.waitingToRoll = true;
				yield return null;
			}
			yield return null;
		}
		EndBoardGame(dice);
	}

	private IEnumerator MoveMarker(int spacesToMove, bool resetBoard)
	{
		int side = 1;
		if (spacesToMove < 0)
		{
			side = -1;
			spacesToMove *= -1;
		}
		for (int i = 0; i < spacesToMove; i++)
		{
			int index = DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED + (1 + i) * side;
			if (index < 0 || index >= boardSpacesObj.Length)
			{
				break;
			}
			float t = 0f;
			Vector3 startPos = marker.position;
			Vector3 endPos = boardSpacesObj[index].Pivot.position;
			Quaternion startRot = marker.rotation;
			Quaternion endRot = boardSpacesObj[index].Pivot.rotation;
			if (!resetBoard)
			{
				markerAnimator.SetTrigger("Move");
				yield return markerAnimator.WaitForAnimationToStart(this, "Move");
			}
			float movement = ((!resetBoard) ? 0.3f : (1f / 12f));
			while (t < movement)
			{
				marker.position = Vector3.Lerp(startPos, endPos, t / movement);
				marker.rotation = Quaternion.Lerp(startRot, endRot, t / movement);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			marker.position = endPos;
			marker.rotation = endRot;
			AudioManager.Play("counter_move");
			emitAudioFromObject.Add("counter_move");
			if (!resetBoard)
			{
				markerAnimator.SetTrigger("Marker");
			}
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
		}
	}

	private Levels SelectLevel(BoardSpaces space)
	{
		Levels result = Levels.DicePalaceMain;
		switch (space)
		{
		case BoardSpaces.Booze:
			result = Levels.DicePalaceBooze;
			break;
		case BoardSpaces.Chips:
			result = Levels.DicePalaceChips;
			break;
		case BoardSpaces.Cigar:
			result = Levels.DicePalaceCigar;
			break;
		case BoardSpaces.Domino:
			result = Levels.DicePalaceDomino;
			break;
		case BoardSpaces.EightBall:
			result = Levels.DicePalaceEightBall;
			break;
		case BoardSpaces.FlyingHorse:
			result = Levels.DicePalaceFlyingHorse;
			break;
		case BoardSpaces.FlyingMemory:
			result = Levels.DicePalaceFlyingMemory;
			break;
		case BoardSpaces.Pachinko:
			result = Levels.DicePalacePachinko;
			break;
		case BoardSpaces.Rabbit:
			result = Levels.DicePalaceRabbit;
			break;
		case BoardSpaces.Roulette:
			result = Levels.DicePalaceRoulette;
			break;
		default:
			Debug.Log("SOMETHING WENT WRONG");
			break;
		}
		Debug.Log("Battle time!");
		return result;
	}

	private IEnumerator start_mini_boss_cr(Levels level)
	{
		kingDiceAni.SetTrigger("OnEat");
		if (DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS == null)
		{
			DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS = new DicePalaceMainLevelGameInfo.PlayerStats();
		}
		PlayerStatsManager playerOne = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats;
		DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.HP = playerOne.Health;
		DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.Super = playerOne.SuperMeter;
		if (PlayerManager.Multiplayer)
		{
			if (DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS == null)
			{
				DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS = new DicePalaceMainLevelGameInfo.PlayerStats();
			}
			PlayerStatsManager stats = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats;
			DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.HP = stats.Health;
			DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.Super = stats.SuperMeter;
		}
		yield return kingDiceAni.WaitForAnimationToStart(this, "Eat_Screen");
		AudioManager.Play("king_dice_eat_screen");
		kingDice.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Player.ToString();
		kingDice.GetComponent<SpriteRenderer>().sortingOrder = 2000;
		Level.ScoringData.time += Level.Current.LevelTime;
		yield return kingDiceAni.WaitForAnimationToEnd(this, "Eat_Screen");
		SceneLoader.LoadLevel(level, SceneLoader.Transition.Fade);
		yield return null;
	}

	private void EndBoardGame(DicePalaceMainLevelDice dice1)
	{
		StopAllCoroutines();
		StartCoroutine(flashEnd_cr());
		StartCoroutine(announcerSfx_cr());
		kingDice.StartKingDiceBattle();
		Object.Destroy(dice1.gameObject);
		DicePalaceMainLevelGameInfo.CleanUpRetry();
	}

	private IEnumerator flashEnd_cr()
	{
		DicePalaceMainLevelBoardSpace endSpace = boardSpacesObj[boardSpacesObj.Length - 1];
		while (true)
		{
			endSpace.Clear = true;
			yield return CupheadTime.WaitForSeconds(this, endSpaceFlashRate);
			endSpace.Clear = false;
			yield return CupheadTime.WaitForSeconds(this, endSpaceFlashRate);
		}
	}

	private IEnumerator announcerSfx_cr()
	{
		AudioManager.Play(Sfx.Level_Announcer_Ready);
		AudioManager.Play("level_bell_intro");
		yield return CupheadTime.WaitForSeconds(this, 2f);
		AudioManager.Play(Sfx.Level_Announcer_Begin);
	}
}
