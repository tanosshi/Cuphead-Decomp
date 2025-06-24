public interface PlmInterface
{
	event OnSuspendHandler OnSuspend;

	event OnResumeHandler OnResume;

	event OnConstrainedHandler OnConstrained;

	event OnUnconstrainedHandler OnUnconstrained;

	void Init();

	bool IsConstrained();
}
