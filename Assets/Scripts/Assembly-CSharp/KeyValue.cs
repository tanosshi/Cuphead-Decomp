using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyValue
{
	public const char PAIR_SEPARATOR = ',';

	public const char VALUE_SEPARATOR = ':';

	public string key = string.Empty;

	public float value;

	public KeyValue()
	{
	}

	public KeyValue(string key, float value)
	{
		this.key = key;
		this.value = value;
	}

	public static KeyValue[] ListFromString(string keyValueString, char[] allowedCharacters)
	{
		List<KeyValue> list = new List<KeyValue>();
		List<char> list2 = new List<char>(allowedCharacters);
		list2.Add(',');
		list2.Add(':');
		keyValueString.Replace(" ", string.Empty);
		for (int i = 0; i < keyValueString.Length; i++)
		{
			bool flag = true;
			foreach (char item in list2)
			{
				if (keyValueString[i] == item)
				{
					flag = false;
				}
			}
			if (flag)
			{
				keyValueString.Remove(i, 1);
			}
		}
		string[] array = keyValueString.Split(',');
		for (int j = 0; j < array.Length; j++)
		{
			string[] array2 = array[j].Split(':');
			if (array2.Length == 2)
			{
				string text = array2[0].Replace(" ", string.Empty);
				float result = 0f;
				if (!float.TryParse(array2[1], out result) || text == null || text == string.Empty)
				{
					Debug.Log("KeyValue pair: " + text + ":" + result + " not properly formatted");
				}
				else
				{
					list.Add(new KeyValue(text, result));
				}
			}
		}
		return list.ToArray();
	}

	public KeyValue Clone()
	{
		return new KeyValue(key, value);
	}
}
