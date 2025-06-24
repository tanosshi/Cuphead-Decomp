public class PlayerDeathParrySwitch : ParrySwitch
{
	public override void OnParryPrePause(AbstractPlayerController player)
	{
		base.OnParryPrePause(player);
		player.stats.OnParry();
	}
}
