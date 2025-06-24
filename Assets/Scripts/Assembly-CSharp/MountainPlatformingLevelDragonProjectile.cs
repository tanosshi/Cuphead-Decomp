using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelDragonProjectile : BasicProjectile
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
		if (numUntilPink <= 0)
		{
			numUntilPink = EnemyDatabase.GetProperties(EnemyID.dragon).MushroomPinkNumber.RandomInt();
			SetParryable(true);
		}
		else
		{
			SetParryable(false);
		}
		StartCoroutine(trail_cr());
	}

	private IEnumerator trail_cr()
	{
		while (!base.dead)
		{
			yield return CupheadTime.WaitForSeconds(this, trailPeriod.RandomFloat());
			Effect effect = trailPrefab.Create((Vector2)trailRoot.position + trailMaxOffset * MathUtils.RandomPointInUnitCircle());
			effect.animator.Play("PuffA");
		}
	}

	public override void OnParryDie()
	{
		base.OnParryDie();
		Object.Destroy(this);
	}
}
