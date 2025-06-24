using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelPauseGUI : AbstractPauseGUI
{
	private enum MenuItems
	{
		Unpause = 0,
		Restart = 1,
		Options = 2,
		Player2Leave = 3,
		ExitToMap = 4,
		ExitToTitle = 5,
		ExitToDesktop = 6
	}

	[SerializeField]
	private Text[] menuItems;

	private PlayerInput[] playerInputs;

	private OptionsGUI options;

	private float _selectionTimer;

	private const float _SELECTION_TIME = 0.15f;

	private int _selection;

	public static Color COLOR_SELECTED { get; private set; }

	public static Color COLOR_INACTIVE { get; private set; }

	private int selection
	{
		get
		{
			return _selection;
		}
		set
		{
			bool flag = value > _selection;
			int num = (int)Mathf.Repeat(value, menuItems.Length);
			while (!menuItems[num].gameObject.activeSelf)
			{
				num = ((!flag) ? (num - 1) : (num + 1));
				num = (int)Mathf.Repeat(num, menuItems.Length);
			}
			_selection = num;
			UpdateSelection();
		}
	}

	protected override bool CanPause
	{
		get
		{
			return Level.Current.Started && !Level.Current.Ending && PauseManager.state != PauseManager.State.Paused;
		}
	}

	public static event Action OnPauseEvent;

	public static event Action OnUnpauseEvent;

	protected override void Awake()
	{
		base.Awake();
		COLOR_SELECTED = menuItems[0].color;
		COLOR_INACTIVE = menuItems[menuItems.Length - 1].color;
	}

	public override void Init(bool checkIfDead, OptionsGUI options)
	{
		base.Init(checkIfDead, options);
		this.options = options;
		if (OnlineManager.Instance.Interface.SupportsMultipleUsers && menuItems.Length > 6)
		{
			menuItems[6].gameObject.SetActive(false);
		}
		options.Init(checkIfDead);
	}

	protected override void OnPause()
	{
		base.OnPause();
		if (CupheadLevelCamera.Current != null)
		{
			CupheadLevelCamera.Current.StartBlur();
		}
		else
		{
			CupheadMapCamera.Current.StartBlur();
		}
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, true);
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerTwo, true);
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		menuItems[3].gameObject.SetActive(PlayerManager.Multiplayer);
		if (LevelPauseGUI.OnPauseEvent != null)
		{
			LevelPauseGUI.OnPauseEvent();
		}
		selection = 0;
	}

	protected override void OnUnpause()
	{
		base.OnUnpause();
		if (CupheadLevelCamera.Current != null)
		{
			CupheadLevelCamera.Current.EndBlur();
		}
		else
		{
			CupheadMapCamera.Current.EndBlur();
		}
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, false);
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerTwo, false);
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, true, true);
		if (LevelPauseGUI.OnUnpauseEvent != null)
		{
			LevelPauseGUI.OnUnpauseEvent();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PauseManager.Unpause();
	}

	protected override void Update()
	{
		base.Update();
		if (base.state != State.Paused || options.optionMenuOpen || options.justClosed)
		{
			return;
		}
		if (GetButtonDown(CupheadButton.Pause) || GetButtonDown(CupheadButton.Cancel))
		{
			Unpause();
		}
		else if (GetButtonDown(CupheadButton.Accept))
		{
			Select();
		}
		else if (_selectionTimer >= 0.15f)
		{
			if (GetButton(CupheadButton.MenuUp))
			{
				MenuSelectSound();
				selection--;
			}
			if (GetButton(CupheadButton.MenuDown))
			{
				MenuSelectSound();
				selection++;
			}
		}
		else
		{
			_selectionTimer += Time.deltaTime;
		}
	}

	private void Select()
	{
		switch (selection)
		{
		case 0:
			Unpause();
			break;
		case 1:
			Restart();
			break;
		case 2:
			Options();
			break;
		case 3:
			Player2Leave();
			break;
		case 4:
			Exit();
			break;
		case 5:
			ExitToTitle();
			break;
		case 6:
			ExitToDesktop();
			break;
		}
	}

	protected override void OnUnpauseSound()
	{
		base.OnUnpauseSound();
	}

	private void UpdateSelection()
	{
		_selectionTimer = 0f;
		for (int i = 0; i < menuItems.Length; i++)
		{
			Text text = menuItems[i];
			if (i == selection)
			{
				text.color = COLOR_SELECTED;
			}
			else
			{
				text.color = COLOR_INACTIVE;
			}
		}
	}

	private void Restart()
	{
		OnUnpauseSound();
		base.state = State.Animating;
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, false);
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerTwo, false);
		SceneLoader.ReloadLevel();
		if (Level.IsDicePalaceMain || Level.IsDicePalace)
		{
			DicePalaceMainLevelGameInfo.CleanUpRetry();
		}
	}

	private void Exit()
	{
		base.state = State.Animating;
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, false);
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerTwo, false);
		if (Level.IsDicePalaceMain || Level.IsDicePalace)
		{
			DicePalaceMainLevelGameInfo.CleanUpRetry();
		}
		SceneLoader.LoadLastMap();
	}

	private void Player2Leave()
	{
		PlayerManager.PlayerLeave(PlayerId.PlayerTwo);
		Unpause();
	}

	private void ExitToTitle()
	{
		base.state = State.Animating;
		PlayerManager.ResetPlayers();
		SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade);
	}

	private void ExitToDesktop()
	{
		Application.Quit();
	}

	private void Options()
	{
		StartCoroutine(in_options_cr());
	}

	private IEnumerator in_options_cr()
	{
		HideImmediate();
		options.ShowMainOptionMenu();
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, false);
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerTwo, false);
		while (options.optionMenuOpen)
		{
			yield return null;
		}
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, true);
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerTwo, true);
		selection = 0;
		ShowImmediate();
		yield return null;
	}

	protected override void InAnimation(float i)
	{
	}

	protected override void OutAnimation(float i)
	{
	}
}
