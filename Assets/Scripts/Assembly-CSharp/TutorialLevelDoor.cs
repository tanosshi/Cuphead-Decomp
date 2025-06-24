using System.Collections;
using UnityEngine;

public class TutorialLevelDoor : AbstractLevelInteractiveEntity
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
		LevelCoin.OnLevelComplete();
		PlayerData.Data.IsTutorialCompleted = true;
		PlayerData.SaveCurrentFile();
		CupheadTime.SetLayerSpeed(CupheadTime.Layer.Player, 0f);
		LevelPlayerController[] array = Object.FindObjectsOfType<LevelPlayerController>();
		foreach (LevelPlayerController levelPlayerController in array)
		{
			levelPlayerController.DisableInput();
			levelPlayerController.PauseAll();
		}
		yield return CupheadTime.WaitForSeconds(this, 1f);
		SceneLoader.LoadScene(Scenes.scene_level_house_elder_kettle, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
	}
}
