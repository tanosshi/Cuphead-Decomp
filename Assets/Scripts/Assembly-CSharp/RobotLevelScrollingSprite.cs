using UnityEngine;

public class RobotLevelScrollingSprite : ScrollingSpriteSpawner
{
	[SerializeField]
	private SpriteLayer layer = SpriteLayer.Default;

	[SerializeField]
	private MinMax yOffset;

	[SerializeField]
	private Sprite[] sprites;

	protected override void OnSpawn(GameObject obj)
	{
		base.OnSpawn(obj);
		SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
		component.sortingLayerName = layer.ToString();
		component.sprite = sprites[Random.Range(0, sprites.Length)];
		Vector3 vector = Vector3.up * Random.Range(yOffset.min, yOffset.max);
		obj.transform.position += vector;
		obj.transform.localScale = new Vector3(base.transform.localScale.x * (float)MathUtils.PlusOrMinus(), base.transform.localScale.y, base.transform.localScale.z);
	}
}
