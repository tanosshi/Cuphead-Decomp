using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class PlatformingLevelEditorSlope : AbstractMonoBehaviour
{
	private PolygonCollider2D _polygonCollider;

	private PolygonCollider2D polygonCollider
	{
		get
		{
			if (_polygonCollider == null)
			{
				_polygonCollider = GetComponent<PolygonCollider2D>();
			}
			return _polygonCollider;
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
		for (int i = 0; i < polygonCollider.points.Length; i++)
		{
			Vector3 vector = polygonCollider.points[i];
			Vector3 to = ((i != polygonCollider.points.Length - 1) ? polygonCollider.points[i + 1] : polygonCollider.points[0]);
			Gizmos.DrawLine(vector, to);
		}
	}
}
