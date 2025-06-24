public class FunhousePlatformingLevelTubaProjectile : BasicProjectile
{
	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return true;
		}
	}

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		DestroyDistance = 0f;
	}
}
