using UnityEngine;

public static class Vector2Extensions
{
	public static Vector2 Set(this Vector2 v, float? x = null, float? y = null)
	{
		Vector2 result = v;
		if (x.HasValue)
		{
			result.x = x.Value;
		}
		if (y.HasValue)
		{
			result.y = y.Value;
		}
		return result;
	}
}
