using System.Collections;
using UnityEngine;

public class DiceGateLevelToNextWorld : AbstractLevelInteractiveEntity
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
		if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_1)
		{
			if (PlayerData.Data.GetMapData(Scenes.scene_map_world_2).sessionStarted)
			{
				SceneLoader.LoadScene(Scenes.scene_map_world_2, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			}
			else
			{
				Cutscene.Load(Scenes.scene_map_world_2, Scenes.scene_cutscene_world2, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			}
		}
		else if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_2)
		{
			if (PlayerData.Data.GetMapData(Scenes.scene_map_world_3).sessionStarted)
			{
				SceneLoader.LoadScene(Scenes.scene_map_world_3, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			}
			else
			{
				Cutscene.Load(Scenes.scene_map_world_3, Scenes.scene_cutscene_world3, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			}
		}
	}

	protected override void Show(PlayerId playerId)
	{
		base.state = State.Ready;
		dialogue = LevelUIInteractionDialogue.Create(dialogueProperties, PlayerManager.GetPlayer(playerId).input, dialogueOffset, 0f, LevelUIInteractionDialogue.TailPosition.Right, false);
	}
}
