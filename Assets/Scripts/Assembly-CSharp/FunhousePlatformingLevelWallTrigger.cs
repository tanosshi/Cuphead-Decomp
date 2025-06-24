using UnityEngine;

public class FunhousePlatformingLevelWallTrigger : MonoBehaviour
{
	private const string PlayerTag = "Player";

	private const string EnemyTag = "Enemy";

	private int colliderCount;

	[SerializeField]
	private FunhousePlatformingLevelWall wall;

	[SerializeField]
	private GameObject cover;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (wall.IsDead && (collider.tag == "Player" || collider.tag == "Enemy"))
		{
			colliderCount = Mathf.Max(0, colliderCount + 1);
			if (colliderCount > 0)
			{
				cover.SetActive(true);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (wall.IsDead && (collider.tag == "Player" || collider.tag == "Enemy"))
		{
			colliderCount = Mathf.Max(0, colliderCount - 1);
			if (colliderCount <= 0)
			{
				cover.SetActive(false);
			}
		}
	}
}
