using System;
using System.Collections;
using UnityEngine;

public class ClownLevelAnimationManager : AbstractPausableComponent
{
	private const float ROTATE_FRAME_TIME = 1f / 12f;

	[SerializeField]
	private Animator headSprite;

	[SerializeField]
	private Transform balloonSprite;

	[SerializeField]
	private Transform pivotPoint;

	[SerializeField]
	private Animator[] animations;

	private bool invert;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		headSprite = headSprite.GetComponent<Animator>();
		pivotPoint.position = balloonSprite.position;
		StartCoroutine(head_cr());
		StartCoroutine(balloon_loop_cr());
		Animator[] array = animations;
		foreach (Animator ani in array)
		{
			StartCoroutine(twentyfour_fps_animation_cr(ani));
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	private IEnumerator head_cr()
	{
		while (true)
		{
			float getSeconds = UnityEngine.Random.Range(3f, 8f);
			headSprite.SetTrigger("Continue");
			yield return CupheadTime.WaitForSeconds(this, getSeconds);
		}
	}

	private IEnumerator balloon_loop_cr()
	{
		float loopSize = 20f;
		float speed = 1f;
		float angle = 0f;
		while (true)
		{
			Vector3 pivotOffset = Vector3.left * 2f * loopSize;
			angle += speed * (float)CupheadTime.Delta;
			if (angle > (float)Math.PI * 2f)
			{
				invert = !invert;
				angle -= (float)Math.PI * 2f;
			}
			if (angle < 0f)
			{
				angle += (float)Math.PI * 2f;
			}
			float value;
			if (invert)
			{
				balloonSprite.position = pivotPoint.position + pivotOffset;
				value = 1f;
			}
			else
			{
				balloonSprite.position = pivotPoint.position;
				value = -1f;
			}
			Vector3 handleRotationX = new Vector3(Mathf.Cos(angle) * value * loopSize, 0f, 0f);
			Vector3 handleRotationY = new Vector3(0f, Mathf.Sin(angle) * loopSize, 0f);
			balloonSprite.position += handleRotationX + handleRotationY;
			yield return null;
		}
	}

	private IEnumerator twentyfour_fps_animation_cr(Animator ani)
	{
		float frameTime = 0f;
		while (true)
		{
			frameTime += (float)CupheadTime.Delta;
			if (frameTime > 1f / 12f)
			{
				frameTime -= 1f / 12f;
				ani.enabled = true;
				ani.Update(1f / 12f);
				ani.enabled = false;
			}
			yield return null;
		}
	}
}
