using UnityEngine;

public class PlatformingLevelGroundMovementEnemySpawner : PlatformingLevelEnemySpawner
{
	public PlatformingLevelGroundMovementEnemy enemyPrefab;

	public bool chooseSideRandomly = true;

	public string patternString = "LR";

	private int patternIndex;

	protected override void Awake()
	{
		base.Awake();
		if (!chooseSideRandomly)
		{
			patternIndex = Random.Range(0, patternString.Length);
		}
	}

	protected override void Spawn()
	{
		PlatformingLevelGroundMovementEnemy.Direction direction;
		if (chooseSideRandomly)
		{
			direction = ((!MathUtils.RandomBool()) ? PlatformingLevelGroundMovementEnemy.Direction.Right : PlatformingLevelGroundMovementEnemy.Direction.Left);
		}
		else
		{
			patternIndex = (patternIndex + 1) % patternString.Length;
			direction = ((patternString[patternIndex] != 'L') ? PlatformingLevelGroundMovementEnemy.Direction.Right : PlatformingLevelGroundMovementEnemy.Direction.Left);
		}
		Vector2 vector = new Vector2((direction != PlatformingLevelGroundMovementEnemy.Direction.Left) ? (CupheadLevelCamera.Current.Bounds.xMin - 50f) : (CupheadLevelCamera.Current.Bounds.xMax + 50f), CupheadLevelCamera.Current.Bounds.yMax);
		PlatformingLevelGroundMovementEnemy platformingLevelGroundMovementEnemy = enemyPrefab.Spawn(vector, direction, destroyEnemyAfterLeavingScreen);
		platformingLevelGroundMovementEnemy.GoToGround();
	}
}
