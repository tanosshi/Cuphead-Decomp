using UnityEngine;

public class MountainPlatformingLevelScalePart : AbstractCollidableObject
{
	public bool steppedOn;

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if (hit.GetComponent<AbstractPlayerController>() != null)
		{
			if (phase == CollisionPhase.Exit)
			{
				steppedOn = false;
			}
			else
			{
				steppedOn = true;
			}
		}
	}
}
