using UnityEngine;

public class HarbourPlatformingLevelOctopusHead : LevelPlatform
{
	[SerializeField]
	private HarbourPlatformingLevelOctopus octopus;

	public override void AddChild(Transform player)
	{
		base.AddChild(player);
		octopus.animator.SetBool("playerOn", true);
	}

	public override void OnPlayerExit(Transform player)
	{
		base.OnPlayerExit(player);
		octopus.animator.SetBool("playerOn", false);
	}
}
