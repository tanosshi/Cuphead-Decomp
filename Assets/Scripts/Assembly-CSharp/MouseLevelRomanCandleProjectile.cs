using UnityEngine;

public class MouseLevelRomanCandleProjectile : HomingProjectile
{
	protected override void Die()
	{
		base.Die();
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
