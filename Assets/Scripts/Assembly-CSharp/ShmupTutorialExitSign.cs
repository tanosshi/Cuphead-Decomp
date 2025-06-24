using System.Collections;
using UnityEngine;

public class ShmupTutorialExitSign : AbstractLevelInteractiveEntity
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

	protected override void Show(PlayerId playerId)
	{
		PlanePlayerController planePlayerController = PlayerManager.GetPlayer(playerId) as PlanePlayerController;
		planePlayerController.parryController.State = PlanePlayerParryController.ParryState.Disabled;
		base.Show(playerId);
	}

	protected override void Hide(PlayerId playerId)
	{
		PlanePlayerController planePlayerController = PlayerManager.GetPlayer(playerId) as PlanePlayerController;
		planePlayerController.parryController.State = PlanePlayerParryController.ParryState.Ready;
		base.Hide(playerId);
	}

	private IEnumerator go_cr()
	{
		activated = true;
		PlayerData.SaveCurrentFile();
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
}
