using System;
using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelFloater : AbstractPausableComponent
{
	private const float BOB_FRAME_TIME = 1f / 24f;

	public float bobAmount;

	public float rotateAmount;

	public float defaultRotation;

	public GameObject trackingWater;

	public float bobSpeed;

	protected override void Start()
	{
		StartCoroutine(loop_cr());
	}

	private IEnumerator loop_cr()
	{
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		float waveWidth = 0f;
		if (trackingWater != null)
		{
			SpriteRenderer component = trackingWater.GetComponent<SpriteRenderer>();
			waveWidth = component.bounds.size.x / 2f;
		}
		float lastY = base.transform.localPosition.y;
		float relativeBobY = 0f;
		float originY = base.transform.localPosition.y;
		float t = 0f;
		float frameTime = 0f;
		while (true)
		{
			if (!base.enabled)
			{
				yield return null;
				continue;
			}
			t += (float)CupheadTime.Delta;
			frameTime += (float)CupheadTime.Delta;
			while (frameTime > 1f / 24f)
			{
				frameTime -= 1f / 24f;
				Quaternion rotation2 = base.transform.rotation;
				float num;
				float num2;
				if (trackingWater != null)
				{
					float x = trackingWater.transform.position.x;
					float x2 = base.transform.position.x;
					num = 1f - Mathf.Cos((x2 - x) * 2f * ((float)Math.PI / waveWidth));
					num2 = Mathf.Sin((x2 - x) * 2f * ((float)Math.PI / waveWidth));
				}
				else
				{
					num = 1f - Mathf.Cos(t * bobSpeed * 2f * (float)Math.PI);
					num2 = Mathf.Sin(t * bobSpeed * 2f * (float)Math.PI);
				}
				relativeBobY = num * bobAmount;
				originY += base.transform.localPosition.y - lastY;
				Quaternion rotation = Quaternion.AngleAxis(defaultRotation + num2 * rotateAmount, Vector3.forward);
				base.transform.SetPosition(null, originY + relativeBobY);
				base.transform.rotation = rotation;
				lastY = base.transform.localPosition.y;
			}
			yield return null;
		}
	}
}
