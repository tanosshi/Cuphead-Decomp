using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelMinerSpawner : AbstractPausableComponent
{
	[SerializeField]
	private PlatformingLevelGroundMovementEnemy enemyPrefab;

	private PlatformingLevelGroundMovementEnemy enemySpawned;

	[SerializeField]
	private float xRange;

	[SerializeField]
	private MinMax deathDelayTime;

	[SerializeField]
	private MinMax spawnTime;

	private bool isRespawning;

	protected override void Start()
	{
		StartCoroutine(spawn_cr());
	}

	private IEnumerator spawn_cr()
	{
		while (true)
		{
			Vector2 spawnPos = base.transform.position;
			spawnPos.x += Random.Range(0f - xRange, xRange);
			if (isRespawning)
			{
				yield return CupheadTime.WaitForSeconds(this, deathDelayTime.RandomFloat());
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, spawnTime.RandomFloat());
			}
			enemySpawned = enemyPrefab.Spawn(spawnPos, (!MathUtils.RandomBool()) ? PlatformingLevelGroundMovementEnemy.Direction.Right : PlatformingLevelGroundMovementEnemy.Direction.Left, false);
			enemySpawned.Float();
			while (enemySpawned != null)
			{
				yield return null;
			}
			isRespawning = true;
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
		Gizmos.color = new Color(1f, 1f, 0f, a);
		Gizmos.DrawLine(base.baseTransform.position - new Vector3(xRange, 0f, 0f), base.baseTransform.position + new Vector3(xRange, 0f, 0f));
	}
}
