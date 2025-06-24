using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VectorPath
{
	public struct Node
	{
		public float x;

		public float y;

		public float z;

		public float distance;

		public Vector3 position
		{
			get
			{
				return new Vector3(x, y, z);
			}
		}

		public Node(Vector3 v)
		{
			x = v.x;
			y = v.y;
			z = v.z;
			distance = 0f;
		}

		public static List<Node> NewList(Vector3[] oldList)
		{
			List<Node> list = new List<Node>();
			for (int i = 0; i < oldList.Length; i++)
			{
				list.Add(new Node(oldList[i]));
			}
			return list;
		}

		public static implicit operator Node(Vector2 v)
		{
			return new Node(v);
		}

		public static implicit operator Vector2(Node t)
		{
			return new Vector2(t.x, t.y);
		}

		public static implicit operator Node(Vector3 v)
		{
			return new Node(v);
		}

		public static implicit operator Vector3(Node t)
		{
			return new Vector3(t.x, t.y, t.z);
		}
	}

	[SerializeField]
	private List<Vector3> _points = new List<Vector3>
	{
		new Vector2(-100f, 0f),
		new Vector2(100f, 0f)
	};

	[SerializeField]
	private bool _closed;

	private float _distance;

	private List<Node> __infoNodes;

	private bool _dirty = true;

	public List<Vector3> Points
	{
		get
		{
			_dirty = true;
			return _points;
		}
	}

	public bool Closed
	{
		get
		{
			return _closed;
		}
		set
		{
			_closed = value;
			_dirty = true;
			Calculate();
		}
	}

	public float Distance
	{
		get
		{
			Calculate();
			return _distance;
		}
	}

	public List<Node> infoNodes
	{
		get
		{
			if (__infoNodes == null)
			{
				_dirty = true;
				Calculate();
			}
			return __infoNodes;
		}
	}

	public static Vector3 Lerp(VectorPath path, float t)
	{
		if (path == null || path._points.Count < 1)
		{
			return Vector3.zero;
		}
		if (path._points.Count == 1)
		{
			return path._points[0];
		}
		if (path._points.Count == 2)
		{
			return Vector3.Lerp(path._points[0], path._points[1], t);
		}
		Vector3 vector = default(Vector3);
		int num = 0;
		path.Calculate();
		for (int i = 0; i < path.infoNodes.Count - 1; i++)
		{
			num = i;
			if (path.infoNodes[i + 1].distance > t)
			{
				break;
			}
		}
		Vector3 vector2 = path.infoNodes[num];
		Vector3 vector3 = path.infoNodes[num + 1];
		float distance = path.infoNodes[num].distance;
		float distance2 = path.infoNodes[num + 1].distance;
		float t2 = (t - distance) / (distance2 - distance);
		return Vector3.Lerp(path.infoNodes[num], path.infoNodes[num + 1], t2);
	}

	private void Calculate()
	{
		_dirty = false;
		__infoNodes = Node.NewList(_points.ToArray());
		if (_closed)
		{
			infoNodes.Add(new Node(_points[0]));
		}
		_distance = 0f;
		for (int i = 1; i < infoNodes.Count; i++)
		{
			_distance += Vector3.Distance(infoNodes[i - 1], infoNodes[i]);
		}
		float num = 0f;
		for (int j = 1; j < infoNodes.Count; j++)
		{
			num += Vector3.Distance(infoNodes[j - 1], infoNodes[j]);
			Node value = infoNodes[j];
			value.distance = num / _distance;
			infoNodes[j] = value;
		}
	}

	public Vector3 Lerp(float t)
	{
		return Lerp(this, t);
	}

	public Vector2 GetClosestPoint(Vector2 originalPosition, Vector2 positionToCheck, bool moveX, bool moveY)
	{
		Vector2 result = originalPosition;
		float num = float.MaxValue;
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		Vector2 zero3 = Vector2.zero;
		Vector2 zero4 = Vector2.zero;
		Vector2 zero5 = Vector2.zero;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		for (int i = 0; i < Points.Count - 1; i++)
		{
			zero = originalPosition;
			zero2 = Points[i];
			zero3 = Points[i + 1];
			zero4 = positionToCheck - zero2;
			zero5 = zero3 - zero2;
			if (moveX)
			{
				num4 = zero4.x / zero5.x;
				zero = ((num4 < 0f) ? zero2 : ((!(num4 > 1f)) ? (zero2 + zero5 * num4) : zero3));
				num5 = Vector2.Distance(positionToCheck, zero);
				if (num5 <= num)
				{
					num = num5;
					result = zero;
				}
			}
			if (moveY)
			{
				num4 = zero4.y / zero5.y;
				zero = ((num4 < 0f) ? zero2 : ((!(num4 > 1f)) ? (zero2 + zero5 * num4) : zero3));
				num5 = Vector2.Distance(positionToCheck, zero);
				if (num5 <= num)
				{
					num = num5;
					result = zero;
				}
			}
		}
		return result;
	}

	public float GetClosestNormalizedPoint(Vector2 originalPosition, Vector2 positionToCheck, bool moveX, bool moveY)
	{
		Vector2 b = originalPosition;
		float num = float.MaxValue;
		Vector2 zero = Vector2.zero;
		Node node = Vector2.zero;
		Node node2 = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		Vector2 zero3 = Vector2.zero;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		Node node3 = Vector2.zero;
		Node node4 = Vector2.zero;
		for (int i = 0; i < Points.Count - 1; i++)
		{
			zero = originalPosition;
			node = infoNodes[i];
			node2 = infoNodes[i + 1];
			zero2 = positionToCheck - (Vector2)node.position;
			zero3 = (Vector2)node2.position - (Vector2)node.position;
			if (moveX)
			{
				num4 = zero2.x / zero3.x;
				zero = ((num4 < 0f) ? ((Vector2)node) : ((!(num4 > 1f)) ? (node + zero3 * num4) : ((Vector2)node2)));
				num5 = Vector2.Distance(positionToCheck, zero);
				if (num5 <= num)
				{
					num = num5;
					b = zero;
					node3 = node;
					node4 = node2;
				}
			}
			if (moveY)
			{
				num4 = zero2.y / zero3.y;
				zero = ((num4 < 0f) ? ((Vector2)node) : ((!(num4 > 1f)) ? (node + zero3 * num4) : ((Vector2)node2)));
				num5 = Vector2.Distance(positionToCheck, zero);
				if (num5 <= num)
				{
					num = num5;
					b = zero;
					node3 = node;
					node4 = node2;
				}
			}
		}
		float num6 = Vector2.Distance(node3.position, node4.position);
		float num7 = Vector2.Distance(node3.position, b);
		return Mathf.Lerp(node3.distance, node4.distance, num7 / num6);
	}

	public void DrawGizmos(Vector3 offset)
	{
		DrawGizmos(1f, offset);
	}

	public void DrawGizmos(float a, Vector3 offset)
	{
		for (int i = 0; i < _points.Count; i++)
		{
			Gizmos.color = new Color(0f, 0f, 1f, a);
			Gizmos.DrawWireSphere(_points[i] + offset, 10f);
			if (i < _points.Count - 1)
			{
				Gizmos.color = new Color(0f, 0f, 1f, a);
				Vector3 vector = _points[i] + offset;
				Vector3 vector2 = _points[i + 1] + offset;
				Gizmos.DrawLine(vector, vector2);
				Vector3 vector3 = Vector3.Lerp(vector, vector2, 0.45f);
				Vector3 to = Vector3.Lerp(vector, vector2, 0.55f);
				Vector3 vector4 = Quaternion.Euler(0f, 0f, 90f) * (vector2 - vector).normalized * 10f;
				Gizmos.color = new Color(0f, 1f, 0f, a);
				Gizmos.DrawLine(vector3 + vector4, to);
				Gizmos.DrawLine(vector3 - vector4, to);
			}
		}
		if (Closed)
		{
			Gizmos.color = new Color(0f, 1f, 0f, a * 0.5f);
			Gizmos.DrawLine(_points[_points.Count - 1] + offset, _points[0] + offset);
		}
	}
}
