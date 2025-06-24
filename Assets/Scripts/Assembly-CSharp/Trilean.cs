using UnityEngine;

public struct Trilean
{
	private int value;

	public int Value
	{
		get
		{
			return value;
		}
		set
		{
			if (value > 0)
			{
				this.value = 1;
			}
			else if (value < 0)
			{
				this.value = -1;
			}
			else
			{
				this.value = 0;
			}
		}
	}

	public Trilean(bool b)
	{
		if (b)
		{
			value = 1;
		}
		else
		{
			value = -1;
		}
	}

	public Trilean(int i)
	{
		value = i;
	}

	public Trilean(float f)
	{
		if (f == 0f)
		{
			value = 0;
		}
		else
		{
			value = (int)Mathf.Sign(f);
		}
	}

	public static implicit operator Trilean(bool b)
	{
		return new Trilean(b);
	}

	public static implicit operator bool(Trilean t)
	{
		return (t.Value >= 0) ? true : false;
	}

	public static implicit operator Trilean(int i)
	{
		return new Trilean(i);
	}

	public static implicit operator int(Trilean t)
	{
		return t.Value;
	}

	public static implicit operator Trilean(float f)
	{
		return new Trilean(f);
	}

	public static implicit operator float(Trilean t)
	{
		return t.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
