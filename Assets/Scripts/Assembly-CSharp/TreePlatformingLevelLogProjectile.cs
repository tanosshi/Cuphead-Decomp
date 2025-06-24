using UnityEngine;

public class TreePlatformingLevelLogProjectile : BasicProjectile
{
	public AbstractProjectile Create(Vector2 position, float rotation, float speed, bool isLeft, bool parry)
	{
		TreePlatformingLevelLogProjectile treePlatformingLevelLogProjectile = base.Create(position, rotation, speed) as TreePlatformingLevelLogProjectile;
		treePlatformingLevelLogProjectile.animator.SetFloat("Direction", isLeft ? 1 : (-1));
		treePlatformingLevelLogProjectile.animator.SetTrigger("Start");
		treePlatformingLevelLogProjectile.SetParryable(parry);
		return treePlatformingLevelLogProjectile;
	}

	public override void SetParryable(bool parryable)
	{
		base.SetParryable(parryable);
		animator.SetBool("Parry", parryable);
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
