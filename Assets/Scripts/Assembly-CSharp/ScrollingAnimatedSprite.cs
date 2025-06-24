using System;
using UnityEngine;

public class ScrollingAnimatedSprite : AbstractPausableComponent
{
	public enum Axis
	{
		X = 0,
		Y = 1
	}

	public Axis axis;

	[SerializeField]
	private bool negativeDirection;

	[SerializeField]
	[Range(0f, 2000f)]
	public float speed;

	[SerializeField]
	private int offset;

	[SerializeField]
	[Range(1f, 10f)]
	private int count = 1;

	[NonSerialized]
	public float playbackSpeed = 1f;

	private int size;

	private static bool copying;

	protected override void Awake()
	{
		base.Awake();
		if (copying)
		{
			return;
		}
		copying = true;
		SpriteRenderer component = base.transform.GetComponent<SpriteRenderer>();
		Animator component2 = base.transform.GetComponent<Animator>();
		size = ((axis != Axis.X) ? ((int)component.sprite.bounds.size.y) : ((int)component.sprite.bounds.size.x)) - offset;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(base.gameObject);
			gameObject.GetComponent<ScrollingAnimatedSprite>().enabled = false;
			gameObject2.GetComponent<ScrollingAnimatedSprite>().enabled = false;
			gameObject.transform.SetParent(base.transform);
			gameObject2.transform.SetParent(base.transform);
			if (axis == Axis.X)
			{
				gameObject.transform.SetLocalPosition(size + size * i, 0f, 0f);
				gameObject2.transform.SetLocalPosition(-(size + size * i), 0f, 0f);
			}
			else
			{
				gameObject.transform.SetLocalPosition(0f, size + size * i, 0f);
				gameObject2.transform.SetLocalPosition(0f, -(size + size * i), 0f);
			}
		}
		copying = false;
	}

	protected override void Update()
	{
		base.Update();
		Vector3 localPosition = base.transform.localPosition;
		if (axis == Axis.X)
		{
			if (localPosition.x <= (float)(-size))
			{
				localPosition.x += size;
			}
			if (localPosition.x >= (float)size)
			{
				localPosition.x -= size;
			}
			localPosition.x -= (float)((!negativeDirection) ? 1 : (-1)) * speed * (float)CupheadTime.Delta * playbackSpeed;
		}
		else
		{
			if (localPosition.y <= (float)(-size))
			{
				localPosition.y += size;
			}
			if (localPosition.y >= (float)size)
			{
				localPosition.y -= size;
			}
			localPosition.y -= (float)((!negativeDirection) ? 1 : (-1)) * speed * (float)CupheadTime.Delta * playbackSpeed;
		}
		base.transform.localPosition = localPosition;
	}
}
