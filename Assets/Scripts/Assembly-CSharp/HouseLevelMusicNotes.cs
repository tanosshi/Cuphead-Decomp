using UnityEngine;

public class HouseLevelMusicNotes : AbstractPausableComponent
{
	private void ChangeAnimation()
	{
		base.animator.SetInteger("Type", Random.Range(0, 4));
	}
}
