using System.Collections;
using UnityEngine;

public class ForestPlatformingLevelMushroomProjectile : BasicProjectile
{
	public static int numUntilPink;

	[SerializeField]
	private Effect trailPrefab;

	[SerializeField]
	private MinMax trailPeriod;

	[SerializeField]
	private float trailMaxOffset;

	[SerializeField]
	private Transform trailRoot;

	protected override float DestroyLifetime
	{
		get
		{
			return -1f;
		}
	}

	protected override void Start()
	{
		base.Start();
		numUntilPink--;
		DestroyDistance = -1f;
		if (numUntilPink == 0)
		{
			numUntilPink = EnemyDatabase.GetProperties(EnemyID.mushroom).MushroomPinkNumber.RandomInt();
			SetInt(Integers.Variant, 1);
			SetParryable(true);
		}
		else
		{
			SetInt(Integers.Variant, 0);
			SetParryable(false);
		}
		StartCoroutine(trail_cr());
	}

	private IEnumerator trail_cr()
	{
		while (!base.dead)
		{
			yield return CupheadTime.WaitForSeconds(this, trailPeriod.RandomFloat());
			trailPrefab.Create((Vector2)trailRoot.position + trailMaxOffset * MathUtils.RandomPointInUnitCircle());
		}
	}

	public override void OnParryDie()
	{
		base.OnParryDie();
		Object.Destroy(this);
	}
}
