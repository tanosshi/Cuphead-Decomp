using UnityEngine;

public class TreePlatformingLevelLadybugSpawner : PlatformingLevelEnemySpawner
{
	[SerializeField]
	private TreePlatformingLevelLadyBug ladybugPrefab;

	[SerializeField]
	private string typeString;

	[SerializeField]
	private string sideString;

	private float spawnOffset = 850f;

	private int typeIndex;

	private int sideIndex;

	private Vector3 spawnPosition;

	protected override void Start()
	{
		base.Start();
		typeIndex = Random.Range(0, typeString.Split(',').Length);
		sideIndex = Random.Range(0, sideString.Split(',').Length);
	}

	protected override void Spawn()
	{
		PlatformingLevelGroundMovementEnemy.Direction dir = PlatformingLevelGroundMovementEnemy.Direction.Right;
		TreePlatformingLevelLadyBug.Type type = TreePlatformingLevelLadyBug.Type.BounceFast;
		if (sideString.Split(',')[sideIndex][0] == 'R')
		{
			spawnPosition.x = CupheadLevelCamera.Current.Bounds.xMin - 50f;
			dir = PlatformingLevelGroundMovementEnemy.Direction.Right;
		}
		else if (sideString.Split(',')[sideIndex][0] == 'L')
		{
			spawnPosition.x = CupheadLevelCamera.Current.Bounds.xMax + 50f;
			dir = PlatformingLevelGroundMovementEnemy.Direction.Left;
		}
		switch (typeString.Split(',')[typeIndex])
		{
		case "BS":
			type = TreePlatformingLevelLadyBug.Type.BounceSlow;
			spawnPosition.x = CupheadLevelCamera.Current.Bounds.xMax + 50f;
			dir = PlatformingLevelGroundMovementEnemy.Direction.Left;
			break;
		case "BF":
			type = TreePlatformingLevelLadyBug.Type.BounceFast;
			spawnPosition.x = CupheadLevelCamera.Current.Bounds.xMax + 50f;
			dir = PlatformingLevelGroundMovementEnemy.Direction.Left;
			break;
		case "GS":
			type = TreePlatformingLevelLadyBug.Type.GroundSlow;
			break;
		case "GF":
			type = TreePlatformingLevelLadyBug.Type.GroundFast;
			break;
		case "P":
			type = TreePlatformingLevelLadyBug.Type.BouncePink;
			spawnPosition.x = CupheadLevelCamera.Current.Bounds.xMax + 50f;
			dir = PlatformingLevelGroundMovementEnemy.Direction.Left;
			break;
		}
		spawnPosition.y = CupheadLevelCamera.Current.transform.position.y;
		ladybugPrefab.Spawn(spawnPosition, dir, true, type);
		typeIndex = (typeIndex + 1) % typeString.Split(',').Length;
		sideIndex = (sideIndex + 1) % sideString.Split(',').Length;
		base.Spawn();
	}
}
