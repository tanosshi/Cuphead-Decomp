using UnityEngine;

public class FunhousePlatformingLevelExplosionFX : Effect
{
	[SerializeField]
	private Effect smoke;

	[SerializeField]
	private Effect firecracker;

	private void SpawnSmoke()
	{
		smoke.Create(base.transform.position);
	}

	private void FirecrackerLines()
	{
		firecracker.Create(base.transform.position);
	}

	private void MiniExplosion()
	{
		GetComponent<EffectRadius>().CreateInRadius();
	}
}
