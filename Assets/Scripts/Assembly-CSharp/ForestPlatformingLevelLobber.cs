using UnityEngine;

public class ForestPlatformingLevelLobber : PlatformingLevelShootingEnemy
{
	protected override void Awake()
	{
		base.Awake();
		base.DeathSound = "level_frogs_tall_spit_shoot";
		base.animator.Play("Idle", 0, Random.Range(0f, 1f));
	}

	protected override void Shoot()
	{
		base.Shoot();
	}

	private void PlayLobberSound()
	{
		AudioManager.Play("level_forestlobber_shoot");
		emitAudioFromObject.Add("level_forestlobber_shoot");
	}
}
