using System.Collections;
using UnityEngine;

public class AirshipCrabLevelGems : ParrySwitch
{
	public enum SideHit
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		None = 4
	}

	public bool parried;

	public bool startTimer;

	public bool moving;

	public SideHit lastSideHit;

	private LevelProperties.AirshipCrab.Gems properties;

	private DamageDealer damageDealer;

	private Color pink;

	private Vector3 velocity;

	private Vector3 startPos;

	private Vector3 collisionPoint;

	private float angle;

	private bool getCollisionPoint;

	private Coroutine currentMovement;

	public void Init(LevelProperties.AirshipCrab.Gems properties, Vector2 pos, float angle)
	{
		this.properties = properties;
		base.transform.position = pos;
		this.angle = angle;
		pink = GetComponent<SpriteRenderer>().color;
		startPos = base.transform.position;
		base.transform.SetEulerAngles(0f, 0f, 0f - angle);
		velocity = -base.transform.right;
	}

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		lastSideHit = SideHit.None;
		parried = false;
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, CollisionPhase.Enter);
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

	public void PickMovement()
	{
		if (!parried)
		{
			if (currentMovement != null)
			{
				StopCoroutine(currentMovement);
			}
			currentMovement = StartCoroutine(move_cr());
		}
		else
		{
			if (currentMovement != null)
			{
				StopCoroutine(currentMovement);
			}
			currentMovement = StartCoroutine(go_back_cr());
		}
	}

	private IEnumerator move_cr()
	{
		moving = true;
		parried = false;
		GetComponent<Collider2D>().enabled = true;
		GetComponent<SpriteRenderer>().color = pink;
		while (moving)
		{
			base.transform.position += velocity * properties.bulletSpeed * CupheadTime.FixedDelta;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator change_direction_cr(Vector3 collisionPoint)
	{
		velocity = 1f * (-2f * Vector3.Dot(velocity, Vector3.Normalize(collisionPoint.normalized)) * Vector3.Normalize(collisionPoint.normalized) + velocity);
		yield return null;
	}

	private IEnumerator go_back_cr()
	{
		velocity = -base.transform.right;
		while (base.transform.position != startPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, startPos, properties.bulletSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		yield return null;
	}

	public override void OnParryPrePause(AbstractPlayerController player)
	{
		base.OnParryPrePause(player);
		player.stats.ParryOneQuarter();
	}

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		base.OnParryPostPause(player);
		startTimer = true;
		parried = true;
		moving = false;
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		PickMovement();
	}
}
