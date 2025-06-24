using UnityEngine;

public class ForestPlatformingLevelLobberProjectile : BasicProjectile
{
	[SerializeField]
	private ForestPlatformingLevelLobberProjectileExplosion explosionPrefab;

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.transform.SetScale(MathUtils.RandomBool() ? 1 : (-1));
		animator.Play("A", 0, Random.Range(0f, 1f));
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		LevelPlatform component = hit.GetComponent<LevelPlatform>();
		if (component == null || (!component.canFallThrough && Mathf.Abs(_accumulativeGravity) > base.transform.right.y * Speed))
		{
			explode();
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionOther(hit, phase);
		LevelPlatform component = hit.GetComponent<LevelPlatform>();
		if (component != null && !component.canFallThrough && Mathf.Abs(_accumulativeGravity) > base.transform.right.y * Speed)
		{
			explode();
		}
	}

	private void explode()
	{
		if (!base.dead)
		{
			explosionPrefab.Create(base.transform.position);
			Die();
		}
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
