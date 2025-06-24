using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelMudmanSpawner : AbstractPausableComponent
{
	[SerializeField]
	private Transform[] spawnPoints;

	[SerializeField]
	private MountainPlatformingLevelMudman bigMudman;

	[SerializeField]
	private MountainPlatformingLevelMudman smallMudman;

	[SerializeField]
	private MinMax spawnDelayRange;

	[SerializeField]
	private MinMax initialDelayRange;

	[SerializeField]
	private string mudmanSizeString;

	[SerializeField]
	private string mudmanBigSpawnString;

	[SerializeField]
	private string mudmanSmallSpawnString;

	protected override void Start()
	{
		base.Start();
	}

	public void SpawnMudmen()
	{
		StartCoroutine(spawn_cr());
	}

	private IEnumerator spawn_cr()
	{
		string[] mudmanSize = mudmanSizeString.Split(',');
		string[] mudmanBig = mudmanBigSpawnString.Split(',');
		string[] mudmanSmall = mudmanSmallSpawnString.Split(',');
		int mudmanSizeIndex = Random.Range(0, mudmanSize.Length);
		int mudmanBigIndex = Random.Range(0, mudmanBig.Length);
		int mudmanSmallIndex = Random.Range(0, mudmanSmall.Length);
		PlatformingLevelGroundMovementEnemy.Direction dir = PlatformingLevelGroundMovementEnemy.Direction.Left;
		yield return CupheadTime.WaitForSeconds(this, initialDelayRange.RandomFloat());
		while (MountainPlatformingLevelElevatorHandler.elevatorIsMoving)
		{
			if (mudmanSize[mudmanSizeIndex][0] == 'B')
			{
				string[] array = mudmanBig[mudmanBigIndex].Split('-');
				string[] array2 = array;
				foreach (string s in array2)
				{
					int result = 1;
					int.TryParse(s, out result);
					dir = ((result < 3) ? PlatformingLevelGroundMovementEnemy.Direction.Right : PlatformingLevelGroundMovementEnemy.Direction.Left);
					MountainPlatformingLevelMudman mountainPlatformingLevelMudman = Object.Instantiate(bigMudman);
					mountainPlatformingLevelMudman.Init(spawnPoints[result - 1].position, dir);
				}
				mudmanBigIndex = (mudmanBigIndex + 1) % mudmanBig.Length;
			}
			else if (mudmanSize[mudmanSizeIndex][0] == 'S')
			{
				string[] array3 = mudmanSmall[mudmanSmallIndex].Split('-');
				string[] array4 = array3;
				foreach (string s2 in array4)
				{
					int result2 = 1;
					int.TryParse(s2, out result2);
					dir = ((result2 < 3) ? PlatformingLevelGroundMovementEnemy.Direction.Right : PlatformingLevelGroundMovementEnemy.Direction.Left);
					MountainPlatformingLevelMudman mountainPlatformingLevelMudman2 = Object.Instantiate(smallMudman);
					mountainPlatformingLevelMudman2.Init(spawnPoints[result2 - 1].position, dir);
				}
				mudmanSmallIndex = (mudmanSmallIndex + 1) % mudmanSmall.Length;
			}
			mudmanSizeIndex = (mudmanSizeIndex + 1) % mudmanSize.Length;
			yield return CupheadTime.WaitForSeconds(this, spawnDelayRange.RandomFloat());
			yield return null;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.2f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		Gizmos.color = new Color(1f, 0f, 0f, a);
		Transform[] array = spawnPoints;
		foreach (Transform transform in array)
		{
			Gizmos.DrawWireSphere(transform.position, 30f);
		}
	}
}
