using UnityEngine;

public class DicePalaceDominoLevelRandomTile : AbstractMonoBehaviour
{
	public Sprite[] sprites;

	public void ChangeTile()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (!(component == null))
		{
			component.sprite = sprites[Random.Range(0, sprites.Length)];
		}
	}
}
