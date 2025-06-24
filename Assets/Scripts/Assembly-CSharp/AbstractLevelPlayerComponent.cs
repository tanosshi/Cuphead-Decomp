public abstract class AbstractLevelPlayerComponent : AbstractPlayerComponent
{
	public LevelPlayerController player { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		player = base.basePlayer as LevelPlayerController;
	}
}
