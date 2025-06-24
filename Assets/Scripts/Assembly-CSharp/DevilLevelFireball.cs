using UnityEngine;

public class DevilLevelFireball : AbstractProjectile
{
	[SerializeField]
	private Effect poofEffect;

	[SerializeField]
	private SpriteDeathParts[] parts;

	private const float SPAWN_Y = 500f;

	private float yVelocity;

	private float gravity;

	public DevilLevelFireball Create(float xPos, float speed, float gravity, float xScale)
	{
		DevilLevelFireball devilLevelFireball = InstantiatePrefab<DevilLevelFireball>();
		devilLevelFireball.transform.position = new Vector2(xPos, 500f);
		devilLevelFireball.yVelocity = 0f - speed;
		devilLevelFireball.gravity = gravity;
		devilLevelFireball.transform.SetScale(xScale);
		return devilLevelFireball;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.dead)
		{
			yVelocity -= gravity * CupheadTime.FixedDelta;
			base.transform.AddPosition(0f, yVelocity * CupheadTime.FixedDelta);
		}
	}

	protected override void Die()
	{
		base.Die();
		poofEffect.Create(base.transform.position);
		SpriteDeathParts[] array = parts;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(base.transform.position);
		}
		Object.Destroy(base.gameObject);
	}
}
