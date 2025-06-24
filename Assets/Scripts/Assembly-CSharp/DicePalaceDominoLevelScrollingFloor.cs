using UnityEngine;

public class DicePalaceDominoLevelScrollingFloor : MonoBehaviour
{
	public float speed;

	public float resetPositionX = 2808f;

	[SerializeField]
	private DicePalaceDominoLevelRandomTile[] dominoLevelRandomTiles;

	[SerializeField]
	private DicePalaceDominoLevelRandomSpike[] dominoLevelRandomSpikes;

	private void Start()
	{
		RefreshTilesAndSpikes();
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		position.x -= speed * (float)CupheadTime.Delta;
		base.transform.position = position;
		if (base.transform.position.x <= 0f)
		{
			position.x += resetPositionX;
			base.transform.position = position;
			RefreshTilesAndSpikes();
		}
	}

	private void RefreshTilesAndSpikes()
	{
		for (int i = 0; i < dominoLevelRandomTiles.Length; i++)
		{
			dominoLevelRandomTiles[i].ChangeTile();
		}
		for (int j = 0; j < dominoLevelRandomSpikes.Length; j++)
		{
			dominoLevelRandomSpikes[j].ChangeSpikes();
		}
	}
}
