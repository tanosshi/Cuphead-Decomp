using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
	public delegate void PlayerDataInitHandler(bool success);

	[Serializable]
	public class PlayerLoadouts
	{
		[Serializable]
		public class PlayerLoadout
		{
			public Weapon primaryWeapon;

			public Weapon secondaryWeapon;

			public Super super;

			public Charm charm;

			public bool HasEquippedSecondaryRegularWeapon { get; set; }

			public bool HasEquippedSecondarySHMUPWeapon { get; set; }

			public bool MustNotifySwitchRegularWeapon { get; set; }

			public bool MustNotifySwitchSHMUPWeapon { get; set; }

			public PlayerLoadout()
			{
				primaryWeapon = Weapon.level_weapon_peashot;
				secondaryWeapon = Weapon.None;
				super = Super.None;
				charm = Charm.None;
			}
		}

		public PlayerLoadout playerOne;

		public PlayerLoadout playerTwo;

		public PlayerLoadouts()
		{
			playerOne = new PlayerLoadout();
			playerTwo = new PlayerLoadout();
		}

		public PlayerLoadouts(PlayerLoadout playerOne, PlayerLoadout playerTwo)
		{
			this.playerOne = playerOne;
			this.playerTwo = playerTwo;
		}

		public PlayerLoadout GetPlayerLoadout(PlayerId player)
		{
			switch (player)
			{
			case PlayerId.PlayerOne:
				return playerOne;
			case PlayerId.PlayerTwo:
				return playerTwo;
			default:
				Debug.LogWarning(string.Concat(player, " loadout not configured!"));
				return null;
			}
		}
	}

	[Serializable]
	public class PlayerInventories
	{
		public int dummy;

		public PlayerInventory playerOne = new PlayerInventory();

		public PlayerInventory playerTwo = new PlayerInventory();

		public PlayerInventory GetPlayer(PlayerId player)
		{
			switch (player)
			{
			case PlayerId.PlayerOne:
				return playerOne;
			case PlayerId.PlayerTwo:
				return playerTwo;
			default:
				Debug.LogWarning(string.Concat(player, " unlocks not configured!"));
				return null;
			}
		}
	}

	[Serializable]
	public class PlayerInventory
	{
		public const int STARTING_MONEY = 0;

		public int money;

		public bool newPurchase;

		public List<Weapon> _weapons;

		public List<Super> _supers;

		public List<Charm> _charms;

		public PlayerInventory()
		{
			money = 0;
			_weapons = new List<Weapon>();
			_supers = new List<Super>();
			_charms = new List<Charm>();
			_weapons.Add(Weapon.level_weapon_peashot);
			_weapons.Add(Weapon.plane_weapon_peashot);
		}

		public bool IsUnlocked(Weapon weapon)
		{
			return _weapons.Contains(weapon);
		}

		public bool IsUnlocked(Super super)
		{
			return _supers.Contains(super);
		}

		public bool IsUnlocked(Charm charm)
		{
			return _charms.Contains(charm);
		}

		public bool Buy(Weapon value)
		{
			if (IsUnlocked(value))
			{
				return false;
			}
			if (money < WeaponProperties.GetValue(value))
			{
				return false;
			}
			money -= WeaponProperties.GetValue(value);
			_weapons.Add(value);
			newPurchase = true;
			return true;
		}

		public bool Buy(Super value)
		{
			if (IsUnlocked(value))
			{
				return false;
			}
			if (money < WeaponProperties.GetValue(value))
			{
				return false;
			}
			money -= WeaponProperties.GetValue(value);
			_supers.Add(value);
			newPurchase = true;
			return true;
		}

		public bool Buy(Charm value)
		{
			Debug.Log("check charm");
			if (IsUnlocked(value))
			{
				return false;
			}
			Debug.Log("not already purchased");
			if (money < WeaponProperties.GetValue(value))
			{
				return false;
			}
			Debug.Log("have enough money!");
			money -= WeaponProperties.GetValue(value);
			_charms.Add(value);
			newPurchase = true;
			return true;
		}
	}

	[Serializable]
	public class PlayerCoinManager
	{
		[Serializable]
		public class LevelAndCoins
		{
			public Levels level;

			public bool Coin1Collected;

			public bool Coin2Collected;

			public bool Coin3Collected;

			public bool Coin4Collected;

			public bool Coin5Collected;
		}

		public int dummy;

		public List<PlayerCoinProperties> coins = new List<PlayerCoinProperties>();

		public List<LevelAndCoins> LevelsAndCoins = new List<LevelAndCoins>();

		public PlayerCoinManager()
		{
			LevelsAndCoins = new List<LevelAndCoins>();
			foreach (Levels value in Enum.GetValues(typeof(Levels)))
			{
				LevelAndCoins item = new LevelAndCoins
				{
					level = value
				};
				LevelsAndCoins.Add(item);
			}
		}

		public bool GetCoinCollected(LevelCoin coin)
		{
			return GetCoinCollected(coin.GlobalID);
		}

		public bool GetCoinCollected(string coinID)
		{
			if (ContainsCoin(coinID))
			{
				return GetCoin(coinID).collected;
			}
			return false;
		}

		public int NumCoinsCollected()
		{
			int num = 0;
			foreach (PlayerCoinProperties coin in coins)
			{
				if (coin.collected)
				{
					num++;
				}
			}
			return num;
		}

		public void SetCoinValue(LevelCoin coin, bool collected, PlayerId player)
		{
			SetCoinValue(coin.GlobalID, collected, player);
		}

		public void SetCoinValue(string coinID, bool collected, PlayerId player)
		{
			if (ContainsCoin(coinID))
			{
				PlayerCoinProperties coin = GetCoin(coinID);
				coin.collected = collected;
				coin.player = player;
			}
			else
			{
				PlayerCoinProperties playerCoinProperties = new PlayerCoinProperties(coinID);
				playerCoinProperties.collected = collected;
				AddCoin(playerCoinProperties);
			}
		}

		private PlayerCoinProperties GetCoin(LevelCoin coin)
		{
			return GetCoin(coin.GlobalID);
		}

		private PlayerCoinProperties GetCoin(string coinID)
		{
			for (int i = 0; i < coins.Count; i++)
			{
				if (coins[i].coinID == coinID)
				{
					return coins[i];
				}
			}
			return null;
		}

		private void AddCoin(LevelCoin coin)
		{
			AddCoin(coin.GlobalID);
		}

		private void AddCoin(string coinID)
		{
			if (!ContainsCoin(coinID))
			{
				coins.Add(new PlayerCoinProperties(coinID));
			}
			RegisterCoin(coinID);
		}

		private void AddCoin(PlayerCoinProperties coin)
		{
			if (!ContainsCoin(coin.coinID))
			{
				coins.Add(coin);
			}
			RegisterCoin(coin.coinID);
		}

		private void RegisterCoin(string coinID)
		{
			PlatformingLevel platformingLevel = Level.Current as PlatformingLevel;
			if ((bool)platformingLevel)
			{
				List<LevelAndCoins> levelsAndCoins = LevelsAndCoins;
				int num = -1;
				for (int i = 0; i < levelsAndCoins.Count; i++)
				{
					if (levelsAndCoins[i].level == platformingLevel.CurrentLevel)
					{
						num = i;
					}
				}
				if (num >= 0)
				{
					for (int j = 0; j < platformingLevel.LevelCoinsIDs.Count; j++)
					{
						if (platformingLevel.LevelCoinsIDs[j].CoinID == coinID)
						{
							switch (j)
							{
							case 0:
								levelsAndCoins[num].Coin1Collected = true;
								break;
							case 1:
								levelsAndCoins[num].Coin2Collected = true;
								break;
							case 2:
								levelsAndCoins[num].Coin3Collected = true;
								break;
							case 3:
								levelsAndCoins[num].Coin4Collected = true;
								break;
							case 4:
								levelsAndCoins[num].Coin5Collected = true;
								break;
							}
							break;
						}
					}
				}
			}
			else if (Map.Current != null)
			{
				List<LevelAndCoins> levelsAndCoins2 = Data.coinManager.LevelsAndCoins;
				int num2 = -1;
				for (int k = 0; k < levelsAndCoins2.Count; k++)
				{
					if (levelsAndCoins2[k].level == Map.Current.level)
					{
						num2 = k;
					}
				}
				if (num2 >= 0)
				{
					for (int l = 0; l < Map.Current.LevelCoinsIDs.Count; l++)
					{
						if (Map.Current.LevelCoinsIDs[l].CoinID == coinID)
						{
							switch (l)
							{
							case 0:
								levelsAndCoins2[num2].Coin1Collected = true;
								break;
							case 1:
								levelsAndCoins2[num2].Coin2Collected = true;
								break;
							case 2:
								levelsAndCoins2[num2].Coin3Collected = true;
								break;
							case 3:
								levelsAndCoins2[num2].Coin4Collected = true;
								break;
							case 4:
								levelsAndCoins2[num2].Coin5Collected = true;
								break;
							}
							break;
						}
					}
				}
			}
			bool flag = true;
			Levels[] platformingLevels = Level.platformingLevels;
			foreach (Levels level in platformingLevels)
			{
				if (Data.GetNumCoinsCollectedInLevel(level) < 5)
				{
					flag = false;
				}
			}
			if (flag)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "FoundAllLevelMoney");
			}
			if (Data.NumCoinsCollected >= 40)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "FoundAllMoney");
			}
		}

		private bool ContainsCoin(LevelCoin coin)
		{
			return ContainsCoin(coin.GlobalID);
		}

		private bool ContainsCoin(string coinID)
		{
			for (int i = 0; i < coins.Count; i++)
			{
				if (coins[i].coinID == coinID)
				{
					return true;
				}
			}
			return false;
		}
	}

	[Serializable]
	public class PlayerCoinProperties
	{
		public string coinID = string.Empty;

		public bool collected;

		public PlayerId player = PlayerId.None;

		public PlayerCoinProperties()
		{
		}

		public PlayerCoinProperties(LevelCoin coin)
		{
			coinID = coin.GlobalID;
		}

		public PlayerCoinProperties(string coinID)
		{
			this.coinID = coinID;
		}
	}

	[Serializable]
	public class MapData
	{
		public Scenes mapId;

		public bool sessionStarted;

		public bool hasVisitedDieHouse;

		public bool hasKingDiceDisappeared;

		public Vector3 playerOnePosition = Vector3.zero;

		public Vector3 playerTwoPosition = Vector3.zero;
	}

	[Serializable]
	public class MapDataManager
	{
		public Scenes currentMap = Scenes.scene_map_world_1;

		public List<MapData> mapData;

		public MapDataManager()
		{
			mapData = new List<MapData>();
		}

		public MapData GetCurrentMapData()
		{
			return GetMapData(currentMap);
		}

		public MapData GetMapData(Scenes map)
		{
			for (int i = 0; i < this.mapData.Count; i++)
			{
				if (this.mapData[i].mapId == map)
				{
					return this.mapData[i];
				}
			}
			MapData mapData = new MapData();
			mapData.mapId = map;
			this.mapData.Add(mapData);
			return mapData;
		}
	}

	[Serializable]
	public class PlayerLevelDataManager
	{
		public int dummy;

		public List<PlayerLevelDataObject> levelObjects;

		public PlayerLevelDataManager()
		{
			levelObjects = new List<PlayerLevelDataObject>();
			Levels[] values = EnumUtils.GetValues<Levels>();
			foreach (Levels levels in values)
			{
				PlayerLevelDataObject item = new PlayerLevelDataObject(levels)
				{
					levelID = levels
				};
				levelObjects.Add(item);
			}
		}

		public PlayerLevelDataObject GetLevelData(Levels levelID)
		{
			for (int i = 0; i < levelObjects.Count; i++)
			{
				if (levelObjects[i].levelID == levelID)
				{
					return levelObjects[i];
				}
			}
			PlayerLevelDataObject playerLevelDataObject = new PlayerLevelDataObject(levelID);
			levelObjects.Add(playerLevelDataObject);
			return playerLevelDataObject;
		}
	}

	[Serializable]
	public class PlayerLevelDataObject
	{
		public Levels levelID;

		public bool completed;

		public bool played;

		public LevelScoringData.Grade grade;

		public Level.Mode difficultyBeaten;

		public float bestTime = float.MaxValue;

		public int bgmPlayListCurrent;

		public PlayerLevelDataObject(Levels id)
		{
			levelID = id;
		}
	}

	[Serializable]
	public class PlayerStats
	{
		public int dummy;

		public PlayerStat playerOne = new PlayerStat();

		public PlayerStat playerTwo = new PlayerStat();

		public PlayerStat GetPlayer(PlayerId player)
		{
			switch (player)
			{
			case PlayerId.PlayerOne:
				return playerOne;
			case PlayerId.PlayerTwo:
				return playerTwo;
			default:
				Debug.LogWarning(string.Concat(player, " unlocks not configured!"));
				return null;
			}
		}
	}

	[Serializable]
	public class PlayerStat
	{
		public int numDeaths;

		public int numParriesInRow;

		public PlayerStat()
		{
			numDeaths = 0;
			numParriesInRow = 0;
		}

		public int DeathCount()
		{
			return numDeaths;
		}

		public void Die()
		{
			numDeaths++;
		}
	}

	private const string KEY = "cuphead_player_data_v1_slot_";

	private static readonly string[] SAVE_FILE_KEYS = new string[3] { "cuphead_player_data_v1_slot_0", "cuphead_player_data_v1_slot_1", "cuphead_player_data_v1_slot_2" };

	private static string emptyDialoguerState = string.Empty;

	private static int _CurrentSaveFileIndex = 0;

	private static bool _initialized = false;

	public static bool inGame = false;

	private static PlayerData[] _saveFiles;

	private static PlayerDataInitHandler _playerDatatInitHandler;

	public bool hasMadeFirstPurchase;

	public bool hasBeatenAnyBossOnEasy;

	public bool hasUnlockedFirstSuper;

	public bool shouldShowShopkeepTooltip;

	public bool shouldShowTurtleTooltip;

	public bool shouldShowCanteenTooltip;

	public bool shouldShowForkTooltip;

	public bool shouldShowKineDiceTooltip;

	public bool shouldShowMausoleumTooltip;

	public int dummy;

	[SerializeField]
	private PlayerLoadouts loadouts = new PlayerLoadouts();

	[SerializeField]
	private bool _isHardModeAvailable;

	[SerializeField]
	private bool _isTutorialCompleted;

	[SerializeField]
	private bool _isFlyingTutorialCompleted;

	[SerializeField]
	private PlayerInventories inventories = new PlayerInventories();

	public string dialoguerState;

	[SerializeField]
	public PlayerCoinManager coinManager = new PlayerCoinManager();

	private PlayerCoinManager levelCoinManager = new PlayerCoinManager();

	public bool unlockedBlackAndWhite;

	public bool unlocked2Strip;

	public bool vintageAudioEnabled;

	public bool pianoAudioEnabled;

	public BlurGamma.Filter filter;

	[SerializeField]
	private MapDataManager mapDataManager = new MapDataManager();

	[SerializeField]
	private PlayerLevelDataManager levelDataManager = new PlayerLevelDataManager();

	[SerializeField]
	private PlayerStats statictics = new PlayerStats();

	public static int CurrentSaveFileIndex
	{
		get
		{
			return Mathf.Clamp(_CurrentSaveFileIndex, 0, SAVE_FILE_KEYS.Length - 1);
		}
		set
		{
			_CurrentSaveFileIndex = Mathf.Clamp(value, 0, SAVE_FILE_KEYS.Length - 1);
			Data.LoadDialogueVariables();
		}
	}

	public static bool Initialized
	{
		get
		{
			return _initialized;
		}
		private set
		{
			_initialized = value;
		}
	}

	public static PlayerData Data
	{
		get
		{
			return GetDataForSlot(CurrentSaveFileIndex);
		}
	}

	public PlayerLoadouts Loadouts
	{
		get
		{
			return loadouts;
		}
	}

	public bool IsHardModeAvailable
	{
		get
		{
			return _isHardModeAvailable;
		}
		set
		{
			_isHardModeAvailable = value;
		}
	}

	public bool IsTutorialCompleted
	{
		get
		{
			return _isTutorialCompleted;
		}
		set
		{
			_isTutorialCompleted = value;
		}
	}

	public bool IsFlyingTutorialCompleted
	{
		get
		{
			return _isFlyingTutorialCompleted;
		}
		set
		{
			_isFlyingTutorialCompleted = value;
		}
	}

	public int NumCoinsCollected
	{
		get
		{
			return coinManager.NumCoinsCollected();
		}
	}

	public MapData CurrentMapData
	{
		get
		{
			return mapDataManager.GetCurrentMapData();
		}
	}

	public Scenes CurrentMap
	{
		get
		{
			return mapDataManager.currentMap;
		}
		set
		{
			mapDataManager.currentMap = value;
		}
	}

	public PlayerData()
	{
		if (string.IsNullOrEmpty(emptyDialoguerState))
		{
			Dialoguer.Initialize();
			emptyDialoguerState = Dialoguer.GetGlobalVariablesState();
		}
		dialoguerState = emptyDialoguerState;
	}

	public static PlayerData GetDataForSlot(int slot)
	{
		if (_saveFiles == null || _saveFiles.Length != SAVE_FILE_KEYS.Length)
		{
			_saveFiles = new PlayerData[SAVE_FILE_KEYS.Length];
			for (int i = 0; i < SAVE_FILE_KEYS.Length; i++)
			{
				_saveFiles[i] = new PlayerData();
			}
			Debug.Log("PlayerData not initialized!! Using default save");
		}
		return _saveFiles[slot];
	}

	public static void ClearSlot(int slot)
	{
		if (_saveFiles != null && _saveFiles.Length == SAVE_FILE_KEYS.Length)
		{
			ResetDialoguer();
			_saveFiles[slot] = new PlayerData();
			Save(slot);
		}
	}

	public static void Init(PlayerDataInitHandler handler)
	{
		_saveFiles = new PlayerData[SAVE_FILE_KEYS.Length];
		for (int i = 0; i < SAVE_FILE_KEYS.Length; i++)
		{
			_saveFiles[i] = new PlayerData();
		}
		_playerDatatInitHandler = handler;
		OnlineManager.Instance.Interface.InitializeCloudStorage(PlayerId.PlayerOne, OnCloudStorageInitialized);
	}

	private void LoadDialogueVariables()
	{
		Dialoguer.Initialize();
		Dialoguer.SetGlobalVariablesState(dialoguerState);
	}

	private static void OnCloudStorageInitialized(bool success)
	{
		if (!success)
		{
			_playerDatatInitHandler(false);
		}
		else
		{
			OnlineManager.Instance.Interface.LoadCloudData(SAVE_FILE_KEYS, OnLoaded);
		}
	}

	private static void OnLoaded(string[] data, CloudLoadResult result)
	{
		switch (result)
		{
		case CloudLoadResult.Failed:
			Debug.Log("[PlayerData] LOAD FAILED");
			OnlineManager.Instance.Interface.LoadCloudData(SAVE_FILE_KEYS, OnLoaded);
			return;
		case CloudLoadResult.NoData:
			Debug.Log("[PlayerData] No data. Saving default data to cloud");
			SaveAll();
			return;
		}
		for (int i = 0; i < SAVE_FILE_KEYS.Length; i++)
		{
			Debug.Log("[PlayerData] Unserializing save slot " + i);
			if (data[i] != null)
			{
				PlayerData playerData = null;
				try
				{
					playerData = JsonUtility.FromJson<PlayerData>(data[i]);
				}
				catch (ArgumentException)
				{
					Debug.Log("Unable to parse player data.");
				}
				if (playerData == null)
				{
					Debug.Log("[PlayerData] Data could not be unserialized for key: " + SAVE_FILE_KEYS[i]);
				}
				else
				{
					_saveFiles[i] = playerData;
				}
			}
		}
		Initialized = true;
		Debug.Log("[PlayerData] Loaded");
		if (_playerDatatInitHandler != null)
		{
			_playerDatatInitHandler(true);
			_playerDatatInitHandler = null;
		}
	}

	private static string GetSaveFileKey(int fileIndex)
	{
		return SAVE_FILE_KEYS[fileIndex];
	}

	private static void Save(int fileIndex)
	{
		_saveFiles[fileIndex].dialoguerState = Dialoguer.GetGlobalVariablesState();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary[SAVE_FILE_KEYS[fileIndex]] = JsonUtility.ToJson(_saveFiles[fileIndex]);
		OnlineManager.Instance.Interface.SaveCloudData(dictionary, OnSaved);
		Debug.Log("[PlayerData] Data saving for key " + SAVE_FILE_KEYS[fileIndex] + ": \n" + dictionary[SAVE_FILE_KEYS[fileIndex]]);
	}

	private static void SaveAll()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < SAVE_FILE_KEYS.Length; i++)
		{
			dictionary[SAVE_FILE_KEYS[i]] = JsonUtility.ToJson(_saveFiles[i]);
			Debug.Log("[PlayerData] Data saving for key " + SAVE_FILE_KEYS[i] + ": \n" + dictionary[SAVE_FILE_KEYS[i]]);
		}
		OnlineManager.Instance.Interface.SaveCloudData(dictionary, OnSavedAll);
	}

	private static void OnSaved(bool success)
	{
		if (success)
		{
			Debug.Log("[PlayerData] Save successful!");
			return;
		}
		Debug.Log("[PlayerData] SAVE FAILED. Retrying...");
		Save(CurrentSaveFileIndex);
	}

	private static void OnSavedAll(bool success)
	{
		if (success)
		{
			Debug.Log("[PlayerData] Save successful!");
			Initialized = true;
			if (_playerDatatInitHandler != null)
			{
				_playerDatatInitHandler(true);
				_playerDatatInitHandler = null;
			}
		}
		else
		{
			Debug.Log("[PlayerData] SAVE FAILED. Retrying...");
			SaveAll();
		}
	}

	public static void SaveCurrentFile()
	{
		Save(CurrentSaveFileIndex);
	}

	public static void ResetDialoguer()
	{
		Dialoguer.SetGlobalVariablesState(emptyDialoguerState);
	}

	public static void ResetAll()
	{
		for (int i = 0; i < SAVE_FILE_KEYS.Length; i++)
		{
			ClearSlot(i);
		}
	}

	public static void Unload()
	{
		_saveFiles = null;
	}

	public bool IsUnlocked(PlayerId player, Weapon value)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			return inventories.GetPlayer(PlayerId.PlayerOne).IsUnlocked(value);
		case PlayerId.PlayerTwo:
			return inventories.GetPlayer(PlayerId.PlayerTwo).IsUnlocked(value);
		case PlayerId.Any:
			return inventories.GetPlayer(PlayerId.PlayerOne).IsUnlocked(value) || inventories.GetPlayer(PlayerId.PlayerTwo).IsUnlocked(value);
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			return false;
		}
	}

	public bool IsUnlocked(PlayerId player, Super value)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			return inventories.GetPlayer(PlayerId.PlayerOne).IsUnlocked(value);
		case PlayerId.PlayerTwo:
			return inventories.GetPlayer(PlayerId.PlayerTwo).IsUnlocked(value);
		case PlayerId.Any:
			return inventories.GetPlayer(PlayerId.PlayerOne).IsUnlocked(value) || inventories.GetPlayer(PlayerId.PlayerTwo).IsUnlocked(value);
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			return false;
		}
	}

	public bool IsUnlocked(PlayerId player, Charm value)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			return inventories.GetPlayer(PlayerId.PlayerOne).IsUnlocked(value);
		case PlayerId.PlayerTwo:
			return inventories.GetPlayer(PlayerId.PlayerTwo).IsUnlocked(value);
		case PlayerId.Any:
			return inventories.GetPlayer(PlayerId.PlayerOne).IsUnlocked(value) || inventories.GetPlayer(PlayerId.PlayerTwo).IsUnlocked(value);
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			return false;
		}
	}

	public bool HasNewPurchase(PlayerId player)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			return inventories.GetPlayer(PlayerId.PlayerOne).newPurchase;
		case PlayerId.PlayerTwo:
			return inventories.GetPlayer(PlayerId.PlayerTwo).newPurchase;
		case PlayerId.Any:
			return inventories.GetPlayer(PlayerId.PlayerOne).newPurchase || inventories.GetPlayer(PlayerId.PlayerTwo).newPurchase;
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			return false;
		}
	}

	public void ResetHasNewPurchase(PlayerId player)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			inventories.GetPlayer(PlayerId.PlayerOne).newPurchase = false;
			break;
		case PlayerId.PlayerTwo:
			inventories.GetPlayer(PlayerId.PlayerTwo).newPurchase = false;
			break;
		case PlayerId.Any:
			inventories.GetPlayer(PlayerId.PlayerOne).newPurchase = false;
			inventories.GetPlayer(PlayerId.PlayerTwo).newPurchase = false;
			break;
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			break;
		}
	}

	public bool Buy(PlayerId player, Weapon value)
	{
		return inventories.GetPlayer(player).Buy(value);
	}

	public bool Buy(PlayerId player, Super value)
	{
		return inventories.GetPlayer(player).Buy(value);
	}

	public bool Buy(PlayerId player, Charm value)
	{
		return inventories.GetPlayer(player).Buy(value);
	}

	public void Gift(PlayerId player, Weapon value)
	{
		inventories.GetPlayer(player)._weapons.Add(value);
	}

	public void Gift(PlayerId player, Super value)
	{
		inventories.GetPlayer(player)._supers.Add(value);
	}

	public void Gift(PlayerId player, Charm value)
	{
		inventories.GetPlayer(player)._charms.Add(value);
	}

	public int NumWeapons(PlayerId player)
	{
		return inventories.GetPlayer(player)._weapons.Count;
	}

	public int NumCharms(PlayerId player)
	{
		return inventories.GetPlayer(player)._charms.Count;
	}

	public int NumSupers(PlayerId player)
	{
		return inventories.GetPlayer(player)._supers.Count;
	}

	public int GetCurrency(PlayerId player)
	{
		return inventories.GetPlayer(player).money;
	}

	public void AddCurrency(PlayerId player, int value)
	{
		inventories.GetPlayer(player).money += value;
	}

	public void ResetLevelCoinManager()
	{
		levelCoinManager = new PlayerCoinManager();
	}

	public bool GetCoinCollected(LevelCoin coin)
	{
		return coinManager.GetCoinCollected(coin);
	}

	public void SetLevelCoinCollected(LevelCoin coin, bool collected, PlayerId player)
	{
		levelCoinManager.SetCoinValue(coin, collected, player);
	}

	public int GetNumCoinsCollectedInLevel(Levels level)
	{
		List<PlayerCoinManager.LevelAndCoins> levelsAndCoins = coinManager.LevelsAndCoins;
		for (int i = 0; i < levelsAndCoins.Count; i++)
		{
			if (levelsAndCoins[i].level == level)
			{
				int num = 0;
				if (levelsAndCoins[i].Coin1Collected)
				{
					num++;
				}
				if (levelsAndCoins[i].Coin2Collected)
				{
					num++;
				}
				if (levelsAndCoins[i].Coin3Collected)
				{
					num++;
				}
				if (levelsAndCoins[i].Coin4Collected)
				{
					num++;
				}
				if (levelsAndCoins[i].Coin5Collected)
				{
					num++;
				}
				return num;
			}
		}
		return 0;
	}

	public void ApplyLevelCoins()
	{
		foreach (PlayerCoinProperties coin in levelCoinManager.coins)
		{
			coinManager.SetCoinValue(coin.coinID, coin.collected, coin.player);
			if (coin.collected)
			{
				Data.AddCurrency(PlayerId.PlayerOne, 1);
				Data.AddCurrency(PlayerId.PlayerTwo, 1);
			}
		}
		levelCoinManager = new PlayerCoinManager();
	}

	public MapData GetMapData(Scenes map)
	{
		return mapDataManager.GetMapData(map);
	}

	public PlayerLevelDataObject GetLevelData(Levels levelID)
	{
		return levelDataManager.GetLevelData(levelID);
	}

	public int CountLevelsCompleted(Levels[] levels)
	{
		int num = 0;
		foreach (Levels levelID in levels)
		{
			PlayerLevelDataObject levelData = GetLevelData(levelID);
			if (levelData.completed)
			{
				num++;
			}
		}
		return num;
	}

	public bool CheckLevelsCompleted(Levels[] levels)
	{
		foreach (Levels levelID in levels)
		{
			PlayerLevelDataObject levelData = GetLevelData(levelID);
			if (!levelData.completed)
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckLevelCompleted(Levels level)
	{
		PlayerLevelDataObject levelData = GetLevelData(level);
		if (!levelData.completed)
		{
			return false;
		}
		return true;
	}

	public int CountLevelsHaveMinGrade(Levels[] levels, LevelScoringData.Grade minGrade)
	{
		int num = 0;
		foreach (Levels levelID in levels)
		{
			PlayerLevelDataObject levelData = GetLevelData(levelID);
			if (levelData.completed && levelData.grade >= minGrade)
			{
				num++;
			}
		}
		return num;
	}

	public bool CheckLevelsHaveMinGrade(Levels[] levels, LevelScoringData.Grade minGrade)
	{
		foreach (Levels levelID in levels)
		{
			PlayerLevelDataObject levelData = GetLevelData(levelID);
			if (!levelData.completed || levelData.grade < minGrade)
			{
				return false;
			}
		}
		return true;
	}

	public int CountLevelsHaveMinDifficulty(Levels[] levels, Level.Mode minDifficulty)
	{
		int num = 0;
		foreach (Levels levelID in levels)
		{
			PlayerLevelDataObject levelData = GetLevelData(levelID);
			if (levelData.completed && levelData.difficultyBeaten >= minDifficulty)
			{
				num++;
			}
		}
		return num;
	}

	public bool CheckLevelsHaveMinDifficulty(Levels[] levels, Level.Mode minDifficulty)
	{
		foreach (Levels levelID in levels)
		{
			PlayerLevelDataObject levelData = GetLevelData(levelID);
			if (!levelData.completed || levelData.difficultyBeaten < minDifficulty)
			{
				return false;
			}
		}
		return true;
	}

	public float GetCompletionPercentage()
	{
		List<Levels> list = new List<Levels>();
		list.AddRange(Level.world1BossLevels);
		list.AddRange(Level.world2BossLevels);
		list.AddRange(Level.world3BossLevels);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		foreach (Levels item in list)
		{
			PlayerLevelDataObject levelData = GetLevelData(item);
			if (levelData.completed)
			{
				num++;
				switch (levelData.difficultyBeaten)
				{
				case Level.Mode.Normal:
					num2++;
					break;
				case Level.Mode.Hard:
					num2++;
					num8++;
					break;
				}
			}
		}
		Levels[] platformingLevels = Level.platformingLevels;
		foreach (Levels levelID in platformingLevels)
		{
			PlayerLevelDataObject levelData2 = GetLevelData(levelID);
			if (levelData2.completed)
			{
				num3++;
			}
		}
		num5 = NumCoinsCollected;
		num4 = NumSupers(PlayerId.PlayerOne);
		PlayerLevelDataObject levelData3 = GetLevelData(Levels.DicePalaceMain);
		if (levelData3.completed)
		{
			num6++;
			if (levelData3.difficultyBeaten == Level.Mode.Hard)
			{
				num9++;
			}
		}
		PlayerLevelDataObject levelData4 = GetLevelData(Levels.Devil);
		if (levelData4.completed)
		{
			num7++;
			if (levelData4.difficultyBeaten == Level.Mode.Hard)
			{
				num10++;
			}
		}
		return (float)num * 1.5f + (float)num3 * 1.5f + (float)num5 * 0.5f + (float)num4 * 1.5f + (float)(num2 * 2) + (float)(num6 * 3) + (float)(num7 * 4) + (float)(num8 * 5) + (float)(num9 * 7) + (float)(num10 * 8);
	}

	public int DeathCount(PlayerId player)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			return statictics.GetPlayer(PlayerId.PlayerOne).DeathCount();
		case PlayerId.PlayerTwo:
			return statictics.GetPlayer(PlayerId.PlayerTwo).DeathCount();
		case PlayerId.Any:
			return statictics.GetPlayer(PlayerId.PlayerOne).DeathCount() + statictics.GetPlayer(PlayerId.PlayerTwo).DeathCount();
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			return 0;
		}
	}

	public void Die(PlayerId player)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			statictics.GetPlayer(PlayerId.PlayerOne).Die();
			break;
		case PlayerId.PlayerTwo:
			statictics.GetPlayer(PlayerId.PlayerTwo).Die();
			break;
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			break;
		}
	}

	public int GetNumParriesInRow(PlayerId player)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			return statictics.GetPlayer(PlayerId.PlayerOne).numParriesInRow;
		case PlayerId.PlayerTwo:
			return statictics.GetPlayer(PlayerId.PlayerTwo).numParriesInRow;
		case PlayerId.Any:
			return Mathf.Max(statictics.GetPlayer(PlayerId.PlayerOne).numParriesInRow, statictics.GetPlayer(PlayerId.PlayerTwo).numParriesInRow);
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			return 0;
		}
	}

	public void SetNumParriesInRow(PlayerId player, int numParriesInRow)
	{
		switch (player)
		{
		case PlayerId.PlayerOne:
			statictics.GetPlayer(PlayerId.PlayerOne).numParriesInRow = numParriesInRow;
			break;
		case PlayerId.PlayerTwo:
			statictics.GetPlayer(PlayerId.PlayerTwo).numParriesInRow = numParriesInRow;
			break;
		default:
			Debug.LogWarning(string.Concat("Player '", player, "' not yet configured!"));
			break;
		}
	}
}
