using UnityEngine;

public class DevilLevelPitchforkJumpingProjectile : AbstractProjectile
{
	public enum State
	{
		Idle = 0,
		Jumping = 1,
		OnGround = 2
	}

	public State state;

	private Vector2 velocity;

	private MinMax launchSpeed;

	private MinMax launchAngle;

	private float gravity;

	private int jumpsRemaining;

	private DevilLevelSittingDevil parent;

	protected override float DestroyLifetime
	{
		get
		{
			return -1f;
		}
	}

	public DevilLevelPitchforkJumpingProjectile Create(Vector2 pos, MinMax launchAngle, MinMax launchSpeed, float gravity, int numJumps, DevilLevelSittingDevil parent)
	{
		DevilLevelPitchforkJumpingProjectile devilLevelPitchforkJumpingProjectile = InstantiatePrefab<DevilLevelPitchforkJumpingProjectile>();
		devilLevelPitchforkJumpingProjectile.transform.position = pos;
		devilLevelPitchforkJumpingProjectile.launchSpeed = launchSpeed;
		devilLevelPitchforkJumpingProjectile.launchAngle = launchAngle;
		devilLevelPitchforkJumpingProjectile.gravity = gravity;
		devilLevelPitchforkJumpingProjectile.parent = parent;
		devilLevelPitchforkJumpingProjectile.jumpsRemaining = numJumps;
		return devilLevelPitchforkJumpingProjectile;
	}

	protected override void Update()
	{
		base.Update();
		if (parent == null)
		{
			Die();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.dead || state != State.Jumping)
		{
			return;
		}
		velocity.y -= gravity * CupheadTime.FixedDelta;
		base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
		float radius = GetComponent<CircleCollider2D>().radius;
		if (base.transform.position.y < (float)Level.Current.Ground + radius)
		{
			base.transform.SetPosition(null, (float)Level.Current.Ground + radius);
			if (jumpsRemaining > 0)
			{
				state = State.OnGround;
			}
			else
			{
				Die();
			}
		}
	}

	public void Jump()
	{
		float num = float.MaxValue;
		Vector2 vector = Vector2.zero;
		Vector3 center = PlayerManager.GetNext().center;
		Vector2 vector2 = center - base.transform.position;
		vector2.x = Mathf.Abs(vector2.x);
		float radius = GetComponent<CircleCollider2D>().radius;
		float num2 = 0f;
		AudioManager.Play("devil_projectile_move");
		emitAudioFromObject.Add("devil_projectile_move");
		num2 = ((!(center.x < base.transform.position.x)) ? ((float)Level.Current.Right - radius - base.transform.position.x) : (base.transform.position.x - ((float)Level.Current.Left + radius)));
		for (float num3 = 0f; num3 < 1f; num3 += 0.01f)
		{
			float floatAt = launchAngle.GetFloatAt(num3);
			float floatAt2 = launchSpeed.GetFloatAt(num3);
			Vector2 vector3 = MathUtils.AngleToDirection(floatAt) * floatAt2;
			float num4 = vector2.x / vector3.x;
			float num5 = vector3.y * num4 - 0.5f * gravity * num4 * num4;
			float num6 = Mathf.Abs(vector2.y - num5);
			float num7 = vector3.y - gravity * num4;
			if (!(num7 > 0f))
			{
				float num8 = num2 / vector3.x;
				float num9 = vector3.y * num8 - 0.5f * gravity * num8 * num8;
				if (!(num9 > (float)Level.Current.Ground + radius) && num6 < num)
				{
					num = num6;
					vector = vector3;
				}
			}
		}
		if (center.x < base.transform.position.x)
		{
			vector.x *= -1f;
		}
		velocity = vector;
		state = State.Jumping;
		jumpsRemaining--;
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
