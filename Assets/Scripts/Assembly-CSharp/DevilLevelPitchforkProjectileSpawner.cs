using System.Collections.Generic;
using UnityEngine;

public class DevilLevelPitchforkProjectileSpawner
{
	private int numProjectiles;

	private string[] angleOffsets;

	private int angleOffsetIndex;

	public DevilLevelPitchforkProjectileSpawner(int numProjectiles, string angleOffsets)
	{
		this.numProjectiles = numProjectiles;
		this.angleOffsets = angleOffsets.Split(',');
		angleOffsetIndex = Random.Range(0, angleOffsets.Length);
	}

	public List<float> getSpawnAngles()
	{
		List<float> list = new List<float>();
		angleOffsetIndex = (angleOffsetIndex + 1) % angleOffsets.Length;
		float result = 0f;
		float.TryParse(angleOffsets[angleOffsetIndex], out result);
		for (int i = 0; i < numProjectiles; i++)
		{
			list.Add((float)i * 360f / (float)numProjectiles + result + 90f);
		}
		return list;
	}
}
