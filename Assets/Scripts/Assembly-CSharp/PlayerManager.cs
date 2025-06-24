using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public static class PlayerManager
{
	private class PlayerSlot
	{
		public enum JoinState
		{
			NotJoining = 0,
			JoinPromptDisplayed = 1,
			JoinRequested = 2,
			Joined = 3,
			Leaving = 4
		}

		public enum ControllerState
		{
			NoController = 0,
			UsingController = 1,
			Disconnected = 2,
			ReconnectPromptDisplayed = 3
		}

		public bool canJoin;

		public JoinState joinState;

		public ControllerState controllerState;

		public bool canSwitch;

		public bool requestedSwitch;

		public bool promptBeforeJoin;

		public int controllerId;

		public bool shouldAssignController;

		public bool controllerDisconnectFromPlm;
	}

	public delegate void PlayerChangedDelegate(PlayerId playerId);

	private const float SINGLE_PLAYER_DAMAGE_MULTIPLIER = 1f;

	private const float MULTIPLAYER_DAMAGE_MULTIPLIER = 0.5f;

	private static PlayerSlot[] playerSlots = new PlayerSlot[2]
	{
		new PlayerSlot(),
		new PlayerSlot()
	};

	public static bool Multiplayer;

	private static PlayerId[] validIDs = new PlayerId[2]
	{
		PlayerId.PlayerOne,
		PlayerId.PlayerTwo
	};

	private static bool shouldGoToSlotSelect = false;

	private static bool shouldGoToStartScreen = false;

	private static bool pausedDueToPlm = false;

	private static bool shouldReinitializeCloudStorage = false;

	public static int player1DisconnectedControllerId;

	private static Dictionary<PlayerId, Player> playerInputs;

	private static Dictionary<PlayerId, AbstractPlayerController> players;

	private static PlayerId currentId;

	public static bool ShouldShowJoinPrompt
	{
		get
		{
			return playerSlots[1].joinState == PlayerSlot.JoinState.JoinPromptDisplayed;
		}
	}

	public static AbstractPlayerController Current
	{
		get
		{
			return GetPlayer(currentId);
		}
	}

	public static int Count
	{
		get
		{
			int num = 0;
			foreach (PlayerId key in players.Keys)
			{
				if (DoesPlayerExist(key) && !GetPlayer(key).IsDead)
				{
					num++;
				}
			}
			return num;
		}
	}

	public static Vector2 Center
	{
		get
		{
			if (!Multiplayer || Count < 2)
			{
				return GetFirst().center;
			}
			return (players[PlayerId.PlayerOne].center + players[PlayerId.PlayerTwo].center) / 2f;
		}
	}

	public static Vector2 CameraCenter
	{
		get
		{
			if (!Multiplayer || Count < 2)
			{
				return GetFirst().CameraCenter;
			}
			return (players[PlayerId.PlayerOne].center + players[PlayerId.PlayerTwo].CameraCenter) / 2f;
		}
	}

	public static float DamageMultiplier
	{
		get
		{
			if (Count > 1)
			{
				return 0.5f;
			}
			return 1f;
		}
	}

	public static event PlayerChangedDelegate OnPlayerJoinedEvent;

	public static event PlayerChangedDelegate OnPlayerLeaveEvent;

	public static event Action OnControlsChanged;

	public static void Awake()
	{
		Multiplayer = false;
		players = new Dictionary<PlayerId, AbstractPlayerController>();
		players.Add(PlayerId.PlayerOne, null);
		players.Add(PlayerId.PlayerTwo, null);
		playerInputs = new Dictionary<PlayerId, Player>();
		playerInputs.Add(PlayerId.PlayerOne, ReInput.players.GetPlayer(0));
		playerInputs.Add(PlayerId.PlayerTwo, ReInput.players.GetPlayer(1));
	}

	public static void Init()
	{
		OnlineManager.Instance.Interface.OnUserSignedIn += OnUserSignedIn;
		OnlineManager.Instance.Interface.OnUserSignedOut += OnUserSignedOut;
		ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
		PlmManager.Instance.Interface.OnUnconstrained += OnUnconstrained;
		PlmManager.Instance.Interface.OnResume += OnResume;
		PlmManager.Instance.Interface.OnSuspend += OnSuspend;
	}

	public static void SetPlayerCanJoin(PlayerId player, bool canJoin, bool promptBeforeJoin)
	{
		PlayerSlot playerSlot = ((player != PlayerId.PlayerOne) ? playerSlots[1] : playerSlots[0]);
		playerSlot.canJoin = canJoin;
		playerSlot.promptBeforeJoin = promptBeforeJoin;
		if (!canJoin && playerSlot.joinState == PlayerSlot.JoinState.JoinPromptDisplayed)
		{
			playerSlot.joinState = PlayerSlot.JoinState.NotJoining;
		}
	}

	public static void ClearJoinPrompt()
	{
		for (int i = 0; i < 2; i++)
		{
			if (playerSlots[i].joinState == PlayerSlot.JoinState.JoinPromptDisplayed)
			{
				playerSlots[i].joinState = PlayerSlot.JoinState.NotJoining;
			}
		}
	}

	public static void SetPlayerCanSwitch(PlayerId player, bool canSwitch)
	{
		PlayerSlot playerSlot = ((player != PlayerId.PlayerOne) ? playerSlots[1] : playerSlots[0]);
		Debug.Log("Setting " + player.ToString() + " can switch: " + canSwitch);
		playerSlot.canSwitch = canSwitch;
		playerSlot.requestedSwitch = false;
	}

	public static void PlayerLeave(PlayerId player)
	{
		PlayerSlot playerSlot = ((player != PlayerId.PlayerOne) ? playerSlots[1] : playerSlots[0]);
		playerSlot.joinState = PlayerSlot.JoinState.Leaving;
	}

	public static void Update()
	{
		if (InterruptingPrompt.IsInterrupting())
		{
			for (int i = 0; i < playerSlots.Length; i++)
			{
				if (playerSlots[i].joinState == PlayerSlot.JoinState.Joined && playerSlots[i].controllerState == PlayerSlot.ControllerState.ReconnectPromptDisplayed)
				{
					PlayerId playerId = ((i != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
					Joystick joystick = CupheadInput.CheckForUnconnectedControllerPress();
					Player playerInput = GetPlayerInput(playerId);
					if (joystick != null)
					{
						playerSlots[i].controllerState = PlayerSlot.ControllerState.UsingController;
						playerSlots[i].controllerId = joystick.id;
						playerSlots[i].controllerDisconnectFromPlm = false;
						playerInput.controllers.AddController(joystick, true);
						ReInput.userDataStore.LoadControllerData(playerInputs[playerId].id, ControllerType.Joystick, playerSlots[i].controllerId);
						ControlsChanged();
						Debug.Log("Player controller reconnected!");
					}
					if (!OnlineManager.Instance.Interface.SupportsMultipleUsers && playerInput.GetAnyButtonDown())
					{
						playerSlots[i].controllerState = PlayerSlot.ControllerState.NoController;
						playerSlots[i].controllerDisconnectFromPlm = false;
						ControlsChanged();
						Debug.Log("Player switch to keyboard");
					}
				}
			}
			return;
		}
		for (int j = 0; j < playerSlots.Length; j++)
		{
			if (!playerSlots[j].canJoin || playerSlots[j].joinState == PlayerSlot.JoinState.Joined)
			{
				continue;
			}
			PlayerId playerId2 = ((j != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
			bool flag = false;
			Joystick joystick2 = CupheadInput.CheckForUnconnectedControllerPress();
			Player playerInput2 = GetPlayerInput(playerId2);
			if (joystick2 != null)
			{
				flag = true;
				playerSlots[j].controllerState = PlayerSlot.ControllerState.UsingController;
				playerSlots[j].controllerId = joystick2.id;
			}
			if (!OnlineManager.Instance.Interface.SupportsMultipleUsers && playerInput2.GetAnyButtonDown())
			{
				flag = true;
				playerSlots[j].controllerState = PlayerSlot.ControllerState.NoController;
			}
			if (!flag)
			{
				continue;
			}
			if (playerSlots[j].joinState == PlayerSlot.JoinState.NotJoining && playerSlots[j].promptBeforeJoin)
			{
				playerSlots[j].joinState = PlayerSlot.JoinState.JoinPromptDisplayed;
				continue;
			}
			bool flag2 = false;
			playerSlots[j].joinState = PlayerSlot.JoinState.JoinRequested;
			if (OnlineManager.Instance.Interface.SupportsMultipleUsers)
			{
				ulong value = (ulong)joystick2.systemId.Value;
				OnlineUser userForController = OnlineManager.Instance.Interface.GetUserForController(value);
				if (userForController != null && ((j == 0 && !userForController.Equals(OnlineManager.Instance.Interface.SecondaryUser)) || (j == 1 && !userForController.Equals(OnlineManager.Instance.Interface.MainUser))))
				{
					OnlineManager.Instance.Interface.SetUser(playerId2, userForController);
					flag2 = true;
				}
				else
				{
					OnlineManager.Instance.Interface.SignInUser(false, playerId2, value);
				}
			}
			else if (OnlineManager.Instance.Interface.SupportsUserSignIn && playerId2 == PlayerId.PlayerOne)
			{
				OnlineManager.Instance.Interface.SignInUser(false, playerId2, 0uL);
			}
			else
			{
				flag2 = true;
			}
			if (flag2)
			{
				if (joystick2 != null)
				{
					playerInput2.controllers.AddController(joystick2, true);
					ReInput.userDataStore.LoadControllerData(playerInputs[playerId2].id, ControllerType.Joystick, playerSlots[j].controllerId);
				}
				Debug.Log(playerId2.ToString() + " joined");
				playerSlots[j].joinState = PlayerSlot.JoinState.Joined;
				if (playerId2 == PlayerId.PlayerTwo)
				{
					Multiplayer = true;
				}
				PlayerManager.OnPlayerJoinedEvent(playerId2);
				AudioManager.Play("player_spawn");
			}
		}
		for (int k = 0; k < playerSlots.Length; k++)
		{
			if (!OnlineManager.Instance.Interface.SupportsUserSignIn || !playerSlots[k].canSwitch || playerSlots[k].joinState != PlayerSlot.JoinState.Joined || (!OnlineManager.Instance.Interface.SupportsMultipleUsers && k == 1))
			{
				continue;
			}
			PlayerId playerId3 = ((k != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
			Player playerInput3 = GetPlayerInput(playerId3);
			if (playerInput3.GetButtonDown(11))
			{
				Debug.Log("Switch requested");
				playerSlots[k].requestedSwitch = true;
				playerSlots[(k + 1) % 2].requestedSwitch = false;
				ulong controllerId = 0uL;
				if (playerInput3.controllers.joystickCount > 0)
				{
					controllerId = (ulong)playerInput3.controllers.Joysticks[0].systemId.Value;
				}
				OnlineManager.Instance.Interface.SwitchUser(playerId3, controllerId);
			}
		}
		for (int l = 0; l < playerSlots.Length; l++)
		{
			if (SceneLoader.CurrentlyLoading)
			{
				break;
			}
			if (playerSlots[l].joinState == PlayerSlot.JoinState.Leaving)
			{
				PlayerId playerId4 = ((l != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
				Player playerInput4 = GetPlayerInput(playerId4);
				playerInput4.controllers.ClearControllersOfType<Joystick>();
				playerSlots[l].joinState = PlayerSlot.JoinState.NotJoining;
				if (playerId4 == PlayerId.PlayerTwo)
				{
					Multiplayer = false;
				}
				OnlineManager.Instance.Interface.SetRichPresenceActive(playerId4, false);
				OnlineManager.Instance.Interface.SetUser(playerId4, null);
				if (playerId4 == PlayerId.PlayerOne)
				{
					shouldGoToStartScreen = true;
				}
				else if (PlayerManager.OnPlayerLeaveEvent != null)
				{
					PlayerManager.OnPlayerLeaveEvent(playerId4);
					AudioManager.Play("player_despawn");
				}
			}
		}
		for (int m = 0; m < playerSlots.Length; m++)
		{
			if (playerSlots[m].shouldAssignController)
			{
				PlayerId playerId5 = ((m != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
				Player playerInput5 = GetPlayerInput(playerId5);
				playerInput5.controllers.AddController<Joystick>(playerSlots[m].controllerId, true);
				ReInput.userDataStore.LoadControllerData(playerInputs[playerId5].id, ControllerType.Joystick, playerSlots[m].controllerId);
				playerSlots[m].shouldAssignController = false;
			}
		}
		if (ControllerDisconnectedPrompt.Instance != null && !ControllerDisconnectedPrompt.Instance.Visible && ControllerDisconnectedPrompt.Instance.allowedToShow)
		{
			for (int n = 0; n < 2; n++)
			{
				PlayerId playerId6 = ((n != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
				if (IsControllerDisconnected(playerId6))
				{
					ControllerDisconnectedPrompt.Instance.Show(playerId6);
					break;
				}
			}
		}
		if (PlmManager.Instance.Interface.IsConstrained())
		{
			if (InterruptingPrompt.CanInterrupt() && PauseManager.state != PauseManager.State.Paused)
			{
				PauseManager.Pause();
				pausedDueToPlm = true;
			}
		}
		else if (pausedDueToPlm)
		{
			PauseManager.Unpause();
			pausedDueToPlm = false;
		}
		if (shouldGoToSlotSelect)
		{
			goToSlotSelect();
			shouldGoToSlotSelect = false;
		}
		if (shouldGoToStartScreen)
		{
			goToStartScreen();
			shouldGoToStartScreen = false;
		}
	}

	public static void ControllerRemapped(PlayerId playerId, bool usingController, int controllerId)
	{
		int num = ((playerId != PlayerId.PlayerOne) ? 1 : 0);
		playerSlots[num].controllerState = (usingController ? PlayerSlot.ControllerState.UsingController : PlayerSlot.ControllerState.NoController);
		playerSlots[num].controllerId = controllerId;
	}

	public static void ControlsChanged()
	{
		if (PlayerManager.OnControlsChanged != null)
		{
			PlayerManager.OnControlsChanged();
		}
	}

	private static void OnUserSignedIn(OnlineUser user)
	{
		for (int i = 0; i < playerSlots.Length; i++)
		{
			if (!playerSlots[i].canJoin || playerSlots[i].joinState != PlayerSlot.JoinState.JoinRequested)
			{
				continue;
			}
			OnlineManager.Instance.Interface.UpdateControllerMapping();
			if (user == null || (i == 0 && user.Equals(OnlineManager.Instance.Interface.SecondaryUser)) || (i == 1 && user.Equals(OnlineManager.Instance.Interface.MainUser)))
			{
				playerSlots[i].joinState = PlayerSlot.JoinState.NotJoining;
				continue;
			}
			PlayerId playerId = ((i != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
			Player playerInput = GetPlayerInput(playerId);
			OnlineManager.Instance.Interface.SetUser(playerId, user);
			if (playerSlots[i].controllerState == PlayerSlot.ControllerState.UsingController)
			{
				playerSlots[i].shouldAssignController = true;
			}
			Debug.Log(playerId.ToString() + " joined");
			playerSlots[i].joinState = PlayerSlot.JoinState.Joined;
			if (playerId == PlayerId.PlayerTwo)
			{
				Multiplayer = true;
			}
			PlayerManager.OnPlayerJoinedEvent(playerId);
		}
		for (int j = 0; j < playerSlots.Length; j++)
		{
			if (!playerSlots[j].canSwitch || !playerSlots[j].requestedSwitch || playerSlots[j].joinState != PlayerSlot.JoinState.Joined)
			{
				continue;
			}
			OnlineManager.Instance.Interface.UpdateControllerMapping();
			playerSlots[j].requestedSwitch = false;
			PlayerId player = ((j != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
			if (user != null && !user.Equals(OnlineManager.Instance.Interface.MainUser) && !user.Equals(OnlineManager.Instance.Interface.SecondaryUser))
			{
				OnlineManager.Instance.Interface.SetUser(player, user);
				Debug.Log(player.ToString() + "switched profiles");
				if (j == 0)
				{
					shouldGoToSlotSelect = true;
				}
			}
		}
	}

	private static void OnUserSignedOut(PlayerId player, string name)
	{
		if (!PlmManager.Instance.Interface.IsConstrained())
		{
			PlayerSlot playerSlot = ((player != PlayerId.PlayerOne) ? playerSlots[1] : playerSlots[0]);
			if (!playerSlot.requestedSwitch)
			{
				PlayerLeave(player);
			}
		}
	}

	private static void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
	{
		if (PlmManager.Instance.Interface.IsConstrained())
		{
			return;
		}
		for (int i = 0; i < playerSlots.Length; i++)
		{
			PlayerId playerId = ((i != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
			if (playerSlots[i].controllerState == PlayerSlot.ControllerState.UsingController && playerSlots[i].controllerId == args.controllerId && playerSlots[i].joinState == PlayerSlot.JoinState.Joined)
			{
				playerInputs[playerId].controllers.RemoveController<Joystick>(args.controllerId);
				playerSlots[i].controllerState = PlayerSlot.ControllerState.Disconnected;
				if (playerId == PlayerId.PlayerOne)
				{
					player1DisconnectedControllerId = args.controllerId;
				}
				Debug.Log(playerId.ToString() + " controller disconnected");
			}
		}
	}

	private static void OnSuspend()
	{
		if (OnlineManager.Instance.Interface.CloudStorageInitialized)
		{
			Debug.Log("UNINITIALIZING CLOUD STORAGE");
			OnlineManager.Instance.Interface.UninitializeCloudStorage();
			shouldReinitializeCloudStorage = true;
		}
	}

	private static void OnResume()
	{
		Debug.Log("PLM: Resumed");
		if (!PlmManager.Instance.Interface.IsConstrained())
		{
			CheckForPairingsChanges();
		}
		if (playerSlots[0].joinState == PlayerSlot.JoinState.Joined && shouldReinitializeCloudStorage)
		{
			OnlineManager.Instance.Interface.InitializeCloudStorage(PlayerId.PlayerOne, OnCloudStorageInitialized);
		}
		shouldReinitializeCloudStorage = false;
	}

	private static void OnCloudStorageInitialized(bool success)
	{
		if (!success)
		{
			OnlineManager.Instance.Interface.InitializeCloudStorage(PlayerId.PlayerOne, OnCloudStorageInitialized);
		}
	}

	private static void OnUnconstrained()
	{
		Debug.Log("PLM: Unconstrained");
		CheckForPairingsChanges();
	}

	private static void CheckForPairingsChanges()
	{
		bool flag = OnlineManager.Instance.Interface.ControllerMappingChanged();
		for (int i = 0; i < playerSlots.Length; i++)
		{
			PlayerId playerId = ((i != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne);
			if (playerSlots[i].joinState != PlayerSlot.JoinState.Joined)
			{
				continue;
			}
			if (!OnlineManager.Instance.Interface.IsUserSignedIn(playerId))
			{
				PlayerLeave(playerId);
				continue;
			}
			if (!flag)
			{
				Debug.Log("PAIRINGS NOT CHANGED");
				if (playerSlots[i].controllerState == PlayerSlot.ControllerState.UsingController && playerInputs[playerId].controllers.joystickCount == 0)
				{
					playerInputs[playerId].controllers.AddController<Joystick>(playerSlots[i].controllerId, true);
					ReInput.userDataStore.LoadControllerData(playerInputs[playerId].id, ControllerType.Joystick, playerSlots[i].controllerId);
				}
				continue;
			}
			Debug.Log("PAIRINGS CHANGED");
			List<ulong> controllersForUser = OnlineManager.Instance.Interface.GetControllersForUser(playerId);
			if (controllersForUser == null || controllersForUser.Count != 1)
			{
				playerInputs[playerId].controllers.ClearControllersOfType<Joystick>();
				playerSlots[i].controllerState = PlayerSlot.ControllerState.Disconnected;
				playerSlots[i].controllerDisconnectFromPlm = true;
				Debug.Log(playerId.ToString() + " controller mapping not found");
				continue;
			}
			ulong num = controllersForUser[0];
			Debug.Log("ControllerID for user: " + num);
			foreach (Joystick joystick in ReInput.controllers.Joysticks)
			{
				Debug.Log("Comparing to rewired systemId: " + (ulong)joystick.systemId.Value);
				if (joystick.systemId.Value == (long)num)
				{
					Debug.Log("Controller found");
					if (playerInputs[playerId].controllers.joystickCount > 0)
					{
						Debug.Log("Current controller id: " + (ulong)playerInputs[playerId].controllers.Joysticks[0].systemId.Value);
					}
					playerInputs[playerId].controllers.ClearControllersOfType<Joystick>();
					playerInputs[playerId].controllers.AddController(joystick, true);
					ReInput.userDataStore.LoadControllerData(playerInputs[playerId].id, ControllerType.Joystick, playerSlots[i].controllerId);
					playerSlots[i].controllerId = joystick.id;
					break;
				}
			}
		}
	}

	public static bool IsControllerDisconnected(PlayerId playerId)
	{
		int num = ((playerId != PlayerId.PlayerOne) ? 1 : 0);
		return playerSlots[num].joinState == PlayerSlot.JoinState.Joined && (playerSlots[num].controllerState == PlayerSlot.ControllerState.Disconnected || playerSlots[num].controllerState == PlayerSlot.ControllerState.ReconnectPromptDisplayed);
	}

	public static void OnDisconnectPromptDisplayed(PlayerId playerId)
	{
		int num = ((playerId != PlayerId.PlayerOne) ? 1 : 0);
		playerSlots[num].controllerState = PlayerSlot.ControllerState.ReconnectPromptDisplayed;
	}

	private static void goToSlotSelect()
	{
		playerSlots[0].canSwitch = false;
		playerSlots[0].requestedSwitch = false;
		playerSlots[0].canJoin = false;
		GetPlayerInput(PlayerId.PlayerTwo).controllers.ClearControllersOfType<Joystick>();
		playerSlots[1] = new PlayerSlot();
		Multiplayer = false;
		OnlineManager.Instance.Interface.SetUser(PlayerId.PlayerTwo, null);
		SceneLoader.LoadScene(Scenes.scene_slot_select, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
	}

	private static void goToStartScreen()
	{
		ResetPlayers();
		SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade);
	}

	public static void ResetPlayers()
	{
		playerSlots[0] = new PlayerSlot();
		playerSlots[1] = new PlayerSlot();
		GetPlayerInput(PlayerId.PlayerOne).controllers.ClearControllersOfType<Joystick>();
		GetPlayerInput(PlayerId.PlayerTwo).controllers.ClearControllersOfType<Joystick>();
		Multiplayer = false;
		if (OnlineManager.Instance.Interface.SupportsMultipleUsers)
		{
			OnlineManager.Instance.Interface.SetUser(PlayerId.PlayerOne, null);
			OnlineManager.Instance.Interface.SetUser(PlayerId.PlayerTwo, null);
		}
	}

	public static Player GetPlayerInput(PlayerId id)
	{
		return playerInputs[id];
	}

	public static void SetPlayer(PlayerId id, AbstractPlayerController player)
	{
		players[id] = player;
	}

	public static void ClearPlayer(PlayerId id)
	{
		players[id] = null;
	}

	public static void ClearPlayers()
	{
		currentId = PlayerId.PlayerOne;
		players[PlayerId.PlayerOne] = null;
		players[PlayerId.PlayerTwo] = null;
	}

	public static AbstractPlayerController GetPlayer(PlayerId id)
	{
		return players[id];
	}

	public static T GetPlayer<T>(PlayerId id) where T : AbstractPlayerController
	{
		return GetPlayer(id) as T;
	}

	public static AbstractPlayerController GetRandom()
	{
		if (!Multiplayer || !DoesPlayerExist(PlayerId.PlayerTwo))
		{
			return players[PlayerId.PlayerOne];
		}
		return GetPlayer(EnumUtils.Random<PlayerId>());
	}

	public static AbstractPlayerController GetNext()
	{
		if (!Multiplayer || !DoesPlayerExist(PlayerId.PlayerTwo))
		{
			return players[PlayerId.PlayerOne];
		}
		if (!DoesPlayerExist(PlayerId.PlayerOne))
		{
			return players[PlayerId.PlayerTwo];
		}
		AbstractPlayerController current = Current;
		PlayerId playerId = currentId;
		if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
		{
			currentId = PlayerId.PlayerTwo;
		}
		else
		{
			currentId = PlayerId.PlayerOne;
		}
		return current;
	}

	private static bool DoesPlayerExist(PlayerId player)
	{
		if (players[player] == null)
		{
			return false;
		}
		if (players[player].IsDead)
		{
			return false;
		}
		return true;
	}

	public static AbstractPlayerController GetFirst()
	{
		if (!DoesPlayerExist(PlayerId.PlayerOne))
		{
			return players[PlayerId.PlayerTwo];
		}
		return players[PlayerId.PlayerOne];
	}
}
