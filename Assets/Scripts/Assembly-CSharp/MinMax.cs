using System;
using UnityEngine;

[Serializable]
public class MinMax
{
	public float min;

	public float max;

	public MinMax(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	public float RandomFloat()
	{
		return UnityEngine.Random.Range(min, max);
	}

	public int RandomInt()
	{
		int num = (int)min;
		int num2 = (int)max;
		return UnityEngine.Random.Range(num, num2);
	}

	public float GetFloatAt(float i)
	{
		return Mathf.Lerp(min, max, i);
	}

	public float GetIntAt(float i)
	{
		return (int)Mathf.Lerp(min, max, i);
	}

	public MinMax Clone()
	{
		return new MinMax(min, max);
	}

	public static implicit operator float(MinMax m)
	{
		return m.RandomFloat();
	}

	public static implicit operator int(MinMax m)
	{
		return m.RandomInt();
	}
}
