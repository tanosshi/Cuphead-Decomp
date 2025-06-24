using System;
using UnityEngine;

public class LevelGUI : AbstractMonoBehaviour
{
	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private LevelPauseGUI pause;

	[SerializeField]
	private LevelGameOverGUI gameOver;

	[SerializeField]
	private OptionsGUI optionsPrefab;

	[SerializeField]
	private RectTransform optionsRoot;

	private OptionsGUI options;

	[Space(10f)]
	[SerializeField]
	private CupheadUICamera uiCameraPrefab;

	private CupheadUICamera camera;

	public static LevelGUI Current { get; private set; }

	public Canvas Canvas
	{
		get
		{
			return canvas;
		}
	}

	public static event Action DebugOnDisableGuiEvent;

	public static void DebugDisableGUI()
	{
		if (LevelGUI.DebugOnDisableGuiEvent != null)
		{
			LevelGUI.DebugOnDisableGuiEvent();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
	}

	protected override void Start()
	{
		base.Start();
		camera = UnityEngine.Object.Instantiate(uiCameraPrefab);
		camera.transform.SetParent(base.transform);
		camera.transform.ResetLocalTransforms();
		canvas.worldCamera = camera.camera;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	public void LevelInit()
	{
		options = optionsPrefab.InstantiatePrefab<OptionsGUI>();
		options.rectTransform.SetParent(optionsRoot, false);
		pause.Init(true, options);
	}
}
