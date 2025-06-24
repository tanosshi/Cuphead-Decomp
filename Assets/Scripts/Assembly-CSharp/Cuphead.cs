using Rewired;
using Rewired.UI.ControlMapper;
using UnityEngine;

public class Cuphead : AbstractMonoBehaviour
{
	private const string PATH = "Core/CupheadCore";

	private static bool didLightInit;

	private static bool didFullInit;

	[SerializeField]
	private AudioNoiseHandler noiseHandler;

	[SerializeField]
	private InputManager rewired;

	public ControlMapper controlMapper;

	[SerializeField]
	private CupheadEventSystem eventSystem;

	[SerializeField]
	private CupheadRenderer renderer;

	[SerializeField]
	private ScoringEditorData scoringProperties;

	public static Cuphead Current { get; private set; }

	public ScoringEditorData ScoringProperties
	{
		get
		{
			return scoringProperties;
		}
	}

	public static void Init(bool lightInit = false)
	{
		if (Current == null)
		{
			Object.Instantiate(Resources.Load<Cuphead>("Core/CupheadCore"));
		}
		else
		{
			if (!didLightInit)
			{
				return;
			}
			didLightInit = false;
		}
		if (lightInit)
		{
			didLightInit = true;
			return;
		}
		Current.rewired.gameObject.SetActive(true);
		Current.eventSystem.gameObject.SetActive(true);
		Current.controlMapper.gameObject.SetActive(true);
		PlayerManager.Awake();
		OnlineManager.Instance.Init();
		PlmManager.Instance.Init();
		PlayerManager.Init();
		didFullInit = true;
	}

	protected override void Awake()
	{
		base.Awake();
		if (Current == null)
		{
			Current = this;
			base.gameObject.name = base.gameObject.name.Replace("(Clone)", string.Empty);
			Object.DontDestroyOnLoad(base.gameObject);
			noiseHandler = Object.Instantiate(noiseHandler);
			noiseHandler.transform.SetParent(base.transform);
			bool hasBootedUpGame = SettingsData.Data.hasBootedUpGame;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (didFullInit)
		{
			PlayerManager.Update();
		}
		Cursor.visible = !Screen.fullScreen;
	}
}
