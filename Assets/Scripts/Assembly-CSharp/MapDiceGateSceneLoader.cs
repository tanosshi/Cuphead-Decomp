using UnityEngine;

public class MapDiceGateSceneLoader : AbstractMapInteractiveEntity
{
	[SerializeField]
	private Scenes nextWorld;

	private readonly Scenes diceGate = Scenes.scene_level_dice_gate;

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
			MapDifficultySelectStartUI.Current.In(player);
			MapDifficultySelectStartUI.Current.OnLoadLevelEvent += OnLoadLevel;
			MapDifficultySelectStartUI.Current.OnBackEvent += OnBack;
			return;
		}
		if (!PlayerData.Data.CurrentMapData.hasKingDiceDisappeared)
		{
			MapBasicStartUI.Current.level = "DieHouse";
		}
		else if (nextWorld == Scenes.scene_map_world_1)
		{
			MapBasicStartUI.Current.level = "MapWorld_1";
		}
		else if (nextWorld == Scenes.scene_map_world_2)
		{
			MapBasicStartUI.Current.level = "MapWorld_2";
		}
		else if (nextWorld == Scenes.scene_map_world_3)
		{
			MapBasicStartUI.Current.level = "MapWorld_3";
		}
		else if (nextWorld == Scenes.scene_map_world_4)
		{
			MapBasicStartUI.Current.level = "Inkwell";
		}
		MapBasicStartUI.Current.In(player);
		MapBasicStartUI.Current.OnLoadLevelEvent += OnLoadLevel;
		MapBasicStartUI.Current.OnBackEvent += OnBack;
	}

	private void OnLoadLevel()
	{
		AudioManager.HandleSnapshot(AudioManager.Snapshots.Paused.ToString(), 0.5f);
		AudioManager.Play("world_map_level_select");
		CheckSceneToLoad();
	}

	private void CheckSceneToLoad()
	{
		if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_1)
		{
			if (PlayerData.Data.CurrentMapData.hasKingDiceDisappeared)
			{
				SceneLoader.LoadScene(nextWorld, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			}
			else
			{
				SceneLoader.LoadScene(diceGate, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			}
			Debug.Log(PlayerData.Data.GetMapData(Scenes.scene_map_world_2).sessionStarted);
		}
		else if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_2)
		{
			if (PlayerData.Data.CurrentMapData.hasKingDiceDisappeared)
			{
				SceneLoader.LoadScene(nextWorld, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			}
			else
			{
				SceneLoader.LoadScene(diceGate, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			}
		}
	}

	private void OnBack()
	{
		ReCheck();
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, true, true);
		if (askDifficulty)
		{
			MapDifficultySelectStartUI.Current.OnLoadLevelEvent -= OnLoadLevel;
			MapDifficultySelectStartUI.Current.OnBackEvent -= OnBack;
		}
		else
		{
			MapConfirmStartUI.Current.OnLoadLevelEvent -= OnLoadLevel;
			MapConfirmStartUI.Current.OnBackEvent -= OnBack;
		}
	}

	protected override void Reset()
	{
		base.Reset();
		dialogueProperties = new AbstractUIInteractionDialogue.Properties("ENTER <sprite=0>");
	}
}
