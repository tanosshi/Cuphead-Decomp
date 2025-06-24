using UnityEngine;

public class CircusPlatformingLevelBalloonSpawner : PlatformingLevelEnemySpawner
{
	[SerializeField]
	private CircusPlatformingLevelBalloon balloonPrefab;

	[SerializeField]
	private string spawnDelayString;

	[SerializeField]
	private string spawnPositionString;

	[SerializeField]
	private string spawnSideString;

	[SerializeField]
	private string spreadCount;

	[SerializeField]
	private string pinkString;

	private float rotation;

	private int delayIndex;

	private int posIndex;

	private int sideIndex;

	private Vector3 spawnPosition;

	private string[] pinkSplits;

	private int pinkIndex;

	protected override void Start()
	{
		base.Start();
		delayIndex = Random.Range(0, spawnDelayString.Split(',').Length);
		posIndex = Random.Range(0, spawnPositionString.Split(',').Length);
		sideIndex = Random.Range(0, spawnSideString.Split(',').Length);
		pinkSplits = pinkString.Split(',');
		pinkIndex = Random.Range(0, pinkSplits.Length);
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
		CircusPlatformingLevelBalloon circusPlatformingLevelBalloon = Object.Instantiate(balloonPrefab);
		circusPlatformingLevelBalloon.Init(spawnPosition, rotation, spreadCount, pinkSplits[pinkIndex]);
		sideIndex = (sideIndex + 1) % spawnSideString.Split(',').Length;
		posIndex = (posIndex + 1) % spawnPositionString.Split(',').Length;
		delayIndex = (delayIndex + 1) % spawnDelayString.Split(',').Length;
		pinkIndex = (pinkIndex + 1) % pinkSplits.Length;
	}
}
