using UnityEngine;

public class CutsceneGUI : AbstractMonoBehaviour
{
	public const string PATH = "UI/Cutscene_UI";

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	public CutscenePauseGUI pause;

	[Space(10f)]
	[SerializeField]
	private CupheadUICamera uiCameraPrefab;

	private CupheadUICamera camera;

	public static CutsceneGUI Current { get; private set; }

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
		Current = this;
	}

	protected override void Start()
	{
		base.Start();
		camera = Object.Instantiate(uiCameraPrefab);
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

	public void CutseneInit()
	{
		pause.Init(false);
	}

	protected virtual void CutsceneSnapshot()
	{
		AudioManager.HandleSnapshot(AudioManager.Snapshots.Cutscene.ToString(), 0.15f);
	}
}
