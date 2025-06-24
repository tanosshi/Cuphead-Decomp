public static class StringExtensions
{
	public static string UpperFirst(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		char[] array = str.ToCharArray();
		array[0] = char.ToUpper(array[0]);
		return new string(array);
	}

	public static string LowerFirst(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		char[] array = str.ToCharArray();
		array[0] = char.ToLower(array[0]);
		return new string(array);
	}

	public static string UppercaseWords(this string str)
	{
		char[] array = str.ToCharArray();
		if (array.Length >= 1 && char.IsLower(array[0]))
		{
			array[0] = char.ToUpper(array[0]);
		}
		for (int i = 1; i < array.Length; i++)
		{
			if ((array[i - 1] == ' ' || array[i - 1] == '_' || array[i - 1] == '/') && char.IsLower(array[i]))
			{
				array[i] = char.ToUpper(array[i]);
			}
		}
		return new string(array);
	}
}
