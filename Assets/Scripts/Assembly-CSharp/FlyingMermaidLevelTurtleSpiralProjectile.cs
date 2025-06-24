using UnityEngine;

public class FlyingMermaidLevelTurtleSpiralProjectile : BasicProjectile
{
	private float rotationSpeed;

	private GameObject rotationBase;

	public virtual FlyingMermaidLevelTurtleSpiralProjectile Create(Vector2 position, float rotation, float speed, float rotationSpeed)
	{
		FlyingMermaidLevelTurtleSpiralProjectile flyingMermaidLevelTurtleSpiralProjectile = base.Create(position, rotation, speed) as FlyingMermaidLevelTurtleSpiralProjectile;
		flyingMermaidLevelTurtleSpiralProjectile.rotationSpeed = rotationSpeed;
		flyingMermaidLevelTurtleSpiralProjectile.rotationBase = new GameObject("SpiralProjectileBase");
		flyingMermaidLevelTurtleSpiralProjectile.rotationBase.transform.position = position;
		flyingMermaidLevelTurtleSpiralProjectile.transform.parent = flyingMermaidLevelTurtleSpiralProjectile.rotationBase.transform;
		flyingMermaidLevelTurtleSpiralProjectile.animator.Play("A", 0, Random.Range(0f, 1f));
		flyingMermaidLevelTurtleSpiralProjectile.animator.Play("A", 1, Random.Range(0f, 1f));
		return flyingMermaidLevelTurtleSpiralProjectile;
	}

	protected override void Move()
	{
		if (Speed == 0f)
		{
			Debug.Log("[Basic Projectile] Speed is 0", base.gameObject);
		}
		base.transform.localPosition += rotationBase.transform.InverseTransformDirection(base.transform.right) * Speed * CupheadTime.FixedDelta;
		rotationBase.transform.AddEulerAngles(0f, 0f, rotationSpeed * 360f * CupheadTime.FixedDelta);
	}

	protected override void Die()
	{
		base.Die();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(rotationBase.gameObject);
	}
}
