using UnityEngine;

public class RetroArcadeOrbiterRobot : RetroArcadeEnemy
{
	[SerializeField]
	private BasicProjectile projectilePrefab;

	[SerializeField]
	private Transform projectileRoot;

	private LevelProperties.RetroArcade.Robots properties;

	private float angle;

	private RetroArcadeBigRobot parent;

	public RetroArcadeOrbiterRobot Create(RetroArcadeBigRobot parent, LevelProperties.RetroArcade.Robots properties, float angle)
	{
		RetroArcadeOrbiterRobot retroArcadeOrbiterRobot = InstantiatePrefab<RetroArcadeOrbiterRobot>();
		retroArcadeOrbiterRobot.transform.position = (Vector2)parent.transform.position + properties.smallRobotRotationDistance * MathUtils.AngleToDirection(angle);
		retroArcadeOrbiterRobot.properties = properties;
		retroArcadeOrbiterRobot.parent = parent;
		retroArcadeOrbiterRobot.angle = angle;
		retroArcadeOrbiterRobot.hp = properties.smallRobotHp;
		return retroArcadeOrbiterRobot;
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
		angle += CupheadTime.FixedDelta * properties.smallRobotRotationSpeed;
		base.transform.position = (Vector2)parent.transform.position + MathUtils.AngleToDirection(angle) * properties.smallRobotRotationDistance;
	}

	public void Shoot()
	{
		projectilePrefab.Create(projectileRoot.position, -90f, properties.smallRobotShootSpeed);
	}
}
