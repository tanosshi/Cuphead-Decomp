using System.Collections;
using UnityEngine;

public class BatLevelCross : AbstractCollidableObject
{
	private LevelProperties.Bat.CrossToss properties;

	private AbstractPlayerController player;

	private DamageDealer damageDealer;

	private Vector3 velocity;

	private Vector3 startPos;

	private int maxCount;

	private bool goBack;

	protected override void Start()
	{
		base.Start();
		damageDealer = DamageDealer.NewEnemy();
	}

	public void Init(Vector2 pos, LevelProperties.Bat.CrossToss properties, int maxCount, AbstractPlayerController player)
	{
		base.transform.position = pos;
		startPos = pos;
		this.properties = properties;
		this.maxCount = maxCount;
		this.player = player;
		FindPlayer();
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionCeiling(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			goBack = true;
		}
	}

	protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionCeiling(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			goBack = true;
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionCeiling(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			goBack = true;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		int count = 0;
		bool startAgain = false;
		while (count < maxCount)
		{
			if (!goBack)
			{
				base.transform.position += velocity * properties.projectileSpeed * CupheadTime.FixedDelta;
			}
			else
			{
				startAgain = false;
				Vector2 vector2 = (Vector2)base.transform.position;
				Vector2 vector = Vector3.MoveTowards(base.transform.position, startPos, properties.projectileSpeed * (float)CupheadTime.Delta);
				base.transform.position = vector;
				if (base.transform.position == startPos && !startAgain)
				{
					count++;
					goBack = false;
					FindPlayer();
					startAgain = true;
				}
			}
			yield return null;
		}
		Die();
		yield return null;
	}

	private void FindPlayer()
	{
		float x = player.transform.position.x - base.transform.position.x;
		float y = player.transform.position.y - base.transform.position.y;
		float value = Mathf.Atan2(y, x) * 57.29578f;
		base.transform.SetEulerAngles(0f, 0f, value);
		velocity = base.transform.right;
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
