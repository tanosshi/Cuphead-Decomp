using UnityEngine;

namespace RektTransform
{
	public struct MinMax
	{
		public Vector2 min;

		public Vector2 max;

		public MinMax(Vector2 min, Vector2 max)
		{
			this.min = new Vector2(Mathf.Clamp01(min.x), Mathf.Clamp01(min.y));
			this.max = new Vector2(Mathf.Clamp01(max.x), Mathf.Clamp01(max.y));
		}

		public MinMax(float minx, float miny, float maxx, float maxy)
		{
			min = new Vector2(Mathf.Clamp01(minx), Mathf.Clamp01(miny));
			max = new Vector2(Mathf.Clamp01(maxx), Mathf.Clamp01(maxy));
		}
	}
}
