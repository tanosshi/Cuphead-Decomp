public class MapLockedEntity : AbstractMapInteractiveEntity
{
	protected override void Reset()
	{
		base.Reset();
		dialogueProperties = new AbstractUIInteractionDialogue.Properties("???");
	}
}
