using UnityEngine;

public class StartHotdog : MonoBehaviour
{
	private const string PlayerTag = "Player";

	[SerializeField]
	private CircusPlatformingLevelHotdog hotdog;

	private void OnTriggerEnter2D(Collider2D c)
	{
		if (c.tag == "Player")
		{
			hotdog.ProjectilesCanHit = true;
			base.gameObject.SetActive(false);
		}
	}
}
