using UnityEngine;

public class AirshipStorkLevelProjectile : BasicProjectile
{
	private int direction;

	private float rotationSpeed;

	private Transform rotationBase;

	public virtual BasicProjectile Create(Vector2 position, float rotation, float speed, float rotationSpeed, int direction)
	{
		AirshipStorkLevelProjectile airshipStorkLevelProjectile = base.Create(position, rotation, speed) as AirshipStorkLevelProjectile;
		airshipStorkLevelProjectile.rotationSpeed = rotationSpeed;
		airshipStorkLevelProjectile.rotationBase = new GameObject("SpiralProjectileBase").transform;
		airshipStorkLevelProjectile.rotationBase.position = position;
		airshipStorkLevelProjectile.transform.parent = airshipStorkLevelProjectile.rotationBase;
		airshipStorkLevelProjectile.direction = direction;
		return airshipStorkLevelProjectile;
	}

	protected override void Move()
	{
		float num = 360f;
		if (direction == 1)
		{
			num = -360f;
		}
		else if (direction == 2)
		{
			num = 360f;
		}
		if (Speed == 0f)
		{
			Debug.Log("[Basic Projectile] Speed is 0", base.gameObject);
		}
		base.transform.localPosition += rotationBase.InverseTransformDirection(base.transform.right) * Speed * CupheadTime.FixedDelta;
		rotationBase.AddEulerAngles(0f, 0f, rotationSpeed * num * CupheadTime.FixedDelta);
		if (base.transform.position.y < -360f || base.transform.position.y > 360f)
		{
			Die();
		}
	}

	protected override void Die()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		base.Die();
	}
}
