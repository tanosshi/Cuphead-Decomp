using System.Collections.Generic;
using UnityEngine;

public class Path2D : MonoBehaviour
{
	public enum Space
	{
		Global = 0,
		Local = 1
	}

	public Space space;

	public List<Vector2> nodes = new List<Vector2>(2);

	protected virtual void OnDrawGizmos()
	{
		DrawGizmos(0.1f);
	}

	protected virtual void OnDrawGizmosSelected()
	{
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (i > 0)
			{
				Gizmos.DrawLine(nodes[i], nodes[i - 1]);
			}
		}
	}
}
