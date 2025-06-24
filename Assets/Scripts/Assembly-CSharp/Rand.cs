using UnityEngine;

public static class Rand
{
	public static bool Bool()
	{
		return Random.Range(0, 2) == 1;
	}

	public static int PosOrNeg()
	{
		return Bool() ? 1 : (-1);
	}
}
