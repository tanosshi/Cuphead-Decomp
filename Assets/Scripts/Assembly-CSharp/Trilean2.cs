using UnityEngine;

public struct Trilean2
{
	public Trilean x;

	public Trilean y;

	public Trilean2(Vector2 v)
	{
		x = v.x;
		y = v.y;
	}

	public Trilean2(bool x, bool y)
	{
		this.x = x;
		this.y = y;
	}

	public Trilean2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Trilean2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public static implicit operator Trilean2(Vector2 v)
	{
		return new Trilean2(v);
	}

	public static implicit operator Vector2(Trilean2 t)
	{
		return new Vector2(t.x, t.y);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return "Trilean2(x:" + x.Value + ", y:" + y.Value + ")";
	}

	public static bool operator ==(Trilean2 a, Trilean2 b)
	{
		return a.x.Value == b.x.Value && a.y.Value == b.y.Value;
	}

	public static bool operator !=(Trilean2 a, Trilean2 b)
	{
		return a.x.Value != b.x.Value || a.y.Value != b.y.Value;
	}
}
