using System;
using UnityEngine;

public class GeometryUtils
{
	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	public static Vector3[] GetCircle(Vector3 center, float radius, Axis axis = Axis.Y, int resolution = 128)
	{
		Vector3[] array = new Vector3[resolution];
		float num = (float)Math.PI * 2f / (float)resolution;
		for (int i = 0; i < resolution; i++)
		{
			float f = num * (float)i;
			float x = radius * Mathf.Cos(f);
			float y = radius * Mathf.Sin(f);
			array[i] = new Vector3(x, y, 0f);
		}
		Quaternion quaternion;
		switch (axis)
		{
		case Axis.X:
			quaternion = Quaternion.AngleAxis(90f, Vector3.up);
			break;
		case Axis.Y:
			quaternion = Quaternion.AngleAxis(90f, Vector3.right);
			break;
		default:
			quaternion = Quaternion.AngleAxis(0f, Vector3.up);
			break;
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = quaternion * array[j] + center;
		}
		return array;
	}
}
