using UnityEngine;

public class LevelHUD : AbstractMonoBehaviour
{
	[SerializeField]
	private Canvas canvas;

	[Space(10f)]
	[SerializeField]
	private LevelHUDPlayer cuphead;

	private LevelHUDPlayer levelHudTemplate;

	private LevelHUDPlayer mugman;

	public static LevelHUD Current { get; private set; }

	public Canvas Canvas
	{
		get
		{
			return canvas;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		LevelGUI.DebugOnDisableGuiEvent += OnDisableGUI;
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeave;
		Current = this;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		LevelGUI.DebugOnDisableGuiEvent -= OnDisableGUI;
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeave;
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Start()
	{
		base.Start();
		canvas.worldCamera = CupheadLevelCamera.Current.camera;
	}

	public void LevelInit()
	{
		AbstractPlayerController player = PlayerManager.GetPlayer(PlayerId.PlayerOne);
		levelHudTemplate = Object.Instantiate(cuphead);
		levelHudTemplate.gameObject.SetActive(false);
		if (PlayerManager.Multiplayer)
		{
			AbstractPlayerController player2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			mugman = Object.Instantiate(levelHudTemplate);
			mugman.gameObject.SetActive(true);
			mugman.transform.SetParent(cuphead.transform.parent, false);
			mugman.Init(player2);
		}
		cuphead.Init(player);
	}

	private void OnDisableGUI()
	{
		canvas.enabled = false;
	}

	private void OnPlayerJoined(PlayerId player)
	{
		if (player == PlayerId.PlayerTwo)
		{
			AbstractPlayerController player2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			mugman = Object.Instantiate(levelHudTemplate);
			mugman.gameObject.SetActive(true);
			mugman.transform.SetParent(cuphead.transform.parent, false);
			mugman.Init(player2, true);
		}
	}

	private void OnPlayerLeave(PlayerId player)
	{
		if (player == PlayerId.PlayerTwo)
		{
			Object.Destroy(mugman.gameObject);
		}
	}
}
