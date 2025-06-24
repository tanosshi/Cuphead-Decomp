using UnityEngine;

public class MapTutorialLoader : AbstractMapInteractiveEntity
{
	protected override void Activate(MapPlayerController player)
	{
		base.Activate(player);
		AudioManager.Play("world_map_level_difficulty_appear");
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		Map.Current.OnLoadShop();
		PlayerData.Data.CurrentMapData.playerOnePosition = (Vector2)base.transform.position + returnPositions.playerOne;
		PlayerData.Data.CurrentMapData.playerTwoPosition = (Vector2)base.transform.position + returnPositions.playerTwo;
		if (!PlayerManager.Multiplayer)
		{
			PlayerData.Data.CurrentMapData.playerOnePosition = (Vector2)base.transform.position + returnPositions.singlePlayer;
		}
		MapBasicStartUI.Current.level = "ElderKettleLevel";
		MapBasicStartUI.Current.In(player);
		MapBasicStartUI.Current.OnLoadLevelEvent += OnLoadLevel;
		MapBasicStartUI.Current.OnBackEvent += OnBack;
	}

	private void OnLoadLevel()
	{
		AudioManager.Play("world_map_level_select");
		SceneLoader.LoadScene(Scenes.scene_level_house_elder_kettle, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
	}

	private void OnBack()
	{
		ReCheck();
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, true, true);
		MapBasicStartUI.Current.OnLoadLevelEvent -= OnLoadLevel;
		MapBasicStartUI.Current.OnBackEvent -= OnBack;
	}
}
