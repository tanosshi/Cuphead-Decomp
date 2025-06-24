using UnityEngine;

public class FlyingGenieLevelObeliskBlock : AbstractProjectile
{
	[SerializeField]
	private BasicProjectile projectile;

	[SerializeField]
	private BasicProjectile pinkProjectile;

	[SerializeField]
	private SpriteRenderer darkSprite;

	private LevelProperties.FlyingGenie.Obelisk properties;

	private Vector3 rootPos;

	public void Init(Vector3 pos, LevelProperties.FlyingGenie.Obelisk properties)
	{
		base.transform.position = pos;
		this.properties = properties;
		rootPos = new Vector3(GetComponent<Renderer>().bounds.size.x / 2f + 10f, 0f, 0f);
	}

	protected override void Start()
	{
		base.Start();
		darkSprite.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	public void ShootRegular(float angle)
	{
		projectile.Create(base.transform.position + rootPos, angle, properties.obeliskShootSpeed);
	}

	public void ShootPink(float angle)
	{
		pinkProjectile.Create(base.transform.position + rootPos, angle, properties.obeliskShootSpeed);
	}
}
