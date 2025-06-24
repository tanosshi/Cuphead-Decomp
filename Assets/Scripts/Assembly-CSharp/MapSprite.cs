using UnityEngine;

public class MapSprite : AbstractPausableComponent
{
	protected virtual bool ChangesDepth
	{
		get
		{
			return true;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		SetLayer(GetComponent<SpriteRenderer>());
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer layer in componentsInChildren)
		{
			SetLayer(layer);
		}
	}

	protected void SetLayer(SpriteRenderer renderer)
	{
		if (ChangesDepth && !(renderer == null))
		{
			renderer.sortingLayerName = "Map";
			renderer.sortingOrder = 0;
		}
	}

	protected override void Update()
	{
		base.Update();
		Vector3 position = base.transform.position;
		base.transform.position = new Vector3(position.x, position.y, position.y);
	}
}
