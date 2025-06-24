using UnityEngine;

public class RetroArcadeRobotBouncingProjectile : AbstractProjectile
{
	private bool bounce;

	private float attackDelay;

	private Vector2 velocity;

	private DevilLevelSittingDevil parent;

	protected override float DestroyLifetime
	{
		get
		{
			return -1f;
		}
	}

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return true;
		}
	}

	public RetroArcadeRobotBouncingProjectile Create(Vector2 pos, float speed, float angle, bool bounce)
	{
		RetroArcadeRobotBouncingProjectile retroArcadeRobotBouncingProjectile = InstantiatePrefab<RetroArcadeRobotBouncingProjectile>();
		retroArcadeRobotBouncingProjectile.transform.position = pos;
		retroArcadeRobotBouncingProjectile.velocity = speed * MathUtils.AngleToDirection(angle);
		retroArcadeRobotBouncingProjectile.bounce = bounce;
		return retroArcadeRobotBouncingProjectile;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.dead)
		{
			return;
		}
		base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
		float radius = GetComponent<CircleCollider2D>().radius;
		if (bounce)
		{
			if ((velocity.x < 0f && base.transform.position.x < (float)Level.Current.Left + radius) || (velocity.x > 0f && base.transform.position.x > (float)Level.Current.Right - radius))
			{
				velocity.x *= -1f;
			}
			if (velocity.y < 0f && base.transform.position.y < (float)Level.Current.Ground + radius)
			{
				velocity.y *= -1f;
			}
		}
	}
}
