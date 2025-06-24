public class HarbourPlatformingLevelBarnacle : PlatformingLevelShootingEnemy
{
	private void AttackSFX()
	{
		AudioManager.Play("harbour_barnacle_attack");
		emitAudioFromObject.Add("harbour_barnacle_attack");
	}
}
