using UnityEngine;

public class PlatformingLevelJumpTrigger : AbstractCollidableObject
{
	[SerializeField]
	private PlatformingLevelGroundMovementEnemy.Direction direction;

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionEnemy(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			PlatformingLevelGroundMovementEnemy component = hit.GetComponent<PlatformingLevelGroundMovementEnemy>();
			if (component != null && component.direction == direction)
			{
				component.Jump();
			}
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.2f);
	}

	private void DrawGizmos(float a)
	{
		BoxCollider2D component = GetComponent<BoxCollider2D>();
		Gizmos.color = new Color(1f, 1f, 0f, a);
		Gizmos.DrawWireCube(component.bounds.center, component.bounds.size);
	}
}
