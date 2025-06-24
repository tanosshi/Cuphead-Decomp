using System;
using UnityEngine;

[Serializable]
public class CupheadBounds
{
	public float left;

	public float right;

	public float top;

	public float bottom;

	public CupheadBounds()
	{
		left = 0f;
		right = 0f;
		top = 0f;
		bottom = 0f;
	}

	public CupheadBounds(float left, float right, float top, float bottom)
	{
		this.left = left;
		this.right = right;
		this.top = top;
		this.bottom = bottom;
	}

	public CupheadBounds(Rect r)
	{
		left = r.center.x - r.x;
		top = r.center.y - r.y;
		right = r.xMax - r.center.x;
		bottom = r.yMax - r.center.y;
	}

	public static implicit operator CupheadBounds(Rect r)
	{
		return new CupheadBounds(r);
	}

	public CupheadBounds Copy()
	{
		return MemberwiseClone() as CupheadBounds;
	}
}
