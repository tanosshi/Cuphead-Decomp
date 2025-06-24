using System;
using UnityEngine;

public class JitterGameObject : MonoBehaviour
{
	public float jitterAmplitude = 0.1f;

	private float jitterDelay = 0.1f;

	private float currentJitterDelay;

	private Transform tr;

	private Vector3 startingPosition;

	private void Start()
	{
		jitterDelay = 1f / 12f;
		tr = base.transform;
		startingPosition = tr.position;
	}

	private void Update()
	{
		currentJitterDelay -= CupheadTime.Delta;
		if (currentJitterDelay <= 0f)
		{
			currentJitterDelay = jitterDelay;
			float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
			tr.position = startingPosition + new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f) * jitterAmplitude;
		}
	}
}
