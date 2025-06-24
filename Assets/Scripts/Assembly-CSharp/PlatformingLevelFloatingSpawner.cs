using System.Collections;
using UnityEngine;

public class PlatformingLevelFloatingSpawner : AbstractPausableComponent
{
	[SerializeField]
	private PlatformingLevelGroundMovementEnemy enemyPrefab;

	[SerializeField]
	private float xRange;

	[SerializeField]
	private MinMax initialSpawnTime;

	[SerializeField]
	private MinMax spawnTime;

	protected override void Start()
	{
		Awake();
		StartCoroutine(spawn_cr());
	}

	private IEnumerator spawn_cr()
	{
		bool hashadSuccessfulSpawn = false;
		while (true)
		{
			if (hashadSuccessfulSpawn)
			{
				yield return CupheadTime.WaitForSeconds(this, spawnTime.RandomFloat());
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, initialSpawnTime.RandomFloat());
			}
			Vector2 spawnPos = base.transform.position;
			spawnPos.x += Random.Range(0f - xRange, xRange);
			if (CupheadLevelCamera.Current.ContainsPoint(spawnPos, new Vector2(0f, 1000f)))
			{
				PlatformingLevelGroundMovementEnemy platformingLevelGroundMovementEnemy = enemyPrefab.Spawn(spawnPos, (!MathUtils.RandomBool()) ? PlatformingLevelGroundMovementEnemy.Direction.Right : PlatformingLevelGroundMovementEnemy.Direction.Left, true);
				platformingLevelGroundMovementEnemy.Float();
				hashadSuccessfulSpawn = true;
			}
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
