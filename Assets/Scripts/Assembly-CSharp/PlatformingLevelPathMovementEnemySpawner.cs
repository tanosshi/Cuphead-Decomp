using UnityEngine;

public class PlatformingLevelPathMovementEnemySpawner : PlatformingLevelEnemySpawner
{
	public PlatformingLevelPathMovementEnemy enemyPrefab;

	[Header("Path")]
	public float startPosition = 0.5f;

	public PlatformingLevelPathMovementEnemy.Direction direction = PlatformingLevelPathMovementEnemy.Direction.Forward;

	public VectorPath path;

	protected override void Spawn()
	{
		enemyPrefab.Spawn(base.transform.position, path, startPosition, destroyEnemyAfterLeavingScreen);
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
		path.DrawGizmos(a, base.baseTransform.position);
		Gizmos.color = new Color(1f, 0f, 0f, a);
		Gizmos.DrawSphere(path.Lerp(startPosition) + base.baseTransform.position, 10f);
		Gizmos.DrawWireSphere(path.Lerp(startPosition) + base.baseTransform.position, 11f);
	}
}
