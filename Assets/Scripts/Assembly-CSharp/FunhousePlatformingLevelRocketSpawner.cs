using UnityEngine;

public class FunhousePlatformingLevelRocketSpawner : PlatformingLevelGroundMovementEnemySpawner
{
	private const string Right = "R";

	[SerializeField]
	private string topBottomString;

	private int topBottomIndex;

	private bool isTop;

	private string[] directionPattern;

	private int directionIndex;

	protected override void Start()
	{
		base.Start();
		topBottomIndex = Random.Range(0, topBottomString.Split(',').Length);
		directionPattern = patternString.Split(',');
		directionIndex = Random.Range(0, directionPattern.Length);
	}

	protected override void Spawn()
	{
		if (topBottomString.Split(',')[topBottomIndex][0] == 'T')
		{
			isTop = true;
		}
		else if (topBottomString.Split(',')[topBottomIndex][0] == 'B')
		{
			isTop = false;
		}
		bool flag = ((!chooseSideRandomly) ? (directionPattern[directionIndex] == "R") : Rand.Bool());
		directionIndex = (directionIndex + 1) % directionPattern.Length;
		float x = ((!flag) ? (CupheadLevelCamera.Current.Bounds.xMin - 50f) : (CupheadLevelCamera.Current.Bounds.xMax + 50f));
		float y = CupheadLevelCamera.Current.transform.position.y;
		PlatformingLevelGroundMovementEnemy platformingLevelGroundMovementEnemy = enemyPrefab.Spawn();
		platformingLevelGroundMovementEnemy.GetComponent<FunhousePlatformingLevelRocket>().Init(new Vector2(x, y), isTop, flag);
		platformingLevelGroundMovementEnemy.GoToGround();
		topBottomIndex = (topBottomIndex + 1) % topBottomString.Split(',').Length;
	}
}
