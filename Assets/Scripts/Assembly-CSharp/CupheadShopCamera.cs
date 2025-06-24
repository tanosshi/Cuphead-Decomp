public class CupheadShopCamera : AbstractCupheadGameCamera
{
	public static CupheadShopCamera Current { get; private set; }

	public override float OrthographicSize
	{
		get
		{
			return 360f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}
}
