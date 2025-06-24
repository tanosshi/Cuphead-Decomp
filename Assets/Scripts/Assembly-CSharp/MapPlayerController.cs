using System;
using System.Collections;
using UnityEngine;

public class MapPlayerController : MapSprite
{
	public enum State
	{
		Walking = 0,
		LadderEnter = 1,
		LadderExit = 2,
		Ladder = 3,
		Stationary = 4
	}

	[Serializable]
	public class InitObject
	{
		public Vector2 position;

		public MapPlayerPose pose;

		public InitObject(Vector2 position, MapPlayerPose pose)
		{
			this.position = position;
			this.pose = pose;
		}
	}

	public delegate void LadderEnterEventHandler(Vector2 point, MapPlayerLadderObject ladder, MapLadder.Location location);

	public delegate void LadderExitEventHandler(Vector2 point, Vector2 exit, MapLadder.Location location);

	public const string TAG = "Player_Map";

	private bool joinedMidGame;

	public State state { get; private set; }

	public PlayerId id { get; private set; }

	public bool EquipMenuOpen { get; private set; }

	public PlayerInput input { get; private set; }

	public MapPlayerMotor motor { get; private set; }

	public MapPlayerAnimationController animationController { get; private set; }

	public MapPlayerLadderManager ladderManager { get; private set; }

	public Vector2 Velocity
	{
		get
		{
			return motor.velocity;
		}
	}

	public MapPlayerAnimationController.Direction Direction
	{
		get
		{
			return animationController.direction;
		}
	}

	public static event Action OnEquipMenuOpenedEvent;

	public static event Action OnEquipMenuClosedEvent;

	public event LadderEnterEventHandler LadderEnterEvent;

	public event LadderExitEventHandler LadderExitEvent;

	public event Action LadderEnterCompleteEvent;

	public event Action LadderExitCompleteEvent;

	public static bool CanMove()
	{
		return MapDifficultySelectStartUI.Current.CurrentState == AbstractMapSceneStartUI.State.Inactive && MapConfirmStartUI.Current.CurrentState == AbstractMapSceneStartUI.State.Inactive && MapBasicStartUI.Current.CurrentState == AbstractMapSceneStartUI.State.Inactive && (!SceneLoader.Exists || !SceneLoader.IsInIrisTransition) && (!MapEventNotification.Current || !MapEventNotification.Current.showing);
	}

	protected override void Awake()
	{
		base.Awake();
		base.tag = "Player_Map";
		input = GetComponent<PlayerInput>();
		motor = GetComponent<MapPlayerMotor>();
		animationController = GetComponent<MapPlayerAnimationController>();
		ladderManager = GetComponent<MapPlayerLadderManager>();
	}

	protected override void Start()
	{
		base.Start();
		OnEquipMenuOpenedEvent += OnEquipMenuOpened;
		OnEquipMenuClosedEvent += OnEquipMenuClosed;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		OnEquipMenuOpenedEvent -= OnEquipMenuOpened;
		OnEquipMenuClosedEvent -= OnEquipMenuClosed;
	}

	public static MapPlayerController Create(PlayerId playerId, InitObject init)
	{
		MapPlayerController mapPlayerController = UnityEngine.Object.Instantiate(Map.Current.MapResources.mapPlayer);
		mapPlayerController.Init(playerId, init);
		return mapPlayerController;
	}

	private void Init(PlayerId playerId, InitObject init)
	{
		base.gameObject.name = playerId.ToString();
		id = playerId;
		input.Init(id);
		animationController.Init(init.pose);
		base.transform.position = init.position;
		switch (init.pose)
		{
		case MapPlayerPose.Default:
			state = State.Walking;
			break;
		case MapPlayerPose.Joined:
			state = State.Stationary;
			StartCoroutine(joined_cr());
			break;
		case MapPlayerPose.Won:
			state = State.Stationary;
			break;
		default:
			Debug.LogWarning(string.Concat("Pose '", init.pose, "' is not configured"));
			break;
		}
	}

	public void Disable()
	{
		state = State.Stationary;
	}

	public void Enable()
	{
		state = State.Walking;
	}

	public void OnLeave()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnJumpAnimationComplete()
	{
		if (joinedMidGame)
		{
			joinedMidGame = false;
			if (id == PlayerId.PlayerTwo)
			{
				MapPlayerController mapPlayerController = Map.Current.players[0];
				if (mapPlayerController.state != State.Stationary)
				{
					Enable();
				}
			}
			else
			{
				Enable();
			}
		}
		else
		{
			Enable();
		}
	}

	private void OnEquipMenuOpened()
	{
		EquipMenuOpen = true;
	}

	private void OnEquipMenuClosed()
	{
		EquipMenuOpen = false;
	}

	public void LadderEnter(Vector2 point, MapPlayerLadderObject ladder, MapLadder.Location location)
	{
		state = State.LadderEnter;
		if (this.LadderEnterEvent != null)
		{
			this.LadderEnterEvent(point, ladder, location);
		}
	}

	public void LadderExit(Vector2 point, Vector2 exit, MapLadder.Location location)
	{
		state = State.LadderExit;
		if (this.LadderExitEvent != null)
		{
			this.LadderExitEvent(point, exit, location);
		}
	}

	public void LadderEnterComplete()
	{
		state = State.Ladder;
		if (this.LadderEnterCompleteEvent != null)
		{
			this.LadderEnterCompleteEvent();
		}
	}

	public void LadderExitComplete()
	{
		state = State.Walking;
		if (this.LadderExitCompleteEvent != null)
		{
			this.LadderExitCompleteEvent();
		}
	}

	public void OnWinComplete()
	{
		animationController.CompleteJump();
	}

	private IEnumerator joined_cr()
	{
		yield return null;
		joinedMidGame = true;
		animationController.CompleteJump();
	}

	public void JumpSFX()
	{
		AudioManager.Play("complete_bounce");
	}
}
