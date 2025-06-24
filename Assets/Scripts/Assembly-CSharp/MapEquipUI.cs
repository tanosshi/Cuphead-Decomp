using UnityEngine;

public class MapEquipUI : AbstractPauseGUI
{
	public new enum State
	{
		Inactive = 0,
		Active = 1
	}

	[SerializeField]
	private MapEquipUICard playerOne;

	private MapEquipUICard playerTwo;

	public static MapEquipUI Current { get; private set; }

	public State CurrentState { get; private set; }

	protected override InputActionSet CheckedActionSet
	{
		get
		{
			return InputActionSet.UIInput;
		}
	}

	protected override bool CanPause
	{
		get
		{
			if (Map.Current.CurrentState != Map.State.Ready)
			{
				return false;
			}
			if (MapDifficultySelectStartUI.Current.CurrentState != AbstractMapSceneStartUI.State.Inactive)
			{
				return false;
			}
			if (MapConfirmStartUI.Current.CurrentState != AbstractMapSceneStartUI.State.Inactive)
			{
				return false;
			}
			return true;
		}
	}

	protected override bool CanUnpause
	{
		get
		{
			return false;
		}
	}

	protected override void InAnimation(float i)
	{
	}

	protected override void OutAnimation(float i)
	{
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		playerTwo = Object.Instantiate(playerOne);
		playerTwo.transform.SetParent(playerOne.transform.parent, false);
		playerTwo.Init(PlayerId.PlayerTwo, this);
		playerTwo.name = "PlayerTwo";
		playerOne.transform.SetSiblingIndex(playerTwo.transform.GetSiblingIndex());
		playerOne.Init(PlayerId.PlayerOne, this);
	}

	protected override void Start()
	{
		base.Start();
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeft;
		if (PlayerManager.Multiplayer)
		{
			Vector2 anchoredPosition = playerOne.container.anchoredPosition;
			playerOne.container.anchoredPosition = anchoredPosition;
			playerTwo.container.anchoredPosition = anchoredPosition;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeft;
		if (Current == this)
		{
			Current = null;
		}
	}

	private void OnPlayerJoined(PlayerId playerId)
	{
		Vector2 anchoredPosition = playerOne.container.anchoredPosition;
		anchoredPosition.y += 10f;
		playerOne.container.anchoredPosition = anchoredPosition;
		playerTwo.container.anchoredPosition = anchoredPosition;
	}

	private void OnPlayerLeft(PlayerId playerId)
	{
		Vector2 anchoredPosition = playerOne.container.anchoredPosition;
		anchoredPosition.y -= 10f;
		playerOne.container.anchoredPosition = anchoredPosition;
		playerTwo.container.anchoredPosition = anchoredPosition;
	}

	protected override void OnPause()
	{
		base.OnPause();
		CurrentState = State.Active;
		playerOne.CanRotate = false;
		playerTwo.CanRotate = false;
		AudioManager.Play("menu_cardup");
		if (PlayerManager.Multiplayer)
		{
			playerOne.SetActive(true);
			playerTwo.SetActive(true);
			playerOne.SetMultiplayerOut(true);
			playerTwo.SetMultiplayerOut(true);
			playerOne.SetMultiplayerIn();
			playerTwo.SetMultiplayerIn();
		}
		else
		{
			playerOne.SetActive(true);
			playerTwo.SetActive(false);
			playerOne.SetSinglePlayerOut(true);
			playerOne.SetSinglePlayerIn();
		}
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
		CupheadMapCamera.Current.StartBlur();
		PlayerData.Data.ResetHasNewPurchase(PlayerId.Any);
	}

	protected override void OnPauseComplete()
	{
		base.OnPauseComplete();
		playerOne.CanRotate = true;
		playerTwo.CanRotate = true;
	}

	protected override void OnUnpause()
	{
		AudioManager.SnapshotReset(SceneLoader.SceneName, 0.1f);
		base.OnUnpause();
		playerOne.CanRotate = false;
		playerTwo.CanRotate = false;
		if (PlayerManager.Multiplayer)
		{
			playerOne.SetMultiplayerOut();
			playerTwo.SetMultiplayerOut();
		}
		else
		{
			playerOne.SetSinglePlayerOut();
		}
		CupheadMapCamera.Current.EndBlur();
	}

	protected override void OnUnpauseComplete()
	{
		base.OnUnpauseComplete();
		CurrentState = State.Inactive;
		PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, true, true);
	}

	public bool Close()
	{
		if (PlayerManager.Multiplayer && !playerOne.ReadyAndWaiting && !playerTwo.ReadyAndWaiting)
		{
			return false;
		}
		AudioManager.Play("menu_carddown");
		Unpause();
		return true;
	}
}
