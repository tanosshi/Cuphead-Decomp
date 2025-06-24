using UnityEngine;

public class CircusPlatformingLevelPretzelSpawner : PlatformingLevelEnemySpawner
{
	[SerializeField]
	private CircusPlatformingLevelPretzel pretzelPrefab;

	[SerializeField]
	private string sideString;

	[SerializeField]
	private Transform[] path;

	private int sideIndex;

	private Vector3 spawnPosition;

	protected override void Start()
	{
		base.Start();
		sideIndex = Random.Range(0, sideString.Split(',').Length);
		if (path == null || path.Length == 0)
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected override void Spawn()
	{
		base.Spawn();
		bool goingLeft = false;
		int startPosition = -1;
		if (sideString.Split(',')[sideIndex][0] == 'L')
		{
			goingLeft = true;
			startPosition = path.Length - 1;
			for (int i = 0; i < path.Length; i++)
			{
				if (path[i].position.x > CupheadLevelCamera.Current.Bounds.xMax + 100f)
				{
					startPosition = i;
					break;
				}
			}
		}
		else if (sideString.Split(',')[sideIndex][0] == 'R')
		{
			goingLeft = true;
			startPosition = 0;
			for (int num = path.Length - 1; num >= 0; num--)
			{
				if (path[num].position.x < CupheadLevelCamera.Current.Bounds.xMin - 100f)
				{
					startPosition = num;
					break;
				}
			}
			goingLeft = false;
		}
		spawnPosition.y = CupheadLevelCamera.Current.transform.position.y;
		CircusPlatformingLevelPretzel circusPlatformingLevelPretzel = pretzelPrefab.Spawn();
		circusPlatformingLevelPretzel.SetPath(path);
		circusPlatformingLevelPretzel.goingLeft = goingLeft;
		circusPlatformingLevelPretzel.SetStartPosition(startPosition);
		sideIndex = (sideIndex + 1) % sideString.Split(',').Length;
	}
}
