using System.Collections;
using UnityEngine;

public class FlyingBirdLevelHeartProjectile : BasicProjectile
{
	[SerializeField]
	private Effect FX;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(spawn_fx_cr());
	}

	private IEnumerator spawn_fx_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.17f);
		while (true)
		{
			FX.Create(base.transform.position).transform.SetEulerAngles(null, null, base.transform.eulerAngles.z);
			yield return CupheadTime.WaitForSeconds(this, 0.2f);
		}
	}
}
