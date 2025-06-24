using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingGenieLevelBouncer : AbstractProjectile
{
	[SerializeField]
	private Transform sprite;

	private LevelProperties.FlyingGenie.Obelisk properties;

	private List<GameObject> collisions;

	private Vector3 velocity;

	private Vector3 average;

	private Vector3 collisionPoint;

	private float angle;

	private float speed;

	public FlyingGenieLevelBouncer Init(Vector3 pos, LevelProperties.FlyingGenie.Obelisk properties, float angle)
	{
		base.transform.position = pos;
		this.properties = properties;
		this.angle = angle;
		collisions = new List<GameObject>();
		base.transform.SetEulerAngles(0f, 0f, angle);
		sprite.transform.SetEulerAngles(0f, 0f, 0f);
		return this;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionWalls(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			if (Vector3.Dot(hit.transform.position, Vector3.right) > 0f)
			{
				Vector3 vector = new Vector3(Level.Current.Left, base.transform.position.y, 0f);
				Vector3 position = base.transform.position;
				collisionPoint = vector - position;
				StartCoroutine(change_dir_cr(collisionPoint));
			}
			else
			{
				Vector3 position2 = base.transform.position;
				Vector3 vector2 = new Vector3(Level.Current.Right, base.transform.position.y, 0f);
				collisionPoint = vector2 - position2;
				StartCoroutine(change_dir_cr(collisionPoint));
			}
		}
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionCeiling(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = new Vector3(base.transform.position.x, Level.Current.Ground, 0f);
			collisionPoint = vector - position;
			StartCoroutine(change_dir_cr(collisionPoint));
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = new Vector3(base.transform.position.x, Level.Current.Ceiling, 0f);
			collisionPoint = vector - position;
			StartCoroutine(change_dir_cr(collisionPoint));
		}
	}

	private IEnumerator move_cr()
	{
		velocity = base.transform.up;
		speed = properties.bouncerSpeed;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (true)
		{
			base.transform.position += velocity * speed * CupheadTime.Delta;
			yield return wait;
		}
	}

	private IEnumerator change_dir_cr(Vector3 collisionPoint)
	{
		velocity = 1f * (-2f * Vector3.Dot(velocity, Vector3.Normalize(collisionPoint.normalized)) * Vector3.Normalize(collisionPoint.normalized) + velocity);
		yield return null;
	}

	private void ChangeSpeed()
	{
		if (Vector3.Dot(velocity, Vector3.right) > 0f)
		{
			float num = properties.bouncerSpeed - properties.obeliskMovementSpeed;
			speed -= num;
		}
		else
		{
			speed = properties.bouncerSpeed;
		}
	}

	protected override void Die()
	{
		base.Die();
	}
}
