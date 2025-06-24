using UnityEngine;

public class RetroArcadeAlien : RetroArcadeEnemy
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float MOVE_Y_SPEED = 500f;

	[SerializeField]
	private BasicProjectile bulletPrefab;

	[SerializeField]
	private Transform bulletRoot;

	private LevelProperties.RetroArcade.Aliens properties;

	private RetroArcadeAlienManager manager;

	public int ColumnIndex { get; private set; }

	public RetroArcadeAlien Create(Vector2 position, int columnIndex, RetroArcadeAlienManager manager, LevelProperties.RetroArcade.Aliens properties)
	{
		RetroArcadeAlien retroArcadeAlien = InstantiatePrefab<RetroArcadeAlien>();
		retroArcadeAlien.transform.position = position;
		retroArcadeAlien.properties = properties;
		retroArcadeAlien.manager = manager;
		retroArcadeAlien.hp = properties.hp;
		retroArcadeAlien.ColumnIndex = columnIndex;
		return retroArcadeAlien;
	}

	protected override void Start()
	{
		base.Start();
		base.PointsWorth = properties.pointsGained;
		base.PointsBonus = properties.pointsBonus;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!movingY)
		{
			base.transform.AddPosition((float)((manager.direction == Direction.Right) ? 1 : (-1)) * manager.moveSpeed * CupheadTime.FixedDelta);
		}
	}

	public void MoveY(float moveAmount)
	{
		MoveY(moveAmount, 500f);
	}

	public override void Die()
	{
		base.Die();
		manager.OnAlienDie(this);
	}

	public void Shoot()
	{
		bulletPrefab.Create(bulletRoot.position, -90f, properties.bulletSpeed);
	}
}
