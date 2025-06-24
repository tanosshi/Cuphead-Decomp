using UnityEngine;

public class PlayerLevelSpreadEx : AbstractProjectile
{
	[SerializeField]
	private PlayerLevelSpreadExChild childPrefab;

	public void Init(float speed, float damage, int childCount, float radius)
	{
		float num = 360 / childCount;
		MeterScoreTracker meterScoreTracker = new MeterScoreTracker(MeterScoreTracker.Type.Ex);
		for (int i = 0; i < childCount; i++)
		{
			BasicProjectile projectile = childPrefab.Create(base.transform.position, num * (float)i, Vector2.one, speed);
			childPrefab.Damage = damage;
			childPrefab.Speed = speed;
			childPrefab.PlayerId = PlayerId;
			childPrefab.transform.AddPositionForward2D(radius);
			meterScoreTracker.Add(projectile);
		}
		Object.Destroy(base.gameObject);
	}

	protected override int GetVariants()
	{
		return 1;
	}

	protected override void SetBool(Booleans boolean, bool b)
	{
	}

	protected override void SetInt(Integers integer, int i)
	{
	}

	protected override void SetTrigger(Triggers trigger)
	{
	}
}
