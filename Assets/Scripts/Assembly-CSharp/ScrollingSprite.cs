using System;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingSprite : AbstractPausableComponent
{
	public enum Axis
	{
		X = 0,
		Y = 1
	}

	public Axis axis;

	[SerializeField]
	protected bool negativeDirection;

	[SerializeField]
	private bool onLeft;

	[SerializeField]
	private bool isRotated;

	[Range(0f, 4000f)]
	public float speed;

	[SerializeField]
	private float offset;

	[SerializeField]
	[Range(1f, 10f)]
	private int count = 1;

	[NonSerialized]
	public float playbackSpeed = 1f;

	protected float size;

	private Vector3 pos;

	private float startY;

	protected int direction;

	public List<SpriteRenderer> copyRenderers { get; private set; }

	protected override void Start()
	{
		base.Start();
		copyRenderers = new List<SpriteRenderer>();
		direction = ((!negativeDirection) ? 1 : (-1));
		SpriteRenderer component = base.transform.GetComponent<SpriteRenderer>();
		copyRenderers.Add(component);
		size = ((axis != Axis.X) ? component.sprite.bounds.size.y : component.sprite.bounds.size.x) - offset;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = new GameObject(base.gameObject.name + " Copy");
			gameObject.transform.parent = base.transform;
			gameObject.transform.ResetLocalTransforms();
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sortingLayerID = component.sortingLayerID;
			spriteRenderer.sortingOrder = component.sortingOrder;
			spriteRenderer.sprite = component.sprite;
			spriteRenderer.material = component.material;
			copyRenderers.Add(spriteRenderer);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			gameObject2.transform.parent = base.transform;
			gameObject2.transform.ResetLocalTransforms();
			copyRenderers.Add(gameObject2.GetComponent<SpriteRenderer>());
			if (axis == Axis.X)
			{
				gameObject.transform.SetLocalPosition((float)direction * (size + size * (float)i), 0f, 0f);
				gameObject2.transform.SetLocalPosition((float)direction * (0f - (size + size * (float)i)), 0f, 0f);
			}
			else
			{
				gameObject.transform.SetLocalPosition(0f, size + size * (float)i, 0f);
				gameObject2.transform.SetLocalPosition(0f, 0f - (size + size * (float)i), 0f);
			}
		}
		startY = base.transform.localPosition.y;
	}

	protected override void Update()
	{
		base.Update();
		pos = base.transform.localPosition;
		if (axis == Axis.X)
		{
			if (pos.x <= 0f - size)
			{
				pos.x += size;
				if (isRotated)
				{
					pos.y = startY;
				}
			}
			if (pos.x >= size)
			{
				pos.x -= size;
			}
			if (!isRotated)
			{
				pos.x -= (float)((!negativeDirection) ? 1 : (-1)) * speed * (float)CupheadTime.Delta * playbackSpeed;
			}
		}
		else
		{
			if (pos.y <= 0f - size)
			{
				pos.y += size;
			}
			if (pos.y >= size)
			{
				pos.y -= size;
			}
			if (!isRotated)
			{
				pos.y -= (float)((!negativeDirection) ? 1 : (-1)) * speed * (float)CupheadTime.Delta * playbackSpeed;
			}
		}
		if (isRotated)
		{
			pos -= base.transform.right * speed * CupheadTime.Delta;
		}
		base.transform.localPosition = pos;
	}
}
