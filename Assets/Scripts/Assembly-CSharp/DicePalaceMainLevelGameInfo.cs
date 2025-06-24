using System.Collections.Generic;
using UnityEngine;

public class DicePalaceMainLevelGameInfo : AbstractMonoBehaviour
{
	public class PlayerStats
	{
		public int HP;

		public int BonusHP;

		public float Super;

		public void LoseBonusHP()
		{
			if (BonusHP > 0)
			{
				BonusHP--;
			}
		}
	}

	private static DicePalaceMainLevelGameInfo gameInfo;

	public static int TURN_COUNTER;

	public static int PLAYER_SPACES_MOVED;

	public static List<int> SAFE_INDEXES;

	public static int[] HEART_INDEXES = new int[3];

	public static PlayerStats PLAYER_ONE_STATS;

	public static PlayerStats PLAYER_TWO_STATS;

	public static bool PLAYED_INTRO_SFX;

	public static DicePalaceMainLevelGameInfo GameInfo
	{
		get
		{
			if (gameInfo == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "GameInfo";
				gameInfo = gameObject.AddComponent<DicePalaceMainLevelGameInfo>();
			}
			return gameInfo;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		gameInfo = this;
		SAFE_INDEXES = new List<int>();
		ChooseHearts();
		Object.DontDestroyOnLoad(this);
	}

	public void CleanUp()
	{
		SAFE_INDEXES.Clear();
		TURN_COUNTER = 0;
		PLAYER_SPACES_MOVED = 0;
		ChooseHearts();
		PLAYER_ONE_STATS = null;
		PLAYER_TWO_STATS = null;
		PLAYED_INTRO_SFX = false;
		Object.Destroy(base.gameObject);
	}

	public static void CleanUpRetry()
	{
		SAFE_INDEXES.Clear();
		TURN_COUNTER = 0;
		PLAYER_SPACES_MOVED = 0;
		ChooseHearts();
		PLAYER_ONE_STATS = null;
		PLAYER_TWO_STATS = null;
		PLAYED_INTRO_SFX = false;
	}

	private static void ChooseHearts()
	{
		HEART_INDEXES[0] = Random.Range(0, 3);
		HEART_INDEXES[1] = Random.Range(4, 7);
		HEART_INDEXES[2] = Random.Range(8, 11);
	}
}
