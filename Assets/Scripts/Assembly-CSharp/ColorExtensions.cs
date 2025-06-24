using UnityEngine;

public static class ColorExtensions
{
	public static string ToHex(this Color color)
	{
		return ColorUtils.ColorToHex(color);
	}

	public static string ToHex(this Color color, bool alpha)
	{
		return ColorUtils.ColorToHex(color, alpha);
	}

	public static string ToNiceString(this Color color)
	{
		return "R:" + color.r + " G:" + color.g + " B:" + color.b + " A:" + color.a;
	}
}
