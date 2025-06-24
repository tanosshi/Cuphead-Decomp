using System.Collections;
using UnityEngine;

public class AirshipClamLevelBarnacle : AbstractProjectile
{
	private int direction;

	private Vector3 velocity;

	private LevelProperties.AirshipClam properties;

	private CircleCollider2D collider;

	private new DamageDealer damageDealer;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		base.Awake();
	}

	protected override void Update()
	{
		damageDealer.Update();
		base.Update();
	}

	public void InitBarnacle(int dir, LevelProperties.AirshipClam properties)
	{
		this.properties = properties;
		direction = dir;
		collider = GetComponent<CircleCollider2D>();
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		velocity = new Vector3(properties.CurrentState.barnacles.initialArcMovementX * (float)direction, properties.CurrentState.barnacles.initialArcMovementY, 0f);
		while (true)
		{
			base.transform.position += velocity * CupheadTime.Delta;
			velocity.y += properties.CurrentState.barnacles.parryGravity;
			yield return null;
		}
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		if (phase == CollisionPhase.Enter)
		{
			velocity.x = 0f;
		}
		base.OnCollisionWalls(hit, phase);
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		velocity.y = 0f;
		velocity.x = properties.CurrentState.barnacles.rollingSpeed * (float)(-direction);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
		Object.Destroy(base.gameObject);
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}
}
