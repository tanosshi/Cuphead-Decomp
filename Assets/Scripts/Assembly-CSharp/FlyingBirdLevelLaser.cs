using UnityEngine;

public class FlyingBirdLevelLaser : AbstractMonoBehaviour
{
	private Vector2 size;

	private float speed;

	public FlyingBirdLevelLaser Create(Vector2 pos, float speed)
	{
		FlyingBirdLevelLaser flyingBirdLevelLaser = InstantiatePrefab<FlyingBirdLevelLaser>();
		flyingBirdLevelLaser.transform.position = pos + new Vector2(flyingBirdLevelLaser.size.x, 0f);
		flyingBirdLevelLaser.speed = speed;
		return flyingBirdLevelLaser;
	}

	protected override void Awake()
	{
		base.Awake();
		SpriteRenderer component = base.transform.GetComponent<SpriteRenderer>();
		size = component.sprite.bounds.size;
	}

	protected override void Update()
	{
		base.Update();
		base.transform.AddPosition((0f - speed) * (float)CupheadTime.Delta);
		if (base.transform.position.x < -740f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
