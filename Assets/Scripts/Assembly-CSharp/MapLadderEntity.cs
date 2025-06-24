using UnityEngine;

public class MapLadderEntity : AbstractMapInteractiveEntity
{
	private MapPlayerLadderObject playerLadder;

	private Vector2 exit;

	private MapLadder.Location location;

	public void Init(MapPlayerLadderObject playerLadder, Vector2 exit, MapLadder.Location location)
	{
		this.playerLadder = playerLadder;
		this.exit = (Vector2)base.transform.position + exit;
		this.location = location;
	}

	protected override MapUIInteractionDialogue Show(PlayerInput player)
	{
		switch (base.playerChecking.state)
		{
		case MapPlayerController.State.Walking:
		case MapPlayerController.State.LadderExit:
			dialogueProperties = MapLadder.DIALOGUE_ENTER;
			break;
		case MapPlayerController.State.LadderEnter:
		case MapPlayerController.State.Ladder:
			dialogueProperties = MapLadder.DIALOGUE_EXIT;
			break;
		}
		return base.Show(player);
	}

	protected override void Activate()
	{
		base.Activate();
		switch (base.playerActivating.state)
		{
		case MapPlayerController.State.Walking:
			base.playerActivating.LadderEnter(base.transform.position, playerLadder, location);
			break;
		case MapPlayerController.State.Ladder:
			base.playerActivating.LadderExit(base.transform.position, exit, location);
			break;
		}
	}
}
