using UnityEngine;

public class HarbourPlatformingLevelFishSpawner : PlatformingLevelEnemySpawner
{
	[SerializeField]
	private HarbourPlatformingLevelFish fishPrefab;

	[SerializeField]
	private string spawnDelayString;

	[SerializeField]
	private string spawnPositionString;

	[SerializeField]
	private string spawnSideString;

	[SerializeField]
	private string typeString;

	[SerializeField]
	private float movementSpeed;

	[SerializeField]
	private float sineSpeed;

	[SerializeField]
	private float sineSize;

	private float rotation;

	private int delayIndex;

	private int posIndex;

	private int sideIndex;

	private int typeIndex;

	private Vector3 spawnPosition;

	protected override void Start()
	{
		base.Start();
		delayIndex = Random.Range(0, spawnDelayString.Split(',').Length);
		posIndex = Random.Range(0, spawnPositionString.Split(',').Length);
		sideIndex = Random.Range(0, spawnSideString.Split(',').Length);
		typeIndex = Random.Range(0, typeString.Split(',').Length);
	}

	protected override void Spawn()
	{
		base.Spawn();
		spawnDelay.min = float.Parse(spawnDelayString.Split(',')[delayIndex]);
		spawnDelay.max = float.Parse(spawnDelayString.Split(',')[delayIndex]);
		if (spawnSideString.Split(',')[sideIndex][0] == 'L')
		{
			spawnPosition.x = CupheadLevelCamera.Current.Bounds.xMin - 50f;
			rotation = 0f;
		}
		else if (spawnSideString.Split(',')[sideIndex][0] == 'R')
		{
			spawnPosition.x = CupheadLevelCamera.Current.Bounds.xMax + 50f;
			rotation = 180f;
		}
		spawnPosition.y = CupheadLevelCamera.Current.transform.position.y + float.Parse(spawnPositionString.Split(',')[posIndex]);
		HarbourPlatformingLevelFish harbourPlatformingLevelFish = Object.Instantiate(fishPrefab);
		harbourPlatformingLevelFish.Init(spawnPosition, rotation, movementSpeed, sineSpeed, sineSize, typeString.Split(',')[typeIndex]);
		sideIndex = (sideIndex + 1) % spawnSideString.Split(',').Length;
		posIndex = (posIndex + 1) % spawnPositionString.Split(',').Length;
		delayIndex = (delayIndex + 1) % spawnDelayString.Split(',').Length;
		typeIndex = (typeIndex + 1) % typeString.Split(',').Length;
	}
}
