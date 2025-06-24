using System;
using System.Collections;
using UnityEngine;

public class MouseLevelBackgroundHopper : AbstractMonoBehaviour
{
	[Serializable]
	public class Hop
	{
		public float height = 50f;

		public float time = 0.25f;
	}

	public Hop[] hops = new Hop[1];

	private Coroutine hopCoroutine;

	private Vector2 startPos;

	protected override void Awake()
	{
		base.Awake();
		startPos = base.transform.localPosition;
	}

	protected override void Start()
	{
		base.Start();
		CupheadLevelCamera.Current.OnShakeEvent += OnShake;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (CupheadLevelCamera.Current != null)
		{
			RemoveShake();
		}
	}

	private void RemoveShake()
	{
		CupheadLevelCamera.Current.OnShakeEvent -= OnShake;
	}

	private void OnShake(float amount, float time)
	{
		if (hopCoroutine != null)
		{
			StopCoroutine(hopCoroutine);
		}
		hopCoroutine = StartCoroutine(hop_cr(amount));
	}

	private IEnumerator hop_cr(float amount)
	{
		for (int i = 0; i < hops.Length; i++)
		{
			float height = hops[i].height;
			float time = hops[i].time;
			Vector2 endPos = startPos + new Vector2(0f, height);
			float ht = time / 2f;
			yield return StartCoroutine(tween_cr(startPos.y, endPos.y, ht, EaseUtils.EaseType.easeOutSine));
			yield return StartCoroutine(tween_cr(endPos.y, startPos.y, ht, EaseUtils.EaseType.easeInSine));
			time /= 2f;
			height /= 2f;
		}
	}

	private IEnumerator tween_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		base.transform.SetLocalPosition(startPos.x, start, 0f);
		float t = 0f;
		while (t < time)
		{
			TransformExtensions.SetLocalPosition(y: EaseUtils.Ease(ease, start, end, t / time), transform: base.transform, x: startPos.x, z: 0f);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetLocalPosition(startPos.x, end, 0f);
	}
}
