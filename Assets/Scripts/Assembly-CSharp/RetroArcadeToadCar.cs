using UnityEngine;

public class RetroArcadeToadCar : AbstractCollidableObject
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float SPAWN_X = 450f;

	[SerializeField]
	private BasicProjectile missilePrefab;

	[SerializeField]
	private Transform missileRoot;

	[SerializeField]
	private Transform flipTransform;

	private LevelProperties.RetroArcade.Toad properties;

	private RetroArcadeToadManager parent;

	private Direction direction;

	public RetroArcadeToadCar Create(LevelProperties.RetroArcade.Toad properties, Direction direction)
	{
		RetroArcadeToadCar retroArcadeToadCar = InstantiatePrefab<RetroArcadeToadCar>();
		retroArcadeToadCar.transform.SetPosition((direction != Direction.Left) ? (-450f) : 450f, Level.Current.Ground);
		retroArcadeToadCar.flipTransform.SetScale((direction == Direction.Right) ? 1 : (-1), 1f, 1f);
		retroArcadeToadCar.properties = properties;
		retroArcadeToadCar.parent = parent;
		retroArcadeToadCar.direction = direction;
		return retroArcadeToadCar;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		base.transform.AddPosition((float)((direction == Direction.Right) ? 1 : (-1)) * properties.carSpeed * CupheadTime.FixedDelta);
		if ((direction != Direction.Left) ? (base.transform.position.x > 450f) : (base.transform.position.x < -450f))
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void ShootMissile()
	{
		missilePrefab.Create(missileRoot.position, 90f, properties.missileSpeed);
	}
}
