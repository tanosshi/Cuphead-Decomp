public class DummyPlmInterface : PlmInterface
{
	public event OnSuspendHandler OnSuspend;

	public event OnResumeHandler OnResume;

	public event OnConstrainedHandler OnConstrained;

	public event OnUnconstrainedHandler OnUnconstrained;

	public void Init()
	{
	}

	public bool IsConstrained()
	{
		return false;
	}
}
