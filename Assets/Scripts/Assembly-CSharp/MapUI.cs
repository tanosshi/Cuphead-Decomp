using System.Collections;
using UnityEngine;

public class MapUI : AbstractMonoBehaviour
{
	[SerializeField]
	private MapPauseUI pauseUI;

	[SerializeField]
	private MapEquipUI equipUI;

	[SerializeField]
	private OptionsGUI optionsPrefab;

	private OptionsGUI optionsUI;

	[SerializeField]
	private RectTransform optionsRoot;

	[Space(10f)]
	[SerializeField]
	public Canvas sceneCanvas;

	[SerializeField]
	public Canvas screenCanvas;

	[SerializeField]
	public Canvas hudCanvas;

	[Space(10f)]
	[SerializeField]
	private CupheadUICamera uiCameraPrefab;

	private CupheadUICamera camera;

	public static MapUI Current { get; private set; }

	public static MapUI Create()
	{
		return Object.Instantiate(Map.Current.MapResources.mapUI);
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		CupheadEventSystem.Init();
		LevelGUI.DebugOnDisableGuiEvent += OnDisableGUI;
	}

	protected override void Start()
	{
		base.Start();
		camera = Object.Instantiate(uiCameraPrefab);
		camera.transform.SetParent(base.transform);
		camera.transform.ResetLocalTransforms();
		screenCanvas.worldCamera = camera.camera;
		sceneCanvas.worldCamera = CupheadMapCamera.Current.camera;
		hudCanvas.worldCamera = CupheadMapCamera.Current.camera;
		StartCoroutine(HandleReturnToMapTooltipEvents());
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		LevelGUI.DebugOnDisableGuiEvent -= OnDisableGUI;
		if (Current == this)
		{
			Current = null;
		}
	}

	public void Init(MapPlayerController[] players)
	{
		optionsUI = optionsPrefab.InstantiatePrefab<OptionsGUI>();
		optionsUI.rectTransform.SetParent(optionsRoot, false);
		pauseUI.Init(false, optionsUI);
		equipUI.Init(false);
	}

	private void OnDisableGUI()
	{
		hudCanvas.enabled = false;
	}

	public void Refresh()
	{
		optionsUI.SetupButtons();
	}

	protected override void Update()
	{
		base.Update();
		if (!MapEventNotification.Current.showing && MapEventNotification.Current.EventQueue.Count > 0)
		{
			MapEventNotification.Current.EventQueue.Dequeue()();
		}
	}

	private IEnumerator HandleReturnToMapTooltipEvents()
	{
		yield return new WaitForSeconds(1f);
		if (PlayerData.Data.shouldShowShopkeepTooltip)
		{
			MapEventNotification.Current.ShowTooltipEvent(TooltipEvent.ShopKeep);
			PlayerData.Data.shouldShowShopkeepTooltip = false;
			PlayerData.SaveCurrentFile();
		}
		if (PlayerData.Data.shouldShowTurtleTooltip)
		{
			MapEventNotification.Current.ShowTooltipEvent(TooltipEvent.Turtle);
			PlayerData.Data.shouldShowTurtleTooltip = false;
			PlayerData.SaveCurrentFile();
		}
		if (PlayerData.Data.shouldShowForkTooltip)
		{
			MapEventNotification.Current.ShowTooltipEvent(TooltipEvent.Professional);
			PlayerData.Data.shouldShowForkTooltip = false;
			PlayerData.SaveCurrentFile();
		}
	}
}
