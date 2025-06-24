using UnityEngine;

public class SpriteDeathParts : AbstractCollidableObject
{
	public float bottomOffset = 100f;

	public float VelocityXMin = -500f;

	public float VelocityXMax = 500f;

	public float VelocityYMin = 500f;

	public float VelocityYMax = 1000f;

	public float GRAVITY = -100f;

	private Vector2 velocity;

	private float accumulatedGravity;

	public SpriteDeathParts CreatePart(Vector3 position)
	{
		SpriteDeathParts spriteDeathParts = InstantiatePrefab<SpriteDeathParts>();
		spriteDeathParts.transform.position = position;
		return spriteDeathParts;
	}

	protected override void Awake()
	{
		base.Awake();
		velocity = new Vector2(Random.Range(VelocityXMin, VelocityXMax), Random.Range(VelocityYMin, VelocityYMax));
	}

	protected override void Update()
	{
		base.Update();
		base.transform.position += (Vector3)(velocity + new Vector2(-300f, accumulatedGravity)) * Time.fixedDeltaTime;
		accumulatedGravity += GRAVITY;
		if (base.transform.position.y < -360f - bottomOffset)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
