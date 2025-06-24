public abstract class AbstractPlanePlayerComponent : AbstractPlayerComponent
{
	public PlanePlayerController player { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		player = GetComponent<PlanePlayerController>();
	}
}
