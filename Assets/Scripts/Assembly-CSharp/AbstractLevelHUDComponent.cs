public class AbstractLevelHUDComponent : AbstractMonoBehaviour
{
	protected bool _parentToHudCanvas;

	protected LevelHUDPlayer _hud { get; private set; }

	protected AbstractPlayerController _player
	{
		get
		{
			return _hud.player;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		ignoreGlobalTime = true;
		timeLayer = CupheadTime.Layer.UI;
	}

	protected override void Start()
	{
		base.Start();
		if (_parentToHudCanvas)
		{
			base.transform.SetParent(LevelHUD.Current.Canvas.transform, false);
		}
	}

	public virtual void Init(LevelHUDPlayer hud)
	{
		_hud = hud;
	}
}
