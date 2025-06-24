public class RobotLevelRobotAnimator : AbstractPausableComponent
{
	private void ContinueMainAnimation()
	{
		base.animator.SetTrigger("StartMainAnim");
	}

	private void SyncAnimationLayers()
	{
		base.animator.SetTrigger("SyncLayers");
	}

	private void MainAnimationStateOff()
	{
		base.animator.SetBool("MainAnimationActive", false);
	}

	private void MainAnimationStateOn()
	{
		base.animator.SetBool("MainAnimationActive", true);
	}
}
