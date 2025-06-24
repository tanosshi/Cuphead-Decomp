using System.Collections;
using UnityEngine;

public class BatLevelPinkCore : ParrySwitch
{
	public enum SideHit
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		None = 4
	}

	private LevelProperties.Bat.BatBouncer properties;

	private DamageDealer damageDealer;

	private Vector3 velocity;

	private Vector3 collisionPoint;

	public SideHit lastSideHit;

	private float angle;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		lastSideHit = SideHit.None;
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public void Init(LevelProperties.Bat.BatBouncer properties, Vector2 pos, float angle)
	{
		this.properties = properties;
		base.transform.position = pos;
		this.angle = angle;
		base.transform.SetEulerAngles(0f, 0f, angle);
		StartCoroutine(move_cr());
	}

	protected IEnumerator move_cr()
	{
		velocity = -base.transform.right;
		while (true)
		{
			base.transform.position += velocity * properties.mainBounceSpeed * CupheadTime.Delta;
			yield return null;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionCeiling(hit, phase);
		if (phase == CollisionPhase.Enter && lastSideHit != SideHit.Top)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = new Vector3(base.transform.position.x, Level.Current.Ground, 0f);
			collisionPoint = vector - position;
			lastSideHit = SideHit.Top;
			StartCoroutine(change_direction_cr(collisionPoint));
		}
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionCeiling(hit, phase);
		if (phase != CollisionPhase.Enter)
		{
			return;
		}
		if (base.transform.position.x > 0f)
		{
			if (lastSideHit != SideHit.Right)
			{
				Vector3 vector = new Vector3(Level.Current.Left, base.transform.position.y, 0f);
				Vector3 position = base.transform.position;
				collisionPoint = vector - position;
				lastSideHit = SideHit.Right;
				StartCoroutine(change_direction_cr(collisionPoint));
			}
		}
		else if (lastSideHit != SideHit.Left)
		{
			Vector3 position2 = base.transform.position;
			Vector3 vector2 = new Vector3(Level.Current.Right, base.transform.position.y, 0f);
			collisionPoint = vector2 - position2;
			lastSideHit = SideHit.Left;
			StartCoroutine(change_direction_cr(collisionPoint));
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionCeiling(hit, phase);
		if (phase == CollisionPhase.Enter && lastSideHit != SideHit.Bottom)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = new Vector3(base.transform.position.x, Level.Current.Ceiling, 0f);
			collisionPoint = vector - position;
			lastSideHit = SideHit.Bottom;
			StartCoroutine(change_direction_cr(collisionPoint));
		}
	}

	protected IEnumerator change_direction_cr(Vector3 collisionPoint)
	{
		velocity = 1f * (-2f * Vector3.Dot(velocity, Vector3.Normalize(collisionPoint.normalized)) * Vector3.Normalize(collisionPoint.normalized) + velocity);
		yield return null;
	}

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		base.OnParryPostPause(player);
		Object.Destroy(base.gameObject);
	}
}
