using UnityEngine;

public class FlyingGenieLevelSpawnerPoint : AbstractProjectile
{
	[SerializeField]
	private Effect effect;

	[SerializeField]
	private BasicProjectile projectile;

	[SerializeField]
	private Transform root;

	private LevelProperties.FlyingGenie.Bullets properties;

	private float rotation;

	private float velocity;

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	public FlyingGenieLevelSpawnerPoint Create(Vector2 pos, float rotation, float velocity, LevelProperties.FlyingGenie.Bullets properties)
	{
		FlyingGenieLevelSpawnerPoint flyingGenieLevelSpawnerPoint = base.Create(pos, rotation) as FlyingGenieLevelSpawnerPoint;
		flyingGenieLevelSpawnerPoint.properties = properties;
		flyingGenieLevelSpawnerPoint.velocity = velocity;
		flyingGenieLevelSpawnerPoint.rotation = rotation;
		return flyingGenieLevelSpawnerPoint;
	}

	public void Shoot()
	{
		effect.Create(root.transform.position);
		int num = Random.Range(1, 4);
		BasicProjectile basicProjectile = projectile.Create(root.transform.position, base.transform.eulerAngles.z - 90f, properties.childSpeed);
		basicProjectile.GetComponent<Animator>().Play("Bullet_" + num);
	}

	public void Dead()
	{
		Die();
		StopAllCoroutines();
	}

	protected override void Die()
	{
		StopAllCoroutines();
	}

	protected override void RandomizeVariant()
	{
	}
}
