using System;
using UnityEngine;

public static class EnumUtils
{
	public static T[] GetValues<T>()
	{
		if (!typeof(T).IsEnum)
		{
			throw new ArgumentException("T must be an enum type");
		}
		return (T[])Enum.GetValues(typeof(T));
	}

	public static string[] GetValuesAsStrings<T>()
	{
		T[] values = GetValues<T>();
		string[] array = new string[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = values[i].ToString();
		}
		return array;
	}

	public static int GetCount<T>()
	{
		return GetValues<T>().Length;
	}

	public static T Random<T>()
	{
		T[] values = GetValues<T>();
		return values[UnityEngine.Random.Range(0, values.Length)];
	}

	public static T Parse<T>(string name)
	{
		T[] values = GetValues<T>();
		for (int i = 0; i < values.Length; i++)
		{
			if (name == values[i].ToString())
			{
				return values[i];
			}
		}
		return values[0];
	}

	public static bool TryParse<T>(string name, out T result)
	{
		T[] values = GetValues<T>();
		for (int i = 0; i < values.Length; i++)
		{
			if (name == values[i].ToString())
			{
				result = values[i];
				return true;
			}
		}
		result = values[0];
		return false;
	}
}
