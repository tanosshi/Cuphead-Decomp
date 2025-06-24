namespace TMPro
{
	public static class TMP_Math
	{
		public static bool Approximately(float a, float b)
		{
			return b - 0.0001f < a && a < b + 0.0001f;
		}
	}
}
