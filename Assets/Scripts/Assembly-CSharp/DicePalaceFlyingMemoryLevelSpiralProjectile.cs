using UnityEngine;

public class DicePalaceFlyingMemoryLevelSpiralProjectile : BasicProjectile
{
	private int direction;

	private float rotationSpeed;

	private Transform rotationBase;

	public virtual BasicProjectile Create(Vector2 position, float rotation, float speed, float rotationSpeed, int direction)
	{
		DicePalaceFlyingMemoryLevelSpiralProjectile dicePalaceFlyingMemoryLevelSpiralProjectile = base.Create(position, rotation, speed) as DicePalaceFlyingMemoryLevelSpiralProjectile;
		dicePalaceFlyingMemoryLevelSpiralProjectile.rotationSpeed = rotationSpeed;
		dicePalaceFlyingMemoryLevelSpiralProjectile.rotationBase = new GameObject("SpiralProjectileBase").transform;
		dicePalaceFlyingMemoryLevelSpiralProjectile.rotationBase.position = position;
		dicePalaceFlyingMemoryLevelSpiralProjectile.transform.parent = dicePalaceFlyingMemoryLevelSpiralProjectile.rotationBase;
		dicePalaceFlyingMemoryLevelSpiralProjectile.direction = direction;
		return dicePalaceFlyingMemoryLevelSpiralProjectile;
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
	}

	protected override void Die()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		base.Die();
	}
}
