public class MausoleumLevelRegularGhost : MausoleumLevelGhostBase
{
	protected override void Start()
	{
		base.Start();
		animator.SetBool("IsA", Rand.Bool());
	}

	public override void OnParry(AbstractPlayerController player)
	{
		AudioManager.Play("mausoleum_regular_ghost_1_death");
		emitAudioFromObject.Add("mausoleum_regular_ghost_1_death");
		base.OnParry(player);
	}
}
