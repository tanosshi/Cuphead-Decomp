public class PlayerJoinPrompt : FlashingPrompt
{
	protected override bool ShouldShow
	{
		get
		{
			return PlayerManager.ShouldShowJoinPrompt;
		}
	}
}
