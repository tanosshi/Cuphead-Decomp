using System.Collections;
using UnityEngine;

public class TrainLevelPassenger : AbstractPausableComponent
{
	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(main_cr());
	}

	private IEnumerator main_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(3f, 8f));
			base.animator.SetTrigger("Continue");
		}
	}
}
