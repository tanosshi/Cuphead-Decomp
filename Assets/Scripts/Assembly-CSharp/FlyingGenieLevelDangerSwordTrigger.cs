using UnityEngine;

public class FlyingGenieLevelDangerSwordTrigger : MonoBehaviour
{
	[SerializeField]
	private GameObject sword;

	private void Update()
	{
		AbstractPlayerController player = PlayerManager.GetPlayer(PlayerId.PlayerOne);
		if (player.transform.position.x > 540f)
		{
			sword.SetActive(true);
		}
		else if (PlayerManager.Multiplayer)
		{
			player = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			if (player.transform.position.x > 540f)
			{
				sword.SetActive(true);
			}
		}
	}
}
