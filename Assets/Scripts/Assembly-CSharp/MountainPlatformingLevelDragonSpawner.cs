using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelDragonSpawner : AbstractPausableComponent
{
	[SerializeField]
	private bool isElevator;

	[SerializeField]
	private Transform[] spawnPoints;

	[SerializeField]
	private MountainPlatformingLevelDragon dragonMiddlePrefab;

	[SerializeField]
	private MountainPlatformingLevelDragon dragonSidePrefab;

	[SerializeField]
	private string spawnString;

	[SerializeField]
	private float spawnDelay;

	private int spawnIndex;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(spawn_cr());
		spawnIndex = Random.Range(0, spawnString.Split(',').Length);
	}

	private IEnumerator spawn_cr()
	{
		while (true)
		{
			if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(0f, 1000f)))
			{
				if ((isElevator && MountainPlatformingLevelElevatorHandler.elevatorIsMoving) || !isElevator)
				{
					MountainPlatformingLevelDragon dragonPrefab = null;
					int scale = 1;
					int spawnPoint = 1;
					int.TryParse(spawnString.Split(',')[spawnIndex], out spawnPoint);
					Vector3 startPos = new Vector3(spawnPoints[spawnPoint - 1].position.x, spawnPoints[spawnPoint - 1].position.y + 500f);
					switch (spawnPoint)
					{
					case 1:
						dragonPrefab = dragonSidePrefab;
						break;
					case 2:
						dragonPrefab = dragonMiddlePrefab;
						break;
					case 3:
						dragonPrefab = dragonSidePrefab;
						scale = -1;
						break;
					}
					MountainPlatformingLevelDragon dragon = Object.Instantiate(dragonPrefab);
					dragon.Init(startPos, spawnPoints[spawnPoint - 1].position);
					dragon.transform.SetScale(scale);
					spawnIndex = (spawnIndex + 1) % spawnString.Split(',').Length;
					yield return CupheadTime.WaitForSeconds(this, spawnDelay);
				}
				yield return null;
			}
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
		Gizmos.color = new Color(1f, 0f, 1f, a);
		Transform[] array = spawnPoints;
		foreach (Transform transform in array)
		{
			Gizmos.DrawWireSphere(transform.position, 30f);
		}
	}
}
