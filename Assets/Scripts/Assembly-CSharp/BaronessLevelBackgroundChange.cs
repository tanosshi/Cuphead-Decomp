using System;
using UnityEngine;

public class BaronessLevelBackgroundChange : AbstractPausableComponent
{
	public enum B_Axis
	{
		X = 0,
		Y = 1
	}

	public B_Axis b_axis;

	private int size;

	private const float X_OUT = -1280f;

	[Range(0f, 2000f)]
	public float speed;

	[SerializeField]
	private bool isClouds;

	[SerializeField]
	protected bool b_negativeDirection;

	[SerializeField]
	protected int b_offset;

	[SerializeField]
	[Range(1f, 10f)]
	protected int b_count = 1;

	[SerializeField]
	private BaronessLevelCastle baroness;

	[SerializeField]
	private OneTimeScrollingSprite sprite;

	[NonSerialized]
	public float b_playbackSpeed = 1f;

	private GameObject copy;

	private Vector3 getOffset;

	protected override void Awake()
	{
		base.Awake();
		SpriteRenderer component = base.transform.GetComponent<SpriteRenderer>();
		size = ((b_axis != B_Axis.X) ? ((int)component.sprite.bounds.size.y) : ((int)component.sprite.bounds.size.x)) - b_offset;
		getOffset.x = base.transform.position.x;
		for (int i = 0; i < b_count; i++)
		{
			copy = new GameObject(base.gameObject.name + " Copy");
			copy.transform.parent = base.transform;
			copy.transform.ResetLocalTransforms();
			SpriteRenderer spriteRenderer = copy.AddComponent<SpriteRenderer>();
			spriteRenderer.sortingLayerID = component.sortingLayerID;
			spriteRenderer.sortingOrder = component.sortingOrder;
			spriteRenderer.sprite = component.sprite;
			spriteRenderer.material = component.material;
			GameObject gameObject = UnityEngine.Object.Instantiate(copy);
			gameObject.transform.parent = base.transform;
			copy.transform.SetLocalPosition(-(size + size * i), 0f, 0f);
			gameObject.transform.SetLocalPosition(-(size * 2 + size * i), 0f, 0f);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!isClouds && baroness.state != BaronessLevelCastle.State.Chase)
		{
			return;
		}
		if (!baroness.pauseScrolling)
		{
			if (GetComponent<ParallaxLayer>() != null)
			{
				GetComponent<ParallaxLayer>().enabled = false;
			}
			if (sprite != null)
			{
				sprite.speed = 0f - speed;
			}
			Vector3 localPosition = base.transform.localPosition;
			if (localPosition.x >= 0f - ((float)size - getOffset.x))
			{
				localPosition.x -= size;
			}
			if (localPosition.x <= (float)size - getOffset.x)
			{
				localPosition.x += size;
			}
			localPosition.x -= (float)((!b_negativeDirection) ? 1 : (-1)) * speed * (float)CupheadTime.Delta * b_playbackSpeed;
			base.transform.localPosition = localPosition;
		}
		else if (sprite != null)
		{
			sprite.GetComponent<OneTimeScrollingSprite>().speed = 0f;
		}
	}
}
