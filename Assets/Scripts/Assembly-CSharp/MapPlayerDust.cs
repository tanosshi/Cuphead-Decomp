using System.Collections;
using UnityEngine;

public class MapPlayerDust : Effect
{
	private const string StartAnimTrigger = "startAnim";

	[SerializeField]
	private MinMax scaleRange;

	[SerializeField]
	private MinMax opacityRange;

	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	public Effect Create(Vector3 position, float offsetRotation, bool isLeft, int sortingOrder)
	{
		Vector3 vector = Vector3.right * offset.x;
		if (isLeft)
		{
			vector *= -1f;
		}
		Vector3 vector2 = Quaternion.Euler(offsetRotation, 0f, 0f) * vector;
		vector2.y += offset.y;
		spriteRenderer.sortingOrder = sortingOrder;
		position.z = position.y - 0.01f;
		return Create(position + vector2);
	}

	protected override void Initialize(Vector3 position, Vector3 scale, bool randomR)
	{
		base.Initialize(position, scale * Random.Range(scaleRange.min, scaleRange.max), randomR);
		Color color = spriteRenderer.color;
		color.a *= Random.Range(opacityRange.min, opacityRange.max);
		spriteRenderer.color = color;
		animator.SetTrigger("startAnim");
		StartCoroutine(dust_cr());
	}

	private IEnumerator dust_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		OnEffectComplete();
	}
}
