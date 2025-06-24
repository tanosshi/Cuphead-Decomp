using UnityEngine;

public class ForestPlatformingLevelAcornPropeller : AbstractPausableComponent
{
	private float speed;

	private float t;

	private const float LIFETIME = 5f;

	protected override void Awake()
	{
		base.Awake();
	}

	public ForestPlatformingLevelAcornPropeller Create(Vector2 position, float speed)
	{
		ForestPlatformingLevelAcornPropeller forestPlatformingLevelAcornPropeller = InstantiatePrefab<ForestPlatformingLevelAcornPropeller>();
		forestPlatformingLevelAcornPropeller.transform.position = position;
		forestPlatformingLevelAcornPropeller.speed = speed;
		return forestPlatformingLevelAcornPropeller;
	}

	protected override void Update()
	{
		base.Update();
		base.transform.AddPosition(0f, speed * (float)CupheadTime.Delta);
		t += CupheadTime.Delta;
		if (t > 5f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
