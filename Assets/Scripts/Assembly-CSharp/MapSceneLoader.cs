using UnityEngine;

public class MapSceneLoader : AbstractMapInteractiveEntity
{
	[SerializeField]
	protected Scenes scene;

	[SerializeField]
	private bool askDifficulty;

	protected override void Activate(MapPlayerController player)
	{
		base.Activate(player);
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		AudioManager.Play("world_map_level_difficulty_appear");
		Map.Current.OnLoadLevel();
		PlayerData.Data.CurrentMapData.playerOnePosition = (Vector2)base.transform.position + returnPositions.playerOne;
		PlayerData.Data.CurrentMapData.playerTwoPosition = (Vector2)base.transform.position + returnPositions.playerTwo;
		if (!PlayerManager.Multiplayer)
		{
			PlayerData.Data.CurrentMapData.playerOnePosition = (Vector2)base.transform.position + returnPositions.singlePlayer;
		}
		Debug.LogWarning("START IN");
		if (askDifficulty)
		{
			if (scene == Scenes.scene_cutscene_kingdice)
			{
				MapDifficultySelectStartUI.Current.level = "DicePalaceMain";
			}
			MapDifficultySelectStartUI.Current.In(player);
			MapDifficultySelectStartUI.Current.OnLoadLevelEvent += OnLoadLevel;
			MapDifficultySelectStartUI.Current.OnBackEvent += OnBack;
			return;
		}
		if (scene == Scenes.scene_map_world_1)
		{
			MapBasicStartUI.Current.level = "MapWorld_1";
		}
		else if (scene == Scenes.scene_map_world_2)
		{
			MapBasicStartUI.Current.level = "MapWorld_2";
		}
		else if (scene == Scenes.scene_map_world_3)
		{
			if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_4)
			{
				MapBasicStartUI.Current.level = "KingDiceToWorld3WorldMap";
			}
			else
			{
				MapBasicStartUI.Current.level = "MapWorld_3";
			}
		}
		else if (scene == Scenes.scene_map_world_4)
		{
			MapBasicStartUI.Current.level = "Inkwell";
		}
		else if (scene == Scenes.scene_cutscene_kingdice)
		{
			MapBasicStartUI.Current.level = "KingDice";
		}
		MapBasicStartUI.Current.In(player);
		MapBasicStartUI.Current.OnLoadLevelEvent += OnLoadLevel;
		MapBasicStartUI.Current.OnBackEvent += OnBack;
	}

	private void OnLoadLevel()
	{
		AudioManager.HandleSnapshot(AudioManager.Snapshots.Paused.ToString(), 0.5f);
		AudioManager.Play("worldmap_level_select");
		LoadScene();
	}

	protected virtual void LoadScene()
	{
		MapDifficultySelectStartUI.Current.OnLoadLevelEvent -= OnLoadLevel;
		MapDifficultySelectStartUI.Current.OnBackEvent -= OnBack;
		MapBasicStartUI.Current.OnLoadLevelEvent -= OnLoadLevel;
		MapBasicStartUI.Current.OnBackEvent -= OnBack;
		SceneLoader.LoadScene(scene, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
	}

	private void OnBack()
	{
		ReCheck();
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, true, true);
		MapDifficultySelectStartUI.Current.OnLoadLevelEvent -= OnLoadLevel;
		MapDifficultySelectStartUI.Current.OnBackEvent -= OnBack;
		MapBasicStartUI.Current.OnLoadLevelEvent -= OnLoadLevel;
		MapBasicStartUI.Current.OnBackEvent -= OnBack;
	}

	protected override void Reset()
	{
		base.Reset();
		dialogueProperties = new AbstractUIInteractionDialogue.Properties("ENTER <sprite=0>");
	}
}
