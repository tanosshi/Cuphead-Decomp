using UnityEngine;

public class ForestPlatformingLevelAcornSpawner : PlatformingLevelEnemySpawner
{
	public ForestPlatformingLevelAcorn enemyPrefab;

	public string leftRightString = "LR";

	public string yString = "150,50";

	private int leftRightIndex;

	private int yIndex;

	private string[] yPattern;

	protected override void Awake()
	{
		base.Awake();
		leftRightIndex = Random.Range(0, leftRightString.Length);
		yPattern = yString.Split(',');
		yIndex = Random.Range(0, yPattern.Length);
	}

	protected override void Spawn()
	{
		leftRightIndex = (leftRightIndex + 1) % leftRightString.Length;
		ForestPlatformingLevelAcorn.Direction direction = ((leftRightString[leftRightIndex] != 'L') ? ForestPlatformingLevelAcorn.Direction.Right : ForestPlatformingLevelAcorn.Direction.Left);
		yIndex = (yIndex + 1) % yPattern.Length;
		float result = 0f;
		float.TryParse(yPattern[yIndex], out result);
		Vector2 position = new Vector2((direction != ForestPlatformingLevelAcorn.Direction.Left) ? (CupheadLevelCamera.Current.Bounds.xMin - 50f) : (CupheadLevelCamera.Current.Bounds.xMax + 50f), CupheadLevelCamera.Current.Bounds.yMax - result);
		ForestPlatformingLevelAcorn forestPlatformingLevelAcorn = enemyPrefab.Spawn(position, direction, false);
	}
}
