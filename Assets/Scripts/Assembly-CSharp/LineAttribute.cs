using UnityEngine;

public class LineAttribute : PropertyAttribute
{
	public int height;

	public LineAttribute(int height)
	{
		this.height = height;
	}
}
