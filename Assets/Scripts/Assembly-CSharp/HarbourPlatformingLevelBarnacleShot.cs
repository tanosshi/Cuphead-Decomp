using UnityEngine;

public class HarbourPlatformingLevelBarnacleShot : BasicProjectile
{
	private const float ProjectileSpeed = 1f;

	public override BasicProjectile Create(Vector2 position, float rotation, float speed)
	{
		HarbourPlatformingLevelBarnacleShot harbourPlatformingLevelBarnacleShot = base.Create(position, rotation, speed) as HarbourPlatformingLevelBarnacleShot;
		harbourPlatformingLevelBarnacleShot.animator.SetFloat("Speed", ((!Rand.Bool()) ? 1f : (-1f)) * 1f * Random.Range(0.9f, 1.1f));
		return harbourPlatformingLevelBarnacleShot;
	}
}
