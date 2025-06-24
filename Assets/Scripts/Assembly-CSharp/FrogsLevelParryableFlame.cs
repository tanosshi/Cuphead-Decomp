public class FrogsLevelParryableFlame : ParrySwitch
{
	public override void OnParryPrePause(AbstractPlayerController player)
	{
		base.OnParryPrePause(player);
		player.stats.ParryOneQuarter();
	}
}
