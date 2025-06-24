using System.Globalization;
using UnityEngine;

public class ColorUtils
{
	public static Color GetAverageColor(Color[] colors)
	{
		return GetAverageColor(colors, 1);
	}

	public static Color GetAverageColor(Color[] colors, int quality)
	{
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		for (num = 0; num < colors.Length && num < colors.Length; num += quality)
		{
			num3 += colors[num].r;
			num4 += colors[num].g;
			num5 += colors[num].b;
			num2++;
		}
		num3 /= (float)num2;
		num4 /= (float)num2;
		num5 /= (float)num2;
		return new Color(num3, num4, num5);
	}

	public static string ColorToHex(Color32 color, bool alpha = false)
	{
		string text = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		if (alpha)
		{
			text += color.a.ToString("X2");
		}
		return text;
	}

	public static Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		return new Color32(r, g, b, byte.MaxValue);
	}
}
