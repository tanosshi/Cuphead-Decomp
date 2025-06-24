using UnityEngine;

public class PlatformingLevelGenericExplosion : Effect
{
	[SerializeField]
	private Effect lightningPrefab;

	[SerializeField]
	private Effect starsPrefab;

	[SerializeField]
	private float lightningOnlyChance;

	[SerializeField]
	private float starOnlyChance;

	[SerializeField]
	private float starsPlusLightningChance;

	public override Effect Create(Vector3 position)
	{
		float num = Random.Range(0f, 1f);
		if (num < lightningOnlyChance + starOnlyChance + starsPlusLightningChance)
		{
			if (num < lightningOnlyChance || num > lightningOnlyChance + starOnlyChance)
			{
				lightningPrefab.Create(position);
			}
			if (num > lightningOnlyChance)
			{
				starsPrefab.Create(position);
			}
		}
		return base.Create(position);
	}

	public override Effect Create(Vector3 position, Vector3 scale)
	{
		float num = Random.Range(0f, 1f);
		if (num < lightningOnlyChance + starOnlyChance + starsPlusLightningChance)
		{
			if (num < lightningOnlyChance || num > lightningOnlyChance + starOnlyChance)
			{
				lightningPrefab.Create(position, scale);
			}
			if (num > lightningOnlyChance)
			{
				starsPrefab.Create(position, scale);
			}
		}
		return base.Create(position, scale);
	}
}
