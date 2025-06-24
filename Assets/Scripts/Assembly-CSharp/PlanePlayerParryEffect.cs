public class PlanePlayerParryEffect : AbstractParryEffect
{
	private PlanePlayerController planePlayer;

	protected override bool IsHit
	{
		get
		{
			return false;
		}
	}

	protected override void SetPlayer(AbstractPlayerController player)
	{
		base.SetPlayer(player);
		planePlayer = player as PlanePlayerController;
	}

	protected override void OnSuccess()
	{
		base.OnSuccess();
		planePlayer.parryController.OnParrySuccess();
	}

	public override void OnPause()
	{
		base.OnPause();
		AudioManager.Play("player_parry");
	}
}
