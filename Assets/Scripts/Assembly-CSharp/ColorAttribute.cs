using UnityEngine;

public class ColorAttribute : PropertyAttribute
{
	public Color color;

	public ColorAttribute(float w)
	{
		color = new Color(w, w, w, 1f);
	}

	public ColorAttribute(float r, float g, float b)
	{
		color = new Color(r, g, b, 1f);
	}

	public ColorAttribute(float r, float g, float b, float a)
	{
		color = new Color(r, g, b, a);
	}
}
