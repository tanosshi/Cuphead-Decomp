public abstract class AbstractArcadePlayerComponent : AbstractPlayerComponent
{
	public ArcadePlayerController player { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		player = base.basePlayer as ArcadePlayerController;
	}
}
