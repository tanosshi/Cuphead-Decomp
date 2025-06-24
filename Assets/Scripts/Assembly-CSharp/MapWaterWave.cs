using System.Collections;
using UnityEngine;

public class MapWaterWave : AbstractPausableComponent
{
	[SerializeField]
	public MinMax offsetRange;

	[SerializeField]
	public MinMax delayRange;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(wave_cr());
	}

	private IEnumerator wave_cr()
	{
		while (true)
		{
			base.animator.Play("Wave", 0, offsetRange.RandomFloat());
			yield return base.animator.WaitForAnimationToEnd(this, "Wave");
			yield return CupheadTime.WaitForSeconds(this, delayRange.RandomFloat());
		}
	}
}
