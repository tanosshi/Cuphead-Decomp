using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Rewired.Integration.UnityUI
{
	[AddComponentMenu("Event/Rewired Standalone Input Module")]
	public class RewiredStandaloneInputModule : PointerInputModule
	{
		private const string DEFAULT_ACTION_MOVE_HORIZONTAL = "UIHorizontal";

		private const string DEFAULT_ACTION_MOVE_VERTICAL = "UIVertical";

		private const string DEFAULT_ACTION_SUBMIT = "UISubmit";

		private const string DEFAULT_ACTION_CANCEL = "UICancel";

		private int[] playerIds;

		private bool recompiling;

		private bool isTouchSupported;

		[SerializeField]
		[Tooltip("Use all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.")]
		private bool useAllRewiredGamePlayers;

		[SerializeField]
		[Tooltip("Allow the Rewired System Player to control the UI.")]
		private bool useRewiredSystemPlayer;

		[SerializeField]
		[Tooltip("A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.")]
		private int[] rewiredPlayerIds = new int[1];

		[SerializeField]
		[Tooltip("Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.")]
		private bool moveOneElementPerAxisPress;

		private float m_PrevActionTime;

		private Vector2 m_LastMoveVector;

		private int m_ConsecutiveMoveCount;

		private Vector2 m_LastMousePosition;

		private Vector2 m_MousePosition;

		[SerializeField]
		private string m_HorizontalAxis = "UIHorizontal";

		[SerializeField]
		[Tooltip("Name of the vertical axis for movement (if axis events are used).")]
		private string m_VerticalAxis = "UIVertical";

		[SerializeField]
		[Tooltip("Name of the action used to submit.")]
		private string m_SubmitButton = "UISubmit";

		[SerializeField]
		[Tooltip("Name of the action used to cancel.")]
		private string m_CancelButton = "UICancel";

		[SerializeField]
		[Tooltip("Number of selection changes allowed per second when a movement button/axis is held in a direction.")]
		private float m_InputActionsPerSecond = 10f;

		[SerializeField]
		[Tooltip("Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.")]
		private float m_RepeatDelay;

		[SerializeField]
		[Tooltip("Allows the mouse to be used to select elements.")]
		private bool m_allowMouseInput = true;

		[SerializeField]
		[Tooltip("Allows the mouse to be used to select elements if the device also supports touch control.")]
		private bool m_allowMouseInputIfTouchSupported = true;

		[SerializeField]
		[FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
		[Tooltip("Forces the module to always be active.")]
		private bool m_ForceModuleActive;

		public bool UseAllRewiredGamePlayers
		{
			get
			{
				return useAllRewiredGamePlayers;
			}
			set
			{
				bool flag = value != useAllRewiredGamePlayers;
				useAllRewiredGamePlayers = value;
				if (flag)
				{
					SetupRewiredVars();
				}
			}
		}

		public bool UseRewiredSystemPlayer
		{
			get
			{
				return useRewiredSystemPlayer;
			}
			set
			{
				bool flag = value != useRewiredSystemPlayer;
				useRewiredSystemPlayer = value;
				if (flag)
				{
					SetupRewiredVars();
				}
			}
		}

		public int[] RewiredPlayerIds
		{
			get
			{
				return (int[])rewiredPlayerIds.Clone();
			}
			set
			{
				rewiredPlayerIds = ((value == null) ? new int[0] : ((int[])value.Clone()));
				SetupRewiredVars();
			}
		}

		public bool MoveOneElementPerAxisPress
		{
			get
			{
				return moveOneElementPerAxisPress;
			}
			set
			{
				moveOneElementPerAxisPress = value;
			}
		}

		public bool allowMouseInput
		{
			get
			{
				return m_allowMouseInput;
			}
			set
			{
				m_allowMouseInput = value;
			}
		}

		public bool allowMouseInputIfTouchSupported
		{
			get
			{
				return m_allowMouseInputIfTouchSupported;
			}
			set
			{
				m_allowMouseInputIfTouchSupported = value;
			}
		}

		private bool isMouseSupported
		{
			get
			{
				if (!m_allowMouseInput)
				{
					return false;
				}
				return !isTouchSupported || m_allowMouseInputIfTouchSupported;
			}
		}

		[Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead")]
		public bool allowActivationOnMobileDevice
		{
			get
			{
				return m_ForceModuleActive;
			}
			set
			{
				m_ForceModuleActive = value;
			}
		}

		public bool forceModuleActive
		{
			get
			{
				return m_ForceModuleActive;
			}
			set
			{
				m_ForceModuleActive = value;
			}
		}

		public float inputActionsPerSecond
		{
			get
			{
				return m_InputActionsPerSecond;
			}
			set
			{
				m_InputActionsPerSecond = value;
			}
		}

		public float repeatDelay
		{
			get
			{
				return m_RepeatDelay;
			}
			set
			{
				m_RepeatDelay = value;
			}
		}

		public string horizontalAxis
		{
			get
			{
				return m_HorizontalAxis;
			}
			set
			{
				m_HorizontalAxis = value;
			}
		}

		public string verticalAxis
		{
			get
			{
				return m_VerticalAxis;
			}
			set
			{
				m_VerticalAxis = value;
			}
		}

		public string submitButton
		{
			get
			{
				return m_SubmitButton;
			}
			set
			{
				m_SubmitButton = value;
			}
		}

		public string cancelButton
		{
			get
			{
				return m_CancelButton;
			}
			set
			{
				m_CancelButton = value;
			}
		}

		protected RewiredStandaloneInputModule()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			isTouchSupported = Input.touchSupported;
			TouchInputModule component = GetComponent<TouchInputModule>();
			if (component != null)
			{
				component.enabled = false;
			}
			InitializeRewired();
		}

		public override void UpdateModule()
		{
			CheckEditorRecompile();
			if (!recompiling && ReInput.isReady && isMouseSupported)
			{
				m_LastMousePosition = m_MousePosition;
				m_MousePosition = ReInput.controllers.Mouse.screenPosition;
			}
		}

		public override bool IsModuleSupported()
		{
			return true;
		}

		public override bool ShouldActivateModule()
		{
			if (!base.ShouldActivateModule())
			{
				return false;
			}
			if (recompiling)
			{
				return false;
			}
			if (!ReInput.isReady)
			{
				return false;
			}
			bool flag = m_ForceModuleActive;
			for (int i = 0; i < playerIds.Length; i++)
			{
				Player player = ReInput.players.GetPlayer(playerIds[i]);
				if (player != null)
				{
					flag |= player.GetButtonDown(m_SubmitButton);
					flag |= player.GetButtonDown(m_CancelButton);
					if (moveOneElementPerAxisPress)
					{
						flag |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
						flag |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
					}
					else
					{
						flag |= !Mathf.Approximately(player.GetAxisRaw(m_HorizontalAxis), 0f);
						flag |= !Mathf.Approximately(player.GetAxisRaw(m_VerticalAxis), 0f);
					}
				}
			}
			if (isMouseSupported)
			{
				flag |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0f;
				flag |= ReInput.controllers.Mouse.GetButtonDown(0);
			}
			if (isTouchSupported)
			{
				for (int j = 0; j < Input.touchCount; j++)
				{
					Touch touch = Input.GetTouch(j);
					flag |= touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
				}
			}
			return flag;
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			if (isMouseSupported)
			{
				m_LastMousePosition = (m_MousePosition = ReInput.controllers.Mouse.screenPosition);
			}
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, GetBaseEventData());
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			ClearSelection();
		}

		public override void Process()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			bool flag = SendUpdateEventToSelectedObject();
			if (base.eventSystem.sendNavigationEvents)
			{
				if (!flag)
				{
					flag |= SendMoveEventToSelectedObject();
				}
				if (!flag)
				{
					SendSubmitEventToSelectedObject();
				}
			}
			if (!ProcessTouchEvents() && isMouseSupported)
			{
				ProcessMouseEvent();
			}
		}

		private bool ProcessTouchEvents()
		{
			if (!isTouchSupported)
			{
				return false;
			}
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if (touch.type != TouchType.Indirect)
				{
					bool pressed;
					bool released;
					PointerEventData touchPointerEventData = GetTouchPointerEventData(touch, out pressed, out released);
					ProcessTouchPress(touchPointerEventData, pressed, released);
					if (!released)
					{
						ProcessMove(touchPointerEventData);
						ProcessDrag(touchPointerEventData);
					}
					else
					{
						RemovePointerData(touchPointerEventData);
					}
				}
			}
			return Input.touchCount > 0;
		}

		private void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
		{
			GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
			if (pressed)
			{
				pointerEvent.eligibleForClick = true;
				pointerEvent.delta = Vector2.zero;
				pointerEvent.dragging = false;
				pointerEvent.useDragThreshold = true;
				pointerEvent.pressPosition = pointerEvent.position;
				pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
				DeselectIfSelectionChanged(gameObject, pointerEvent);
				if (pointerEvent.pointerEnter != gameObject)
				{
					HandlePointerExitAndEnter(pointerEvent, gameObject);
					pointerEvent.pointerEnter = gameObject;
				}
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy(gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == pointerEvent.lastPress)
				{
					float num = unscaledTime - pointerEvent.clickTime;
					if (num < 0.3f)
					{
						pointerEvent.clickCount++;
					}
					else
					{
						pointerEvent.clickCount = 1;
					}
					pointerEvent.clickTime = unscaledTime;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.pointerPress = gameObject2;
				pointerEvent.rawPointerPress = gameObject;
				pointerEvent.clickTime = unscaledTime;
				pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (pointerEvent.pointerDrag != null)
				{
					ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (released)
			{
				ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (pointerEvent.pointerPress == eventHandler && pointerEvent.eligibleForClick)
				{
					ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
				}
				else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					ExecuteEvents.ExecuteHierarchy(gameObject, pointerEvent, ExecuteEvents.dropHandler);
				}
				pointerEvent.eligibleForClick = false;
				pointerEvent.pointerPress = null;
				pointerEvent.rawPointerPress = null;
				if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
				}
				pointerEvent.dragging = false;
				pointerEvent.pointerDrag = null;
				if (pointerEvent.pointerDrag != null)
				{
					ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
				}
				pointerEvent.pointerDrag = null;
				ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
				pointerEvent.pointerEnter = null;
			}
		}

		protected bool SendSubmitEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			if (recompiling)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			for (int i = 0; i < playerIds.Length; i++)
			{
				Player player = ReInput.players.GetPlayer(playerIds[i]);
				if (player != null)
				{
					if (player.GetButtonDown(m_SubmitButton))
					{
						ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
						break;
					}
					if (player.GetButtonDown(m_CancelButton))
					{
						ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
						break;
					}
				}
			}
			return baseEventData.used;
		}

		private Vector2 GetRawMoveVector()
		{
			if (recompiling)
			{
				return Vector2.zero;
			}
			Vector2 zero = Vector2.zero;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < playerIds.Length; i++)
			{
				Player player = ReInput.players.GetPlayer(playerIds[i]);
				if (player == null)
				{
					continue;
				}
				if (moveOneElementPerAxisPress)
				{
					float num = 0f;
					if (player.GetButtonDown(m_HorizontalAxis))
					{
						num = 1f;
					}
					else if (player.GetNegativeButtonDown(m_HorizontalAxis))
					{
						num = -1f;
					}
					float num2 = 0f;
					if (player.GetButtonDown(m_VerticalAxis))
					{
						num2 = 1f;
					}
					else if (player.GetNegativeButtonDown(m_VerticalAxis))
					{
						num2 = -1f;
					}
					zero.x += num;
					zero.y += num2;
				}
				else
				{
					zero.x += player.GetAxisRaw(m_HorizontalAxis);
					zero.y += player.GetAxisRaw(m_VerticalAxis);
				}
				flag |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
				flag2 |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
			}
			if (flag)
			{
				if (zero.x < 0f)
				{
					zero.x = -1f;
				}
				if (zero.x > 0f)
				{
					zero.x = 1f;
				}
			}
			if (flag2)
			{
				if (zero.y < 0f)
				{
					zero.y = -1f;
				}
				if (zero.y > 0f)
				{
					zero.y = 1f;
				}
			}
			return zero;
		}

		protected bool SendMoveEventToSelectedObject()
		{
			if (recompiling)
			{
				return false;
			}
			float unscaledTime = Time.unscaledTime;
			Vector2 rawMoveVector = GetRawMoveVector();
			if (Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f))
			{
				m_ConsecutiveMoveCount = 0;
				return false;
			}
			bool flag = Vector2.Dot(rawMoveVector, m_LastMoveVector) > 0f;
			bool flag2 = CheckButtonOrKeyMovement(unscaledTime);
			bool flag3 = flag2;
			if (!flag3)
			{
				flag3 = ((!(m_RepeatDelay > 0f)) ? (unscaledTime > m_PrevActionTime + 1f / m_InputActionsPerSecond) : ((!flag || m_ConsecutiveMoveCount != 1) ? (unscaledTime > m_PrevActionTime + 1f / m_InputActionsPerSecond) : (unscaledTime > m_PrevActionTime + m_RepeatDelay)));
			}
			if (!flag3)
			{
				return false;
			}
			AxisEventData axisEventData = GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
			if (axisEventData.moveDir == MoveDirection.None)
			{
				return false;
			}
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			if (!flag)
			{
				m_ConsecutiveMoveCount = 0;
			}
			m_ConsecutiveMoveCount++;
			m_PrevActionTime = unscaledTime;
			m_LastMoveVector = rawMoveVector;
			return axisEventData.used;
		}

		private bool CheckButtonOrKeyMovement(float time)
		{
			bool flag = false;
			for (int i = 0; i < playerIds.Length; i++)
			{
				Player player = ReInput.players.GetPlayer(playerIds[i]);
				if (player != null)
				{
					flag |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
					flag |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
				}
			}
			return flag;
		}

		protected void ProcessMouseEvent()
		{
			ProcessMouseEvent(0);
		}

		protected void ProcessMouseEvent(int id)
		{
			MouseState mousePointerEventData = GetMousePointerEventData(id);
			MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
			ProcessMousePress(eventData);
			ProcessMove(eventData.buttonData);
			ProcessDrag(eventData.buttonData);
			ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
			ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
			ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
			ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
			if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
			{
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject);
				ExecuteEvents.ExecuteHierarchy(eventHandler, eventData.buttonData, ExecuteEvents.scrollHandler);
			}
		}

		protected bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = GetBaseEventData();
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}

		protected void ProcessMousePress(MouseButtonEventData data)
		{
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == buttonData.lastPress)
				{
					float num = unscaledTime - buttonData.clickTime;
					if (num < 0.3f)
					{
						buttonData.clickCount++;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = gameObject2;
				buttonData.rawPointerPress = gameObject;
				buttonData.clickTime = unscaledTime;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
				}
				else if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.ExecuteHierarchy(gameObject, buttonData, ExecuteEvents.dropHandler);
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.Execute(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
				}
				buttonData.dragging = false;
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					HandlePointerExitAndEnter(buttonData, null);
					HandlePointerExitAndEnter(buttonData, gameObject);
				}
			}
		}

		private void InitializeRewired()
		{
			if (!ReInput.isReady)
			{
				Debug.LogError("Rewired is not initialized! Are you missing a Rewired Input Manager in your scene?");
				return;
			}
			ReInput.EditorRecompileEvent += OnEditorRecompile;
			SetupRewiredVars();
		}

		private void SetupRewiredVars()
		{
			if (useAllRewiredGamePlayers)
			{
				IList<Player> list = ((!useRewiredSystemPlayer) ? ReInput.players.Players : ReInput.players.AllPlayers);
				playerIds = new int[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					playerIds[i] = list[i].id;
				}
				return;
			}
			int num = rewiredPlayerIds.Length + (useRewiredSystemPlayer ? 1 : 0);
			playerIds = new int[num];
			for (int j = 0; j < rewiredPlayerIds.Length; j++)
			{
				playerIds[j] = ReInput.players.GetPlayer(rewiredPlayerIds[j]).id;
			}
			if (useRewiredSystemPlayer)
			{
				playerIds[num - 1] = ReInput.players.GetSystemPlayer().id;
			}
		}

		private void CheckEditorRecompile()
		{
			if (recompiling && ReInput.isReady)
			{
				recompiling = false;
				InitializeRewired();
			}
		}

		private void OnEditorRecompile()
		{
			recompiling = true;
			ClearRewiredVars();
		}

		private void ClearRewiredVars()
		{
			Array.Clear(playerIds, 0, playerIds.Length);
		}
	}
}
