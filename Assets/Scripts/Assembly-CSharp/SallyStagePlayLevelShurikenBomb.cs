using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelShurikenBomb : AbstractProjectile
{
	[SerializeField]
	private Sprite shuriken;

	[SerializeField]
	private Sprite explosion;

	private float currentYVelocity;

	private float horizontalVelocity;

	private float gravity;

	private AbstractPlayerController target;

	private float speed;

	private int childSpawnCount;

	private bool isActive;

	private LevelProperties.SallyStagePlay properties;

	private BoxCollider2D boxCollider;

	public void InitShuriken(LevelProperties.SallyStagePlay properties, int direction, AbstractPlayerController target)
	{
		boxCollider = GetComponent<BoxCollider2D>();
		this.properties = properties;
		speed = properties.CurrentState.shuriken.InitialMovementSpeed;
		this.target = target;
		isActive = true;
		childSpawnCount = 0;
		StartCoroutine(move_cr());
	}

	public void InitChildShuriken(int direction, int childSpawnCount, AbstractPlayerController target, LevelProperties.SallyStagePlay properties)
	{
		this.properties = properties;
		GetComponent<SpriteRenderer>().sprite = shuriken;
		if (childSpawnCount > 1)
		{
			currentYVelocity = properties.CurrentState.shuriken.ArcTwoVerticalVelocity;
			horizontalVelocity = properties.CurrentState.shuriken.ArcTwoHorizontalVelocity * Mathf.Sign(direction);
			gravity = properties.CurrentState.shuriken.ArcTwoGravity;
		}
		else
		{
			currentYVelocity = properties.CurrentState.shuriken.ArcOneVerticalVelocity;
			horizontalVelocity = properties.CurrentState.shuriken.ArcOneHorizontalVelocity * Mathf.Sign(direction);
			gravity = properties.CurrentState.shuriken.ArcOneGravity;
		}
		this.target = target;
		isActive = true;
		this.childSpawnCount = childSpawnCount;
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		if (target == null || target.IsDead)
		{
			target = PlayerManager.GetNext();
		}
		Vector3 direction = (new Vector3(target.center.x, Level.Current.Ground, 0f) - base.transform.position).normalized;
		while (true)
		{
			if (boxCollider != null)
			{
				boxCollider.enabled = true;
			}
			if (childSpawnCount > 0)
			{
				base.transform.position += (Vector3.right * horizontalVelocity + Vector3.up * currentYVelocity) * CupheadTime.Delta;
				currentYVelocity -= gravity;
			}
			else
			{
				base.transform.position += direction * speed * CupheadTime.Delta;
			}
			yield return null;
		}
	}

	private void Explode()
	{
		GetComponent<SpriteRenderer>().sprite = explosion;
		if (childSpawnCount < properties.CurrentState.shuriken.NumberOfChildSpawns)
		{
			childSpawnCount++;
			float x = GetComponent<SpriteRenderer>().bounds.size.x;
			for (int i = -1; i < 1; i++)
			{
				AbstractProjectile abstractProjectile = Create(base.transform.position + Vector3.right * x / 2f * Mathf.Sign(i) + Vector3.up * 50f);
				abstractProjectile.GetComponent<SallyStagePlayLevelShurikenBomb>().InitChildShuriken(i, childSpawnCount, target, properties);
			}
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		if (isActive)
		{
			isActive = false;
			Explode();
			StopAllCoroutines();
			Object.Destroy(base.gameObject, 0.1f);
		}
		base.OnCollisionGround(hit, phase);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}
}
