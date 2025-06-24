using System.Collections;

public class HouseLevelTutorial : AbstractLevelInteractiveEntity
{
	private bool activated;

	protected override void Activate()
	{
		if (!activated)
		{
			base.Activate();
			StartCoroutine(go_cr());
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CupheadTime.SetLayerSpeed(CupheadTime.Layer.Player, 1f);
	}

	private IEnumerator go_cr()
	{
		activated = true;
		HouseLevel level = Level.Current as HouseLevel;
		if ((bool)level)
		{
			level.StartTutorial();
		}
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		SceneLoader.LoadScene(Scenes.scene_level_tutorial, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
	}

	protected override void Show(PlayerId playerId)
	{
		base.state = State.Ready;
		dialogue = LevelUIInteractionDialogue.Create(dialogueProperties, PlayerManager.GetPlayer(playerId).input, dialogueOffset, 0f, LevelUIInteractionDialogue.TailPosition.Bottom, false);
	}
}
