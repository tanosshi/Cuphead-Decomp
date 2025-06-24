using UnityEngine;

public class MapLayerChanger : AbstractMonoBehaviour
{
	[SerializeField]
	private int sortingOrder;

	protected override void OnTriggerEnter2D(Collider2D collider)
	{
		base.OnTriggerEnter2D(collider);
		SpriteRenderer[] componentsInChildren = collider.GetComponentsInChildren<SpriteRenderer>();
		SpriteRenderer[] array = componentsInChildren;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			spriteRenderer.sortingOrder = sortingOrder;
		}
	}

	protected override void OnTriggerStay2D(Collider2D collider)
	{
		base.OnTriggerStay2D(collider);
		SpriteRenderer[] componentsInChildren = collider.GetComponentsInChildren<SpriteRenderer>();
		SpriteRenderer[] array = componentsInChildren;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			spriteRenderer.sortingOrder = sortingOrder;
		}
	}
}
