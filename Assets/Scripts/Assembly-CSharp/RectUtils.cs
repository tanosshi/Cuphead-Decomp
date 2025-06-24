using UnityEngine;

public class RectUtils
{
	public static Rect OffsetRect(Rect rect, int offset)
	{
		return new Rect(rect.x - (float)offset, rect.y - (float)offset, rect.width + (float)(offset * 2), rect.height + (float)(offset * 2));
	}

	public static Rect[] HorizontalDivide(Rect rect, int sections, float space)
	{
		Rect[] array = new Rect[sections];
		float num = rect.width / (float)sections - space * (float)(sections - 1) / (float)sections;
		for (int i = 0; i < sections; i++)
		{
			array[i] = new Rect(rect.x + num * (float)i + space * (float)i, rect.y, num, rect.height);
		}
		return array;
	}

	public static Rect NewFromCenter(float xCenter, float yCenter, float width, float height)
	{
		return new Rect(xCenter - width * 0.5f, yCenter - height * 0.5f, width, height);
	}
}
