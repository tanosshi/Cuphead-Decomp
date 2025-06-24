using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelEelSegment : AbstractPausableComponent
{
	[SerializeField]
	private float gravity;

	[SerializeField]
	private MinMax angleRange;

	[SerializeField]
	private float launchSpeed;

	[SerializeField]
	private float despawnY;

	private Vector2 velocity;

	public FlyingMermaidLevelEelSegment Create(Vector2 position, string sortingLayer, int sortingOrder)
	{
		FlyingMermaidLevelEelSegment flyingMermaidLevelEelSegment = Object.Instantiate(this);
		flyingMermaidLevelEelSegment.transform.position = position;
		flyingMermaidLevelEelSegment.velocity = launchSpeed * MathUtils.AngleToDirection(angleRange.RandomFloat());
		flyingMermaidLevelEelSegment.transform.Rotate(0f, 0f, Random.Range(0f, 360f));
		if (Random.Range(0f, 1f) > 0.5f)
		{
			flyingMermaidLevelEelSegment.transform.SetScale(-1f);
		}
		SpriteRenderer component = flyingMermaidLevelEelSegment.GetComponent<SpriteRenderer>();
		component.sortingLayerName = sortingLayer;
		component.sortingOrder = sortingOrder;
		flyingMermaidLevelEelSegment.animator.Play("Idle", 0, Random.Range(0f, 1f));
		flyingMermaidLevelEelSegment.StartCoroutine(flyingMermaidLevelEelSegment.move_cr());
		return flyingMermaidLevelEelSegment;
	}

	private IEnumerator move_cr()
	{
		while (base.transform.position.y > despawnY || velocity.y > 0f)
		{
			velocity.y -= gravity * (float)CupheadTime.Delta;
			base.transform.AddPosition(velocity.x * (float)CupheadTime.Delta, velocity.y * (float)CupheadTime.Delta);
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
