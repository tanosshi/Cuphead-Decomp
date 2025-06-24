using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlatformingLevelEditorCircle : AbstractMonoBehaviour
{
	private CircleCollider2D _circleCollider;

	private CircleCollider2D circleCollider
	{
		get
		{
			if (_circleCollider == null)
			{
				_circleCollider = GetComponent<CircleCollider2D>();
			}
			return _circleCollider;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.5f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
	}

	private void DrawGizmos(float alpha)
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere((Vector2)base.transform.position + circleCollider.offset, circleCollider.radius);
	}
}
