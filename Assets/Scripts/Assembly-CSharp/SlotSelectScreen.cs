using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlotSelectScreen : AbstractMonoBehaviour
{
	public enum State
	{
		Intro = 0,
		InitializeStorage = 1,
		MainMenu = 2,
		OptionsMenu = 3,
		SlotSelect = 4,
		ConfirmDelete = 5
	}

	public enum MainMenuItem
	{
		Start = 0,
		Options = 1,
		Exit = 2
	}

	public enum ConfirmDeleteItem
	{
		Yes = 0,
		No = 1
	}

	private State state;

	[SerializeField]
	private RectTransform LoadingChild;

	[SerializeField]
	private RectTransform mainMenuChild;

	[SerializeField]
	private RectTransform slotSelectChild;

	[SerializeField]
	private RectTransform confirmDeleteChild;

	[SerializeField]
	private Text[] mainMenuItems;

	[SerializeField]
	private SlotSelectScreenSlot[] slots;

	[SerializeField]
	private Text[] confirmDeleteItems;

	[SerializeField]
	private RectTransform playerProfiles;

	[SerializeField]
	private RectTransform confirmPrompt;

	[SerializeField]
	private RectTransform confirmGlyph;

	[SerializeField]
	private RectTransform confirmSpacer;

	[SerializeField]
	private RectTransform backPrompt;

	[SerializeField]
	private RectTransform backGlyph;

	[SerializeField]
	private RectTransform backSpacer;

	[SerializeField]
	private RectTransform deletePrompt;

	[SerializeField]
	private RectTransform deleteGlyph;

	[SerializeField]
	private RectTransform deleteSpacer;

	[SerializeField]
	private RectTransform prompts;

	[SerializeField]
	private Color mainMenuSelectedColor;

	[SerializeField]
	private Color mainMenuUnselectedColor;

	[SerializeField]
	private Color confirmDeleteSelectedColor;

	[SerializeField]
	private Color confirmDeleteUnselectedColor;

	[SerializeField]
	private OptionsGUI optionsPrefab;

	[SerializeField]
	private RectTransform optionsRoot;

	[SerializeField]
	private Text confirmDeleteSlotText;

	private OptionsGUI options;

	private float _selectionTimer;

	private int _slotSelection;

	private int _mainMenuSelection;

	private int _confirmDeleteSelection;

	private CupheadInput.AnyPlayerInput input;

	private bool isConsole;

	private bool alreadyInitializedStorage;

	private const string PATH = "Audio/TitleScreenAudio";

	private bool RespondToDeadPlayer
	{
		get
		{
			return true;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Cuphead.Init();
		input = new CupheadInput.AnyPlayerInput();
		isConsole = OnlineManager.Instance.Interface.SupportsMultipleUsers;
		PlayerData.inGame = false;
		if (isConsole)
		{
			mainMenuItems[2].gameObject.SetActive(false);
		}
	}

	protected override void Update()
	{
		base.Update();
		switch (state)
		{
		case State.MainMenu:
			UpdateMainMenu();
			break;
		case State.OptionsMenu:
			UpdateOptionsMenu();
			break;
		case State.SlotSelect:
			UpdateSlotSelect();
			break;
		case State.ConfirmDelete:
			UpdateConfirmDelete();
			break;
		}
	}

	protected override void Start()
	{
		if (StartScreenAudio.Instance == null)
		{
			StartScreenAudio startScreenAudio = Object.Instantiate(Resources.Load("Audio/TitleScreenAudio")) as StartScreenAudio;
			SceneLoader.OnLoaderCompleteEvent += PlayMusic;
		}
		options = optionsPrefab.InstantiatePrefab<OptionsGUI>();
		options.rectTransform.SetParent(optionsRoot, false);
		options.Init(false);
		CupheadLevelCamera.Current.StartSmoothShake(8f, 3f, 2);
		StartCoroutine(intro_cr());
	}

	private void PlayMusic()
	{
		AudioManager.PlayBGMPlaylistManually(true);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SceneLoader.OnLoaderCompleteEvent -= PlayMusic;
	}

	private void SetState(State state)
	{
		this.state = state;
		LoadingChild.gameObject.SetActive(state == State.InitializeStorage);
		mainMenuChild.gameObject.SetActive(state == State.MainMenu || state == State.Intro);
		slotSelectChild.gameObject.SetActive(state == State.SlotSelect || state == State.ConfirmDelete);
		confirmDeleteChild.gameObject.SetActive(state == State.ConfirmDelete);
		confirmPrompt.gameObject.SetActive(state == State.MainMenu || state == State.OptionsMenu || state == State.SlotSelect || state == State.ConfirmDelete);
		confirmGlyph.gameObject.SetActive(confirmPrompt.gameObject.activeSelf);
		confirmSpacer.gameObject.SetActive(confirmPrompt.gameObject.activeSelf);
		backPrompt.gameObject.SetActive(state == State.OptionsMenu || state == State.SlotSelect || state == State.ConfirmDelete);
		backGlyph.gameObject.SetActive(backPrompt.gameObject.activeSelf);
		backSpacer.gameObject.SetActive(backPrompt.gameObject.activeSelf);
		deletePrompt.gameObject.SetActive(state == State.SlotSelect);
		deleteGlyph.gameObject.SetActive(deletePrompt.gameObject.activeSelf);
		deleteSpacer.gameObject.SetActive(deletePrompt.gameObject.activeSelf);
		playerProfiles.gameObject.SetActive(state == State.SlotSelect || state == State.MainMenu);
		PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, state == State.SlotSelect || state == State.MainMenu);
	}

	private void UpdateMainMenu()
	{
		if (GetButtonDown(CupheadButton.MenuDown))
		{
			_mainMenuSelection = (_mainMenuSelection + 1) % ((!isConsole) ? 3 : 2);
		}
		if (GetButtonDown(CupheadButton.MenuUp))
		{
			_mainMenuSelection--;
			if (_mainMenuSelection < 0)
			{
				_mainMenuSelection = (isConsole ? 1 : 2);
			}
		}
		for (int i = 0; i < 3; i++)
		{
			mainMenuItems[i].color = ((_mainMenuSelection != i) ? mainMenuUnselectedColor : mainMenuSelectedColor);
		}
		if (!GetButtonDown(CupheadButton.Accept))
		{
			return;
		}
		switch ((MainMenuItem)_mainMenuSelection)
		{
		case MainMenuItem.Start:
			if (alreadyInitializedStorage)
			{
				SetState(State.SlotSelect);
				break;
			}
			SetState(State.InitializeStorage);
			PlayerData.Init(OnPlayerDataInitialized);
			ControllerDisconnectedPrompt.Instance.allowedToShow = false;
			break;
		case MainMenuItem.Options:
			SetState(State.OptionsMenu);
			options.ShowMainOptionMenu();
			break;
		case MainMenuItem.Exit:
			Application.Quit();
			break;
		}
	}

	private void UpdateOptionsMenu()
	{
		prompts.gameObject.SetActive(!Cuphead.Current.controlMapper.isOpen);
		if (!options.optionMenuOpen && !options.justClosed)
		{
			SetState(State.MainMenu);
		}
	}

	private void UpdateSlotSelect()
	{
		if (GetButtonDown(CupheadButton.MenuDown))
		{
			_slotSelection = (_slotSelection + 1) % 3;
		}
		if (GetButtonDown(CupheadButton.MenuUp))
		{
			_slotSelection--;
			if (_slotSelection < 0)
			{
				_slotSelection = 2;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			slots[i].SetSelected(_slotSelection == i);
		}
		if (GetButtonDown(CupheadButton.Accept))
		{
			PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, false);
			PlayerData.CurrentSaveFileIndex = _slotSelection;
			Level.ResetPreviousLevelInfo();
			if (!slots[_slotSelection].IsEmpty)
			{
				SceneLoader.LoadScene(PlayerData.Data.CurrentMap, SceneLoader.Transition.Fade, SceneLoader.Transition.Iris);
			}
			else
			{
				Cutscene.Load(Scenes.scene_level_house_elder_kettle, Scenes.scene_cutscene_intro, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade);
			}
			PlayerData.inGame = true;
			if (StartScreenAudio.Instance != null)
			{
				Object.Destroy(StartScreenAudio.Instance.gameObject);
			}
		}
		else if (GetButtonDown(CupheadButton.Cancel))
		{
			SetState(State.MainMenu);
		}
		else if (!slots[_slotSelection].IsEmpty && GetButtonDown(CupheadButton.EquipMenu))
		{
			SetState(State.ConfirmDelete);
			_confirmDeleteSelection = 1;
			confirmDeleteSlotText.text = slots[_slotSelection].getSlotText() + "?";
		}
	}

	private void UpdateConfirmDelete()
	{
		if (GetButtonDown(CupheadButton.MenuDown))
		{
			_confirmDeleteSelection = (_confirmDeleteSelection + 1) % 2;
		}
		if (GetButtonDown(CupheadButton.MenuUp))
		{
			_confirmDeleteSelection--;
			if (_confirmDeleteSelection < 0)
			{
				_confirmDeleteSelection = 1;
			}
		}
		for (int i = 0; i < 2; i++)
		{
			confirmDeleteItems[i].color = ((_confirmDeleteSelection != i) ? confirmDeleteUnselectedColor : confirmDeleteSelectedColor);
		}
		if (GetButtonDown(CupheadButton.Accept))
		{
			switch ((ConfirmDeleteItem)_confirmDeleteSelection)
			{
			case ConfirmDeleteItem.Yes:
				PlayerData.ClearSlot(_slotSelection);
				slots[_slotSelection].Init(_slotSelection);
				SetState(State.SlotSelect);
				break;
			case ConfirmDeleteItem.No:
				SetState(State.SlotSelect);
				break;
			}
		}
		if (GetButtonDown(CupheadButton.Cancel))
		{
			SetState(State.SlotSelect);
		}
	}

	private IEnumerator intro_cr()
	{
		SetState(State.Intro);
		yield return new WaitForSeconds(0.75f);
		ControllerDisconnectedPrompt.Instance.allowedToShow = true;
		SetState(State.MainMenu);
	}

	private void OnPlayerDataInitialized(bool success)
	{
		Debug.Log("OnPlayerDataInitialized()");
		if (!success)
		{
			PlayerData.Init(OnPlayerDataInitialized);
			return;
		}
		SetState(State.SlotSelect);
		for (int i = 0; i < 3; i++)
		{
			slots[i].Init(i);
		}
		ControllerDisconnectedPrompt.Instance.allowedToShow = true;
		alreadyInitializedStorage = true;
	}

	protected bool GetButtonDown(CupheadButton button)
	{
		if (input.GetButtonDown(button))
		{
			AudioManager.Play("level_menu_select");
			return true;
		}
		return false;
	}
}
