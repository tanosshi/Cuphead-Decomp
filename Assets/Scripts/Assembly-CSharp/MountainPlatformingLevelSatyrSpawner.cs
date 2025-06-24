using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelSatyrSpawner : AbstractPausableComponent
{
	[SerializeField]
	private string directionString;

	[SerializeField]
	private string spawnString;

	[SerializeField]
	private float xRange;

	[SerializeField]
	private MountainPlatformingLevelSatyr satyrPrefab;

	[SerializeField]
	private MinMax spawnDelayRange;

	private int directionIndex;

	private int spawnIndex;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(spawn_cr());
		directionIndex = Random.Range(0, directionString.Split(',').Length);
		spawnIndex = Random.Range(0, spawnString.Split(',').Length);
	}

	private IEnumerator spawn_cr()
	{
		PlatformingLevelGroundMovementEnemy.Direction direction = PlatformingLevelGroundMovementEnemy.Direction.Right;
		bool isForeground = false;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, spawnDelayRange.RandomFloat());
			Vector2 spawnPos = base.transform.position;
			spawnPos.y = base.transform.position.y;
			spawnPos.x += Random.Range(0f - xRange, xRange);
			AbstractPlayerController player = PlayerManager.GetNext();
			if (CupheadLevelCamera.Current.ContainsPoint(spawnPos, new Vector2(0f, 1000f)))
			{
				if (spawnString.Split(',')[spawnIndex][0] == 'F')
				{
					isForeground = true;
				}
				else if (spawnString.Split(',')[spawnIndex][0] == 'B')
				{
					isForeground = false;
				}
				if (directionString.Split(',')[directionIndex][0] == 'L')
				{
					direction = PlatformingLevelGroundMovementEnemy.Direction.Left;
				}
				else if (directionString.Split(',')[directionIndex][0] == 'R')
				{
					direction = PlatformingLevelGroundMovementEnemy.Direction.Right;
				}
				else if (directionString.Split(',')[directionIndex][0] == 'P')
				{
					direction = ((!(player.transform.position.x < spawnPos.x)) ? PlatformingLevelGroundMovementEnemy.Direction.Right : PlatformingLevelGroundMovementEnemy.Direction.Left);
				}
				MountainPlatformingLevelSatyr mountainPlatformingLevelSatyr = satyrPrefab.Spawn(spawnPos, direction, true) as MountainPlatformingLevelSatyr;
				mountainPlatformingLevelSatyr.Init(direction, isForeground);
				directionIndex = (directionIndex + 1) % directionString.Split(',').Length;
				spawnIndex = (spawnIndex + 1) % spawnString.Split(',').Length;
			}
			yield return null;
		}
	}

	protected override void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 1f);
		Gizmos.DrawLine(base.baseTransform.position - new Vector3(xRange, 0f, 0f), base.baseTransform.position + new Vector3(xRange, 0f, 0f));
	}
}
