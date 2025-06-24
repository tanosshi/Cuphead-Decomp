using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : AbstractMonoBehaviour
{
	public enum State
	{
		Starting = 0,
		Ready = 1,
		Event = 2,
		Exiting = 3
	}

	[Serializable]
	public class Camera
	{
		public bool moveX = true;

		public bool moveY = true;

		public CupheadBounds bounds = new CupheadBounds(-6.4f, 6.4f, 3.6f, -3.6f);
	}

	public MapResources MapResources;

	[SerializeField]
	private Camera cameraProperties;

	[Space(10f)]
	[SerializeField]
	private AbstractMapInteractiveEntity firstNode;

	private MapUI ui;

	private Scenes scene;

	private CupheadMapCamera camera;

	public Levels level;

	public List<CoinPositionAndID> LevelCoinsIDs = new List<CoinPositionAndID>();

	public static Map Current { get; private set; }

	public State CurrentState { get; set; }

	public MapPlayerController[] players { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		Cuphead.Init();
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeave;
		scene = EnumUtils.Parse<Scenes>(SceneManager.GetActiveScene().name);
		PlayerData.Data.CurrentMap = scene;
		CreateUI();
		CreatePlayers();
		ui.Init(players);
		camera = UnityEngine.Object.FindObjectOfType<CupheadMapCamera>();
		camera.Init(cameraProperties);
		CupheadTime.SetAll(1f);
		AudioManager.StopBGM();
		SceneLoader.OnLoaderCompleteEvent += SelectMusic;
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

	protected override void Start()
	{
		base.Start();
		AudioManager.PlayLoop(string.Empty);
		StartCoroutine(start_cr());
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SceneLoader.OnLoaderCompleteEvent -= SelectMusic;
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeave;
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawLine(new Vector3(cameraProperties.bounds.left, cameraProperties.bounds.top), new Vector3(cameraProperties.bounds.left, cameraProperties.bounds.bottom));
		Gizmos.DrawLine(new Vector3(cameraProperties.bounds.right, cameraProperties.bounds.top), new Vector3(cameraProperties.bounds.right, cameraProperties.bounds.bottom));
		Gizmos.DrawLine(new Vector3(cameraProperties.bounds.right, cameraProperties.bounds.top), new Vector3(cameraProperties.bounds.left, cameraProperties.bounds.top));
		Gizmos.DrawLine(new Vector3(cameraProperties.bounds.right, cameraProperties.bounds.bottom), new Vector3(cameraProperties.bounds.left, cameraProperties.bounds.bottom));
	}

	private void CreateUI()
	{
		ui = UnityEngine.Object.FindObjectOfType<MapUI>();
		if (ui == null)
		{
			ui = MapUI.Create();
		}
	}

	private void CreatePlayers()
	{
		if (!PlayerData.Data.CurrentMapData.sessionStarted)
		{
			PlayerData.Data.CurrentMapData.sessionStarted = true;
			PlayerData.Data.CurrentMapData.playerOnePosition = (Vector2)firstNode.transform.position + firstNode.returnPositions.playerOne;
			PlayerData.Data.CurrentMapData.playerTwoPosition = (Vector2)firstNode.transform.position + firstNode.returnPositions.playerTwo;
			if (!PlayerManager.Multiplayer)
			{
				PlayerData.Data.CurrentMapData.playerOnePosition = (Vector2)firstNode.transform.position + firstNode.returnPositions.singlePlayer;
			}
		}
		PlayerData.SaveCurrentFile();
		MapPlayerPose pose = MapPlayerPose.Default;
		if (Level.Won)
		{
			pose = MapPlayerPose.Won;
		}
		players = new MapPlayerController[2];
		players[0] = MapPlayerController.Create(PlayerId.PlayerOne, new MapPlayerController.InitObject(PlayerData.Data.CurrentMapData.playerOnePosition, pose));
		if (PlayerManager.Multiplayer)
		{
			players[1] = MapPlayerController.Create(PlayerId.PlayerTwo, new MapPlayerController.InitObject(PlayerData.Data.CurrentMapData.playerTwoPosition, pose));
		}
	}

	private void OnPlayerJoined(PlayerId playerId)
	{
		if (playerId != PlayerId.PlayerTwo)
		{
			return;
		}
		Vector3 position = players[0].transform.position;
		Vector3 vector = position + new Vector3(0.05f, 0.05f, 0f);
		LayerMask layerMask = -257;
		for (int i = 0; i < 10; i++)
		{
			float num = 36 * -i + 150;
			Vector2 vector2 = new Vector2(Mathf.Cos((float)Math.PI / 180f * num), Mathf.Sin((float)Math.PI / 180f * num));
			if (!(Physics2D.CircleCast(position, 0.2f, vector2, 0.7f, layerMask.value).collider != null))
			{
				vector = position + (Vector3)(vector2 * 0.7f);
				break;
			}
		}
		players[1] = MapPlayerController.Create(PlayerId.PlayerTwo, new MapPlayerController.InitObject(vector, MapPlayerPose.Joined));
		LevelNewPlayerGUI.Current.Init();
		SetRichPresence();
	}

	private void OnPlayerLeave(PlayerId playerId)
	{
		if (playerId == PlayerId.PlayerTwo)
		{
			players[1].OnLeave();
		}
	}

	public void OnLoadLevel()
	{
	}

	public void OnLoadShop()
	{
	}

	private IEnumerator start_cr()
	{
		SetRichPresence();
		if (Level.Won)
		{
			yield return CupheadTime.WaitForSeconds(this, 1.5f);
			bool longPlayerAnimation = true;
			bool cameraMoved = false;
			Vector3 cameraStartPos = camera.transform.position;
			if (AbstractMapLevelDependentEntity.RegisteredEntities != null)
			{
				Debug.Log("Registered Entities: " + AbstractMapLevelDependentEntity.RegisteredEntities.Count + "\n\rStarting Events");
				while (AbstractMapLevelDependentEntity.RegisteredEntities.Count > 0)
				{
					yield return null;
					CurrentState = State.Event;
					AbstractMapLevelDependentEntity entity = AbstractMapLevelDependentEntity.RegisteredEntities[0];
					foreach (AbstractMapLevelDependentEntity registeredEntity in AbstractMapLevelDependentEntity.RegisteredEntities)
					{
						if (!registeredEntity.panCamera)
						{
							entity = registeredEntity;
							break;
						}
						if (!(registeredEntity == entity))
						{
							float num = Vector2.Distance(camera.transform.position, registeredEntity.CameraPosition);
							if (num < Vector2.Distance(camera.transform.position, entity.CameraPosition))
							{
								entity = registeredEntity;
							}
						}
					}
					AbstractMapLevelDependentEntity.RegisteredEntities.Remove(entity);
					Debug.Log("Entity: " + entity.gameObject.name);
					if (entity.panCamera)
					{
						yield return camera.MoveToPosition(entity.CameraPosition, 0.5f, 0.9f);
						cameraMoved = true;
					}
					entity.MapMeetCondition();
					while (entity.CurrentState != AbstractMapLevelDependentEntity.State.Complete)
					{
						yield return null;
					}
					yield return CupheadTime.WaitForSeconds(this, 0.25f);
					longPlayerAnimation = false;
				}
				if (cameraMoved)
				{
					camera.MoveToPosition(cameraStartPos, 0.75f, 1f);
				}
			}
			yield return CupheadTime.WaitForSeconds(this, (!longPlayerAnimation) ? 1f : 2.5f);
			players[0].OnWinComplete();
			if (PlayerManager.Multiplayer)
			{
				players[1].OnWinComplete();
			}
			if (!Level.PreviouslyWon || Level.PreviousDifficulty < Level.Mode.Normal || Level.PreviousLevel == Levels.Mausoleum)
			{
				if (!Level.IsDicePalace && !Level.IsDicePalaceMain && Level.PreviousLevel != Levels.Devil && Level.PreviousLevel != Levels.Mausoleum && Level.PreviousLevelType == Level.Type.Battle)
				{
					if (Level.Difficulty == Level.Mode.Easy && !PlayerData.Data.hasBeatenAnyBossOnEasy && (!PlayerData.Data.CheckLevelsHaveMinDifficulty(Level.world1BossLevels, Level.Mode.Normal) || !PlayerData.Data.CheckLevelsHaveMinDifficulty(Level.world2BossLevels, Level.Mode.Normal) || !PlayerData.Data.CheckLevelsHaveMinDifficulty(Level.world3BossLevels, Level.Mode.Normal)))
					{
						MapEventNotification.Current.ShowTooltipEvent(TooltipEvent.KingDice);
						PlayerData.Data.hasBeatenAnyBossOnEasy = true;
						PlayerData.SaveCurrentFile();
					}
					if (Level.Difficulty >= Level.Mode.Normal)
					{
						MapEventNotification.Current.ShowEvent(MapEventNotification.Type.SoulContract);
						while (MapEventNotification.Current.showing)
						{
							yield return null;
						}
						longPlayerAnimation = false;
						yield return CupheadTime.WaitForSeconds(this, 0.25f);
					}
				}
				else if (Level.SuperUnlocked)
				{
					MapEventNotification.Current.ShowEvent(MapEventNotification.Type.Super);
					if (!PlayerData.Data.hasUnlockedFirstSuper)
					{
						MapEventNotification.Current.ShowTooltipEvent(TooltipEvent.Mausoleum);
						PlayerData.Data.hasUnlockedFirstSuper = true;
						PlayerData.SaveCurrentFile();
					}
					longPlayerAnimation = false;
				}
			}
			while ((bool)MapEventNotification.Current && MapEventNotification.Current.showing)
			{
				yield return null;
			}
		}
		CurrentState = State.Ready;
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, true, true);
		InterruptingPrompt.SetCanInterrupt(true);
		Level.ResetPreviousLevelInfo();
	}

	private void SetRichPresence()
	{
		OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "WorldMap", SceneLoader.SceneName);
		OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "Exploring", true);
	}
}
