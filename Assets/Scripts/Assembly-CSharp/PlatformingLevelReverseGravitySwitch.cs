using System.Collections;
using UnityEngine;

public class PlatformingLevelReverseGravitySwitch : ParrySwitch
{
	[SerializeField]
	private float spinTimer;

	[SerializeField]
	private float coolDown;

	public override void OnParryPrePause(AbstractPlayerController player)
	{
		base.OnParryPrePause(player);
		GetComponent<Collider2D>().enabled = false;
	}

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		base.OnParryPostPause(player);
		LevelPlayerController levelPlayerController = player as LevelPlayerController;
		levelPlayerController.motor.SetGravityReversed(!levelPlayerController.motor.GravityReversed);
		StartCoroutine(start_spin_cr(levelPlayerController));
		StartCoroutine(timer_cr());
	}

	private IEnumerator timer_cr()
	{
		float t = 0f;
		while (t < coolDown)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		GetComponent<Collider2D>().enabled = true;
		yield return null;
	}

	private IEnumerator start_spin_cr(LevelPlayerController player)
	{
		base.animator.SetBool("IsSpin", true);
		base.animator.SetBool("IsUp", player.motor.GravityReversed);
		float t = 0f;
		while (t < spinTimer)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.animator.SetBool("IsSpin", false);
		yield return null;
	}
}
