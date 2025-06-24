using UnityEngine;

public class FlyingMermaidLevelEelBullet : BasicProjectile
{
	[SerializeField]
	private Transform spark;

	private void RotateSpark()
	{
		spark.transform.SetEulerAngles(null, null, Random.Range(0, 360));
	}
}
