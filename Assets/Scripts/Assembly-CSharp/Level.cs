using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : AbstractPausableComponent
{
	public enum Type
	{
		Battle = 0,
		Tutorial = 1,
		Platforming = 2
	}

	public enum Mode
	{
		Easy = 0,
		Normal = 1,
		Hard = 2
	}

	[Serializable]
	public class Bounds
	{
		public enum Side
		{
			Left = 0,
			Right = 1,
			Top = 2,
			Bottom = 3
		}

		public int left;

		public int right;

		public int top;

		public int bottom;

		public bool topEnabled = true;

		public bool bottomEnabled = true;

		public bool leftEnabled = true;

		public bool rightEnabled = true;

		public Dictionary<Side, BoxCollider2D> colliders = new Dictionary<Side, BoxCollider2D>();

		public int Width
		{
			get
			{
				return left + right;
			}
		}

		public int Height
		{
			get
			{
				return top + bottom;
			}
		}

		public Vector2 Center
		{
			get
			{
				return new Vector2(right - left, top - bottom) / 2f;
			}
		}

		public Bounds()
		{
			left = 0;
			right = 0;
			top = 0;
			bottom = 0;
		}

		public Bounds(int left, int right, int top, int bottom)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		public void SetColliderPositions()
		{
			Rect rect = new Rect
			{
				xMin = -left,
				xMax = right,
				yMin = -bottom,
				yMax = top
			};
			if (colliders.ContainsKey(Side.Left) && colliders[Side.Left] != null)
			{
				colliders[Side.Left].transform.position = new Vector2(-left - 200, rect.center.y);
			}
			if (colliders.ContainsKey(Side.Right) && colliders[Side.Right] != null)
			{
				colliders[Side.Right].transform.position = new Vector2(right + 200, rect.center.y);
			}
			if (colliders.ContainsKey(Side.Top) && colliders[Side.Top] != null)
			{
				colliders[Side.Top].transform.position = new Vector2(rect.center.x, top + 200);
			}
			if (colliders.ContainsKey(Side.Bottom) && colliders[Side.Bottom] != null)
			{
				colliders[Side.Bottom].transform.position = new Vector2(rect.center.x, -bottom - 200);
			}
		}

		public int GetValue(Side side)
		{
			switch (side)
			{
			default:
				return bottom;
			case Side.Top:
				return top;
			case Side.Left:
				return left;
			case Side.Right:
				return right;
			}
		}

		public void SetValue(Side side, int value)
		{
			switch (side)
			{
			default:
				bottom = value;
				break;
			case Side.Top:
				top = value;
				break;
			case Side.Left:
				left = value;
				break;
			case Side.Right:
				right = value;
				break;
			}
		}

		public bool GetEnabled(Side side)
		{
			switch (side)
			{
			default:
				return bottomEnabled;
			case Side.Top:
				return topEnabled;
			case Side.Left:
				return leftEnabled;
			case Side.Right:
				return rightEnabled;
			}
		}

		public void SetEnabled(Side side, bool value)
		{
			switch (side)
			{
			default:
				bottomEnabled = value;
				break;
			case Side.Top:
				topEnabled = value;
				break;
			case Side.Left:
				leftEnabled = value;
				break;
			case Side.Right:
				rightEnabled = value;
				break;
			}
		}

		public Bounds Copy()
		{
			return MemberwiseClone() as Bounds;
		}
	}

	[Serializable]
	public class Spawns
	{
		public Vector2 playerOne = new Vector2(-460f, 0f);

		public Vector2 playerTwo = new Vector2(-580f, 0f);

		public Vector2 playerOneSingle = new Vector2(-520f, 0f);

		public Vector2 this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return playerOne;
				case 1:
					return playerTwo;
				case 2:
					return playerOneSingle;
				default:
					Debug.LogError("Spawn index '" + i + "' not in range");
					return Vector2.zero;
				}
			}
		}
	}

	[Serializable]
	public class Camera
	{
		public CupheadLevelCamera.Mode mode = CupheadLevelCamera.Mode.Relative;

		[Space(10f)]
		[Range(0.5f, 2f)]
		public float zoom = 1f;

		[Space(10f)]
		public bool moveX;

		public bool moveY;

		public bool stabilizeY;

		public float stabilizePaddingTop = 50f;

		public float stabilizePaddingBottom = 100f;

		[Space(10f)]
		public bool colliders;

		[Space(10f)]
		public Bounds bounds;

		[HideInInspector]
		public VectorPath path;

		public bool pathMovesOnlyForward;

		public Camera(CupheadLevelCamera.Mode mode, int left, int right, int top, int bottom)
		{
			this.mode = mode;
			bounds = new Bounds(left, right, top, bottom);
		}
	}

	public class GoalTimes
	{
		public readonly float easy;

		public readonly float normal;

		public readonly float hard;

		public GoalTimes(float easy, float normal, float hard)
		{
			this.easy = easy;
			this.normal = normal;
			this.hard = hard;
		}
	}

	[Serializable]
	public class IntroProperties
	{
		[NonSerialized]
		public bool introComplete;

		[NonSerialized]
		public bool readyComplete;

		public void OnIntroAnimComplete()
		{
			introComplete = true;
		}

		public void OnReadyAnimComplete()
		{
			readyComplete = true;
		}
	}

	public class Timeline
	{
		public class Event
		{
			public string name { get; private set; }

			public float percentage { get; private set; }

			public Event(string name, float percentage)
			{
				this.name = name;
				this.percentage = percentage;
			}
		}

		public float health;

		public float damage { get; private set; }

		public List<Event> events { get; private set; }

		public float cuphead { get; private set; }

		public float mugman { get; private set; }

		public Timeline()
		{
			health = 0f;
			damage = 0f;
			cuphead = -1f;
			mugman = -1f;
			events = new List<Event>();
		}

		public int GetHealthOfLastEvent()
		{
			int num = 0;
			float num2 = 1f;
			for (int i = 0; i < events.Count; i++)
			{
				if (events[i].percentage < num2)
				{
					num2 = events[i].percentage;
				}
			}
			return (int)(health * (1f - num2));
		}

		public void DealDamage(float damage)
		{
			this.damage += damage;
		}

		public void OnPlayerDeath(PlayerId playerId)
		{
			if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
			{
				cuphead = damage;
			}
			else
			{
				mugman = damage;
			}
		}

		public void OnPlayerRevive(PlayerId playerId)
		{
			if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
			{
				cuphead = -1f;
			}
			else
			{
				mugman = -1f;
			}
		}

		public void SetPlayerDamage(PlayerId playerId, float value)
		{
			if (playerId == PlayerId.PlayerOne || playerId != PlayerId.PlayerTwo)
			{
				cuphead = value;
			}
			else
			{
				mugman = value;
			}
		}

		public void AddEvent(Event e)
		{
			events.Add(e);
		}

		public void AddEventAtHealth(string eventName, int targetHealth)
		{
			float percentage = 1f - (float)targetHealth / health;
			AddEvent(new Event(eventName, percentage));
		}
	}

	private const int BOUND_COLLIDER_SIZE = 400;

	private const float IRIS_NO_INTRO_DELAY = 0.4f;

	private const float IRIS_OPEN_DELAY = 1f;

	public const string GENERIC_STATE_NAME = "Generic";

	public static readonly Levels[] world1BossLevels;

	public static readonly Levels[] world2BossLevels;

	public static readonly Levels[] world3BossLevels;

	public static readonly Levels[] world4BossLevels;

	public static readonly Levels[] platformingLevels;

	public LevelResources LevelResources;

	[SerializeField]
	protected Type type;

	[SerializeField]
	public PlayerMode playerMode;

	[SerializeField]
	protected bool allowMultiplayer = true;

	[SerializeField]
	protected IntroProperties intro;

	[SerializeField]
	protected Spawns spawns;

	[SerializeField]
	protected Bounds bounds = new Bounds(640, 640, 360, 200);

	[SerializeField]
	protected Camera camera = new Camera(CupheadLevelCamera.Mode.Lerp, 640, 640, 360, 360);

	protected LevelGUI gui;

	protected LevelHUD hud;

	protected AbstractPlayerController[] players;

	protected Transform collidersRoot;

	protected GoalTimes goalTimes;

	protected bool waitingForPlayerJoin;

	protected bool isMausoleum;

	protected bool isDevil;

	public int BGMPlaylistCurrent;

	private readonly Vector3 player1PlaneSpawnPos = new Vector3(-550f, 74.3f);

	private readonly Vector3 player2PlaneSpawnPos = new Vector3(-450f, -79.8f);

	private const int PLAYER_DEATH_DELAY = 5;

	public static Level Current { get; private set; }

	public static Mode CurrentMode { get; private set; }

	public static bool PreviouslyWon { get; private set; }

	public static bool Won { get; private set; }

	public static LevelScoringData.Grade Grade { get; private set; }

	public static LevelScoringData.Grade PreviousGrade { get; private set; }

	public static Mode Difficulty { get; private set; }

	public static Mode PreviousDifficulty { get; private set; }

	public static LevelScoringData ScoringData { get; private set; }

	public static Levels PreviousLevel { get; private set; }

	public static Type PreviousLevelType { get; private set; }

	public static bool IsDicePalace { get; protected set; }

	public static bool IsDicePalaceMain { get; protected set; }

	public static bool SuperUnlocked { get; protected set; }

	public static bool OverrideDifficulty { get; protected set; }

	public Mode mode { get; protected set; }

	public bool PlayersCreated { get; private set; }

	public bool Initialized { get; private set; }

	public bool Started { get; private set; }

	public float LevelTime { get; private set; }

	public int Ground
	{
		get
		{
			return -bounds.bottom;
		}
	}

	public int Ceiling
	{
		get
		{
			return bounds.top;
		}
	}

	public int Left
	{
		get
		{
			return -bounds.left;
		}
	}

	public int Right
	{
		get
		{
			return bounds.right;
		}
	}

	public int Width
	{
		get
		{
			return bounds.left + bounds.right;
		}
	}

	public int Height
	{
		get
		{
			return bounds.top + bounds.bottom;
		}
	}

	public Type LevelType
	{
		get
		{
			return type;
		}
	}

	public bool IntroComplete
	{
		get
		{
			return intro.introComplete;
		}
	}

	public Timeline timeline { get; protected set; }

	public abstract Levels CurrentLevel { get; }

	public abstract Scenes CurrentScene { get; }

	public Camera CameraSettings
	{
		get
		{
			return camera;
		}
	}

	public abstract Sprite BossPortrait { get; }

	public abstract string BossQuote { get; }

	public bool Ending { get; private set; }

	protected virtual float LevelIntroTime
	{
		get
		{
			return 1f;
		}
	}

	protected virtual float BossDeathTime
	{
		get
		{
			return 2f;
		}
	}

	public event Action OnLevelStartEvent;

	public event Action OnLevelEndEvent;

	public event Action OnStateChangedEvent;

	public event Action OnWinEvent;

	public event Action OnPreWinEvent;

	public event Action OnLoseEvent;

	public event Action OnPreLoseEvent;

	public event Action OnTransitionInCompleteEvent;

	public event Action OnIntroEvent;

	public event Action OnBossDeathExplosionsEvent;

	public event Action OnBossDeathExplosionsEndEvent;

	public event Action OnBossDeathExplosionsFalloffEvent;

	static Level()
	{
		world1BossLevels = new Levels[5]
		{
			Levels.Veggies,
			Levels.Slime,
			Levels.FlyingBlimp,
			Levels.Flower,
			Levels.Frogs
		};
		world2BossLevels = new Levels[5]
		{
			Levels.Baroness,
			Levels.Clown,
			Levels.FlyingGenie,
			Levels.Dragon,
			Levels.FlyingBird
		};
		world3BossLevels = new Levels[7]
		{
			Levels.Bee,
			Levels.Pirate,
			Levels.SallyStagePlay,
			Levels.Mouse,
			Levels.Robot,
			Levels.FlyingMermaid,
			Levels.Train
		};
		world4BossLevels = new Levels[2]
		{
			Levels.DicePalaceMain,
			Levels.Devil
		};
		platformingLevels = new Levels[6]
		{
			Levels.Platforming_Level_1_1,
			Levels.Platforming_Level_1_2,
			Levels.Platforming_Level_2_1,
			Levels.Platforming_Level_2_2,
			Levels.Platforming_Level_3_1,
			Levels.Platforming_Level_3_2
		};
		CurrentMode = Mode.Normal;
	}

	public static void SetCurrentMode(Mode mode)
	{
		CurrentMode = mode;
	}

	public static void ResetPreviousLevelInfo()
	{
		Won = false;
		SuperUnlocked = false;
	}

	public static string GetLevelName(Levels level)
	{
		return Localization.Translate(level.ToString()).text;
	}

	protected override void Awake()
	{
		base.Awake();
		CheckIfDicePalace();
		Cuphead.Init();
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeave;
		DamageDealer.didDamageWithNonSmallPlaneWeapon = false;
		mode = CurrentMode;
		Current = this;
		PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(CurrentLevel);
		Won = false;
		BGMPlaylistCurrent = levelData.bgmPlayListCurrent;
		PreviousLevel = CurrentLevel;
		PreviousLevelType = type;
		PreviouslyWon = levelData.completed;
		PreviousGrade = levelData.grade;
		PreviousDifficulty = levelData.difficultyBeaten;
		SuperUnlocked = false;
		Ending = false;
		PartialInit();
		Application.targetFrameRate = 60;
		CreateUI();
		CreateHUD();
		LevelCoin.OnLevelStart();
		SceneLoader.SetCurrentLevel(CurrentLevel);
	}

	protected virtual void CheckIfDicePalace()
	{
		IsDicePalace = false;
		IsDicePalaceMain = false;
	}

	protected virtual void PartialInit()
	{
		if (ScoringData == null || ((type == Type.Battle || type == Type.Platforming) && (!IsDicePalace || (IsDicePalaceMain && DicePalaceMainLevelGameInfo.TURN_COUNTER == 0))))
		{
			ScoringData = new LevelScoringData();
			ScoringData.goalTime = ((mode == Mode.Easy) ? goalTimes.easy : ((mode != Mode.Normal) ? goalTimes.hard : goalTimes.normal));
		}
		if (IsDicePalace && !IsDicePalaceMain)
		{
			ScoringData.goalTime += ((mode == Mode.Easy) ? goalTimes.easy : ((mode != Mode.Normal) ? goalTimes.hard : goalTimes.normal));
		}
		ScoringData.difficulty = mode;
	}

	protected override void Start()
	{
		base.Start();
		CupheadTime.SetAll(1f);
		switch (type)
		{
		default:
			StartCoroutine(startBattle_cr());
			break;
		case Type.Tutorial:
			StartCoroutine(startNonBattle_cr());
			break;
		case Type.Platforming:
			StartCoroutine(startPlatforming_cr());
			break;
		}
		CreateAudio();
		CreateColliders();
		CreatePlayers();
		CreateCamera();
		gui.LevelInit();
		hud.LevelInit();
		SetRichPresence();
		DebugConsole.Init();
		Initialized = true;
	}

	protected override void Update()
	{
		base.Update();
		LevelTime += CupheadTime.Delta;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerManager.ClearPlayers();
		Current = null;
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeave;
	}

	public void SetBounds(int? left, int? right, int? top, int? bottom)
	{
		if (left.HasValue)
		{
			bounds.left = left.Value;
		}
		if (right.HasValue)
		{
			bounds.right = right.Value;
		}
		if (top.HasValue)
		{
			bounds.top = top.Value;
		}
		if (bottom.HasValue)
		{
			bounds.bottom = bottom.Value;
		}
		bounds.SetColliderPositions();
	}

	protected void CleanUpScore()
	{
		ScoringData = null;
	}

	private void CreateAudio()
	{
		LevelAudio.Create();
	}

	protected virtual void CreatePlayers()
	{
		PlayersCreated = true;
		AbstractPlayerController[] array = UnityEngine.Object.FindObjectsOfType<AbstractPlayerController>();
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			Debug.LogWarning("Player prefab should not be present in level scene!");
			UnityEngine.Object.Destroy(abstractPlayerController.gameObject);
		}
		players = new AbstractPlayerController[2];
		if (playerMode == PlayerMode.Custom)
		{
			Debug.Log("[Level] PlayerMode is Custom, make sure to override CreatePlayers method to set up custom players.\nMake sure to set PlayersCreated to true");
		}
		else if (PlayerManager.Multiplayer && allowMultiplayer)
		{
			Vector3 vector = ((playerMode != PlayerMode.Plane) ? ((Vector3)spawns.playerOne) : player1PlaneSpawnPos);
			players[0] = AbstractPlayerController.Create(PlayerId.PlayerOne, vector, playerMode);
			players[0].stats.OnPlayerDeathEvent += OnPlayerDeath;
			players[0].stats.OnPlayerReviveEvent += OnPlayerRevive;
			Vector3 vector2 = ((playerMode != PlayerMode.Plane) ? ((Vector3)spawns.playerTwo) : player2PlaneSpawnPos);
			players[1] = AbstractPlayerController.Create(PlayerId.PlayerTwo, vector2, playerMode);
			players[1].stats.OnPlayerDeathEvent += OnPlayerDeath;
			players[1].stats.OnPlayerReviveEvent += OnPlayerRevive;
		}
		else
		{
			Vector3 vector3 = ((playerMode != PlayerMode.Plane) ? ((Vector3)spawns.playerOneSingle) : player1PlaneSpawnPos);
			players[0] = AbstractPlayerController.Create(PlayerId.PlayerOne, vector3, playerMode);
			players[0].stats.OnPlayerDeathEvent += OnPlayerDeath;
			players[0].stats.OnPlayerReviveEvent += OnPlayerRevive;
		}
	}

	protected virtual void CreatePlayerTwoOnJoin()
	{
		if (PlayerManager.Multiplayer && allowMultiplayer)
		{
			players[1] = AbstractPlayerController.Create(PlayerId.PlayerTwo, players[0].center, playerMode);
			players[1].stats.OnPlayerDeathEvent += OnPlayerDeath;
			players[1].stats.OnPlayerReviveEvent += OnPlayerRevive;
			players[1].LevelJoin(players[0].center);
		}
	}

	private void CreateCamera()
	{
		if (players == null)
		{
			Debug.LogError("Level.CreateCamera() must be called AFTER Level.CreatePlayers()");
		}
		CupheadLevelCamera cupheadLevelCamera = UnityEngine.Object.FindObjectOfType<CupheadLevelCamera>();
		cupheadLevelCamera.Init(camera);
	}

	private void CreateUI()
	{
		gui = UnityEngine.Object.FindObjectOfType<LevelGUI>();
		if (gui == null)
		{
			gui = LevelResources.levelGUI.InstantiatePrefab<LevelGUI>();
		}
		else
		{
			Debug.LogWarning("Remove Level_UI from scene!");
		}
	}

	private void CreateHUD()
	{
		hud = UnityEngine.Object.FindObjectOfType<LevelHUD>();
		if (hud == null)
		{
			hud = LevelResources.levelHUD.InstantiatePrefab<LevelHUD>();
		}
		else
		{
			Debug.LogWarning("Remove Level_HUD from scene!");
		}
	}

	private void CreateColliders()
	{
		if (playerMode != PlayerMode.Plane)
		{
			collidersRoot = new GameObject("Colliders").transform;
			collidersRoot.parent = base.transform;
			collidersRoot.ResetLocalTransforms();
			SetupCollider(Bounds.Side.Left);
			SetupCollider(Bounds.Side.Right);
			SetupCollider(Bounds.Side.Top);
			SetupCollider(Bounds.Side.Bottom);
		}
	}

	private Transform SetupCollider(Bounds.Side side)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		int layer = 0;
		int num = 0;
		Vector2 zero = Vector2.zero;
		switch (side)
		{
		case Bounds.Side.Left:
			text = "Level_Wall_Left";
			text2 = Tags.Wall.ToString();
			layer = LayerMask.NameToLayer(Layers.Bounds_Walls.ToString());
			num = 90;
			break;
		case Bounds.Side.Right:
			text = "Level_Wall_Right";
			text2 = Tags.Wall.ToString();
			layer = LayerMask.NameToLayer(Layers.Bounds_Walls.ToString());
			num = -90;
			break;
		case Bounds.Side.Top:
			text = "Level_Ceiling";
			text2 = Tags.Ceiling.ToString();
			layer = LayerMask.NameToLayer(Layers.Bounds_Ceiling.ToString());
			break;
		case Bounds.Side.Bottom:
			text = "Level_Ground";
			text2 = Tags.Ground.ToString();
			layer = LayerMask.NameToLayer(Layers.Bounds_Ground.ToString());
			num = 180;
			break;
		}
		GameObject gameObject = new GameObject(text);
		gameObject.tag = text2;
		gameObject.layer = layer;
		gameObject.transform.ResetLocalTransforms();
		gameObject.transform.SetPosition(zero.x, zero.y);
		gameObject.transform.SetEulerAngles(null, null, num);
		gameObject.transform.parent = collidersRoot;
		BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
		boxCollider2D.isTrigger = true;
		boxCollider2D.size = new Vector2(10000f, 400f);
		Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
		rigidbody2D.gravityScale = 0f;
		rigidbody2D.drag = 0f;
		rigidbody2D.angularDrag = 0f;
		rigidbody2D.isKinematic = true;
		bounds.colliders.Add(side, boxCollider2D);
		bounds.SetColliderPositions();
		gameObject.SetActive(bounds.GetEnabled(side));
		return gameObject.transform;
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(spawns.playerOne, 20f);
		Gizmos.DrawSphere(spawns.playerTwo, 20f);
		Gizmos.DrawSphere(spawns.playerOneSingle, 30f);
		Gizmos.color = Color.red;
		Gizmos.DrawCube(spawns.playerOneSingle, new Vector3(20f, 20f, 20f));
		Gizmos.DrawWireSphere(spawns.playerOne, 20f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(spawns.playerTwo, 20f);
		Gizmos.color = Color.white;
		if (camera.bounds.topEnabled)
		{
			Gizmos.DrawLine(new Vector3(camera.bounds.right, camera.bounds.top, 0f), new Vector3(-camera.bounds.left, camera.bounds.top, 0f));
		}
		if (camera.bounds.bottomEnabled)
		{
			Gizmos.DrawLine(new Vector3(camera.bounds.right, -camera.bounds.bottom, 0f), new Vector3(-camera.bounds.left, -camera.bounds.bottom, 0f));
		}
		if (camera.bounds.leftEnabled)
		{
			Gizmos.DrawLine(new Vector3(-camera.bounds.left, camera.bounds.top, 0f), new Vector3(-camera.bounds.left, -camera.bounds.bottom, 0f));
		}
		if (camera.bounds.rightEnabled)
		{
			Gizmos.DrawLine(new Vector3(camera.bounds.right, camera.bounds.top, 0f), new Vector3(camera.bounds.right, -camera.bounds.bottom, 0f));
		}
		if (bounds.topEnabled)
		{
			Gizmos.color = Color.blue;
		}
		else
		{
			Gizmos.color = Color.black;
		}
		Gizmos.DrawLine(new Vector3(bounds.right, bounds.top, 0f), new Vector3(-bounds.left, bounds.top, 0f));
		if (bounds.bottomEnabled)
		{
			Gizmos.color = Color.green;
		}
		else
		{
			Gizmos.color = Color.black;
		}
		Gizmos.DrawLine(new Vector3(bounds.right, -bounds.bottom, 0f), new Vector3(-bounds.left, -bounds.bottom, 0f));
		if (bounds.leftEnabled)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = Color.black;
		}
		Gizmos.DrawLine(new Vector3(-bounds.left, bounds.top, 0f), new Vector3(-bounds.left, -bounds.bottom, 0f));
		if (bounds.rightEnabled)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = Color.black;
		}
		Gizmos.DrawLine(new Vector3(bounds.right, bounds.top, 0f), new Vector3(bounds.right, -bounds.bottom, 0f));
	}

	private void OnPlayerJoined(PlayerId playerId)
	{
		LevelNewPlayerGUI.Current.Init();
		CreatePlayerTwoOnJoin();
		SetRichPresence();
	}

	private void OnPlayerLeave(PlayerId playerId)
	{
		if (playerId == PlayerId.PlayerTwo)
		{
			AbstractPlayerController player = PlayerManager.GetPlayer(playerId);
			if (player != null)
			{
				player.OnLeave(playerId);
			}
			if (PlayerManager.GetPlayer(PlayerId.PlayerOne).IsDead)
			{
				_OnLose();
			}
		}
	}

	private void SetRichPresence()
	{
		if (CurrentLevel != Levels.Mausoleum)
		{
			switch (type)
			{
			case Type.Battle:
				OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "Boss", SceneLoader.SceneName);
				OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "Fighting", true);
				break;
			case Type.Platforming:
				OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "PlatformingLevel", SceneLoader.SceneName);
				OnlineManager.Instance.Interface.SetRichPresence(PlayerId.Any, "Playing", true);
				break;
			}
		}
	}

	private void OnPlayerDeath(PlayerId playerId)
	{
		if (timeline != null && LevelType != Type.Platforming)
		{
			timeline.OnPlayerDeath(playerId);
		}
		StartCoroutine(playerDeath_cr());
	}

	private void OnPlayerRevive(PlayerId playerId)
	{
		timeline.OnPlayerRevive(playerId);
	}

	private void _OnLevelStart()
	{
		OnLevelStart();
		if (this.OnLevelStartEvent != null)
		{
			this.OnLevelStartEvent();
		}
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, true, true);
		InterruptingPrompt.SetCanInterrupt(true);
		PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(CurrentLevel);
		if (levelData != null)
		{
			levelData.played = true;
		}
	}

	private void _OnLevelEnd()
	{
		Ending = true;
		OnLevelEnd();
		if (this.OnLevelEndEvent != null)
		{
			this.OnLevelEndEvent();
		}
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		PlayerManager.ClearJoinPrompt();
	}

	protected void zHack_OnStateChanged()
	{
		OnStateChanged();
		if (this.OnStateChangedEvent != null)
		{
			this.OnStateChangedEvent();
		}
	}

	protected void zHack_OnWin()
	{
		Won = true;
		Difficulty = mode;
		Debug.Log("[Level] Level Won! Saving LevelDataObject");
		PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(CurrentLevel);
		levelData.completed = true;
		ScoringData.time += LevelTime;
		if ((type == Type.Battle || type == Type.Platforming) && (!IsDicePalace || IsDicePalaceMain))
		{
			Grade = ScoringData.CalculateGrade();
			float time = ScoringData.time;
			if (Difficulty > PreviousDifficulty || !PreviouslyWon)
			{
				levelData.difficultyBeaten = Difficulty;
			}
			if (Grade > PreviousGrade || !PreviouslyWon)
			{
				levelData.grade = Grade;
				levelData.bestTime = time;
			}
			else if (Grade == PreviousGrade && time < levelData.bestTime)
			{
				levelData.bestTime = time;
			}
		}
		if (CurrentLevel == Levels.Devil)
		{
			PlayerData.Data.IsHardModeAvailable = true;
		}
		_OnLevelEnd();
		_OnPreWin();
		if (LevelType == Type.Battle)
		{
			StartCoroutine(bossDeath_cr());
		}
		OnWin();
		if (this.OnWinEvent != null)
		{
			this.OnWinEvent();
		}
		PlayerData.SaveCurrentFile();
		Scenes currentMap = PlayerData.Data.CurrentMap;
		Levels[] levels;
		string text;
		switch (currentMap)
		{
		case Scenes.scene_map_world_1:
			levels = world1BossLevels;
			text = "World1";
			break;
		case Scenes.scene_map_world_2:
			levels = world2BossLevels;
			text = "World2";
			break;
		case Scenes.scene_map_world_3:
			levels = world3BossLevels;
			text = "World3";
			break;
		default:
			levels = world4BossLevels;
			text = "World4";
			break;
		}
		if (currentMap == Scenes.scene_map_world_4)
		{
			if (CurrentLevel == Levels.DicePalaceMain)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "CompleteDicePalace");
			}
			else if (CurrentLevel == Levels.Devil)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "CompleteDevil");
			}
		}
		else if (type == Type.Battle && PlayerData.Data.CheckLevelsCompleted(levels))
		{
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "Complete" + text);
		}
		if (type == Type.Battle && Difficulty == Mode.Hard && PlayerData.Data.CheckLevelsHaveMinDifficulty(world1BossLevels, Mode.Hard) && PlayerData.Data.CheckLevelsHaveMinDifficulty(world2BossLevels, Mode.Hard) && PlayerData.Data.CheckLevelsHaveMinDifficulty(world3BossLevels, Mode.Hard) && PlayerData.Data.CheckLevelsHaveMinDifficulty(world4BossLevels, Mode.Hard))
		{
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "NewGamePlus");
		}
		if (type == Type.Battle && Grade >= LevelScoringData.Grade.AMinus && PlayerData.Data.CheckLevelsHaveMinGrade(levels, LevelScoringData.Grade.AMinus))
		{
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "ARank" + text);
		}
		if (type == Type.Platforming && !isMausoleum && ScoringData.pacifistRun && PlayerData.Data.CheckLevelsHaveMinGrade(platformingLevels, LevelScoringData.Grade.P))
		{
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "PacifistRun");
		}
		if ((type == Type.Battle || type == Type.Platforming) && (!IsDicePalace || IsDicePalaceMain) && !isMausoleum && ScoringData.numTimesHit == 0)
		{
			if (IsDicePalaceMain)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "NoHitsTakenDicePalace");
			}
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "NoHitsTaken");
		}
		if (type == Type.Battle)
		{
			if (DamageDealer.lastPlayerDamageSource == DamageDealer.DamageSource.Super)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(DamageDealer.lastPlayer, "SuperWin");
			}
			if (DamageDealer.lastPlayerDamageSource == DamageDealer.DamageSource.Ex)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(DamageDealer.lastPlayer, "ExWin");
			}
			if (playerMode == PlayerMode.Plane && !DamageDealer.didDamageWithNonSmallPlaneWeapon)
			{
				OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "SmallPlaneOnlyWin");
			}
		}
		int num = 0;
		int num2 = 0;
		List<Levels> list = new List<Levels>(world1BossLevels);
		list.AddRange(world2BossLevels);
		list.AddRange(world3BossLevels);
		foreach (Levels item in list)
		{
			PlayerData.PlayerLevelDataObject levelData2 = PlayerData.Data.GetLevelData(item);
			if (levelData2.completed && levelData2.difficultyBeaten >= Mode.Normal)
			{
				num2++;
			}
		}
		List<Levels> list2 = new List<Levels>(world1BossLevels);
		list2.AddRange(world2BossLevels);
		list2.AddRange(world3BossLevels);
		list2.AddRange(world4BossLevels);
		list2.AddRange(platformingLevels);
		foreach (Levels item2 in list2)
		{
			PlayerData.PlayerLevelDataObject levelData3 = PlayerData.Data.GetLevelData(item2);
			if (levelData3.completed && levelData3.grade >= LevelScoringData.Grade.AMinus)
			{
				num++;
			}
		}
		if (type == Type.Battle && CurrentLevel != Levels.Mausoleum)
		{
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "DefeatBoss");
		}
		if (Grade == LevelScoringData.Grade.S && CurrentLevel != Levels.Mausoleum)
		{
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "SRank");
		}
		OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "ARanks", num);
		OnlineManager.Instance.Interface.SetStat(PlayerId.Any, "BossesDefeatedNormal", num2);
		OnlineManager.Instance.Interface.SyncAchievementsAndStats();
		InterruptingPrompt.SetCanInterrupt(false);
	}

	private void _OnPreWin()
	{
		OnPreWin();
		if (this.OnPreWinEvent != null)
		{
			this.OnPreWinEvent();
		}
	}

	protected void _OnLose()
	{
		_OnLevelEnd();
		_OnPreLose();
		OnLose();
		if (this.OnLoseEvent != null)
		{
			this.OnLoseEvent();
		}
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		LevelEnd.Lose();
		PlayerData.SaveCurrentFile();
	}

	private void _OnPreLose()
	{
		OnPreLose();
		if (this.OnPreLoseEvent != null)
		{
			this.OnPreLoseEvent();
		}
	}

	private void _OnTransitionInComplete()
	{
		OnTransitionInComplete();
		if (this.OnTransitionInCompleteEvent != null)
		{
			this.OnTransitionInCompleteEvent();
		}
	}

	private void OnStartExplosions()
	{
		if (this.OnBossDeathExplosionsEvent != null)
		{
			this.OnBossDeathExplosionsEvent();
		}
	}

	private void OnEndExplosions()
	{
		if (this.OnBossDeathExplosionsEndEvent != null)
		{
			this.OnBossDeathExplosionsEndEvent();
		}
	}

	private void OnFalloffExplosions()
	{
		if (this.OnBossDeathExplosionsFalloffEvent != null)
		{
			this.OnBossDeathExplosionsFalloffEvent();
		}
	}

	protected virtual void OnLevelStart()
	{
	}

	protected virtual void OnStateChanged()
	{
	}

	protected virtual void OnWin()
	{
	}

	protected virtual void OnPreWin()
	{
	}

	protected virtual void OnLose()
	{
	}

	protected virtual void OnPreLose()
	{
	}

	protected virtual void OnTransitionInComplete()
	{
	}

	protected virtual void OnBossDeath()
	{
		Debug.LogWarning("Override this to handle boss death animation! Don't forget to override BossDeathTime too!");
	}

	protected virtual IEnumerator startBattle_cr()
	{
		LevelIntroAnimation introAnim = LevelIntroAnimation.Create(intro.OnReadyAnimComplete);
		yield return new WaitForSeconds(0.4f + SceneLoader.EndTransitionDelay);
		if (!IsDicePalaceMain)
		{
			AudioManager.Play(Sfx.Level_Announcer_Ready);
			AudioManager.Play("level_bell_intro");
		}
		yield return new WaitForSeconds(0.25f);
		if (players[0] != null)
		{
			players[0].PlayIntro();
		}
		if (players[1] != null)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.7f);
			players[1].PlayIntro();
		}
		yield return new WaitForSeconds(0.25f);
		_OnTransitionInComplete();
		if (this.OnIntroEvent != null)
		{
			this.OnIntroEvent();
		}
		this.OnIntroEvent = null;
		yield return new WaitForSeconds(LevelIntroTime);
		if (!IsDicePalaceMain)
		{
			introAnim.Play();
			while (!intro.readyComplete)
			{
				yield return null;
			}
			AudioManager.Play(Sfx.Level_Announcer_Begin);
		}
		else
		{
			yield return CupheadTime.WaitForSeconds(this, 1.5f);
		}
		AbstractPlayerController[] array = players;
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			if (!(abstractPlayerController == null))
			{
				abstractPlayerController.LevelStart();
			}
		}
		Started = true;
		_OnLevelStart();
	}

	protected virtual IEnumerator startPlatforming_cr()
	{
		PlatformingLevelIntroAnimation introAnim = PlatformingLevelIntroAnimation.Create(intro.OnReadyAnimComplete);
		yield return new WaitForSeconds(0.4f + SceneLoader.EndTransitionDelay);
		_OnTransitionInComplete();
		if (this.OnIntroEvent != null)
		{
			this.OnIntroEvent();
		}
		this.OnIntroEvent = null;
		introAnim.Play();
		AudioManager.Play("level_announcer_begin");
		while (!intro.readyComplete)
		{
			yield return null;
		}
		AbstractPlayerController[] array = players;
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			if (!(abstractPlayerController == null))
			{
				abstractPlayerController.LevelStart();
			}
		}
		Started = true;
		_OnLevelStart();
	}

	protected virtual IEnumerator startNonBattle_cr()
	{
		LevelIntroAnimation.Create(intro.OnReadyAnimComplete);
		yield return new WaitForSeconds(0.4f + SceneLoader.EndTransitionDelay - 0.25f);
		if (playerMode == PlayerMode.Plane)
		{
			yield return new WaitForSeconds(0.5f);
			if (players[0] != null)
			{
				players[0].PlayIntro();
			}
			if (players[1] != null)
			{
				yield return CupheadTime.WaitForSeconds(this, 0.7f);
				players[1].PlayIntro();
			}
			yield return new WaitForSeconds(0.25f);
		}
		_OnTransitionInComplete();
		if (this.OnIntroEvent != null)
		{
			this.OnIntroEvent();
		}
		this.OnIntroEvent = null;
		if (playerMode == PlayerMode.Plane)
		{
			yield return new WaitForSeconds(1.25f);
		}
		AbstractPlayerController[] array = players;
		foreach (AbstractPlayerController abstractPlayerController in array)
		{
			if (!(abstractPlayerController == null))
			{
				abstractPlayerController.LevelStart();
			}
		}
		Started = true;
		_OnLevelStart();
	}

	protected virtual IEnumerator bossDeath_cr()
	{
		LevelEnd.Win(OnBossDeath, OnStartExplosions, OnFalloffExplosions, OnEndExplosions, players, BossDeathTime, (type == Type.Battle || type == Type.Platforming) && (!IsDicePalace || IsDicePalaceMain) && !isMausoleum, isMausoleum, isDevil);
		yield return null;
	}

	protected IEnumerator playerDeath_cr()
	{
		for (int i = 0; i < 5; i++)
		{
			yield return null;
		}
		if (PlayerManager.Multiplayer)
		{
			if (players[0].IsDead && players[1].IsDead)
			{
				_OnLose();
			}
		}
		else
		{
			_OnLose();
		}
		yield return null;
	}
}
