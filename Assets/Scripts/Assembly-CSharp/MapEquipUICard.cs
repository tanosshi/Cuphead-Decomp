using System;
using System.Collections;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class MapEquipUICard : AbstractMonoBehaviour
{
	public enum Side
	{
		Front = 0,
		Back = 1
	}

	public enum Slot
	{
		SHOT_A = 0,
		SHOT_B = 1,
		SUPER = 2,
		CHARM = 3
	}

	public enum Back
	{
		Select = 0,
		Ready = 1,
		Checklist = 2
	}

	private const float POSITION_SPEED = 10f;

	private const float ROLL_SPEED = 8f;

	public RectTransform container;

	[Header("Cards")]
	[SerializeField]
	private MapEquipUICardFront front;

	[SerializeField]
	private MapEquipUICardBackSelect backSelect;

	[SerializeField]
	private MapEquipUICardBackReady backReady;

	[SerializeField]
	private MapEquipUIChecklist checkList;

	[Header("Sprite Assets")]
	public Image[] cupheadImages;

	public Image[] mugmanImages;

	[NonSerialized]
	public bool CanRotate;

	private bool inputEnabled = true;

	private MapEquipUI equipUI;

	private Side side;

	private Back back;

	private Vector2 position;

	private float rotation;

	private float roll;

	private PlayerId playerID;

	private Player playerInput;

	private const float ROTATION_FRONT = 0f;

	private const float ROTATION_BACK = 180f;

	private const float ROTATION_TIME = 0.15f;

	private EaseUtils.EaseType ROTATION_EASE = EaseUtils.EaseType.easeOutBack;

	private IEnumerator rotationCoroutine;

	private const float ROLL_MIN = 1f;

	private const float ROLL_MAX = 4f;

	public bool ReadyAndWaiting { get; private set; }

	protected override void Start()
	{
		base.Start();
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeft;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeft;
	}

	protected override void Update()
	{
		base.Update();
		HandleInput();
	}

	private void HandleInput()
	{
		if (playerInput == null || !inputEnabled || equipUI.CurrentState != MapEquipUI.State.Active || InterruptingPrompt.IsInterrupting())
		{
			return;
		}
		switch (side)
		{
		case Side.Front:
			HandleInputFront();
			break;
		case Side.Back:
			switch (back)
			{
			case Back.Select:
				HandleInputBackSelect();
				break;
			case Back.Ready:
				HandleInputBackReady();
				break;
			case Back.Checklist:
				HandleInputChecklistReady();
				break;
			default:
				Debug.LogWarning(string.Concat("Back '", back, "' not yet configured!"));
				break;
			}
			break;
		default:
			Debug.LogWarning(string.Concat("Side '", side, "' not yet configured!"));
			break;
		}
	}

	private void HandleInputFront()
	{
		if (playerInput.GetButtonDown(14))
		{
			Close();
		}
		else if (playerInput.GetButtonDown(18))
		{
			front.ChangeSelection(-1);
		}
		else if (playerInput.GetButtonDown(20))
		{
			front.ChangeSelection(1);
		}
		else if (front.checkListSelected)
		{
			if (playerInput.GetButtonDown(13))
			{
				int cursorPosition = 0;
				if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_2)
				{
					cursorPosition = 1;
				}
				else if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_3)
				{
					cursorPosition = 2;
				}
				else if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_4)
				{
					cursorPosition = 3;
				}
				checkList.SetCursorPosition(cursorPosition);
				RotateToCheckList();
			}
		}
		else if (playerInput.GetButtonDown(13))
		{
			RotateToBackSelect(front.Slot);
		}
	}

	private void HandleInputBackSelect()
	{
		if (playerInput.GetButtonDown(14))
		{
			front.ChangeSelection(0);
			RotateToFront();
		}
		else if (playerInput.GetButtonDown(15))
		{
			front.ChangeSelection(0);
			RotateToFront();
		}
		else if (playerInput.GetButtonDown(18))
		{
			backSelect.ChangeSelection(new Trilean2(-1, 0));
		}
		else if (playerInput.GetButtonDown(20))
		{
			backSelect.ChangeSelection(new Trilean2(1, 0));
		}
		else if (playerInput.GetButtonDown(16))
		{
			backSelect.ChangeSelection(new Trilean2(0, 1));
		}
		else if (playerInput.GetButtonDown(19))
		{
			backSelect.ChangeSelection(new Trilean2(0, -1));
		}
		else if (playerInput.GetButtonDown(11))
		{
			backSelect.ChangeSlot(1);
		}
		else if (playerInput.GetButtonDown(12))
		{
			backSelect.ChangeSlot(-1);
		}
		else if (playerInput.GetButtonDown(13))
		{
			backSelect.Accept();
		}
	}

	private void HandleInputBackReady()
	{
		if (playerInput.GetButtonDown(13))
		{
			RotateToFront();
		}
		else if (playerInput.GetButtonDown(15))
		{
			RotateToFront();
		}
	}

	private void HandleInputChecklistReady()
	{
		if (playerInput.GetButtonDown(14))
		{
			RotateToFront();
		}
		else if (playerInput.GetButtonDown(18))
		{
			checkList.ChangeSelection(-1);
		}
		else if (playerInput.GetButtonDown(20))
		{
			checkList.ChangeSelection(1);
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		base.transform.localPosition = Vector2.Lerp(base.transform.localPosition, position, Time.deltaTime * 10f);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(0f, 0f, roll), Time.deltaTime * 8f);
		if (rotation > 90f)
		{
			side = Side.Back;
			SetBackActive(true);
			front.SetActive(false);
		}
		else
		{
			side = Side.Front;
			SetBackActive(false);
			front.SetActive(true);
		}
	}

	private void Close()
	{
		if (!equipUI.Close())
		{
			RotateToBackReady();
		}
	}

	public void Init(PlayerId id, MapEquipUI equipUI)
	{
		playerID = id;
		this.equipUI = equipUI;
		playerInput = PlayerManager.GetPlayerInput(id);
		backSelect.transform.SetScale(-1f);
		backReady.transform.SetScale(-1f);
		checkList.transform.SetScale(-1f);
		Image[] array = cupheadImages;
		foreach (Image image in array)
		{
			image.gameObject.SetActive(false);
		}
		Image[] array2 = mugmanImages;
		foreach (Image image2 in array2)
		{
			image2.gameObject.SetActive(false);
		}
		switch (playerID)
		{
		case PlayerId.PlayerOne:
		{
			Image[] array4 = cupheadImages;
			foreach (Image image4 in array4)
			{
				image4.gameObject.SetActive(true);
			}
			break;
		}
		case PlayerId.PlayerTwo:
		{
			Image[] array3 = mugmanImages;
			foreach (Image image3 in array3)
			{
				image3.gameObject.SetActive(true);
			}
			break;
		}
		case PlayerId.Any:
		case PlayerId.None:
			Debug.LogWarning(string.Concat("Player.", playerID, " not supported! Something is wrong"));
			break;
		default:
			Debug.LogWarning(string.Concat("Player '", playerID, "' not configured!"));
			break;
		}
		front.Init(playerID);
		backSelect.Init(playerID);
		checkList.Init(playerID);
	}

	private void OnPlayerJoined(PlayerId playerId)
	{
		SetActive(true);
		if (playerID == PlayerId.PlayerTwo)
		{
			SetMultiplayerOut(true);
		}
		SetMultiplayerIn();
	}

	private void OnPlayerLeft(PlayerId playerId)
	{
		if (playerID == PlayerId.PlayerTwo)
		{
			SetMultiplayerOut();
		}
		else
		{
			SetSinglePlayerIn();
		}
	}

	public void SetActive(bool active)
	{
		if (!(base.gameObject == null))
		{
			base.gameObject.SetActive(active);
		}
	}

	private void SetBackActive(bool active)
	{
		backSelect.SetActive(false);
		backReady.SetActive(false);
		checkList.SetActive(false);
		if (active)
		{
			switch (back)
			{
			case Back.Select:
				backSelect.SetActive(active);
				break;
			case Back.Ready:
				backReady.SetActive(active);
				break;
			case Back.Checklist:
				checkList.SetActive(active);
				break;
			default:
				Debug.LogWarning("Back '' not configured");
				break;
			}
		}
	}

	public void SetSinglePlayerIn(bool instant = false)
	{
		ResetToFront();
		SetPosition(Vector2.zero, instant);
		SetRoll(UnityEngine.Random.Range(-4f, 4f), instant);
	}

	public void SetSinglePlayerOut(bool instant = false)
	{
		SetPosition(new Vector2(0f, -720f), instant);
		SetRoll(0f, instant);
	}

	public void SetMultiplayerIn(bool instant = false)
	{
		ResetToFront();
		Vector2 pos = new Vector2(-320f, 0f);
		if (playerID == PlayerId.PlayerTwo)
		{
			pos.x *= -1f;
		}
		SetPosition(pos, instant);
		SetRoll(UnityEngine.Random.Range(-4f, 4f), instant);
	}

	public void SetMultiplayerOut(bool instant = false)
	{
		Vector2 pos = new Vector2(-1280f, 0f);
		if (playerID == PlayerId.PlayerTwo)
		{
			pos.x *= -1f;
		}
		SetPosition(pos, instant);
		SetRoll(0f, instant);
	}

	private void SetPosition(Vector2 pos, bool instant)
	{
		position = pos;
		if (instant)
		{
			base.transform.localPosition = position;
		}
	}

	private void RotateToFront()
	{
		AudioManager.Play("menu_cardflip");
		front.Refresh();
		if (CanRotate)
		{
			ReadyAndWaiting = false;
			StartRotation(rotation, 0f);
		}
	}

	private void ResetToFront()
	{
		StopRotation();
		SetRotation(0f);
		ReadyAndWaiting = false;
	}

	private void RotateToBackSelect(Slot slot)
	{
		AudioManager.Play("menu_cardflip");
		backSelect.Setup(slot);
		if (CanRotate)
		{
			back = Back.Select;
			StartRotation(rotation, 180f);
		}
	}

	private void RotateToBackReady()
	{
		if (CanRotate)
		{
			ReadyAndWaiting = true;
			back = Back.Ready;
			AudioManager.Play("menu_ready");
			StartRotation(rotation, 180f);
		}
	}

	private void RotateToCheckList()
	{
		AudioManager.Play("menu_cardflip");
		if (CanRotate)
		{
			back = Back.Checklist;
			StartRotation(rotation, 180f);
		}
	}

	private void StartRotation(float start, float end)
	{
		StopRotation();
		if (CanRotate)
		{
			rotationCoroutine = rotate_cr(start, end, 0.15f);
			StartCoroutine(rotationCoroutine);
		}
	}

	private void StopRotation()
	{
		if (rotationCoroutine != null)
		{
			StopCoroutine(rotationCoroutine);
		}
		rotationCoroutine = null;
	}

	private void SetRotation(float r)
	{
		rotation = r;
		container.SetLocalEulerAngles(null, rotation);
	}

	private IEnumerator rotate_cr(float start, float end, float time)
	{
		inputEnabled = false;
		float t = 0f;
		while (t < time)
		{
			SetRotation(EaseUtils.Ease(value: t / time, ease: ROTATION_EASE, start: start, end: end));
			t += Time.deltaTime;
			yield return null;
		}
		SetRotation(end);
		inputEnabled = true;
	}

	private void SetRoll(float r, bool instant)
	{
		roll = r;
		if (instant)
		{
			base.transform.SetLocalEulerAngles(null, null, r);
		}
	}
}
