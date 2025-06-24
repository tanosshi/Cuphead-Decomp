using System.Collections;
using UnityEngine;

public class OneTimeScrollingSprite : AbstractPausableComponent
{
	private const float X_OUT = -1280f;

	public float speed;

	protected override void Start()
	{
		StartCoroutine(loop_cr());
	}

	private IEnumerator loop_cr()
	{
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		while (base.transform.position.x + spriteRenderer.bounds.size.x / 2f > -1280f)
		{
			Vector2 position = base.transform.localPosition;
			position.x -= speed * (float)CupheadTime.Delta;
			base.transform.localPosition = position;
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
