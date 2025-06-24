using UnityEngine;

public static class DebugConsoleProperties
{
	public static bool RunCommand(string s, DebugConsole.Arguments args)
	{
		switch (s)
		{
		case "console.test":
			console_test(args.values[0].intValue, args.values[1].stringValue);
			return true;
		case "audio.bgm.disable":
			audio_bgm_disable();
			return true;
		case "fps":
			fps();
			return true;
		case "gui.disable":
			gui_disable();
			return true;
		case "scene.load":
			scene_load(args.values[0].stringValue);
			return true;
		case "scene.select":
			scene_select();
			return true;
		case "scene.names":
			scene_names();
			return true;
		case "scene.reset":
			scene_reset();
			return true;
		case "player.invincible":
			player_invincible();
			return true;
		case "player.megaDamage":
			player_megaDamage();
			return true;
		case "player.multiplayer":
			player_multiplayer(args.values[0].boolValue);
			return true;
		case "player.super.add":
			player_super_add();
			return true;
		case "player.super.fill":
			player_super_fill();
			return true;
		case "show.sound.playing":
			show_sound_playing();
			return true;
		case "player.coin.add":
			player_coin_add();
			return true;
		case "player.coin.remove":
			player_coin_remove();
			return true;
		case "player.more.pacific":
			player_more_pacific();
			return true;
		case "player.more.elite":
			player_more_elite();
			return true;
		default:
			return false;
		}
	}

	public static int GetCommandIndex(string s)
	{
		switch (s)
		{
		case "console.test":
			return 0;
		case "audio.bgm.disable":
			return 1;
		case "fps":
			return 2;
		case "gui.disable":
			return 3;
		case "scene.load":
			return 4;
		case "scene.select":
			return 5;
		case "scene.names":
			return 6;
		case "scene.reset":
			return 7;
		case "player.invincible":
			return 8;
		case "player.megaDamage":
			return 9;
		case "player.multiplayer":
			return 10;
		case "player.super.add":
			return 11;
		case "player.super.fill":
			return 12;
		case "show.sound.playing":
			return 13;
		case "player.coin.add":
			return 14;
		case "player.coin.remove":
			return 15;
		case "player.more.pacific":
			return 16;
		case "player.more.elite":
			return 17;
		default:
			return -1;
		}
	}

