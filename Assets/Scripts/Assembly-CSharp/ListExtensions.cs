using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
	public static void Move<T>(this List<T> list, int index, int direction)
	{
		if (direction < 0)
		{
			if (index != 0)
			{
				T value = list[index - 1];
				list[index - 1] = list[index];
				list[index] = value;
			}
		}
		else if (direction > 0 && index < list.Count - 1)
		{
			T value2 = list[index + 1];
			list[index + 1] = list[index];
			list[index] = value2;
		}
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			int index = Random.Range(i, list.Count);
			T value = list[i];
			list[i] = list[index];
			list[index] = value;
		}
	}

	public static T RandomChoice<T>(this IList<T> list)
	{
		return list[Random.Range(0, list.Count)];
	}
}
