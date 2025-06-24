public class MapDicePalaceSceneLoader : MapSceneLoader
{
	protected override void LoadScene()
	{
		if (!PlayerData.Data.GetLevelData(Levels.DicePalaceMain).played)
		{
			SceneLoader.LoadScene(scene, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
		}
		else
		{
			SceneLoader.LoadScene(Scenes.scene_level_dice_palace_main, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
		}
	}
}
