using UnityEngine;

public class RetroArcadeBonusRobot : RetroArcadeEnemy
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float SPAWN_X = 400f;

	private const float SPAWN_Y = 250f;

	private LevelProperties.RetroArcade.Robots properties;

	private Direction direction;

	public RetroArcadeBonusRobot Create(Direction direction, LevelProperties.RetroArcade.Robots properties)
	{
		RetroArcadeBonusRobot retroArcadeBonusRobot = InstantiatePrefab<RetroArcadeBonusRobot>();
		retroArcadeBonusRobot.transform.position = new Vector2((direction != Direction.Left) ? (-400f) : 400f, 250f);
		retroArcadeBonusRobot.properties = properties;
		retroArcadeBonusRobot.direction = direction;
		retroArcadeBonusRobot.hp = properties.bonusHp;
		return retroArcadeBonusRobot;
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
		base.transform.AddPosition((float)((direction == Direction.Right) ? 1 : (-1)) * properties.bonusMoveSpeed * CupheadTime.FixedDelta);
		if ((direction != Direction.Left) ? (base.transform.position.x > 400f) : (base.transform.position.x < -400f))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
