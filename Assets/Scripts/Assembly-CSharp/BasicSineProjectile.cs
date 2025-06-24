using UnityEngine;

public class BasicSineProjectile : BasicProjectile
{
	protected Vector2 direction;

	protected Vector2 normalized;

	public float velocity;

	public float sinVelocity;

	protected float angle;

	public float rotation;

	public float sinSize;

	public BasicSineProjectile Create(Vector2 pos, float rotation, float velocity, float sinVelocity, float sinSize)
	{
		BasicSineProjectile basicSineProjectile = Create(pos) as BasicSineProjectile;
		basicSineProjectile.velocity = velocity;
		basicSineProjectile.rotation = rotation;
		basicSineProjectile.sinSize = sinSize;
		basicSineProjectile.sinVelocity = sinVelocity;
		return basicSineProjectile;
	}

	protected override void Start()
	{
		base.Start();
		CalculateSin();
	}

	private void CalculateSin()
	{
		Vector2 zero = Vector2.zero;
		zero.x = (direction.x + base.transform.position.x) / 2f;
		zero.y = (direction.y + base.transform.position.y) / 2f;
		float num = 0f - (direction.x - base.transform.position.x) / (direction.y - base.transform.position.y);
		float num2 = zero.y - num * zero.x;
		Vector2 zero2 = Vector2.zero;
		zero2.x = zero.x + 1f;
		zero2.y = num * zero2.x + num2;
		normalized = Vector3.zero;
		normalized = zero2 - zero;
		normalized.Normalize();
	}

	protected override void Move()
	{
		direction = MathUtils.AngleToDirection(rotation);
		Vector2 vector = base.transform.position;
		angle += sinVelocity * (float)CupheadTime.Delta;
		vector += normalized * Mathf.Sin(angle) * sinSize;
		vector += direction * velocity * CupheadTime.Delta;
		base.transform.position = vector;
	}
}
