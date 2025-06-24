using UnityEngine;

public class DicePalacePachinkoLevelPlatforms : AbstractCollidableObject
{
	private const int NUMBER_OF_ROWS = 3;

	private const int MAX_NUMBER_OF_COLUMNS = 4;

	[SerializeField]
	private SpriteRenderer platformSprite;

	private LevelProperties.DicePalacePachinko properties;

	public void InitPlatforms(LevelProperties.DicePalacePachinko properties)
	{
		this.properties = properties;
		Vector3 vector = default(Vector3);
		vector.y = Level.Current.Ground;
		vector.x = Level.Current.Left;
		vector.z = 0f;
		Vector2 vector2 = default(Vector2);
		for (int i = 0; i < 3; i++)
		{
			int num = ((i == 0) ? 3 : ((i % 2 != 0) ? 4 : 3));
			for (int j = 0; j < num; j++)
			{
				GameObject gameObject = platformSprite.gameObject;
				if (num == 4)
				{
					vector2.x = properties.CurrentState.pachinko.platformWidthFour;
				}
				else
				{
					vector2.x = properties.CurrentState.pachinko.platformWidthThree;
				}
				vector2.y = 1f;
				gameObject.transform.localScale = vector2;
				Vector3 position = vector;
				if (num == 3)
				{
					float num2 = (float)Level.Current.Width - gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x * 3.6f;
					position.x = position.x + num2 / (float)(num - 1) * (float)j + gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x * 1.8f;
				}
				else
				{
					position.x += Level.Current.Width / (num - 1) * j;
				}
				position.y = (float)Level.Current.Ground + float.Parse(properties.CurrentState.pachinko.platformHeights.Split(',')[i]) + gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y / 2f;
				if (j == 0)
				{
					position.x += gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f;
				}
				else if (j == num - 1)
				{
					position.x -= gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2f;
				}
				GameObject gameObject2 = new GameObject();
				gameObject2.AddComponent<LevelPlatform>();
				gameObject2.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x * vector2.x, gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y);
				gameObject2.transform.position = position;
				Object.Instantiate(gameObject, position, Quaternion.identity);
			}
		}
	}
}