	public static DebugConsole.Arguments GetExpectedArgs(string s)
	{
		switch (s)
		{
		case "console.test":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[2]
			{
				DebugConsoleData.Command.Argument.Type.Int,
				DebugConsoleData.Command.Argument.Type.String
			});
		case "audio.bgm.disable":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "fps":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "gui.disable":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "scene.load":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[1] { DebugConsoleData.Command.Argument.Type.String });
		case "scene.select":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "scene.names":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "scene.reset":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "player.invincible":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "player.megaDamage":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "player.multiplayer":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[1] { DebugConsoleData.Command.Argument.Type.Bool });
		case "player.super.add":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "player.super.fill":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "show.sound.playing":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "player.coin.add":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "player.coin.remove":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "player.more.pacific":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		case "player.more.elite":
			return new DebugConsole.Arguments(new DebugConsoleData.Command.Argument.Type[0]);
		default:
			return null;
		}
	}

	public static void console_test(int intVal, string stringVal)
	{
		Debug.Log("console.test\nint:" + intVal + " string:" + stringVal);
	}

	public static void audio_bgm_disable()
	{
		AudioManager.StopBGM();
	}

	public static void fps()
	{
		FramerateCounter.Init();
		FramerateCounter.SHOW = !FramerateCounter.SHOW;
	}

	public static void gui_disable()
	{
		LevelGUI.DebugDisableGUI();
	}

	public static void scene_load(string s)
	{
		Scenes result;
		if (!EnumUtils.TryParse<Scenes>(s, out result))
		{
			DebugConsole.PrintError("Scene name \"" + s + " \" is not valid");
		}
		else if (s.Contains("level"))
		{
			for (int i = 0; i < LevelProperties.levels.Length; i++)
			{
				if (LevelProperties.levels[i] == s)
				{
					SceneLoader.LoadLevel((Levels)i, SceneLoader.Transition.Fade);
					DebugConsole.Hide();
					break;
				}
			}
		}
		else
		{
			SceneLoader.LoadScene(result, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade);
			DebugConsole.Hide();
		}
	}

	public static void scene_select()
	{
		SceneLoader.LoadScene(Scenes.scene_menu, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade);
	}

	public static void scene_names()
	{
		DebugConsole.Break();
		DebugConsole.Print("Scene names:");
		DebugConsole.Line();
		string[] valuesAsStrings = EnumUtils.GetValuesAsStrings<Scenes>();
		foreach (string s in valuesAsStrings)
		{
			DebugConsole.Print(s);
		}
		DebugConsole.Line();
		DebugConsole.Print("Use these scene names in conjunction with scene.load");
		DebugConsole.Break();
	}

	public static void scene_reset()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public static void player_invincible()
	{
		PlayerStatsManager.DebugToggleInvincible();
	}

	public static void player_megaDamage()
	{
		DamageReceiver.Debug_ToggleMegaDamage();
	}

	public static void player_multiplayer(bool b)
	{
		PlayerManager.Multiplayer = b;
	}

	public static void player_super_add()
	{
		PlayerStatsManager[] array = Object.FindObjectsOfType<PlayerStatsManager>();
		foreach (PlayerStatsManager playerStatsManager in array)
		{
			playerStatsManager.DebugAddSuper();
		}
	}

	public static void player_super_fill()
	{
		PlayerStatsManager[] array = Object.FindObjectsOfType<PlayerStatsManager>();
		foreach (PlayerStatsManager playerStatsManager in array)
		{
			playerStatsManager.DebugFillSuper();
		}
	}

	public static void show_sound_playing()
	{
		AudioManagerComponent.ShowAudioPlaying = true;
		AudioManagerComponent.ShowAudioVariations = false;
	}

	public static void player_coin_add()
	{
		if (PlayerData.Data.GetCurrency(PlayerId.PlayerOne) < 60)
		{
			PlayerData.Data.AddCurrency(PlayerId.PlayerOne, 1);
		}
		if (PlayerData.Data.GetCurrency(PlayerId.PlayerTwo) < 60)
		{
			PlayerData.Data.AddCurrency(PlayerId.PlayerTwo, 1);
		}
	}

	public static void player_coin_remove()
	{
		if (PlayerData.Data.GetCurrency(PlayerId.PlayerOne) > 0)
		{
			PlayerData.Data.AddCurrency(PlayerId.PlayerOne, -1);
		}
		if (PlayerData.Data.GetCurrency(PlayerId.PlayerTwo) > 0)
		{
			PlayerData.Data.AddCurrency(PlayerId.PlayerTwo, -1);
		}
	}

	public static void player_more_pacific()
	{
		for (int i = 0; i < Level.platformingLevels.Length; i++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinGrade(new Levels[1] { Level.platformingLevels[i] }, LevelScoringData.Grade.P))
			{
				PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(Level.platformingLevels[i]);
				levelData.grade = LevelScoringData.Grade.P;
				levelData.completed = true;
				levelData.played = true;
				break;
			}
		}
	}

	public static void player_more_elite()
	{
		for (int i = 0; i < Level.world1BossLevels.Length; i++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinGrade(new Levels[1] { Level.world1BossLevels[i] }, LevelScoringData.Grade.AMinus))
			{
				PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(Level.world1BossLevels[i]);
				levelData.grade = LevelScoringData.Grade.AMinus;
				levelData.completed = true;
				levelData.played = true;
				return;
			}
		}
		for (int j = 0; j < Level.world2BossLevels.Length; j++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinGrade(new Levels[1] { Level.world2BossLevels[j] }, LevelScoringData.Grade.AMinus))
			{
				PlayerData.PlayerLevelDataObject levelData2 = PlayerData.Data.GetLevelData(Level.world2BossLevels[j]);
				levelData2.grade = LevelScoringData.Grade.AMinus;
				levelData2.completed = true;
				levelData2.played = true;
				return;
			}
		}
		for (int k = 0; k < Level.world3BossLevels.Length; k++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinGrade(new Levels[1] { Level.world3BossLevels[k] }, LevelScoringData.Grade.AMinus))
			{
				PlayerData.PlayerLevelDataObject levelData3 = PlayerData.Data.GetLevelData(Level.world3BossLevels[k]);
				levelData3.grade = LevelScoringData.Grade.AMinus;
				levelData3.completed = true;
				levelData3.played = true;
				return;
			}
		}
		for (int l = 0; l < Level.world4BossLevels.Length; l++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinGrade(new Levels[1] { Level.world4BossLevels[l] }, LevelScoringData.Grade.AMinus))
			{
				PlayerData.PlayerLevelDataObject levelData4 = PlayerData.Data.GetLevelData(Level.world4BossLevels[l]);
				levelData4.grade = LevelScoringData.Grade.AMinus;
				levelData4.completed = true;
				levelData4.played = true;
				return;
			}
		}
		for (int m = 0; m < Level.world1BossLevels.Length; m++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Level.world1BossLevels[m] }, Level.Mode.Normal))
			{
				PlayerData.PlayerLevelDataObject levelData5 = PlayerData.Data.GetLevelData(Level.world1BossLevels[m]);
				levelData5.difficultyBeaten = Level.Mode.Normal;
				return;
			}
		}
		for (int n = 0; n < Level.world2BossLevels.Length; n++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Level.world2BossLevels[n] }, Level.Mode.Normal))
			{
				PlayerData.PlayerLevelDataObject levelData6 = PlayerData.Data.GetLevelData(Level.world2BossLevels[n]);
				levelData6.difficultyBeaten = Level.Mode.Normal;
				return;
			}
		}
		for (int num = 0; num < Level.world3BossLevels.Length; num++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Level.world3BossLevels[num] }, Level.Mode.Normal))
			{
				PlayerData.PlayerLevelDataObject levelData7 = PlayerData.Data.GetLevelData(Level.world3BossLevels[num]);
				levelData7.difficultyBeaten = Level.Mode.Normal;
				return;
			}
		}
		for (int num2 = 0; num2 < Level.world4BossLevels.Length; num2++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Level.world4BossLevels[num2] }, Level.Mode.Normal))
			{
				PlayerData.PlayerLevelDataObject levelData8 = PlayerData.Data.GetLevelData(Level.world4BossLevels[num2]);
				levelData8.difficultyBeaten = Level.Mode.Normal;
				return;
			}
		}
		for (int num3 = 0; num3 < Level.world1BossLevels.Length; num3++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Level.world1BossLevels[num3] }, Level.Mode.Hard))
			{
				PlayerData.PlayerLevelDataObject levelData9 = PlayerData.Data.GetLevelData(Level.world1BossLevels[num3]);
				levelData9.difficultyBeaten = Level.Mode.Hard;
				return;
			}
		}
		for (int num4 = 0; num4 < Level.world2BossLevels.Length; num4++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Level.world2BossLevels[num4] }, Level.Mode.Hard))
			{
				PlayerData.PlayerLevelDataObject levelData10 = PlayerData.Data.GetLevelData(Level.world2BossLevels[num4]);
				levelData10.difficultyBeaten = Level.Mode.Hard;
				return;
			}
		}
		for (int num5 = 0; num5 < Level.world3BossLevels.Length; num5++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Level.world3BossLevels[num5] }, Level.Mode.Hard))
			{
				PlayerData.PlayerLevelDataObject levelData11 = PlayerData.Data.GetLevelData(Level.world3BossLevels[num5]);
				levelData11.difficultyBeaten = Level.Mode.Hard;
				return;
			}
		}
		for (int num6 = 0; num6 < Level.world4BossLevels.Length; num6++)
		{
			if (!PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Level.world4BossLevels[num6] }, Level.Mode.Hard))
			{
				PlayerData.PlayerLevelDataObject levelData12 = PlayerData.Data.GetLevelData(Level.world4BossLevels[num6]);
				levelData12.difficultyBeaten = Level.Mode.Hard;
				break;
			}
		}
	}
}
