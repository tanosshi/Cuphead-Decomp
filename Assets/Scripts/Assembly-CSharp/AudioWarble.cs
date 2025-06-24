using System;
using UnityEngine;

public class AudioWarble : AbstractPausableComponent
{
	[Serializable]
	public class WarbleAttributes
	{
		public float minVal;

		public float maxVal;

		public float warbleTime;

		public float playTime;
	}

	[SerializeField]
	private WarbleAttributes[] warbles;

	public void HandleWarble()
	{
		float[] array = new float[warbles.Length];
		float[] array2 = new float[warbles.Length];
		float[] array3 = new float[warbles.Length];
		float[] array4 = new float[warbles.Length];
		for (int i = 0; i < warbles.Length; i++)
		{
			array[i] = warbles[i].minVal;
			array2[i] = warbles[i].maxVal;
			array3[i] = warbles[i].warbleTime;
			array4[i] = warbles[i].playTime;
		}
		AudioManager.WarbleBGMPitch(array, array2, array3, array4);
	}
}
