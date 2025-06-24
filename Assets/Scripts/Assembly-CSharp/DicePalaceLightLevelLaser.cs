using UnityEngine;

public class DicePalaceLightLevelLaser : AbstractCollidableObject
{
	[SerializeField]
	private CollisionChild[] collisionChildren;

	private LevelProperties.DicePalaceLight.SixWayLaser properties;

	private DamageDealer damageDealer;

	private float speed;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		CollisionChild[] array = collisionChildren;
		foreach (CollisionChild collisionChild in array)
		{
			collisionChild.OnPlayerCollision += OnCollisionPlayer;
		}
	}

	public void Properties(LevelProperties.DicePalaceLight.SixWayLaser properties)
	{
		this.properties = properties;
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
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}
}
