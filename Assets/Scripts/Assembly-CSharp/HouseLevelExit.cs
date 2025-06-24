using System.Collections;
using UnityEngine;

public class HouseLevelExit : AbstractLevelInteractiveEntity
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
		CupheadTime.SetLayerSpeed(CupheadTime.Layer.Player, 0f);
		LevelPlayerController[] array = Object.FindObjectsOfType<LevelPlayerController>();
		foreach (LevelPlayerController levelPlayerController in array)
		{
			levelPlayerController.DisableInput();
			levelPlayerController.PauseAll();
		}
		yield return CupheadTime.WaitForSeconds(this, 1f);
		SceneLoader.LoadScene(Scenes.scene_map_world_1, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
	}

	protected override void Show(PlayerId playerId)
	{
		base.state = State.Ready;
		dialogue = LevelUIInteractionDialogue.Create(dialogueProperties, PlayerManager.GetPlayer(playerId).input, dialogueOffset, 0f, LevelUIInteractionDialogue.TailPosition.Left, false);
	}
}
