using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class ControlMapper : MonoBehaviour
	{
		private abstract class GUIElement
		{
			public readonly GameObject gameObject;

			protected readonly Text text;

			public readonly Selectable selectable;

			protected readonly UIElementInfo uiElementInfo;

			protected bool permanentStateSet;

			protected readonly List<GUIElement> children;

			public RectTransform rectTransform { get; private set; }

			public GUIElement(GameObject gameObject)
			{
				if (gameObject == null)
				{
					Debug.LogError("Rewired Control Mapper: gameObject is null!");
					return;
				}
				selectable = gameObject.GetComponent<Selectable>();
				if (selectable == null)
				{
					Debug.LogError("Rewired Control Mapper: Selectable is null!");
					return;
				}
				this.gameObject = gameObject;
				rectTransform = gameObject.GetComponent<RectTransform>();
				text = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
				uiElementInfo = gameObject.GetComponent<UIElementInfo>();
				children = new List<GUIElement>();
			}

			public GUIElement(Selectable selectable, Text label)
			{
				if (selectable == null)
				{
					Debug.LogError("Rewired Control Mapper: Selectable is null!");
					return;
				}
				this.selectable = selectable;
				gameObject = selectable.gameObject;
				rectTransform = gameObject.GetComponent<RectTransform>();
				text = label;
				uiElementInfo = gameObject.GetComponent<UIElementInfo>();
				children = new List<GUIElement>();
			}

			public virtual void SetInteractible(bool state, bool playTransition)
			{
				SetInteractible(state, playTransition, false);
			}

			public virtual void SetInteractible(bool state, bool playTransition, bool permanent)
			{
				for (int i = 0; i < children.Count; i++)
				{
					if (children[i] != null)
					{
						children[i].SetInteractible(state, playTransition, permanent);
					}
				}
				if (!permanentStateSet && !(selectable == null))
				{
					if (permanent)
					{
						permanentStateSet = true;
					}
					if (selectable.interactable != state)
					{
						UITools.SetInteractable(selectable, state, playTransition);
					}
				}
			}

			public virtual void SetTextWidth(int value)
			{
				if (!(text == null))
				{
					LayoutElement layoutElement = text.GetComponent<LayoutElement>();
					if (layoutElement == null)
					{
						layoutElement = text.gameObject.AddComponent<LayoutElement>();
					}
					layoutElement.preferredWidth = value;
				}
			}

			public virtual void SetFirstChildObjectWidth(LayoutElementSizeType type, int value)
			{
				if (rectTransform.childCount != 0)
				{
					Transform child = rectTransform.GetChild(0);
					LayoutElement layoutElement = child.GetComponent<LayoutElement>();
					if (layoutElement == null)
					{
						layoutElement = child.gameObject.AddComponent<LayoutElement>();
					}
					switch (type)
					{
					case LayoutElementSizeType.MinSize:
						layoutElement.minWidth = value;
						break;
					case LayoutElementSizeType.PreferredSize:
						layoutElement.preferredWidth = value;
						break;
					default:
						throw new NotImplementedException();
					}
				}
			}

			public virtual void SetLabel(string label)
			{
				if (!(text == null))
				{
					text.text = label;
				}
			}

			public virtual string GetLabel()
			{
				if (text == null)
				{
					return string.Empty;
				}
				return text.text;
			}

			public virtual void AddChild(GUIElement child)
			{
				children.Add(child);
			}

			public void SetElementInfoData(string identifier, int intData)
			{
				if (!(uiElementInfo == null))
				{
					uiElementInfo.identifier = identifier;
					uiElementInfo.intData = intData;
				}
			}

			public virtual void SetActive(bool state)
			{
				if (!(gameObject == null))
				{
					gameObject.SetActive(state);
				}
			}

			protected virtual bool Init()
			{
				bool result = true;
				for (int i = 0; i < children.Count; i++)
				{
					if (children[i] != null && !children[i].Init())
					{
						result = false;
					}
				}
				if (selectable == null)
				{
					Debug.LogError("Rewired Control Mapper: UI Element is missing Selectable component!");
					result = false;
				}
				if (rectTransform == null)
				{
					Debug.LogError("Rewired Control Mapper: UI Element is missing RectTransform component!");
					result = false;
				}
				if (uiElementInfo == null)
				{
					Debug.LogError("Rewired Control Mapper: UI Element is missing UIElementInfo component!");
					result = false;
				}
				return result;
			}
		}

		private class GUIButton : GUIElement
		{
			protected Button button
			{
				get
				{
					return selectable as Button;
				}
			}

			public ButtonInfo buttonInfo
			{
				get
				{
					return uiElementInfo as ButtonInfo;
				}
			}

			public GUIButton(GameObject gameObject)
				: base(gameObject)
			{
				if (Init())
				{
				}
			}

			public GUIButton(Button button, Text label)
				: base(button, label)
			{
				if (Init())
				{
				}
			}

			public void SetButtonInfoData(string identifier, int intData)
			{
				SetElementInfoData(identifier, intData);
			}

			public void SetOnClickCallback(Action<ButtonInfo> callback)
			{
				if (!(button == null))
				{
					button.onClick.AddListener(delegate
					{
						callback(buttonInfo);
					});
				}
			}
		}

		private class GUIInputField : GUIElement
		{
			protected Button button
			{
				get
				{
					return selectable as Button;
				}
			}

			public InputFieldInfo fieldInfo
			{
				get
				{
					return uiElementInfo as InputFieldInfo;
				}
			}

			public bool hasToggle
			{
				get
				{
					return toggle != null;
				}
			}

			public GUIToggle toggle { get; private set; }

			public int actionElementMapId
			{
				get
				{
					if (fieldInfo == null)
					{
						return -1;
					}
					return fieldInfo.actionElementMapId;
				}
				set
				{
					if (!(fieldInfo == null))
					{
						fieldInfo.actionElementMapId = value;
					}
				}
			}

			public int controllerId
			{
				get
				{
					if (fieldInfo == null)
					{
						return -1;
					}
					return fieldInfo.controllerId;
				}
				set
				{
					if (!(fieldInfo == null))
					{
						fieldInfo.controllerId = value;
					}
				}
			}

			public GUIInputField(GameObject gameObject)
				: base(gameObject)
			{
				if (Init())
				{
				}
			}

			public GUIInputField(Button button, Text label)
				: base(button, label)
			{
				if (Init())
				{
				}
			}

			public void SetFieldInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
			{
				SetElementInfoData(string.Empty, intData);
				if (!(fieldInfo == null))
				{
					fieldInfo.actionId = actionId;
					fieldInfo.axisRange = axisRange;
					fieldInfo.controllerType = controllerType;
				}
			}

			public void SetOnClickCallback(Action<InputFieldInfo> callback)
			{
				if (!(button == null))
				{
					button.onClick.AddListener(delegate
					{
						callback(fieldInfo);
					});
				}
			}

			public virtual void SetInteractable(bool state, bool playTransition, bool permanent)
			{
				if (!permanentStateSet)
				{
					if (hasToggle && !state)
					{
						toggle.SetInteractible(state, playTransition, permanent);
					}
					base.SetInteractible(state, playTransition, permanent);
				}
			}

			public void AddToggle(GUIToggle toggle)
			{
				if (toggle != null)
				{
					this.toggle = toggle;
				}
			}
		}

		private class GUIToggle : GUIElement
		{
			protected Toggle toggle
			{
				get
				{
					return selectable as Toggle;
				}
			}

			public ToggleInfo toggleInfo
			{
				get
				{
					return uiElementInfo as ToggleInfo;
				}
			}

			public int actionElementMapId
			{
				get
				{
					if (toggleInfo == null)
					{
						return -1;
					}
					return toggleInfo.actionElementMapId;
				}
				set
				{
					if (!(toggleInfo == null))
					{
						toggleInfo.actionElementMapId = value;
					}
				}
			}

			public GUIToggle(GameObject gameObject)
				: base(gameObject)
			{
				if (Init())
				{
				}
			}

			public GUIToggle(Toggle toggle, Text label)
				: base(toggle, label)
			{
				if (Init())
				{
				}
			}

			public void SetToggleInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
			{
				SetElementInfoData(string.Empty, intData);
				if (!(toggleInfo == null))
				{
					toggleInfo.actionId = actionId;
					toggleInfo.axisRange = axisRange;
					toggleInfo.controllerType = controllerType;
				}
			}

			public void SetOnSubmitCallback(Action<ToggleInfo, bool> callback)
			{
				if (toggle == null)
				{
					return;
				}
				EventTrigger eventTrigger = toggle.GetComponent<EventTrigger>();
				if (eventTrigger == null)
				{
					eventTrigger = toggle.gameObject.AddComponent<EventTrigger>();
				}
				EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
				triggerEvent.AddListener(delegate(BaseEventData data)
				{
					PointerEventData pointerEventData = data as PointerEventData;
					if (pointerEventData == null || pointerEventData.button == PointerEventData.InputButton.Left)
					{
						callback(toggleInfo, toggle.isOn);
					}
				});
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.callback = triggerEvent;
				entry.eventID = EventTriggerType.Submit;
				EventTrigger.Entry item = entry;
				entry = new EventTrigger.Entry();
				entry.callback = triggerEvent;
				entry.eventID = EventTriggerType.PointerClick;
				EventTrigger.Entry item2 = entry;
				if (eventTrigger.triggers != null)
				{
					eventTrigger.triggers.Clear();
				}
				else
				{
					eventTrigger.triggers = new List<EventTrigger.Entry>();
				}
				eventTrigger.triggers.Add(item);
				eventTrigger.triggers.Add(item2);
			}

			public void SetToggleState(bool state)
			{
				if (!(toggle == null))
				{
					toggle.isOn = state;
				}
			}
		}

		private class GUILabel
		{
			public GameObject gameObject { get; private set; }

			private Text text { get; set; }

			public RectTransform rectTransform { get; private set; }

			public GUILabel(GameObject gameObject)
			{
				if (gameObject == null)
				{
					Debug.LogError("Rewired Control Mapper: gameObject is null!");
					return;
				}
				text = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
				Check();
			}

			public GUILabel(Text label)
			{
				text = label;
				if (Check())
				{
				}
			}

			public void SetSize(int width, int height)
			{
				if (!(text == null))
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				}
			}

			public void SetWidth(int width)
			{
				if (!(text == null))
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				}
			}

			public void SetHeight(int height)
			{
				if (!(text == null))
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				}
			}

			public void SetLabel(string label)
			{
				if (!(text == null))
				{
					text.text = label;
				}
			}

			public void SetFontStyle(FontStyle style)
			{
				if (!(text == null))
				{
					text.fontStyle = style;
				}
			}

			public void SetTextAlignment(TextAnchor alignment)
			{
				if (!(text == null))
				{
					text.alignment = alignment;
				}
			}

			public void SetActive(bool state)
			{
				if (!(gameObject == null))
				{
					gameObject.SetActive(state);
				}
			}

			private bool Check()
			{
				bool result = true;
				if (text == null)
				{
					Debug.LogError("Rewired Control Mapper: Button is missing Text child component!");
					result = false;
				}
				gameObject = text.gameObject;
				rectTransform = text.GetComponent<RectTransform>();
				return result;
			}
		}

		[Serializable]
		public class MappingSet
		{
			public enum ActionListMode
			{
				ActionCategory = 0,
				Action = 1
			}

			[SerializeField]
			[Tooltip("The Map Category that will be displayed to the user for remapping.")]
			private int _mapCategoryId;

			[SerializeField]
			[Tooltip("Choose whether you want to list Actions to display for this Map Category by individual Action or by all the Actions in an Action Category.")]
			private ActionListMode _actionListMode;

			[SerializeField]
			private int[] _actionCategoryIds;

			[SerializeField]
			private int[] _actionIds;

			private IList<int> _actionCategoryIdsReadOnly;

			private IList<int> _actionIdsReadOnly;

			public int mapCategoryId
			{
				get
				{
					return _mapCategoryId;
				}
			}

			public ActionListMode actionListMode
			{
				get
				{
					return _actionListMode;
				}
			}

			public IList<int> actionCategoryIds
			{
				get
				{
					if (_actionCategoryIds == null)
					{
						return null;
					}
					if (_actionCategoryIdsReadOnly == null)
					{
						_actionCategoryIdsReadOnly = new ReadOnlyCollection<int>(_actionCategoryIds);
					}
					return _actionCategoryIdsReadOnly;
				}
			}

			public IList<int> actionIds
			{
				get
				{
					if (_actionIds == null)
					{
						return null;
					}
					if (_actionIdsReadOnly == null)
					{
						_actionIdsReadOnly = new ReadOnlyCollection<int>(_actionIds);
					}
					return _actionIds;
				}
			}

			public bool isValid
			{
				get
				{
					if (_mapCategoryId < 0 || ReInput.mapping.GetMapCategory(_mapCategoryId) == null)
					{
						return false;
					}
					return true;
				}
			}

			public static MappingSet Default
			{
				get
				{
					return new MappingSet(0, ActionListMode.ActionCategory, new int[1], new int[0]);
				}
			}

			public MappingSet()
			{
				_mapCategoryId = -1;
				_actionCategoryIds = new int[0];
				_actionIds = new int[0];
				_actionListMode = ActionListMode.ActionCategory;
			}

			private MappingSet(int mapCategoryId, ActionListMode actionListMode, int[] actionCategoryIds, int[] actionIds)
			{
				_mapCategoryId = mapCategoryId;
				_actionListMode = actionListMode;
				_actionCategoryIds = actionCategoryIds;
				_actionIds = actionIds;
			}
		}

		[Serializable]
		public class InputBehaviorSettings
		{
			[SerializeField]
			[Tooltip("The Input Behavior that will be displayed to the user for modification.")]
			private int _inputBehaviorId = -1;

			[SerializeField]
			[Tooltip("If checked, a slider will be displayed so the user can change this value.")]
			private bool _showJoystickAxisSensitivity = true;

			[SerializeField]
			[Tooltip("If checked, a slider will be displayed so the user can change this value.")]
			private bool _showMouseXYAxisSensitivity = true;

			[SerializeField]
			[Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed as the title for the Input Behavior control set. Otherwise, the name field of the InputBehavior will be used.")]
			private string _labelLanguageKey = string.Empty;

			[SerializeField]
			[Tooltip("If set to a non-blank value, this name will be displayed above the individual slider control. Otherwise, no name will be displayed.")]
			private string _joystickAxisSensitivityLabelLanguageKey = string.Empty;

			[SerializeField]
			[Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed above the individual slider control. Otherwise, no name will be displayed.")]
			private string _mouseXYAxisSensitivityLabelLanguageKey = string.Empty;

			[SerializeField]
			[Tooltip("The icon to display next to the slider. Set to none for no icon.")]
			private Sprite _joystickAxisSensitivityIcon;

			[SerializeField]
			[Tooltip("The icon to display next to the slider. Set to none for no icon.")]
			private Sprite _mouseXYAxisSensitivityIcon;

			[SerializeField]
			[Tooltip("Minimum value the user is allowed to set for this property.")]
			private float _joystickAxisSensitivityMin;

			[SerializeField]
			[Tooltip("Maximum value the user is allowed to set for this property.")]
			private float _joystickAxisSensitivityMax = 2f;

			[SerializeField]
			[Tooltip("Minimum value the user is allowed to set for this property.")]
			private float _mouseXYAxisSensitivityMin;

			[SerializeField]
			[Tooltip("Maximum value the user is allowed to set for this property.")]
			private float _mouseXYAxisSensitivityMax = 2f;

			public int inputBehaviorId
			{
				get
				{
					return _inputBehaviorId;
				}
			}

			public bool showJoystickAxisSensitivity
			{
				get
				{
					return _showJoystickAxisSensitivity;
				}
			}

			public bool showMouseXYAxisSensitivity
			{
				get
				{
					return _showMouseXYAxisSensitivity;
				}
			}

			public string labelLanguageKey
			{
				get
				{
					return _labelLanguageKey;
				}
			}

			public string joystickAxisSensitivityLabelLanguageKey
			{
				get
				{
					return _joystickAxisSensitivityLabelLanguageKey;
				}
			}

			public string mouseXYAxisSensitivityLabelLanguageKey
			{
				get
				{
					return _mouseXYAxisSensitivityLabelLanguageKey;
				}
			}

			public Sprite joystickAxisSensitivityIcon
			{
				get
				{
					return _joystickAxisSensitivityIcon;
				}
			}

			public Sprite mouseXYAxisSensitivityIcon
			{
				get
				{
					return _mouseXYAxisSensitivityIcon;
				}
			}

			public float joystickAxisSensitivityMin
			{
				get
				{
					return _joystickAxisSensitivityMin;
				}
			}

			public float joystickAxisSensitivityMax
			{
				get
				{
					return _joystickAxisSensitivityMax;
				}
			}

			public float mouseXYAxisSensitivityMin
			{
				get
				{
					return _mouseXYAxisSensitivityMin;
				}
			}

			public float mouseXYAxisSensitivityMax
			{
				get
				{
					return _mouseXYAxisSensitivityMax;
				}
			}

			public bool isValid
			{
				get
				{
					return _inputBehaviorId >= 0 && (_showJoystickAxisSensitivity || _showMouseXYAxisSensitivity);
				}
			}
		}

		[Serializable]
		private class Prefabs
		{
			[SerializeField]
			private GameObject _button;

			[SerializeField]
			private GameObject _fitButton;

			[SerializeField]
			private GameObject _inputGridLabel;

			[SerializeField]
			private GameObject _inputGridHeaderLabel;

			[SerializeField]
			private GameObject _inputGridFieldButton;

			[SerializeField]
			private GameObject _inputGridFieldInvertToggle;

			[SerializeField]
			private GameObject _window;

			[SerializeField]
			private GameObject _windowTitleText;

			[SerializeField]
			private GameObject _windowContentText;

			[SerializeField]
			private GameObject _fader;

			[SerializeField]
			private GameObject _calibrationWindow;

			[SerializeField]
			private GameObject _inputBehaviorsWindow;

			[SerializeField]
			private GameObject _centerStickGraphic;

			[SerializeField]
			private GameObject _moveStickGraphic;

			public GameObject button
			{
				get
				{
					return _button;
				}
			}

			public GameObject fitButton
			{
				get
				{
					return _fitButton;
				}
			}

			public GameObject inputGridLabel
			{
				get
				{
					return _inputGridLabel;
				}
			}

			public GameObject inputGridHeaderLabel
			{
				get
				{
					return _inputGridHeaderLabel;
				}
			}

			public GameObject inputGridFieldButton
			{
				get
				{
					return _inputGridFieldButton;
				}
			}

			public GameObject inputGridFieldInvertToggle
			{
				get
				{
					return _inputGridFieldInvertToggle;
				}
			}

			public GameObject window
			{
				get
				{
					return _window;
				}
			}

			public GameObject windowTitleText
			{
				get
				{
					return _windowTitleText;
				}
			}

			public GameObject windowContentText
			{
				get
				{
					return _windowContentText;
				}
			}

			public GameObject fader
			{
				get
				{
					return _fader;
				}
			}

			public GameObject calibrationWindow
			{
				get
				{
					return _calibrationWindow;
				}
			}

			public GameObject inputBehaviorsWindow
			{
				get
				{
					return _inputBehaviorsWindow;
				}
			}

			public GameObject centerStickGraphic
			{
				get
				{
					return _centerStickGraphic;
				}
			}

			public GameObject moveStickGraphic
			{
				get
				{
					return _moveStickGraphic;
				}
			}

			public bool Check()
			{
				if (_button == null || _fitButton == null || _inputGridLabel == null || _inputGridHeaderLabel == null || _inputGridFieldButton == null || _inputGridFieldInvertToggle == null || _window == null || _windowTitleText == null || _windowContentText == null || _fader == null || _calibrationWindow == null || _inputBehaviorsWindow == null)
				{
					return false;
				}
				return true;
			}
		}

		[Serializable]
		private class References
		{
			[SerializeField]
			private Canvas _canvas;

			[SerializeField]
			private CanvasGroup _mainCanvasGroup;

			[SerializeField]
			private Transform _mainContent;

			[SerializeField]
			private Transform _mainContentInner;

			[SerializeField]
			private UIGroup _playersGroup;

			[SerializeField]
			private Transform _controllerGroup;

			[SerializeField]
			private Transform _controllerGroupLabelGroup;

			[SerializeField]
			private UIGroup _controllerSettingsGroup;

			[SerializeField]
			private UIGroup _assignedControllersGroup;

			[SerializeField]
			private Transform _settingsAndMapCategoriesGroup;

			[SerializeField]
			private UIGroup _settingsGroup;

			[SerializeField]
			private UIGroup _mapCategoriesGroup;

			[SerializeField]
			private Transform _inputGridGroup;

			[SerializeField]
			private Transform _inputGridContainer;

			[SerializeField]
			private Transform _inputGridHeadersGroup;

			[SerializeField]
			private Scrollbar _inputGridVScrollbar;

			[SerializeField]
			private ScrollRect _inputGridScrollRect;

			[SerializeField]
			private Transform _inputGridInnerGroup;

			[SerializeField]
			private Text _controllerNameLabel;

			[SerializeField]
			private Button _removeControllerButton;

			[SerializeField]
			private Button _assignControllerButton;

			[SerializeField]
			private Button _calibrateControllerButton;

			[SerializeField]
			private Button _doneButton;

			[SerializeField]
			private Button _restoreDefaultsButton;

			[SerializeField]
			private Selectable _defaultSelection;

			[SerializeField]
			private GameObject[] _fixedSelectableUIElements;

			[SerializeField]
			private Image _mainBackgroundImage;

			public Canvas canvas
			{
				get
				{
					return _canvas;
				}
			}

			public CanvasGroup mainCanvasGroup
			{
				get
				{
					return _mainCanvasGroup;
				}
			}

			public Transform mainContent
			{
				get
				{
					return _mainContent;
				}
			}

			public Transform mainContentInner
			{
				get
				{
					return _mainContentInner;
				}
			}

			public UIGroup playersGroup
			{
				get
				{
					return _playersGroup;
				}
			}

			public Transform controllerGroup
			{
				get
				{
					return _controllerGroup;
				}
			}

			public Transform controllerGroupLabelGroup
			{
				get
				{
					return _controllerGroupLabelGroup;
				}
			}

			public UIGroup controllerSettingsGroup
			{
				get
				{
					return _controllerSettingsGroup;
				}
			}

			public UIGroup assignedControllersGroup
			{
				get
				{
					return _assignedControllersGroup;
				}
			}

			public Transform settingsAndMapCategoriesGroup
			{
				get
				{
					return _settingsAndMapCategoriesGroup;
				}
			}

			public UIGroup settingsGroup
			{
				get
				{
					return _settingsGroup;
				}
			}

			public UIGroup mapCategoriesGroup
			{
				get
				{
					return _mapCategoriesGroup;
				}
			}

			public Transform inputGridGroup
			{
				get
				{
					return _inputGridGroup;
				}
			}

			public Transform inputGridContainer
			{
				get
				{
					return _inputGridContainer;
				}
			}

			public Transform inputGridHeadersGroup
			{
				get
				{
					return _inputGridHeadersGroup;
				}
			}

			public Scrollbar inputGridVScrollbar
			{
				get
				{
					return _inputGridVScrollbar;
				}
			}

			public ScrollRect inputGridScrollRect
			{
				get
				{
					return _inputGridScrollRect;
				}
			}

			public Transform inputGridInnerGroup
			{
				get
				{
					return _inputGridInnerGroup;
				}
			}

			public Text controllerNameLabel
			{
				get
				{
					return _controllerNameLabel;
				}
			}

			public Button removeControllerButton
			{
				get
				{
					return _removeControllerButton;
				}
			}

			public Button assignControllerButton
			{
				get
				{
					return _assignControllerButton;
				}
			}

			public Button calibrateControllerButton
			{
				get
				{
					return _calibrateControllerButton;
				}
			}

			public Button doneButton
			{
				get
				{
					return _doneButton;
				}
			}

			public Button restoreDefaultsButton
			{
				get
				{
					return _restoreDefaultsButton;
				}
			}

			public Selectable defaultSelection
			{
				get
				{
					return _defaultSelection;
				}
			}

			public GameObject[] fixedSelectableUIElements
			{
				get
				{
					return _fixedSelectableUIElements;
				}
			}

			public Image mainBackgroundImage
			{
				get
				{
					return _mainBackgroundImage;
				}
			}

			public LayoutElement inputGridLayoutElement { get; set; }

			public Transform inputGridActionColumn { get; set; }

			public Transform inputGridKeyboardColumn { get; set; }

			public Transform inputGridMouseColumn { get; set; }

			public Transform inputGridControllerColumn { get; set; }

			public Transform inputGridHeader1 { get; set; }

			public Transform inputGridHeader2 { get; set; }

			public Transform inputGridHeader3 { get; set; }

			public Transform inputGridHeader4 { get; set; }

			public bool Check()
			{
				if (_canvas == null || _mainCanvasGroup == null || _mainContent == null || _mainContentInner == null || _playersGroup == null || _controllerGroup == null || _controllerGroupLabelGroup == null || _controllerSettingsGroup == null || _assignedControllersGroup == null || _settingsAndMapCategoriesGroup == null || _settingsGroup == null || _mapCategoriesGroup == null || _inputGridGroup == null || _inputGridContainer == null || _inputGridHeadersGroup == null || _inputGridVScrollbar == null || _inputGridScrollRect == null || _inputGridInnerGroup == null || _controllerNameLabel == null || _removeControllerButton == null || _assignControllerButton == null || _calibrateControllerButton == null || _doneButton == null || _restoreDefaultsButton == null || _defaultSelection == null)
				{
					return false;
				}
				return true;
			}
		}

		private class InputActionSet
		{
			private int _actionId;

			private AxisRange _axisRange;

			public int actionId
			{
				get
				{
					return _actionId;
				}
			}

			public AxisRange axisRange
			{
				get
				{
					return _axisRange;
				}
			}

			public InputActionSet(int actionId, AxisRange axisRange)
			{
				_actionId = actionId;
				_axisRange = axisRange;
			}
		}

		private class InputMapping
		{
			public string actionName { get; private set; }

			public InputFieldInfo fieldInfo { get; private set; }

			public ControllerMap map { get; private set; }

			public ActionElementMap aem { get; private set; }

			public ControllerType controllerType { get; private set; }

			public int controllerId { get; private set; }

			public ControllerPollingInfo pollingInfo { get; set; }

			public ModifierKeyFlags modifierKeyFlags { get; set; }

			public AxisRange axisRange
			{
				get
				{
					AxisRange result = AxisRange.Positive;
					if (pollingInfo.elementType == ControllerElementType.Axis)
					{
						result = ((fieldInfo.axisRange != AxisRange.Full) ? ((pollingInfo.axisPole == Pole.Positive) ? AxisRange.Positive : AxisRange.Negative) : AxisRange.Full);
					}
					return result;
				}
			}

			public string elementName
			{
				get
				{
					if (controllerType == ControllerType.Keyboard && modifierKeyFlags != ModifierKeyFlags.None)
					{
						return string.Format("{0} + {1}", Keyboard.ModifierKeyFlagsToString(modifierKeyFlags), pollingInfo.elementIdentifierName);
					}
					string text = pollingInfo.elementIdentifierName;
					if (pollingInfo.elementType == ControllerElementType.Axis)
					{
						if (axisRange == AxisRange.Positive)
						{
							text += " +";
						}
						else if (axisRange == AxisRange.Negative)
						{
							text += " -";
						}
					}
					return text;
				}
			}

			public InputMapping(string actionName, InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, ControllerType controllerType, int controllerId)
			{
				this.actionName = actionName;
				this.fieldInfo = fieldInfo;
				this.map = map;
				this.aem = aem;
				this.controllerType = controllerType;
				this.controllerId = controllerId;
			}

			public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo)
			{
				this.pollingInfo = pollingInfo;
				return ToElementAssignment();
			}

			public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo, ModifierKeyFlags modifierKeyFlags)
			{
				this.pollingInfo = pollingInfo;
				this.modifierKeyFlags = modifierKeyFlags;
				return ToElementAssignment();
			}

			public ElementAssignment ToElementAssignment()
			{
				return new ElementAssignment(controllerType, pollingInfo.elementType, pollingInfo.elementIdentifierId, axisRange, pollingInfo.keyboardKey, modifierKeyFlags, fieldInfo.actionId, (fieldInfo.axisRange == AxisRange.Negative) ? Pole.Negative : Pole.Positive, false, (aem == null) ? (-1) : aem.id);
			}
		}

		private class AxisCalibrator
		{
			public AxisCalibrationData data;

			public readonly Joystick joystick;

			public readonly int axisIndex;

			private Controller.Axis axis;

			private bool firstRun;

			public bool isValid
			{
				get
				{
					return axis != null;
				}
			}

			public AxisCalibrator(Joystick joystick, int axisIndex)
			{
				data = default(AxisCalibrationData);
				this.joystick = joystick;
				this.axisIndex = axisIndex;
				if (joystick != null && axisIndex >= 0 && joystick.axisCount > axisIndex)
				{
					axis = joystick.Axes[axisIndex];
					data = joystick.calibrationMap.GetAxis(axisIndex).GetData();
				}
				firstRun = true;
			}

			public void RecordMinMax()
			{
				if (axis != null)
				{
					float valueRaw = axis.valueRaw;
					if (firstRun || valueRaw < data.min)
					{
						data.min = valueRaw;
					}
					if (firstRun || valueRaw > data.max)
					{
						data.max = valueRaw;
					}
					firstRun = false;
				}
			}

			public void RecordZero()
			{
				if (axis != null)
				{
					data.zero = axis.valueRaw;
				}
			}

			public void Commit()
			{
				if (axis != null)
				{
					AxisCalibration axisCalibration = joystick.calibrationMap.GetAxis(axisIndex);
					if (axisCalibration != null && !((double)Mathf.Abs(data.max - data.min) < 0.1))
					{
						axisCalibration.SetData(data);
					}
				}
			}
		}

		private class IndexedDictionary<TKey, TValue>
		{
			private class Entry
			{
				public TKey key;

				public TValue value;

				public Entry(TKey key, TValue value)
				{
					this.key = key;
					this.value = value;
				}
			}

			private List<Entry> list;

			public int Count
			{
				get
				{
					return list.Count;
				}
			}

			public TValue this[int index]
			{
				get
				{
					return list[index].value;
				}
			}

			public IndexedDictionary()
			{
				list = new List<Entry>();
			}

			public TValue Get(TKey key)
			{
				int num = IndexOfKey(key);
				if (num < 0)
				{
					throw new Exception("Key does not exist!");
				}
				return list[num].value;
			}

			public bool TryGet(TKey key, out TValue value)
			{
				value = default(TValue);
				int num = IndexOfKey(key);
				if (num < 0)
				{
					return false;
				}
				value = list[num].value;
				return true;
			}

			public void Add(TKey key, TValue value)
			{
				if (ContainsKey(key))
				{
					throw new Exception("Key " + key.ToString() + " is already in use!");
				}
				list.Add(new Entry(key, value));
			}

			public int IndexOfKey(TKey key)
			{
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					if (EqualityComparer<TKey>.Default.Equals(list[i].key, key))
					{
						return i;
					}
				}
				return -1;
			}

			public bool ContainsKey(TKey key)
			{
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					if (EqualityComparer<TKey>.Default.Equals(list[i].key, key))
					{
						return true;
					}
				}
				return false;
			}

			public void Clear()
			{
				list.Clear();
			}
		}

		private enum LayoutElementSizeType
		{
			MinSize = 0,
			PreferredSize = 1
		}

		private enum WindowType
		{
			None = 0,
			ChooseJoystick = 1,
			JoystickAssignmentConflict = 2,
			ElementAssignment = 3,
			ElementAssignmentPrePolling = 4,
			ElementAssignmentPolling = 5,
			ElementAssignmentResult = 6,
			ElementAssignmentConflict = 7,
			Calibration = 8,
			CalibrateStep1 = 9,
			CalibrateStep2 = 10
		}

		private class InputGrid
		{
			private InputGridEntryList list;

			private List<GameObject> groups;

			public InputGrid()
			{
				list = new InputGridEntryList();
				groups = new List<GameObject>();
			}

			public void AddMapCategory(int mapCategoryId)
			{
				list.AddMapCategory(mapCategoryId);
			}

			public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				list.AddAction(mapCategoryId, action, axisRange);
			}

			public void AddActionCategory(int mapCategoryId, int actionCategoryId)
			{
				list.AddActionCategory(mapCategoryId, actionCategoryId);
			}

			public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
			{
				list.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, fieldSetContainer);
			}

			public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, GUIInputField inputField)
			{
				list.AddInputField(mapCategoryId, action, axisRange, controllerType, fieldIndex, inputField);
			}

			public void AddGroup(GameObject group)
			{
				groups.Add(group);
			}

			public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, GUILabel label)
			{
				list.AddActionLabel(mapCategoryId, actionId, axisRange, label);
			}

			public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, GUILabel label)
			{
				list.AddActionCategoryLabel(mapCategoryId, actionCategoryId, label);
			}

			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				return list.Contains(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
			}

			public GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				return list.GetGUIInputField(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
			}

			public IEnumerable<InputActionSet> GetActionSets(int mapCategoryId)
			{
				return list.GetActionSets(mapCategoryId);
			}

			public void SetColumnHeight(int mapCategoryId, float height)
			{
				list.SetColumnHeight(mapCategoryId, height);
			}

			public float GetColumnHeight(int mapCategoryId)
			{
				return list.GetColumnHeight(mapCategoryId);
			}

			public void SetFieldsActive(int mapCategoryId, bool state)
			{
				list.SetFieldsActive(mapCategoryId, state);
			}

			public void SetFieldLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
			{
				list.SetLabel(mapCategoryId, actionId, axisRange, controllerType, index, label);
			}

			public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
			{
				list.PopulateField(mapCategoryId, actionId, axisRange, controllerType, controllerId, index, actionElementMapId, label, invert);
			}

			public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
			{
				list.SetFixedFieldData(mapCategoryId, actionId, axisRange, controllerType, controllerId);
			}

			public void InitializeFields(int mapCategoryId)
			{
				list.InitializeFields(mapCategoryId);
			}

			public void Show(int mapCategoryId)
			{
				list.Show(mapCategoryId);
			}

			public void HideAll()
			{
				list.HideAll();
			}

			public void ClearLabels(int mapCategoryId)
			{
				list.ClearLabels(mapCategoryId);
			}

			private void ClearGroups()
			{
				for (int i = 0; i < groups.Count; i++)
				{
					if (!(groups[i] == null))
					{
						UnityEngine.Object.Destroy(groups[i]);
					}
				}
			}

			public void ClearAll()
			{
				ClearGroups();
				list.Clear();
			}
		}

		private class InputGridEntryList
		{
			private class MapCategoryEntry
			{
				private List<ActionEntry> _actionList;

				private IndexedDictionary<int, ActionCategoryEntry> _actionCategoryList;

				private float _columnHeight;

				public List<ActionEntry> actionList
				{
					get
					{
						return _actionList;
					}
				}

				public IndexedDictionary<int, ActionCategoryEntry> actionCategoryList
				{
					get
					{
						return _actionCategoryList;
					}
				}

				public float columnHeight
				{
					get
					{
						return _columnHeight;
					}
					set
					{
						_columnHeight = value;
					}
				}

				public MapCategoryEntry()
				{
					_actionList = new List<ActionEntry>();
					_actionCategoryList = new IndexedDictionary<int, ActionCategoryEntry>();
				}

				public ActionEntry GetActionEntry(int actionId, AxisRange axisRange)
				{
					int num = IndexOfActionEntry(actionId, axisRange);
					if (num < 0)
					{
						return null;
					}
					return _actionList[num];
				}

				public int IndexOfActionEntry(int actionId, AxisRange axisRange)
				{
					int count = _actionList.Count;
					for (int i = 0; i < count; i++)
					{
						if (_actionList[i].Matches(actionId, axisRange))
						{
							return i;
						}
					}
					return -1;
				}

				public bool ContainsActionEntry(int actionId, AxisRange axisRange)
				{
					return IndexOfActionEntry(actionId, axisRange) >= 0;
				}

				public ActionEntry AddAction(InputAction action, AxisRange axisRange)
				{
					if (action == null)
					{
						return null;
					}
					if (ContainsActionEntry(action.id, axisRange))
					{
						return null;
					}
					_actionList.Add(new ActionEntry(action, axisRange));
					return _actionList[_actionList.Count - 1];
				}

				public ActionCategoryEntry GetActionCategoryEntry(int actionCategoryId)
				{
					if (!_actionCategoryList.ContainsKey(actionCategoryId))
					{
						return null;
					}
					return _actionCategoryList.Get(actionCategoryId);
				}

				public ActionCategoryEntry AddActionCategory(int actionCategoryId)
				{
					if (actionCategoryId < 0)
					{
						return null;
					}
					if (_actionCategoryList.ContainsKey(actionCategoryId))
					{
						return null;
					}
					_actionCategoryList.Add(actionCategoryId, new ActionCategoryEntry(actionCategoryId));
					return _actionCategoryList.Get(actionCategoryId);
				}

				public void SetAllActive(bool state)
				{
					for (int i = 0; i < _actionCategoryList.Count; i++)
					{
						_actionCategoryList[i].SetActive(state);
					}
					for (int j = 0; j < _actionList.Count; j++)
					{
						_actionList[j].SetActive(state);
					}
				}
			}

			private class ActionEntry
			{
				private IndexedDictionary<int, FieldSet> fieldSets;

				public GUILabel label;

				public readonly InputAction action;

				public readonly AxisRange axisRange;

				public readonly InputActionSet actionSet;

				public ActionEntry(InputAction action, AxisRange axisRange)
				{
					this.action = action;
					this.axisRange = axisRange;
					actionSet = new InputActionSet(action.id, axisRange);
					fieldSets = new IndexedDictionary<int, FieldSet>();
				}

				public void SetLabel(GUILabel label)
				{
					this.label = label;
				}

				public bool Matches(int actionId, AxisRange axisRange)
				{
					if (action.id != actionId)
					{
						return false;
					}
					if (this.axisRange != axisRange)
					{
						return false;
					}
					return true;
				}

				public void AddInputFieldSet(ControllerType controllerType, GameObject fieldSetContainer)
				{
					if (!fieldSets.ContainsKey((int)controllerType))
					{
						fieldSets.Add((int)controllerType, new FieldSet(fieldSetContainer));
					}
				}

				public void AddInputField(ControllerType controllerType, int fieldIndex, GUIInputField inputField)
				{
					if (fieldSets.ContainsKey((int)controllerType))
					{
						FieldSet fieldSet = fieldSets.Get((int)controllerType);
						if (!fieldSet.fields.ContainsKey(fieldIndex))
						{
							fieldSet.fields.Add(fieldIndex, inputField);
						}
					}
				}

				public GUIInputField GetGUIInputField(ControllerType controllerType, int fieldIndex)
				{
					if (!fieldSets.ContainsKey((int)controllerType))
					{
						return null;
					}
					if (!fieldSets.Get((int)controllerType).fields.ContainsKey(fieldIndex))
					{
						return null;
					}
					return fieldSets.Get((int)controllerType).fields.Get(fieldIndex);
				}

				public bool Contains(ControllerType controllerType, int fieldId)
				{
					if (!fieldSets.ContainsKey((int)controllerType))
					{
						return false;
					}
					if (!fieldSets.Get((int)controllerType).fields.ContainsKey(fieldId))
					{
						return false;
					}
					return true;
				}

				public void SetFieldLabel(ControllerType controllerType, int index, string label)
				{
					if (fieldSets.ContainsKey((int)controllerType) && fieldSets.Get((int)controllerType).fields.ContainsKey(index))
					{
						fieldSets.Get((int)controllerType).fields.Get(index).SetLabel(label);
					}
				}

				public void PopulateField(ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
				{
					if (fieldSets.ContainsKey((int)controllerType) && fieldSets.Get((int)controllerType).fields.ContainsKey(index))
					{
						GUIInputField gUIInputField = fieldSets.Get((int)controllerType).fields.Get(index);
						gUIInputField.SetLabel(label);
						gUIInputField.actionElementMapId = actionElementMapId;
						gUIInputField.controllerId = controllerId;
						if (gUIInputField.hasToggle)
						{
							gUIInputField.toggle.SetInteractible(true, false);
							gUIInputField.toggle.SetToggleState(invert);
							gUIInputField.toggle.actionElementMapId = actionElementMapId;
						}
					}
				}

				public void SetFixedFieldData(ControllerType controllerType, int controllerId)
				{
					if (fieldSets.ContainsKey((int)controllerType))
					{
						FieldSet fieldSet = fieldSets.Get((int)controllerType);
						int count = fieldSet.fields.Count;
						for (int i = 0; i < count; i++)
						{
							fieldSet.fields[i].controllerId = controllerId;
						}
					}
				}

				public void Initialize()
				{
					for (int i = 0; i < fieldSets.Count; i++)
					{
						FieldSet fieldSet = fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							GUIInputField gUIInputField = fieldSet.fields[j];
							if (gUIInputField.hasToggle)
							{
								gUIInputField.toggle.SetInteractible(false, false);
								gUIInputField.toggle.SetToggleState(false);
								gUIInputField.toggle.actionElementMapId = -1;
							}
							gUIInputField.SetLabel(string.Empty);
							gUIInputField.actionElementMapId = -1;
							gUIInputField.controllerId = -1;
						}
					}
				}

				public void SetActive(bool state)
				{
					if (label != null)
					{
						label.SetActive(state);
					}
					int count = fieldSets.Count;
					for (int i = 0; i < count; i++)
					{
						fieldSets[i].groupContainer.SetActive(state);
					}
				}

				public void ClearLabels()
				{
					for (int i = 0; i < fieldSets.Count; i++)
					{
						FieldSet fieldSet = fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							GUIInputField gUIInputField = fieldSet.fields[j];
							gUIInputField.SetLabel(string.Empty);
						}
					}
				}

				public void SetFieldsActive(bool state)
				{
					for (int i = 0; i < fieldSets.Count; i++)
					{
						FieldSet fieldSet = fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							GUIInputField gUIInputField = fieldSet.fields[j];
							gUIInputField.SetInteractible(state, false);
							if (gUIInputField.hasToggle && (!state || gUIInputField.toggle.actionElementMapId >= 0))
							{
								gUIInputField.toggle.SetInteractible(state, false);
							}
						}
					}
				}
			}

			private class FieldSet
			{
				public readonly GameObject groupContainer;

				public readonly IndexedDictionary<int, GUIInputField> fields;

				public FieldSet(GameObject groupContainer)
				{
					this.groupContainer = groupContainer;
					fields = new IndexedDictionary<int, GUIInputField>();
				}
			}

			private class ActionCategoryEntry
			{
				public readonly int actionCategoryId;

				public GUILabel label;

				public ActionCategoryEntry(int actionCategoryId)
				{
					this.actionCategoryId = actionCategoryId;
				}

				public void SetLabel(GUILabel label)
				{
					this.label = label;
				}

				public void SetActive(bool state)
				{
					if (label != null)
					{
						label.SetActive(state);
					}
				}
			}

			private IndexedDictionary<int, MapCategoryEntry> entries;

			public InputGridEntryList()
			{
				entries = new IndexedDictionary<int, MapCategoryEntry>();
			}

			public void AddMapCategory(int mapCategoryId)
			{
				if (mapCategoryId >= 0 && !entries.ContainsKey(mapCategoryId))
				{
					entries.Add(mapCategoryId, new MapCategoryEntry());
				}
			}

			public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				AddActionEntry(mapCategoryId, action, axisRange);
			}

			private ActionEntry AddActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				if (action == null)
				{
					return null;
				}
				MapCategoryEntry value;
				if (!entries.TryGet(mapCategoryId, out value))
				{
					return null;
				}
				return value.AddAction(action, axisRange);
			}

			public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, GUILabel label)
			{
				MapCategoryEntry value;
				if (entries.TryGet(mapCategoryId, out value))
				{
					ActionEntry actionEntry = value.GetActionEntry(actionId, axisRange);
					if (actionEntry != null)
					{
						actionEntry.SetLabel(label);
					}
				}
			}

			public void AddActionCategory(int mapCategoryId, int actionCategoryId)
			{
				AddActionCategoryEntry(mapCategoryId, actionCategoryId);
			}

			private ActionCategoryEntry AddActionCategoryEntry(int mapCategoryId, int actionCategoryId)
			{
				MapCategoryEntry value;
				if (!entries.TryGet(mapCategoryId, out value))
				{
					return null;
				}
				return value.AddActionCategory(actionCategoryId);
			}

			public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, GUILabel label)
			{
				MapCategoryEntry value;
				if (entries.TryGet(mapCategoryId, out value))
				{
					ActionCategoryEntry actionCategoryEntry = value.GetActionCategoryEntry(actionCategoryId);
					if (actionCategoryEntry != null)
					{
						actionCategoryEntry.SetLabel(label);
					}
				}
			}

			public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
			{
				ActionEntry actionEntry = GetActionEntry(mapCategoryId, action, axisRange);
				if (actionEntry != null)
				{
					actionEntry.AddInputFieldSet(controllerType, fieldSetContainer);
				}
			}

			public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, GUIInputField inputField)
			{
				ActionEntry actionEntry = GetActionEntry(mapCategoryId, action, axisRange);
				if (actionEntry != null)
				{
					actionEntry.AddInputField(controllerType, fieldIndex, inputField);
				}
			}

			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange)
			{
				return GetActionEntry(mapCategoryId, actionId, axisRange) != null;
			}

			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				ActionEntry actionEntry = GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return false;
				}
				return actionEntry.Contains(controllerType, fieldIndex);
			}

			public void SetColumnHeight(int mapCategoryId, float height)
			{
				MapCategoryEntry value;
				if (entries.TryGet(mapCategoryId, out value))
				{
					value.columnHeight = height;
				}
			}

			public float GetColumnHeight(int mapCategoryId)
			{
				MapCategoryEntry value;
				if (!entries.TryGet(mapCategoryId, out value))
				{
					return 0f;
				}
				return value.columnHeight;
			}

			public GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				ActionEntry actionEntry = GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return null;
				}
				return actionEntry.GetGUIInputField(controllerType, fieldIndex);
			}

			private ActionEntry GetActionEntry(int mapCategoryId, int actionId, AxisRange axisRange)
			{
				if (actionId < 0)
				{
					return null;
				}
				MapCategoryEntry value;
				if (!entries.TryGet(mapCategoryId, out value))
				{
					return null;
				}
				return value.GetActionEntry(actionId, axisRange);
			}

			private ActionEntry GetActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				if (action == null)
				{
					return null;
				}
				return GetActionEntry(mapCategoryId, action.id, axisRange);
			}

			public IEnumerable<InputActionSet> GetActionSets(int mapCategoryId)
			{
				MapCategoryEntry entry;
				if (entries.TryGet(mapCategoryId, out entry))
				{
					List<ActionEntry> list = entry.actionList;
					int count = ((list != null) ? list.Count : 0);
					for (int i = 0; i < count; i++)
					{
						yield return list[i].actionSet;
					}
				}
			}

			public void SetFieldsActive(int mapCategoryId, bool state)
			{
				MapCategoryEntry value;
				if (entries.TryGet(mapCategoryId, out value))
				{
					List<ActionEntry> actionList = value.actionList;
					int num = ((actionList != null) ? actionList.Count : 0);
					for (int i = 0; i < num; i++)
					{
						actionList[i].SetFieldsActive(state);
					}
				}
			}

			public void SetLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
			{
				ActionEntry actionEntry = GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry != null)
				{
					actionEntry.SetFieldLabel(controllerType, index, label);
				}
			}

			public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
			{
				ActionEntry actionEntry = GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry != null)
				{
					actionEntry.PopulateField(controllerType, controllerId, index, actionElementMapId, label, invert);
				}
			}

			public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
			{
				ActionEntry actionEntry = GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry != null)
				{
					actionEntry.SetFixedFieldData(controllerType, controllerId);
				}
			}

			public void InitializeFields(int mapCategoryId)
			{
				MapCategoryEntry value;
				if (entries.TryGet(mapCategoryId, out value))
				{
					List<ActionEntry> actionList = value.actionList;
					int num = ((actionList != null) ? actionList.Count : 0);
					for (int i = 0; i < num; i++)
					{
						actionList[i].Initialize();
					}
				}
			}

			public void Show(int mapCategoryId)
			{
				MapCategoryEntry value;
				if (entries.TryGet(mapCategoryId, out value))
				{
					value.SetAllActive(true);
				}
			}

			public void HideAll()
			{
				for (int i = 0; i < entries.Count; i++)
				{
					entries[i].SetAllActive(false);
				}
			}

			public void ClearLabels(int mapCategoryId)
			{
				MapCategoryEntry value;
				if (entries.TryGet(mapCategoryId, out value))
				{
					List<ActionEntry> actionList = value.actionList;
					int num = ((actionList != null) ? actionList.Count : 0);
					for (int i = 0; i < num; i++)
					{
						actionList[i].ClearLabels();
					}
				}
			}

			public void Clear()
			{
				entries.Clear();
			}
		}

		private class WindowManager
		{
			private List<Window> windows;

			private GameObject windowPrefab;

			private Transform parent;

			private GameObject fader;

			private int idCounter;

			public bool isWindowOpen
			{
				get
				{
					for (int num = windows.Count - 1; num >= 0; num--)
					{
						if (!(windows[num] == null))
						{
							return true;
						}
					}
					return false;
				}
			}

			public Window topWindow
			{
				get
				{
					for (int num = windows.Count - 1; num >= 0; num--)
					{
						if (!(windows[num] == null))
						{
							return windows[num];
						}
					}
					return null;
				}
			}

			public WindowManager(GameObject windowPrefab, GameObject faderPrefab, Transform parent)
			{
				this.windowPrefab = windowPrefab;
				this.parent = parent;
				windows = new List<Window>();
				fader = UnityEngine.Object.Instantiate(faderPrefab);
				fader.transform.SetParent(parent, false);
				fader.GetComponent<RectTransform>().localScale = Vector2.one;
				SetFaderActive(false);
			}

			public Window OpenWindow(string name, int width, int height)
			{
				Window result = InstantiateWindow(name, width, height);
				UpdateFader();
				return result;
			}

			public Window OpenWindow(GameObject windowPrefab, string name)
			{
				if (windowPrefab == null)
				{
					Debug.LogError("Rewired Control Mapper: Window Prefab is null!");
					return null;
				}
				Window result = InstantiateWindow(name, windowPrefab);
				UpdateFader();
				return result;
			}

			public void CloseTop()
			{
				int num = windows.Count - 1;
				while (num >= 0)
				{
					if (windows[num] == null)
					{
						windows.RemoveAt(num);
						num--;
						continue;
					}
					DestroyWindow(windows[num]);
					windows.RemoveAt(num);
					break;
				}
				UpdateFader();
			}

			public void CloseWindow(int windowId)
			{
				CloseWindow(GetWindow(windowId));
			}

			public void CloseWindow(Window window)
			{
				if (window == null)
				{
					return;
				}
				for (int num = windows.Count - 1; num >= 0; num--)
				{
					if (windows[num] == null)
					{
						windows.RemoveAt(num);
					}
					else if (!(windows[num] != window))
					{
						DestroyWindow(windows[num]);
						windows.RemoveAt(num);
						break;
					}
				}
				UpdateFader();
				FocusTopWindow();
			}

			public void CloseAll()
			{
				SetFaderActive(false);
				for (int num = windows.Count - 1; num >= 0; num--)
				{
					if (windows[num] == null)
					{
						windows.RemoveAt(num);
					}
					else
					{
						DestroyWindow(windows[num]);
						windows.RemoveAt(num);
					}
				}
				UpdateFader();
			}

			public void CancelAll()
			{
				if (!isWindowOpen)
				{
					return;
				}
				for (int num = windows.Count - 1; num >= 0; num--)
				{
					if (!(windows[num] == null))
					{
						windows[num].Cancel();
					}
				}
				CloseAll();
			}

			public Window GetWindow(int windowId)
			{
				if (windowId < 0)
				{
					return null;
				}
				for (int num = windows.Count - 1; num >= 0; num--)
				{
					if (!(windows[num] == null) && windows[num].id == windowId)
					{
						return windows[num];
					}
				}
				return null;
			}

			public bool IsFocused(int windowId)
			{
				if (windowId < 0)
				{
					return false;
				}
				if (topWindow == null)
				{
					return false;
				}
				return topWindow.id == windowId;
			}

			public void Focus(int windowId)
			{
				Focus(GetWindow(windowId));
			}

			public void Focus(Window window)
			{
				if (!(window == null))
				{
					window.TakeInputFocus();
					DefocusOtherWindows(window.id);
				}
			}

			private void DefocusOtherWindows(int focusedWindowId)
			{
				if (focusedWindowId < 0)
				{
					return;
				}
				for (int num = windows.Count - 1; num >= 0; num--)
				{
					if (!(windows[num] == null) && windows[num].id != focusedWindowId)
					{
						windows[num].Disable();
					}
				}
			}

			private void UpdateFader()
			{
				if (!isWindowOpen)
				{
					SetFaderActive(false);
					return;
				}
				Transform transform = topWindow.transform.parent;
				if (!(transform == null))
				{
					SetFaderActive(true);
					fader.transform.SetAsLastSibling();
					int siblingIndex = topWindow.transform.GetSiblingIndex();
					fader.transform.SetSiblingIndex(siblingIndex);
				}
			}

			private void FocusTopWindow()
			{
				if (!(topWindow == null))
				{
					topWindow.TakeInputFocus();
				}
			}

			private void SetFaderActive(bool state)
			{
				fader.SetActive(state);
			}

			private Window InstantiateWindow(string name, int width, int height)
			{
				if (string.IsNullOrEmpty(name))
				{
					name = "Window";
				}
				GameObject gameObject = UITools.InstantiateGUIObject<Window>(windowPrefab, parent, name);
				if (gameObject == null)
				{
					return null;
				}
				Window component = gameObject.GetComponent<Window>();
				if (component != null)
				{
					component.Initialize(GetNewId(), IsFocused);
					windows.Add(component);
					component.SetSize(width, height);
				}
				return component;
			}

			private Window InstantiateWindow(string name, GameObject windowPrefab)
			{
				if (string.IsNullOrEmpty(name))
				{
					name = "Window";
				}
				if (windowPrefab == null)
				{
					return null;
				}
				GameObject gameObject = UITools.InstantiateGUIObject<Window>(windowPrefab, parent, name);
				if (gameObject == null)
				{
					return null;
				}
				Window component = gameObject.GetComponent<Window>();
				if (component != null)
				{
					component.Initialize(GetNewId(), IsFocused);
					windows.Add(component);
				}
				return component;
			}

			private void DestroyWindow(Window window)
			{
				if (!(window == null))
				{
					UnityEngine.Object.Destroy(window.gameObject);
				}
			}

			private int GetNewId()
			{
				int result = idCounter;
				idCounter++;
				return result;
			}

			public void ClearCompletely()
			{
				CloseAll();
				if (fader != null)
				{
					UnityEngine.Object.Destroy(fader);
				}
			}
		}

		private const float blockInputOnFocusTimeout = 0.1f;

		private const string buttonIdentifier_playerSelection = "PlayerSelection";

		private const string buttonIdentifier_removeController = "RemoveController";

		private const string buttonIdentifier_assignController = "AssignController";

		private const string buttonIdentifier_calibrateController = "CalibrateController";

		private const string buttonIdentifier_editInputBehaviors = "EditInputBehaviors";

		private const string buttonIdentifier_mapCategorySelection = "MapCategorySelection";

		private const string buttonIdentifier_assignedControllerSelection = "AssignedControllerSelection";

		private const string buttonIdentifier_done = "Done";

		private const string buttonIdentifier_restoreDefaults = "RestoreDefaults";

		[SerializeField]
		[Tooltip("Must be assigned a Rewired Input Manager scene object or prefab.")]
		private InputManager _rewiredInputManager;

		[SerializeField]
		[Tooltip("Open the control mapping screen immediately on start. Mainly used for testing.")]
		private bool _openOnStart;

		[SerializeField]
		[Tooltip("The Layout of the Keyboard Maps to be displayed.")]
		private int _keyboardMapDefaultLayout;

		[SerializeField]
		[Tooltip("The Layout of the Mouse Maps to be displayed.")]
		private int _mouseMapDefaultLayout;

		[SerializeField]
		[Tooltip("The Layout of the Mouse Maps to be displayed.")]
		private int _joystickMapDefaultLayout;

		[SerializeField]
		private MappingSet[] _mappingSets = new MappingSet[1] { MappingSet.Default };

		[SerializeField]
		[Tooltip("Display a selectable list of Players. If your game only supports 1 player, you can disable this.")]
		private bool _showPlayers = true;

		[SerializeField]
		[Tooltip("Display the Controller column for input mapping.")]
		private bool _showControllers = true;

		[SerializeField]
		[Tooltip("Display the Keyboard column for input mapping.")]
		private bool _showKeyboard = true;

		[SerializeField]
		[Tooltip("Display the Mouse column for input mapping.")]
		private bool _showMouse = true;

		[SerializeField]
		[Tooltip("The maximum number of controllers allowed to be assigned to a Player. If set to any value other than 1, a selectable list of currently-assigned controller will be displayed to the user. [0 = infinite]")]
		private int _maxControllersPerPlayer = 1;

		[SerializeField]
		[Tooltip("Display section labels for each Action Category in the input field grid. Only applies if Action Categories are used to display the Action list.")]
		private bool _showActionCategoryLabels;

		[SerializeField]
		[Tooltip("The number of input fields to display for the keyboard. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		private int _keyboardInputFieldCount = 2;

		[SerializeField]
		[Tooltip("The number of input fields to display for the mouse. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		private int _mouseInputFieldCount = 1;

		[SerializeField]
		[Tooltip("The number of input fields to display for joysticks. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		private int _controllerInputFieldCount = 1;

		[SerializeField]
		[Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to make the conflicting assignment anyway.")]
		private bool _allowElementAssignmentConflicts;

		[SerializeField]
		[Tooltip("The width in relative pixels of the Action label column.")]
		private int _actionLabelWidth = 200;

		[SerializeField]
		[Tooltip("The width in relative pixels of the Keyboard column.")]
		private int _keyboardColMaxWidth = 360;

		[SerializeField]
		[Tooltip("The width in relative pixels of the Mouse column.")]
		private int _mouseColMaxWidth = 200;

		[SerializeField]
		[Tooltip("The width in relative pixels of the Controller column.")]
		private int _controllerColMaxWidth = 200;

		[SerializeField]
		[Tooltip("The height in relative pixels of the input grid button rows.")]
		private int _inputRowHeight = 40;

		[SerializeField]
		[Tooltip("The width in relative pixels of spacing between columns.")]
		private int _inputColumnSpacing = 40;

		[SerializeField]
		[Tooltip("The height in relative pixels of the space between Action Category sections. Only applicable if Show Action Category Labels is checked.")]
		private int _inputRowCategorySpacing = 20;

		[SerializeField]
		[Tooltip("The width in relative pixels of the invert toggle buttons.")]
		private int _invertToggleWidth = 40;

		[SerializeField]
		[Tooltip("The width in relative pixels of generated popup windows.")]
		private int _defaultWindowWidth = 500;

		[SerializeField]
		[Tooltip("The height in relative pixels of generated popup windows.")]
		private int _defaultWindowHeight = 400;

		[SerializeField]
		[Tooltip("The time in seconds the user has to press an element on a controller when assigning a controller to a Player. If this time elapses with no user input a controller, the assignment will be canceled.")]
		private float _controllerAssignmentTimeout = 5f;

		[SerializeField]
		[Tooltip("The time in seconds the user has to press an element on a controller while waiting for axes to be centered before assigning input.")]
		private float _preInputAssignmentTimeout = 5f;

		[SerializeField]
		[Tooltip("The time in seconds the user has to press an element on a controller when assigning input. If this time elapses with no user input on the target controller, the assignment will be canceled.")]
		private float _inputAssignmentTimeout = 5f;

		[SerializeField]
		[Tooltip("The time in seconds the user has to press an element on a controller during calibration.")]
		private float _axisCalibrationTimeout = 5f;

		[SerializeField]
		[Tooltip("If checked, mouse X-axis movement will always be ignored during input assignment. Check this if you don't want the horizontal mouse axis to be user-assignable to any Actions.")]
		private bool _ignoreMouseXAxisAssignment = true;

		[SerializeField]
		[Tooltip("If checked, mouse Y-axis movement will always be ignored during input assignment. Check this if you don't want the vertical mouse axis to be user-assignable to any Actions.")]
		private bool _ignoreMouseYAxisAssignment = true;

		[SerializeField]
		[Tooltip("An Action that when activated will alternately close or open the main screen as long as no popup windows are open.")]
		private int _screenToggleAction = -1;

		[SerializeField]
		[Tooltip("An Action that when activated will open the main screen if it is closed.")]
		private int _screenOpenAction = -1;

		[SerializeField]
		[Tooltip("An Action that when activated will close the main screen as long as no popup windows are open.")]
		private int _screenCloseAction = -1;

		[SerializeField]
		[Tooltip("An Action that when activated will cancel and close any open popup window. Use with care because the element assigned to this Action can never be mapped by the user (because it would just cancel his assignment).")]
		private int _universalCancelAction = -1;

		[SerializeField]
		[Tooltip("If enabled, Universal Cancel will also close the main screen if pressed when no windows are open.")]
		private bool _universalCancelClosesScreen = true;

		[SerializeField]
		[Tooltip("If checked, controls will be displayed which will allow the user to customize certain Input Behavior settings.")]
		private bool _showInputBehaviorSettings;

		[SerializeField]
		[Tooltip("Customizable settings for user-modifiable Input Behaviors. This can be used for settings like Mouse Look Sensitivity.")]
		private InputBehaviorSettings[] _inputBehaviorSettings;

		[SerializeField]
		[Tooltip("If enabled, UI elements will be themed based on the settings in Theme Settings.")]
		private bool _useThemeSettings = true;

		[SerializeField]
		[Tooltip("Must be assigned a ThemeSettings object. Used to theme UI elements.")]
		private ThemeSettings _themeSettings;

		[SerializeField]
		[Tooltip("Must be assigned a LanguageData object. Used to retrieve language entries for UI elements.")]
		private LanguageData _language;

		[SerializeField]
		[Tooltip("A list of prefabs. You should not have to modify this.")]
		private Prefabs prefabs;

		[SerializeField]
		[Tooltip("A list of references to elements in the hierarchy. You should not have to modify this.")]
		private References references;

		[SerializeField]
		[Tooltip("Show the label for the Players button group?")]
		private bool _showPlayersGroupLabel = true;

		[SerializeField]
		[Tooltip("Show the label for the Controller button group?")]
		private bool _showControllerGroupLabel = true;

		[SerializeField]
		[Tooltip("Show the label for the Assigned Controllers button group?")]
		private bool _showAssignedControllersGroupLabel = true;

		[SerializeField]
		[Tooltip("Show the label for the Settings button group?")]
		private bool _showSettingsGroupLabel = true;

		[SerializeField]
		[Tooltip("Show the label for the Map Categories button group?")]
		private bool _showMapCategoriesGroupLabel = true;

		[SerializeField]
		[Tooltip("Show the label for the current controller name?")]
		private bool _showControllerNameLabel = true;

		[SerializeField]
		[Tooltip("Show the Assigned Controllers group? If joystick auto-assignment is enabled in the Rewired Input Manager and the max joysticks per player is set to any value other than 1, the Assigned Controllers group will always be displayed.")]
		private bool _showAssignedControllers = true;

		private static ControlMapper Instance;

		private bool inputEventsInitialized;

		private bool initialized;

		private int playerCount;

		private InputGrid inputGrid;

		private WindowManager windowManager;

		private int currentPlayerId;

		private int currentMapCategoryId;

		private List<GUIButton> playerButtons;

		private List<GUIButton> mapCategoryButtons;

		private List<GUIButton> assignedControllerButtons;

		private GUIButton assignedControllerButtonsPlaceholder;

		private GameObject canvas;

		private GameObject lastUISelection;

		private int currentJoystickId = -1;

		private float blockInputOnFocusEndTime;

		private InputMapping pendingInputMapping;

		private AxisCalibrator pendingAxisCalibration;

		private Action<InputFieldInfo> inputFieldActivatedDelegate;

		private Action<ToggleInfo, bool> inputFieldInvertToggleStateChangedDelegate;

		public InputManager rewiredInputManager
		{
			get
			{
				return _rewiredInputManager;
			}
			set
			{
				_rewiredInputManager = value;
				InspectorPropertyChanged();
			}
		}

		public int keyboardMapDefaultLayout
		{
			get
			{
				return _keyboardMapDefaultLayout;
			}
			set
			{
				_keyboardMapDefaultLayout = value;
				InspectorPropertyChanged();
			}
		}

		public int mouseMapDefaultLayout
		{
			get
			{
				return _mouseMapDefaultLayout;
			}
			set
			{
				_mouseMapDefaultLayout = value;
				InspectorPropertyChanged();
			}
		}

		public int joystickMapDefaultLayout
		{
			get
			{
				return _joystickMapDefaultLayout;
			}
			set
			{
				_joystickMapDefaultLayout = value;
				InspectorPropertyChanged();
			}
		}

		public bool showPlayers
		{
			get
			{
				return _showPlayers && ReInput.players.playerCount > 1;
			}
			set
			{
				_showPlayers = value;
				InspectorPropertyChanged();
			}
		}

		public bool showControllers
		{
			get
			{
				return _showControllers;
			}
			set
			{
				_showControllers = value;
				InspectorPropertyChanged();
			}
		}

		public bool showKeyboard
		{
			get
			{
				return _showKeyboard;
			}
			set
			{
				_showKeyboard = value;
				InspectorPropertyChanged();
			}
		}

		public bool showMouse
		{
			get
			{
				return _showMouse;
			}
			set
			{
				_showMouse = value;
				InspectorPropertyChanged();
			}
		}

		public int maxControllersPerPlayer
		{
			get
			{
				return _maxControllersPerPlayer;
			}
			set
			{
				_maxControllersPerPlayer = value;
				InspectorPropertyChanged();
			}
		}

		public bool showActionCategoryLabels
		{
			get
			{
				return _showActionCategoryLabels;
			}
			set
			{
				_showActionCategoryLabels = value;
				InspectorPropertyChanged();
			}
		}

		public int keyboardInputFieldCount
		{
			get
			{
				return _keyboardInputFieldCount;
			}
			set
			{
				_keyboardInputFieldCount = value;
				InspectorPropertyChanged();
			}
		}

		public int mouseInputFieldCount
		{
			get
			{
				return _mouseInputFieldCount;
			}
			set
			{
				_mouseInputFieldCount = value;
				InspectorPropertyChanged();
			}
		}

		public int controllerInputFieldCount
		{
			get
			{
				return _controllerInputFieldCount;
			}
			set
			{
				_controllerInputFieldCount = value;
				InspectorPropertyChanged();
			}
		}

		public bool allowElementAssignmentConflicts
		{
			get
			{
				return _allowElementAssignmentConflicts;
			}
			set
			{
				_allowElementAssignmentConflicts = value;
				InspectorPropertyChanged();
			}
		}

		public int actionLabelWidth
		{
			get
			{
				return _actionLabelWidth;
			}
			set
			{
				_actionLabelWidth = value;
				InspectorPropertyChanged();
			}
		}

		public int keyboardColMaxWidth
		{
			get
			{
				return _keyboardColMaxWidth;
			}
			set
			{
				_keyboardColMaxWidth = value;
				InspectorPropertyChanged();
			}
		}

		public int mouseColMaxWidth
		{
			get
			{
				return _mouseColMaxWidth;
			}
			set
			{
				_mouseColMaxWidth = value;
				InspectorPropertyChanged();
			}
		}

		public int controllerColMaxWidth
		{
			get
			{
				return _controllerColMaxWidth;
			}
			set
			{
				_controllerColMaxWidth = value;
				InspectorPropertyChanged();
			}
		}

		public int inputRowHeight
		{
			get
			{
				return _inputRowHeight;
			}
			set
			{
				_inputRowHeight = value;
				InspectorPropertyChanged();
			}
		}

		public int inputColumnSpacing
		{
			get
			{
				return _inputColumnSpacing;
			}
			set
			{
				_inputColumnSpacing = value;
				InspectorPropertyChanged();
			}
		}

		public int inputRowCategorySpacing
		{
			get
			{
				return _inputRowCategorySpacing;
			}
			set
			{
				_inputRowCategorySpacing = value;
				InspectorPropertyChanged();
			}
		}

		public int invertToggleWidth
		{
			get
			{
				return _invertToggleWidth;
			}
			set
			{
				_invertToggleWidth = value;
				InspectorPropertyChanged();
			}
		}

		public int defaultWindowWidth
		{
			get
			{
				return _defaultWindowWidth;
			}
			set
			{
				_defaultWindowWidth = value;
				InspectorPropertyChanged();
			}
		}

		public int defaultWindowHeight
		{
			get
			{
				return _defaultWindowHeight;
			}
			set
			{
				_defaultWindowHeight = value;
				InspectorPropertyChanged();
			}
		}

		public float controllerAssignmentTimeout
		{
			get
			{
				return _controllerAssignmentTimeout;
			}
			set
			{
				_controllerAssignmentTimeout = value;
				InspectorPropertyChanged();
			}
		}

		public float preInputAssignmentTimeout
		{
			get
			{
				return _preInputAssignmentTimeout;
			}
			set
			{
				_preInputAssignmentTimeout = value;
				InspectorPropertyChanged();
			}
		}

		public float inputAssignmentTimeout
		{
			get
			{
				return _inputAssignmentTimeout;
			}
			set
			{
				_inputAssignmentTimeout = value;
				InspectorPropertyChanged();
			}
		}

		public float axisCalibrationTimeout
		{
			get
			{
				return _axisCalibrationTimeout;
			}
			set
			{
				_axisCalibrationTimeout = value;
				InspectorPropertyChanged();
			}
		}

		public bool ignoreMouseXAxisAssignment
		{
			get
			{
				return _ignoreMouseXAxisAssignment;
			}
			set
			{
				_ignoreMouseXAxisAssignment = value;
				InspectorPropertyChanged();
			}
		}

		public bool ignoreMouseYAxisAssignment
		{
			get
			{
				return _ignoreMouseYAxisAssignment;
			}
			set
			{
				_ignoreMouseYAxisAssignment = value;
				InspectorPropertyChanged();
			}
		}

		public bool universalCancelClosesScreen
		{
			get
			{
				return _universalCancelClosesScreen;
			}
			set
			{
				_universalCancelClosesScreen = value;
				InspectorPropertyChanged();
			}
		}

		public bool showInputBehaviorSettings
		{
			get
			{
				return _showInputBehaviorSettings;
			}
			set
			{
				_showInputBehaviorSettings = value;
				InspectorPropertyChanged();
			}
		}

		public bool useThemeSettings
		{
			get
			{
				return _useThemeSettings;
			}
			set
			{
				_useThemeSettings = value;
				InspectorPropertyChanged();
			}
		}

		public LanguageData language
		{
			get
			{
				return _language;
			}
			set
			{
				_language = value;
				if (_language != null)
				{
					_language.Initialize();
				}
				InspectorPropertyChanged();
			}
		}

		public bool showPlayersGroupLabel
		{
			get
			{
				return _showPlayersGroupLabel;
			}
			set
			{
				_showPlayersGroupLabel = value;
				InspectorPropertyChanged();
			}
		}

		public bool showControllerGroupLabel
		{
			get
			{
				return _showControllerGroupLabel;
			}
			set
			{
				_showControllerGroupLabel = value;
				InspectorPropertyChanged();
			}
		}

		public bool showAssignedControllersGroupLabel
		{
			get
			{
				return _showAssignedControllersGroupLabel;
			}
			set
			{
				_showAssignedControllersGroupLabel = value;
				InspectorPropertyChanged();
			}
		}

		public bool showSettingsGroupLabel
		{
			get
			{
				return _showSettingsGroupLabel;
			}
			set
			{
				_showSettingsGroupLabel = value;
				InspectorPropertyChanged();
			}
		}

		public bool showMapCategoriesGroupLabel
		{
			get
			{
				return _showMapCategoriesGroupLabel;
			}
			set
			{
				_showMapCategoriesGroupLabel = value;
				InspectorPropertyChanged();
			}
		}

		public bool showControllerNameLabel
		{
			get
			{
				return _showControllerNameLabel;
			}
			set
			{
				_showControllerNameLabel = value;
				InspectorPropertyChanged();
			}
		}

		public bool showAssignedControllers
		{
			get
			{
				return _showAssignedControllers;
			}
			set
			{
				_showAssignedControllers = value;
				InspectorPropertyChanged();
			}
		}

		public bool isOpen
		{
			get
			{
				if (!initialized)
				{
					return references.canvas != null && references.canvas.gameObject.activeInHierarchy;
				}
				return canvas.activeInHierarchy;
			}
		}

		private bool isFocused
		{
			get
			{
				if (!initialized)
				{
					return false;
				}
				return !windowManager.isWindowOpen;
			}
		}

		private bool inputAllowed
		{
			get
			{
				if (blockInputOnFocusEndTime > Time.unscaledTime)
				{
					return false;
				}
				if (InterruptingPrompt.IsInterrupting())
				{
					return false;
				}
				return true;
			}
		}

		private int inputGridColumnCount
		{
			get
			{
				int num = 1;
				if (_showKeyboard)
				{
					num++;
				}
				if (_showMouse)
				{
					num++;
				}
				if (_showControllers)
				{
					num++;
				}
				return num;
			}
		}

		private int inputGridWidth
		{
			get
			{
				return _actionLabelWidth + (_showKeyboard ? _keyboardColMaxWidth : 0) + (_showMouse ? _mouseColMaxWidth : 0) + (_showControllers ? _controllerColMaxWidth : 0) + (inputGridColumnCount - 1) * _inputColumnSpacing;
			}
		}

		private Player currentPlayer
		{
			get
			{
				return ReInput.players.GetPlayer(currentPlayerId);
			}
		}

		private InputCategory currentMapCategory
		{
			get
			{
				return ReInput.mapping.GetMapCategory(currentMapCategoryId);
			}
		}

		private MappingSet currentMappingSet
		{
			get
			{
				if (currentMapCategoryId < 0)
				{
					return null;
				}
				for (int i = 0; i < _mappingSets.Length; i++)
				{
					if (_mappingSets[i].mapCategoryId == currentMapCategoryId)
					{
						return _mappingSets[i];
					}
				}
				return null;
			}
		}

		private Joystick currentJoystick
		{
			get
			{
				return ReInput.controllers.GetJoystick(currentJoystickId);
			}
		}

		private bool isJoystickSelected
		{
			get
			{
				return currentJoystickId >= 0;
			}
		}

		private GameObject currentUISelection
		{
			get
			{
				return (!(EventSystem.current != null)) ? null : EventSystem.current.currentSelectedGameObject;
			}
		}

		private bool showSettings
		{
			get
			{
				return _showInputBehaviorSettings && _inputBehaviorSettings.Length > 0;
			}
		}

		private bool showMapCategories
		{
			get
			{
				if (_mappingSets == null)
				{
					return false;
				}
				if (_mappingSets.Length <= 1)
				{
					return false;
				}
				return true;
			}
		}

		public event Action ScreenClosedEvent;

		public event Action ScreenOpenedEvent;

		private void Awake()
		{
			PreInitialize();
			if (isOpen)
			{
				Initialize();
				Open(true);
			}
		}

		private void Start()
		{
			if (_openOnStart)
			{
				Open(false);
			}
		}

		private void Update()
		{
			if (isOpen && initialized)
			{
				CheckUISelection();
			}
		}

		private void OnDestroy()
		{
			ReInput.ControllerConnectedEvent -= OnJoystickConnected;
			ReInput.ControllerDisconnectedEvent -= OnJoystickDisconnected;
			ReInput.ControllerPreDisconnectEvent -= OnJoystickPreDisconnect;
			UnsubscribeMenuControlInputEvents();
		}

		private void PreInitialize()
		{
			if (!ReInput.isReady)
			{
				Debug.LogError("Rewired Control Mapper: Rewired has not been initialized! Are you missing a Rewired Input Manager in your scene?");
			}
			else
			{
				SubscribeMenuControlInputEvents();
			}
		}

		private void Initialize()
		{
			if (initialized || !ReInput.isReady)
			{
				return;
			}
			if (_rewiredInputManager == null)
			{
				_rewiredInputManager = UnityEngine.Object.FindObjectOfType<InputManager>();
				if (_rewiredInputManager == null)
				{
					Debug.LogError("Rewired Control Mapper: A Rewired Input Manager was not assigned in the inspector or found in the current scene! Control Mapper will not function.");
					return;
				}
			}
			if (Instance != null)
			{
				Debug.LogError("Rewired Control Mapper: Only one ControlMapper can exist at one time!");
				return;
			}
			Instance = this;
			if (prefabs == null || !prefabs.Check())
			{
				Debug.LogError("Rewired Control Mapper: All prefabs must be assigned in the inspector!");
				return;
			}
			if (references == null || !references.Check())
			{
				Debug.LogError("Rewired Control Mapper: All references must be assigned in the inspector!");
				return;
			}
			references.inputGridLayoutElement = references.inputGridContainer.GetComponent<LayoutElement>();
			if (references.inputGridLayoutElement == null)
			{
				Debug.LogError("Rewired Control Mapper: InputGridContainer is missing LayoutElement component!");
				return;
			}
			if (_showKeyboard && _keyboardInputFieldCount < 1)
			{
				Debug.LogWarning("Rewired Control Mapper: Keyboard Input Fields must be at least 1!");
				_keyboardInputFieldCount = 1;
			}
			if (_showMouse && _mouseInputFieldCount < 1)
			{
				Debug.LogWarning("Rewired Control Mapper: Mouse Input Fields must be at least 1!");
				_mouseInputFieldCount = 1;
			}
			if (_showControllers && _controllerInputFieldCount < 1)
			{
				Debug.LogWarning("Rewired Control Mapper: Controller Input Fields must be at least 1!");
				_controllerInputFieldCount = 1;
			}
			if (_maxControllersPerPlayer < 0)
			{
				Debug.LogWarning("Rewired Control Mapper: Max Controllers Per Player must be at least 0 (no limit)!");
				_maxControllersPerPlayer = 0;
			}
			if (_useThemeSettings && _themeSettings == null)
			{
				Debug.LogWarning("Rewired Control Mapper: To use theming, Theme Settings must be set in the inspector! Theming has been disabled.");
				_useThemeSettings = false;
			}
			if (_language == null)
			{
				Debug.LogError("Rawired UI: Language must be set in the inspector!");
				return;
			}
			_language.Initialize();
			inputFieldActivatedDelegate = OnInputFieldActivated;
			inputFieldInvertToggleStateChangedDelegate = OnInputFieldInvertToggleStateChanged;
			ReInput.ControllerConnectedEvent += OnJoystickConnected;
			ReInput.ControllerDisconnectedEvent += OnJoystickDisconnected;
			ReInput.ControllerPreDisconnectEvent += OnJoystickPreDisconnect;
			playerCount = ReInput.players.playerCount;
			canvas = references.canvas.gameObject;
			windowManager = new WindowManager(prefabs.window, prefabs.fader, references.canvas.transform);
			playerButtons = new List<GUIButton>();
			mapCategoryButtons = new List<GUIButton>();
			assignedControllerButtons = new List<GUIButton>();
			currentMapCategoryId = _mappingSets[0].mapCategoryId;
			Draw();
			CreateInputGrid();
			CreateLayout();
			SubscribeFixedUISelectionEvents();
			initialized = true;
		}

		private void OnJoystickConnected(ControllerStatusChangedEventArgs args)
		{
			if (initialized && _showControllers)
			{
				ClearVarsOnJoystickChange();
				ForceRefresh();
			}
		}

		private void OnJoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (initialized && _showControllers)
			{
				ClearVarsOnJoystickChange();
				ForceRefresh();
			}
		}

		private void OnJoystickPreDisconnect(ControllerStatusChangedEventArgs args)
		{
			if (initialized && _showControllers)
			{
			}
		}

		public void OnButtonActivated(ButtonInfo buttonInfo)
		{
			if (initialized && inputAllowed)
			{
				switch (buttonInfo.identifier)
				{
				case "PlayerSelection":
					OnPlayerSelected(buttonInfo.intData, true);
					break;
				case "AssignedControllerSelection":
					OnControllerSelected(buttonInfo.intData);
					break;
				case "RemoveController":
					OnRemoveCurrentController();
					break;
				case "AssignController":
					ShowAssignControllerWindow();
					break;
				case "CalibrateController":
					ShowCalibrateControllerWindow();
					break;
				case "EditInputBehaviors":
					ShowEditInputBehaviorsWindow();
					break;
				case "MapCategorySelection":
					OnMapCategorySelected(buttonInfo.intData, true);
					break;
				case "Done":
					Close(true);
					break;
				case "RestoreDefaults":
					OnRestoreDefaults();
					break;
				}
			}
		}

		public void OnInputFieldActivated(InputFieldInfo fieldInfo)
		{
			if (!initialized || !inputAllowed || currentPlayer == null)
			{
				return;
			}
			InputAction action = ReInput.mapping.GetAction(fieldInfo.actionId);
			if (action == null)
			{
				return;
			}
			string text;
			if (action.type == InputActionType.Button)
			{
				text = action.descriptiveName;
			}
			else
			{
				if (action.type != InputActionType.Axis)
				{
					throw new NotImplementedException();
				}
				if (fieldInfo.axisRange == AxisRange.Full)
				{
					text = action.descriptiveName;
				}
				else if (fieldInfo.axisRange == AxisRange.Positive)
				{
					text = ((!string.IsNullOrEmpty(action.positiveDescriptiveName)) ? action.positiveDescriptiveName : (action.descriptiveName + " +"));
				}
				else
				{
					if (fieldInfo.axisRange != AxisRange.Negative)
					{
						throw new NotImplementedException();
					}
					text = ((!string.IsNullOrEmpty(action.negativeDescriptiveName)) ? action.negativeDescriptiveName : (action.descriptiveName + " -"));
				}
			}
			string text2 = Localization.Translate(text).text;
			text = ((text2 != null) ? text2 : text);
			ControllerMap controllerMap = GetControllerMap(fieldInfo.controllerType);
			if (controllerMap != null)
			{
				ActionElementMap actionElementMap = ((fieldInfo.actionElementMapId < 0) ? null : controllerMap.GetElementMap(fieldInfo.actionElementMapId));
				if (actionElementMap != null)
				{
					ShowBeginElementAssignmentReplacementWindow(fieldInfo, action, controllerMap, actionElementMap, text);
				}
				else
				{
					ShowCreateNewElementAssignmentWindow(fieldInfo, action, controllerMap, text);
				}
			}
		}

		public void OnInputFieldInvertToggleStateChanged(ToggleInfo toggleInfo, bool newState)
		{
			if (initialized && inputAllowed)
			{
				SetActionAxisInverted(newState, toggleInfo.controllerType, toggleInfo.actionElementMapId);
			}
		}

		private void OnPlayerSelected(int playerId, bool redraw)
		{
			if (initialized)
			{
				currentPlayerId = playerId;
				ClearVarsOnPlayerChange();
				if (redraw)
				{
					Redraw(true, true);
				}
			}
		}

		private void OnControllerSelected(int joystickId)
		{
			if (initialized)
			{
				currentJoystickId = joystickId;
				Redraw(true, true);
			}
		}

		private void OnRemoveCurrentController()
		{
			if (currentPlayer != null && currentJoystickId >= 0)
			{
				RemoveController(currentPlayer, currentJoystickId);
				ClearVarsOnJoystickChange();
				Redraw(false, false);
			}
		}

		private void OnMapCategorySelected(int id, bool redraw)
		{
			if (initialized)
			{
				currentMapCategoryId = id;
				if (redraw)
				{
					Redraw(true, true);
				}
			}
		}

		private void OnRestoreDefaults()
		{
			if (initialized)
			{
				ShowRestoreDefaultsWindow();
			}
		}

		private void OnScreenToggleActionPressed(InputActionEventData data)
		{
			if (!isOpen)
			{
				Open();
			}
			else if (initialized && isFocused)
			{
				Close(true);
			}
		}

		private void OnScreenOpenActionPressed(InputActionEventData data)
		{
			Open();
		}

		private void OnScreenCloseActionPressed(InputActionEventData data)
		{
			if (initialized && isOpen && isFocused)
			{
				Close(true);
			}
		}

		private void OnUniversalCancelActionPressed(InputActionEventData data)
		{
			if (!initialized || !isOpen)
			{
				return;
			}
			if (_universalCancelClosesScreen)
			{
				if (isFocused)
				{
					Close(true);
				}
			}
			else if (!isFocused)
			{
			}
		}

		private void OnWindowCancel(int windowId)
		{
			if (initialized && windowId >= 0)
			{
				CloseWindow(windowId);
			}
		}

		private void OnRemoveElementAssignment(int windowId, ControllerMap map, ActionElementMap aem)
		{
			if (map != null && aem != null)
			{
				map.DeleteElementMap(aem.id);
				CloseWindow(windowId);
			}
		}

		private void OnBeginElementAssignment(InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, string actionName)
		{
			if (!(fieldInfo == null) && map != null)
			{
				pendingInputMapping = new InputMapping(actionName, fieldInfo, map, aem, fieldInfo.controllerType, fieldInfo.controllerId);
				switch (fieldInfo.controllerType)
				{
				case ControllerType.Joystick:
					ShowElementAssignmentPrePollingWindow();
					break;
				case ControllerType.Keyboard:
					ShowElementAssignmentPollingWindow();
					break;
				case ControllerType.Mouse:
					ShowElementAssignmentPollingWindow();
					break;
				default:
					throw new NotImplementedException();
				}
			}
		}

		private void OnControllerAssignmentConfirmed(int windowId, Player player, int controllerId)
		{
			if (windowId >= 0 && player != null && controllerId >= 0)
			{
				AssignController(player, controllerId);
				CloseWindow(windowId);
			}
		}

		private void OnMouseAssignmentConfirmed(int windowId, Player player)
		{
			if (windowId < 0 || player == null)
			{
				return;
			}
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != player)
				{
					players[i].controllers.hasMouse = false;
				}
			}
			player.controllers.hasMouse = true;
			CloseWindow(windowId);
		}

		private void OnElementAssignmentConflictReplaceConfirmed(int windowId, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (currentPlayer == null || mapping == null)
			{
				return;
			}
			ElementAssignmentConflictCheck conflictCheck;
			if (!CreateConflictCheck(mapping, assignment, out conflictCheck))
			{
				Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
				CloseWindow(windowId);
				return;
			}
			if (skipOtherPlayers)
			{
				ReInput.players.SystemPlayer.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
				currentPlayer.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
			}
			else
			{
				ReInput.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
			}
			mapping.map.ReplaceOrCreateElementMap(assignment);
			CloseWindow(windowId);
		}

		private void OnElementAssignmentAddConfirmed(int windowId, InputMapping mapping, ElementAssignment assignment)
		{
			if (currentPlayer != null && mapping != null)
			{
				mapping.map.ReplaceOrCreateElementMap(assignment);
				CloseWindow(windowId);
			}
		}

		private void OnRestoreDefaultsConfirmed(int windowId)
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (_showControllers)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
				}
				if (_showKeyboard)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
				}
				if (_showMouse)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
				}
			}
			CloseWindow(windowId);
		}

		private void OnAssignControllerWindowUpdate(int windowId)
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (window.timer.finished)
			{
				CloseWindow(windowId);
				return;
			}
			ControllerPollingInfo controllerPollingInfo = ReInput.controllers.polling.PollAllControllersOfTypeForFirstElementDown(ControllerType.Joystick);
			if (controllerPollingInfo.success)
			{
				if (!ReInput.controllers.IsControllerAssigned(ControllerType.Joystick, controllerPollingInfo.controllerId) || currentPlayer.controllers.ContainsController(ControllerType.Joystick, controllerPollingInfo.controllerId))
				{
					OnControllerAssignmentConfirmed(windowId, currentPlayer, controllerPollingInfo.controllerId);
				}
			}
			else
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			}
		}

		private void OnElementAssignmentPrePollingWindowUpdate(int windowId)
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = windowManager.GetWindow(windowId);
			if (windowId < 0 || pendingInputMapping == null)
			{
				return;
			}
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				ControllerPollingInfo controllerPollingInfo;
				switch (pendingInputMapping.controllerType)
				{
				case ControllerType.Joystick:
					if (currentPlayer.controllers.joystickCount == 0)
					{
						return;
					}
					controllerPollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(pendingInputMapping.controllerType, currentJoystick.id);
					break;
				case ControllerType.Keyboard:
				case ControllerType.Mouse:
					controllerPollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(pendingInputMapping.controllerType, 0);
					break;
				default:
					throw new NotImplementedException();
				}
				if (!controllerPollingInfo.success)
				{
					return;
				}
			}
			ShowElementAssignmentPollingWindow();
		}

		private void OnJoystickElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = windowManager.GetWindow(windowId);
			if (windowId < 0 || pendingInputMapping == null)
			{
				return;
			}
			if (window.timer.finished)
			{
				CloseWindow(windowId);
				return;
			}
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			if (currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
			if (pollingInfo.success)
			{
				ElementAssignment elementAssignment = pendingInputMapping.ToElementAssignment(pollingInfo);
				if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, elementAssignment, false))
				{
					pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
					CloseWindow(windowId);
				}
				else
				{
					ShowElementAssignmentConflictWindow(elementAssignment, false);
				}
			}
		}

		private void OnKeyboardElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = windowManager.GetWindow(windowId);
			if (windowId < 0 || pendingInputMapping == null)
			{
				return;
			}
			if (window.timer.finished)
			{
				CloseWindow(windowId);
				return;
			}
			ControllerPollingInfo pollingInfo;
			bool modifierKeyPressed;
			ModifierKeyFlags modifierFlags;
			string label;
			PollKeyboardForAssignment(out pollingInfo, out modifierKeyPressed, out modifierFlags, out label);
			if (modifierKeyPressed)
			{
				window.timer.Start(_inputAssignmentTimeout);
			}
			window.SetContentText((!modifierKeyPressed) ? Mathf.CeilToInt(window.timer.remaining).ToString() : string.Empty, 2);
			window.SetContentText(label, 1);
			if (pollingInfo.success)
			{
				ElementAssignment elementAssignment = pendingInputMapping.ToElementAssignment(pollingInfo, modifierFlags);
				if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, elementAssignment, false))
				{
					pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
					CloseWindow(windowId);
				}
				else
				{
					ShowElementAssignmentConflictWindow(elementAssignment, false);
				}
			}
		}

		private void OnMouseElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = windowManager.GetWindow(windowId);
			if (windowId < 0 || pendingInputMapping == null)
			{
				return;
			}
			if (window.timer.finished)
			{
				CloseWindow(windowId);
				return;
			}
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			ControllerPollingInfo pollingInfo;
			if (_ignoreMouseXAxisAssignment || _ignoreMouseYAxisAssignment)
			{
				pollingInfo = default(ControllerPollingInfo);
				foreach (ControllerPollingInfo item in ReInput.controllers.polling.PollControllerForAllElementsDown(ControllerType.Mouse, 0))
				{
					if (item.elementType == ControllerElementType.Axis && ((_ignoreMouseXAxisAssignment && item.elementIndex == 0) || (_ignoreMouseYAxisAssignment && item.elementIndex == 1)))
					{
						continue;
					}
					pollingInfo = item;
					break;
				}
			}
			else
			{
				pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Mouse, 0);
			}
			if (pollingInfo.success)
			{
				ElementAssignment elementAssignment = pendingInputMapping.ToElementAssignment(pollingInfo);
				if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, elementAssignment, true))
				{
					pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
					CloseWindow(windowId);
				}
				else
				{
					ShowElementAssignmentConflictWindow(elementAssignment, true);
				}
			}
		}

		private void OnCalibrateAxisStep1WindowUpdate(int windowId)
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = windowManager.GetWindow(windowId);
			if (windowId < 0 || pendingAxisCalibration == null || !pendingAxisCalibration.isValid)
			{
				return;
			}
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				if (currentPlayer.controllers.joystickCount == 0 || !pendingAxisCalibration.joystick.PollForFirstButtonDown().success)
				{
					return;
				}
			}
			pendingAxisCalibration.RecordZero();
			CloseWindow(windowId);
			ShowCalibrateAxisStep2Window();
		}

		private void OnCalibrateAxisStep2WindowUpdate(int windowId)
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = windowManager.GetWindow(windowId);
			if (windowId < 0 || pendingAxisCalibration == null || !pendingAxisCalibration.isValid)
			{
				return;
			}
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				pendingAxisCalibration.RecordMinMax();
				if (currentPlayer.controllers.joystickCount == 0 || !pendingAxisCalibration.joystick.PollForFirstButtonDown().success)
				{
					return;
				}
			}
			EndAxisCalibration();
			CloseWindow(windowId);
		}

		private void ShowAssignControllerWindow()
		{
			if (currentPlayer != null && ReInput.controllers.joystickCount != 0)
			{
				Window window = OpenWindow(true);
				if (!(window == null))
				{
					window.SetUpdateCallback(OnAssignControllerWindowUpdate);
					window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.assignControllerWindowTitle);
					window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.assignControllerWindowMessage);
					window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
					window.timer.Start(_controllerAssignmentTimeout);
					windowManager.Focus(window);
				}
			}
		}

		private void ShowControllerAssignmentConflictWindow(int controllerId)
		{
			if (currentPlayer == null || ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			Window window = OpenWindow(true);
			if (window == null)
			{
				return;
			}
			string otherPlayerName = string.Empty;
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != currentPlayer && players[i].controllers.ContainsController(ControllerType.Joystick, controllerId))
				{
					otherPlayerName = players[i].descriptiveName;
					break;
				}
			}
			Joystick joystick = ReInput.controllers.GetJoystick(controllerId);
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.controllerAssignmentConflictWindowTitle);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetControllerAssignmentConflictWindowMessage(joystick.name, otherPlayerName, currentPlayer.descriptiveName));
			UnityAction unityAction = delegate
			{
				OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.yes, delegate
			{
				OnControllerAssignmentConfirmed(window.id, currentPlayer, controllerId);
			}, unityAction, true);
			window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.no, unityAction, unityAction, false);
			windowManager.Focus(window);
		}

		private void ShowBeginElementAssignmentReplacementWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, ActionElementMap aem, string actionName)
		{
			GUIInputField gUIInputField = inputGrid.GetGUIInputField(currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
			if (gUIInputField == null)
			{
				return;
			}
			Window window = OpenWindow(true);
			if (!(window == null))
			{
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, actionName);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), gUIInputField.GetLabel());
				UnityAction unityAction = delegate
				{
					OnWindowCancel(window.id);
				};
				window.cancelCallback = unityAction;
				window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.replace, delegate
				{
					OnBeginElementAssignment(fieldInfo, map, aem, actionName);
				}, unityAction, true);
				window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.remove, delegate
				{
					OnRemoveElementAssignment(window.id, map, aem);
				}, unityAction, false);
				window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.cancel, unityAction, unityAction, false);
				windowManager.Focus(window);
			}
		}

		private void ShowCreateNewElementAssignmentWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, string actionName)
		{
			GUIInputField gUIInputField = inputGrid.GetGUIInputField(currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
			if (gUIInputField != null)
			{
				OnBeginElementAssignment(fieldInfo, map, null, actionName);
			}
		}

		private void ShowElementAssignmentPrePollingWindow()
		{
			if (pendingInputMapping == null)
			{
				return;
			}
			Window window = OpenWindow(true);
			if (!(window == null))
			{
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.elementAssignmentPrePollingWindowMessage);
				if (prefabs.centerStickGraphic != null)
				{
					window.AddContentImage(prefabs.centerStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
				}
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
				window.SetUpdateCallback(OnElementAssignmentPrePollingWindowUpdate);
				window.timer.Start(_preInputAssignmentTimeout);
				windowManager.Focus(window);
			}
		}

		private void ShowElementAssignmentPollingWindow()
		{
			if (pendingInputMapping == null)
			{
				return;
			}
			switch (pendingInputMapping.controllerType)
			{
			case ControllerType.Joystick:
				ShowJoystickElementAssignmentPollingWindow();
				break;
			case ControllerType.Keyboard:
				ShowKeyboardElementAssignmentPollingWindow();
				break;
			case ControllerType.Mouse:
				if (currentPlayer.controllers.hasMouse)
				{
					ShowMouseElementAssignmentPollingWindow();
				}
				else
				{
					ShowMouseAssignmentConflictWindow();
				}
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private void ShowJoystickElementAssignmentPollingWindow()
		{
			if (pendingInputMapping != null)
			{
				Window window = OpenWindow(true);
				if (!(window == null))
				{
					window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
					window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetJoystickElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
					window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
					window.SetUpdateCallback(OnJoystickElementAssignmentPollingWindowUpdate);
					window.timer.Start(_inputAssignmentTimeout);
					windowManager.Focus(window);
				}
			}
		}

		private void ShowKeyboardElementAssignmentPollingWindow()
		{
			if (pendingInputMapping != null)
			{
				Window window = OpenWindow(true);
				if (!(window == null))
				{
					window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
					window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetKeyboardElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
					window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, 0f - (window.GetContentTextHeight(0) + 50f)), string.Empty);
					window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
					window.SetUpdateCallback(OnKeyboardElementAssignmentPollingWindowUpdate);
					window.timer.Start(_inputAssignmentTimeout);
					windowManager.Focus(window);
				}
			}
		}

		private void ShowMouseElementAssignmentPollingWindow()
		{
			if (pendingInputMapping != null)
			{
				Window window = OpenWindow(true);
				if (!(window == null))
				{
					window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
					window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetMouseElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
					window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
					window.SetUpdateCallback(OnMouseElementAssignmentPollingWindowUpdate);
					window.timer.Start(_inputAssignmentTimeout);
					windowManager.Focus(window);
				}
			}
		}

		private void ShowElementAssignmentConflictWindow(ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (pendingInputMapping == null)
			{
				return;
			}
			bool flag = IsBlockingAssignmentConflict(pendingInputMapping, assignment, skipOtherPlayers);
			string text = ((!flag) ? _language.GetElementAlreadyInUseCanReplace(pendingInputMapping.elementName, _allowElementAssignmentConflicts) : _language.GetElementAlreadyInUseBlocked(pendingInputMapping.elementName));
			Window window = OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.elementAssignmentConflictWindowMessage);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), text);
			UnityAction unityAction = delegate
			{
				OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			if (flag)
			{
				window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.okay, unityAction, unityAction, true);
			}
			else
			{
				window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.replace, delegate
				{
					OnElementAssignmentConflictReplaceConfirmed(window.id, pendingInputMapping, assignment, skipOtherPlayers);
				}, unityAction, true);
				if (_allowElementAssignmentConflicts)
				{
					window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.add, delegate
					{
						OnElementAssignmentAddConfirmed(window.id, pendingInputMapping, assignment);
					}, unityAction, false);
				}
				window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.cancel, unityAction, unityAction, false);
			}
			windowManager.Focus(window);
		}

		private void ShowMouseAssignmentConflictWindow()
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = OpenWindow(true);
			if (window == null)
			{
				return;
			}
			string otherPlayerName = string.Empty;
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != currentPlayer && players[i].controllers.hasMouse)
				{
					otherPlayerName = players[i].descriptiveName;
					break;
				}
			}
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.mouseAssignmentConflictWindowTitle);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetMouseAssignmentConflictWindowMessage(otherPlayerName, currentPlayer.descriptiveName));
			UnityAction unityAction = delegate
			{
				OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.yes, delegate
			{
				OnMouseAssignmentConfirmed(window.id, currentPlayer);
			}, unityAction, true);
			window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.no, unityAction, unityAction, false);
			windowManager.Focus(window);
		}

		private void ShowCalibrateControllerWindow()
		{
			if (currentPlayer != null && currentPlayer.controllers.joystickCount != 0)
			{
				CalibrationWindow calibrationWindow = OpenWindow(prefabs.calibrationWindow, "CalibrationWindow", true) as CalibrationWindow;
				if (!(calibrationWindow == null))
				{
					Joystick joystick = currentJoystick;
					calibrationWindow.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateControllerWindowTitle);
					calibrationWindow.SetJoystick(currentPlayer.id, joystick);
					calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Done, CloseWindow);
					calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Calibrate, StartAxisCalibration);
					calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Cancel, CloseWindow);
					windowManager.Focus(calibrationWindow);
				}
			}
		}

		private void ShowCalibrateAxisStep1Window()
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = OpenWindow(false);
			if (window == null || pendingAxisCalibration == null)
			{
				return;
			}
			Joystick joystick = pendingAxisCalibration.joystick;
			if (joystick.axisCount == 0)
			{
				return;
			}
			int axisIndex = pendingAxisCalibration.axisIndex;
			if (axisIndex >= 0 && axisIndex < joystick.axisCount)
			{
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateAxisStep1WindowTitle);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetCalibrateAxisStep1WindowMessage(joystick.AxisElementIdentifiers[axisIndex].name));
				if (prefabs.centerStickGraphic != null)
				{
					window.AddContentImage(prefabs.centerStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
				}
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
				window.SetUpdateCallback(OnCalibrateAxisStep1WindowUpdate);
				window.timer.Start(_axisCalibrationTimeout);
				windowManager.Focus(window);
			}
		}

		private void ShowCalibrateAxisStep2Window()
		{
			if (currentPlayer == null)
			{
				return;
			}
			Window window = OpenWindow(false);
			if (window == null || pendingAxisCalibration == null)
			{
				return;
			}
			Joystick joystick = pendingAxisCalibration.joystick;
			if (joystick.axisCount == 0)
			{
				return;
			}
			int axisIndex = pendingAxisCalibration.axisIndex;
			if (axisIndex >= 0 && axisIndex < joystick.axisCount)
			{
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateAxisStep2WindowTitle);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetCalibrateAxisStep2WindowMessage(joystick.AxisElementIdentifiers[axisIndex].name));
				if (prefabs.moveStickGraphic != null)
				{
					window.AddContentImage(prefabs.moveStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
				}
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
				window.SetUpdateCallback(OnCalibrateAxisStep2WindowUpdate);
				window.timer.Start(_axisCalibrationTimeout);
				windowManager.Focus(window);
			}
		}

		private void ShowEditInputBehaviorsWindow()
		{
			if (currentPlayer != null && _inputBehaviorSettings != null)
			{
				InputBehaviorWindow inputBehaviorWindow = OpenWindow(prefabs.inputBehaviorsWindow, "EditInputBehaviorsWindow", true) as InputBehaviorWindow;
				if (!(inputBehaviorWindow == null))
				{
					inputBehaviorWindow.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.inputBehaviorSettingsWindowTitle);
					inputBehaviorWindow.SetData(currentPlayer.id, _inputBehaviorSettings);
					inputBehaviorWindow.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Done, CloseWindow);
					inputBehaviorWindow.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Cancel, CloseWindow);
					windowManager.Focus(inputBehaviorWindow);
				}
			}
		}

		private void ShowRestoreDefaultsWindow()
		{
			if (currentPlayer != null)
			{
				OpenModal(_language.restoreDefaultsWindowTitle, _language.restoreDefaultsWindowMessage, _language.yes, OnRestoreDefaultsConfirmed, _language.no, OnWindowCancel, true);
			}
		}

		private void CreateInputGrid()
		{
			InitializeInputGrid();
			CreateHeaderLabels();
			CreateActionLabelColumn();
			CreateKeyboardInputFieldColumn();
			CreateMouseInputFieldColumn();
			CreateControllerInputFieldColumn();
			CreateInputActionLabels();
			CreateInputFields();
			inputGrid.HideAll();
			ResetInputGridScrollBar();
		}

		private void InitializeInputGrid()
		{
			if (inputGrid == null)
			{
				inputGrid = new InputGrid();
			}
			else
			{
				inputGrid.ClearAll();
			}
			for (int i = 0; i < _mappingSets.Length; i++)
			{
				MappingSet mappingSet = _mappingSets[i];
				if (mappingSet == null || !mappingSet.isValid)
				{
					continue;
				}
				InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(mappingSet.mapCategoryId);
				if (mapCategory == null || !mapCategory.userAssignable)
				{
					continue;
				}
				inputGrid.AddMapCategory(mappingSet.mapCategoryId);
				if (mappingSet.actionListMode == MappingSet.ActionListMode.ActionCategory)
				{
					IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
					for (int j = 0; j < actionCategoryIds.Count; j++)
					{
						int num = actionCategoryIds[j];
						InputCategory actionCategory = ReInput.mapping.GetActionCategory(num);
						if (actionCategory == null || !actionCategory.userAssignable)
						{
							continue;
						}
						inputGrid.AddActionCategory(mappingSet.mapCategoryId, num);
						foreach (InputAction item in ReInput.mapping.UserAssignableActionsInCategory(num))
						{
							if (item.type == InputActionType.Axis)
							{
								inputGrid.AddAction(mappingSet.mapCategoryId, item, AxisRange.Full);
								inputGrid.AddAction(mappingSet.mapCategoryId, item, AxisRange.Positive);
								inputGrid.AddAction(mappingSet.mapCategoryId, item, AxisRange.Negative);
							}
							else if (item.type == InputActionType.Button)
							{
								inputGrid.AddAction(mappingSet.mapCategoryId, item, AxisRange.Positive);
							}
						}
					}
					continue;
				}
				IList<int> actionIds = mappingSet.actionIds;
				for (int k = 0; k < actionIds.Count; k++)
				{
					InputAction action = ReInput.mapping.GetAction(actionIds[k]);
					if (action != null)
					{
						if (action.type == InputActionType.Axis)
						{
							inputGrid.AddAction(mappingSet.mapCategoryId, action, AxisRange.Full);
							inputGrid.AddAction(mappingSet.mapCategoryId, action, AxisRange.Positive);
							inputGrid.AddAction(mappingSet.mapCategoryId, action, AxisRange.Negative);
						}
						else if (action.type == InputActionType.Button)
						{
							inputGrid.AddAction(mappingSet.mapCategoryId, action, AxisRange.Positive);
						}
					}
				}
			}
			references.inputGridInnerGroup.GetComponent<HorizontalLayoutGroup>().spacing = _inputColumnSpacing;
			references.inputGridLayoutElement.flexibleWidth = 0f;
			references.inputGridLayoutElement.preferredWidth = inputGridWidth;
		}

		private void RefreshInputGridStructure()
		{
			if (currentMappingSet != null)
			{
				inputGrid.HideAll();
				inputGrid.Show(currentMappingSet.mapCategoryId);
				references.inputGridInnerGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inputGrid.GetColumnHeight(currentMappingSet.mapCategoryId));
			}
		}

		private void CreateHeaderLabels()
		{
			references.inputGridHeader1 = CreateNewColumnGroup("ActionsHeader", references.inputGridHeadersGroup, _actionLabelWidth).transform;
			CreateLabel(prefabs.inputGridHeaderLabel, _language.actionColumnLabel, references.inputGridHeader1, Vector2.zero);
			if (_showKeyboard)
			{
				references.inputGridHeader2 = CreateNewColumnGroup("KeybordHeader", references.inputGridHeadersGroup, _keyboardColMaxWidth).transform;
				GUILabel gUILabel = CreateLabel(prefabs.inputGridHeaderLabel, _language.keyboardColumnLabel, references.inputGridHeader2, Vector2.zero);
				gUILabel.SetTextAlignment(TextAnchor.MiddleCenter);
			}
			if (_showMouse)
			{
				references.inputGridHeader3 = CreateNewColumnGroup("MouseHeader", references.inputGridHeadersGroup, _mouseColMaxWidth).transform;
				GUILabel gUILabel = CreateLabel(prefabs.inputGridHeaderLabel, _language.mouseColumnLabel, references.inputGridHeader3, Vector2.zero);
				gUILabel.SetTextAlignment(TextAnchor.MiddleCenter);
			}
			if (_showControllers)
			{
				references.inputGridHeader4 = CreateNewColumnGroup("ControllerHeader", references.inputGridHeadersGroup, _controllerColMaxWidth).transform;
				GUILabel gUILabel = CreateLabel(prefabs.inputGridHeaderLabel, _language.controllerColumnLabel, references.inputGridHeader4, Vector2.zero);
				gUILabel.SetTextAlignment(TextAnchor.MiddleCenter);
			}
		}

		private void CreateActionLabelColumn()
		{
			Transform inputGridActionColumn = CreateNewColumnGroup("ActionLabelColumn", references.inputGridInnerGroup, _actionLabelWidth).transform;
			references.inputGridActionColumn = inputGridActionColumn;
		}

		private void CreateKeyboardInputFieldColumn()
		{
			if (_showKeyboard)
			{
				CreateInputFieldColumn("KeyboardColumn", ControllerType.Keyboard, _keyboardColMaxWidth, _keyboardInputFieldCount, true);
			}
		}

		private void CreateMouseInputFieldColumn()
		{
			if (_showMouse)
			{
				CreateInputFieldColumn("MouseColumn", ControllerType.Mouse, _mouseColMaxWidth, _mouseInputFieldCount, false);
			}
		}

		private void CreateControllerInputFieldColumn()
		{
			if (_showControllers)
			{
				CreateInputFieldColumn("ControllerColumn", ControllerType.Joystick, _controllerColMaxWidth, _controllerInputFieldCount, false);
			}
		}

		private void CreateInputFieldColumn(string name, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
		{
			Transform transform = CreateNewColumnGroup(name, references.inputGridInnerGroup, maxWidth).transform;
			switch (controllerType)
			{
			case ControllerType.Joystick:
				references.inputGridControllerColumn = transform;
				break;
			case ControllerType.Keyboard:
				references.inputGridKeyboardColumn = transform;
				break;
			case ControllerType.Mouse:
				references.inputGridMouseColumn = transform;
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private void CreateInputActionLabels()
		{
			Transform inputGridActionColumn = references.inputGridActionColumn;
			for (int i = 0; i < _mappingSets.Length; i++)
			{
				MappingSet mappingSet = _mappingSets[i];
				if (mappingSet == null || !mappingSet.isValid)
				{
					continue;
				}
				int num = 0;
				if (mappingSet.actionListMode == MappingSet.ActionListMode.ActionCategory)
				{
					int num2 = 0;
					IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
					for (int j = 0; j < actionCategoryIds.Count; j++)
					{
						InputCategory actionCategory = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
						if (actionCategory == null || !actionCategory.userAssignable || CountIEnumerable(ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id)) == 0)
						{
							continue;
						}
						if (_showActionCategoryLabels)
						{
							if (num2 > 0)
							{
								num -= _inputRowCategorySpacing;
							}
							GUILabel gUILabel = CreateLabel(actionCategory.descriptiveName, inputGridActionColumn, new Vector2(0f, num));
							gUILabel.SetFontStyle(FontStyle.Bold);
							gUILabel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionCategoryLabel(mappingSet.mapCategoryId, actionCategory.id, gUILabel);
							num -= _inputRowHeight;
						}
						foreach (InputAction item in ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id, true))
						{
							if (item.type == InputActionType.Axis)
							{
								GUILabel gUILabel2 = CreateLabel(item.descriptiveName, inputGridActionColumn, new Vector2(0f, num));
								gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, item.id, AxisRange.Full, gUILabel2);
								num -= _inputRowHeight;
								string labelText = (string.IsNullOrEmpty(item.positiveDescriptiveName) ? (item.descriptiveName + " +") : item.positiveDescriptiveName);
								gUILabel2 = CreateLabel(labelText, inputGridActionColumn, new Vector2(0f, num));
								gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, item.id, AxisRange.Positive, gUILabel2);
								num -= _inputRowHeight;
								string labelText2 = (string.IsNullOrEmpty(item.negativeDescriptiveName) ? (item.descriptiveName + " -") : item.negativeDescriptiveName);
								gUILabel2 = CreateLabel(labelText2, inputGridActionColumn, new Vector2(0f, num));
								gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, item.id, AxisRange.Negative, gUILabel2);
								num -= _inputRowHeight;
							}
							else if (item.type == InputActionType.Button)
							{
								GUILabel gUILabel2 = CreateLabel(item.descriptiveName, inputGridActionColumn, new Vector2(0f, num));
								gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, item.id, AxisRange.Positive, gUILabel2);
								num -= _inputRowHeight;
							}
						}
						num2++;
					}
				}
				else
				{
					IList<int> actionIds = mappingSet.actionIds;
					for (int k = 0; k < actionIds.Count; k++)
					{
						InputAction action = ReInput.mapping.GetAction(actionIds[k]);
						if (action == null || !action.userAssignable)
						{
							continue;
						}
						InputCategory actionCategory2 = ReInput.mapping.GetActionCategory(action.categoryId);
						if (actionCategory2 != null && actionCategory2.userAssignable)
						{
							if (action.type == InputActionType.Axis)
							{
								GUILabel gUILabel3 = CreateLabel(action.descriptiveName, inputGridActionColumn, new Vector2(0f, num));
								gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, AxisRange.Full, gUILabel3);
								num -= _inputRowHeight;
								gUILabel3 = CreateLabel(action.positiveDescriptiveName, inputGridActionColumn, new Vector2(0f, num));
								gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, AxisRange.Positive, gUILabel3);
								num -= _inputRowHeight;
								gUILabel3 = CreateLabel(action.negativeDescriptiveName, inputGridActionColumn, new Vector2(0f, num));
								gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, AxisRange.Negative, gUILabel3);
								num -= _inputRowHeight;
							}
							else if (action.type == InputActionType.Button)
							{
								GUILabel gUILabel3 = CreateLabel(action.descriptiveName, inputGridActionColumn, new Vector2(0f, num));
								gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, AxisRange.Positive, gUILabel3);
								num -= _inputRowHeight;
							}
						}
					}
				}
				inputGrid.SetColumnHeight(mappingSet.mapCategoryId, -num);
			}
		}

		private void CreateInputFields()
		{
			if (_showControllers)
			{
				CreateInputFields(references.inputGridControllerColumn, ControllerType.Joystick, _controllerColMaxWidth, _controllerInputFieldCount, false);
			}
			if (_showKeyboard)
			{
				CreateInputFields(references.inputGridKeyboardColumn, ControllerType.Keyboard, _keyboardColMaxWidth, _keyboardInputFieldCount, true);
			}
			if (_showMouse)
			{
				CreateInputFields(references.inputGridMouseColumn, ControllerType.Mouse, _mouseColMaxWidth, _mouseInputFieldCount, false);
			}
		}

		private void CreateInputFields(Transform columnXform, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
		{
			for (int i = 0; i < _mappingSets.Length; i++)
			{
				MappingSet mappingSet = _mappingSets[i];
				if (mappingSet == null || !mappingSet.isValid)
				{
					continue;
				}
				int fieldWidth = maxWidth / cols;
				int yPos = 0;
				int num = 0;
				if (mappingSet.actionListMode == MappingSet.ActionListMode.ActionCategory)
				{
					IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
					for (int j = 0; j < actionCategoryIds.Count; j++)
					{
						InputCategory actionCategory = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
						if (actionCategory == null || !actionCategory.userAssignable || CountIEnumerable(ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id)) == 0)
						{
							continue;
						}
						if (_showActionCategoryLabels)
						{
							yPos -= ((num <= 0) ? _inputRowHeight : (_inputRowHeight + _inputRowCategorySpacing));
						}
						foreach (InputAction item in ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id, true))
						{
							if (item.type == InputActionType.Axis)
							{
								CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, item, AxisRange.Full, controllerType, cols, fieldWidth, ref yPos, disableFullAxis);
								CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, item, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, false);
								CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, item, AxisRange.Negative, controllerType, cols, fieldWidth, ref yPos, false);
							}
							else if (item.type == InputActionType.Button)
							{
								CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, item, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, false);
							}
							num++;
						}
					}
					continue;
				}
				IList<int> actionIds = mappingSet.actionIds;
				for (int k = 0; k < actionIds.Count; k++)
				{
					InputAction action = ReInput.mapping.GetAction(actionIds[k]);
					if (action == null || !action.userAssignable)
					{
						continue;
					}
					InputCategory actionCategory2 = ReInput.mapping.GetActionCategory(action.categoryId);
					if (actionCategory2 != null && actionCategory2.userAssignable)
					{
						if (action.type == InputActionType.Axis)
						{
							CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, AxisRange.Full, controllerType, cols, fieldWidth, ref yPos, disableFullAxis);
							CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, false);
							CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, AxisRange.Negative, controllerType, cols, fieldWidth, ref yPos, false);
						}
						else if (action.type == InputActionType.Button)
						{
							CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, false);
						}
					}
				}
			}
		}

		private void CreateInputFieldSet(Transform parent, int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int cols, int fieldWidth, ref int yPos, bool disableFullAxis)
		{
			GameObject gameObject = CreateNewGUIObject("FieldLayoutGroup", parent, new Vector2(0f, yPos));
			HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(1f, 1f);
			component.pivot = new Vector2(0f, 1f);
			component.sizeDelta = Vector2.zero;
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
			inputGrid.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, gameObject);
			for (int i = 0; i < cols; i++)
			{
				int num = ((axisRange == AxisRange.Full) ? _invertToggleWidth : 0);
				GUIInputField gUIInputField = CreateInputField(horizontalLayoutGroup.transform, Vector2.zero, string.Empty, action.id, axisRange, controllerType, i);
				gUIInputField.SetFirstChildObjectWidth(LayoutElementSizeType.PreferredSize, fieldWidth - num);
				inputGrid.AddInputField(mapCategoryId, action, axisRange, controllerType, i, gUIInputField);
				if (axisRange == AxisRange.Full)
				{
					if (!disableFullAxis)
					{
						GUIToggle gUIToggle = CreateToggle(prefabs.inputGridFieldInvertToggle, horizontalLayoutGroup.transform, Vector2.zero, string.Empty, action.id, axisRange, controllerType, i);
						gUIToggle.SetFirstChildObjectWidth(LayoutElementSizeType.MinSize, num);
						gUIInputField.AddToggle(gUIToggle);
					}
					else
					{
						gUIInputField.SetInteractible(false, false, true);
					}
				}
			}
			yPos -= _inputRowHeight;
		}

		private void PopulateInputFields()
		{
			inputGrid.InitializeFields(currentMapCategoryId);
			if (currentPlayer == null)
			{
				return;
			}
			inputGrid.SetFieldsActive(currentMapCategoryId, true);
			foreach (InputActionSet actionSet in inputGrid.GetActionSets(currentMapCategoryId))
			{
				if (_showKeyboard)
				{
					if (currentPlayerId == 0)
					{
						ControllerType controllerType = ControllerType.Keyboard;
						int controllerId = 0;
						int layoutId = _keyboardMapDefaultLayout;
						int maxFields = _keyboardInputFieldCount;
						ControllerMap controllerMapOrCreateNew = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
						PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew, controllerType, controllerId, maxFields);
					}
					else
					{
						DisableInputFieldGroup(actionSet, ControllerType.Keyboard, _keyboardInputFieldCount);
					}
				}
				if (_showMouse)
				{
					ControllerType controllerType = ControllerType.Mouse;
					int controllerId = 0;
					int layoutId = _mouseMapDefaultLayout;
					int maxFields = _mouseInputFieldCount;
					ControllerMap controllerMapOrCreateNew2 = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
					if (currentPlayer.controllers.hasMouse)
					{
						PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew2, controllerType, controllerId, maxFields);
					}
				}
				if (isJoystickSelected && currentPlayer.controllers.joystickCount > 0)
				{
					ControllerType controllerType = ControllerType.Joystick;
					int controllerId = currentJoystick.id;
					int layoutId = _joystickMapDefaultLayout;
					int maxFields = _controllerInputFieldCount;
					ControllerMap controllerMapOrCreateNew3 = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
					PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew3, controllerType, controllerId, maxFields);
				}
				else
				{
					DisableInputFieldGroup(actionSet, ControllerType.Joystick, _controllerInputFieldCount);
				}
			}
		}

		private void PopulateInputFieldGroup(InputActionSet actionSet, ControllerMap controllerMap, ControllerType controllerType, int controllerId, int maxFields)
		{
			if (controllerMap == null)
			{
				return;
			}
			int num = 0;
			inputGrid.SetFixedFieldData(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId);
			foreach (ActionElementMap item in controllerMap.ElementMapsWithAction(actionSet.actionId))
			{
				if (item.elementType == ControllerElementType.Button)
				{
					if (actionSet.axisRange == AxisRange.Full)
					{
						continue;
					}
					if (actionSet.axisRange == AxisRange.Positive)
					{
						if (item.axisContribution == Pole.Negative)
						{
							continue;
						}
					}
					else if (actionSet.axisRange == AxisRange.Negative && item.axisContribution == Pole.Positive)
					{
						continue;
					}
					inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, item.id, item.elementIdentifierName, false);
				}
				else if (item.elementType == ControllerElementType.Axis)
				{
					if (actionSet.axisRange == AxisRange.Full)
					{
						if (item.axisRange != AxisRange.Full)
						{
							continue;
						}
						inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, item.id, item.elementIdentifierName, item.invert);
					}
					else if (actionSet.axisRange == AxisRange.Positive)
					{
						if (item.axisRange == AxisRange.Full || item.axisContribution == Pole.Negative)
						{
							continue;
						}
						inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, item.id, item.elementIdentifierName, false);
					}
					else if (actionSet.axisRange == AxisRange.Negative)
					{
						if (item.axisRange == AxisRange.Full || item.axisContribution == Pole.Positive)
						{
							continue;
						}
						inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, item.id, item.elementIdentifierName, false);
					}
				}
				num++;
				if (num > maxFields)
				{
					break;
				}
			}
		}

		private void DisableInputFieldGroup(InputActionSet actionSet, ControllerType controllerType, int fieldCount)
		{
			for (int i = 0; i < fieldCount; i++)
			{
				GUIInputField gUIInputField = inputGrid.GetGUIInputField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, i);
				if (gUIInputField != null)
				{
					gUIInputField.SetInteractible(false, false);
				}
			}
		}

		private void ResetInputGridScrollBar()
		{
			references.inputGridInnerGroup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			references.inputGridVScrollbar.value = 1f;
		}

		private void CreateLayout()
		{
			references.playersGroup.gameObject.SetActive(showPlayers);
			references.controllerGroup.gameObject.SetActive(_showControllers && showControllerGroupLabel);
			references.assignedControllersGroup.gameObject.SetActive(_showControllers && ShowAssignedControllers());
			references.settingsAndMapCategoriesGroup.gameObject.SetActive(showSettings || showMapCategories);
			references.settingsGroup.gameObject.SetActive(showSettings);
			references.mapCategoriesGroup.gameObject.SetActive(showMapCategories);
		}

		private void Draw()
		{
			DrawPlayersGroup();
			DrawControllersGroup();
			DrawSettingsGroup();
			DrawMapCategoriesGroup();
			DrawWindowButtonsGroup();
		}

		private void DrawPlayersGroup()
		{
			if (!showPlayers)
			{
				return;
			}
			references.playersGroup.labelText = _language.playersGroupLabel;
			references.playersGroup.SetLabelActive(_showPlayersGroupLabel);
			for (int i = 0; i < playerCount; i++)
			{
				Player player = ReInput.players.GetPlayer(i);
				if (player != null)
				{
					GameObject gameObject = UITools.InstantiateGUIObject<ButtonInfo>(prefabs.button, references.playersGroup.content, "Player" + i + "Button");
					GUIButton gUIButton = new GUIButton(gameObject);
					gUIButton.SetLabel(player.descriptiveName);
					gUIButton.SetButtonInfoData("PlayerSelection", player.id);
					gUIButton.SetOnClickCallback(OnButtonActivated);
					gUIButton.buttonInfo.OnSelectedEvent += OnUIElementSelected;
					playerButtons.Add(gUIButton);
				}
			}
		}

		private void DrawControllersGroup()
		{
			if (_showControllers)
			{
				references.controllerSettingsGroup.labelText = _language.controllerSettingsGroupLabel;
				references.controllerSettingsGroup.SetLabelActive(_showControllerGroupLabel);
				references.controllerNameLabel.gameObject.SetActive(_showControllerNameLabel);
				references.controllerGroupLabelGroup.gameObject.SetActive(_showControllerGroupLabel || _showControllerNameLabel);
				if (ShowAssignedControllers())
				{
					references.assignedControllersGroup.labelText = _language.assignedControllersGroupLabel;
					references.assignedControllersGroup.SetLabelActive(_showAssignedControllersGroupLabel);
				}
				ButtonInfo component = references.removeControllerButton.GetComponent<ButtonInfo>();
				component.text.text = _language.removeControllerButtonLabel;
				component = references.calibrateControllerButton.GetComponent<ButtonInfo>();
				component.text.text = _language.calibrateControllerButtonLabel;
				component = references.assignControllerButton.GetComponent<ButtonInfo>();
				component.text.text = _language.assignControllerButtonLabel;
				GUIButton gUIButton = CreateButton(_language.none, references.assignedControllersGroup.content, Vector2.zero);
				gUIButton.SetInteractible(false, false, true);
				assignedControllerButtonsPlaceholder = gUIButton;
			}
		}

		private void DrawSettingsGroup()
		{
			if (showSettings)
			{
				references.settingsGroup.labelText = _language.settingsGroupLabel;
				references.settingsGroup.SetLabelActive(_showSettingsGroupLabel);
				GUIButton gUIButton = CreateButton(_language.inputBehaviorSettingsButtonLabel, references.settingsGroup.content, Vector2.zero);
				gUIButton.buttonInfo.OnSelectedEvent += OnUIElementSelected;
				gUIButton.SetButtonInfoData("EditInputBehaviors", 0);
				gUIButton.SetOnClickCallback(OnButtonActivated);
			}
		}

		private void DrawMapCategoriesGroup()
		{
			if (!showMapCategories || _mappingSets == null)
			{
				return;
			}
			references.mapCategoriesGroup.labelText = _language.mapCategoriesGroupLabel;
			references.mapCategoriesGroup.SetLabelActive(_showMapCategoriesGroupLabel);
			for (int i = 0; i < _mappingSets.Length; i++)
			{
				MappingSet mappingSet = _mappingSets[i];
				if (mappingSet != null)
				{
					InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(mappingSet.mapCategoryId);
					if (mapCategory != null)
					{
						GameObject gameObject = UITools.InstantiateGUIObject<ButtonInfo>(prefabs.button, references.mapCategoriesGroup.content, mapCategory.name + "Button");
						GUIButton gUIButton = new GUIButton(gameObject);
						gUIButton.SetLabel(mapCategory.descriptiveName);
						gUIButton.SetButtonInfoData("MapCategorySelection", mapCategory.id);
						gUIButton.SetOnClickCallback(OnButtonActivated);
						gUIButton.buttonInfo.OnSelectedEvent += OnUIElementSelected;
						mapCategoryButtons.Add(gUIButton);
					}
				}
			}
		}

		private void DrawWindowButtonsGroup()
		{
			references.doneButton.GetComponent<ButtonInfo>().text.text = _language.doneButtonLabel;
			references.restoreDefaultsButton.GetComponent<ButtonInfo>().text.text = _language.restoreDefaultsButtonLabel;
		}

		private void Redraw(bool listsChanged, bool playTransitions)
		{
			RedrawPlayerGroup(playTransitions);
			RedrawControllerGroup();
			RedrawMapCategoriesGroup(playTransitions);
			RedrawInputGrid(listsChanged);
			if (currentUISelection == null || !currentUISelection.activeInHierarchy)
			{
				RestoreLastUISelection();
			}
		}

		private void RedrawPlayerGroup(bool playTransitions)
		{
			if (showPlayers)
			{
				for (int i = 0; i < playerButtons.Count; i++)
				{
					bool state = currentPlayerId != playerButtons[i].buttonInfo.intData;
					playerButtons[i].SetInteractible(state, playTransitions);
				}
			}
		}

		private void RedrawControllerGroup()
		{
			int num = -1;
			references.controllerNameLabel.text = _language.none;
			UITools.SetInteractable(references.removeControllerButton, false, false);
			UITools.SetInteractable(references.assignControllerButton, false, false);
			UITools.SetInteractable(references.calibrateControllerButton, false, false);
			if (ShowAssignedControllers())
			{
				foreach (GUIButton assignedControllerButton in assignedControllerButtons)
				{
					if (!(assignedControllerButton.gameObject == null))
					{
						if (currentUISelection == assignedControllerButton.gameObject)
						{
							num = assignedControllerButton.buttonInfo.intData;
						}
						UnityEngine.Object.Destroy(assignedControllerButton.gameObject);
					}
				}
				assignedControllerButtons.Clear();
				assignedControllerButtonsPlaceholder.SetActive(true);
			}
			Player player = ReInput.players.GetPlayer(currentPlayerId);
			if (player == null)
			{
				return;
			}
			if (ShowAssignedControllers())
			{
				if (player.controllers.joystickCount > 0)
				{
					assignedControllerButtonsPlaceholder.SetActive(false);
				}
				foreach (Joystick joystick in player.controllers.Joysticks)
				{
					GUIButton gUIButton = CreateButton(joystick.name, references.assignedControllersGroup.content, Vector2.zero);
					gUIButton.SetButtonInfoData("AssignedControllerSelection", joystick.id);
					gUIButton.SetOnClickCallback(OnButtonActivated);
					gUIButton.buttonInfo.OnSelectedEvent += OnUIElementSelected;
					assignedControllerButtons.Add(gUIButton);
					if (joystick.id == currentJoystickId)
					{
						gUIButton.SetInteractible(false, true);
					}
				}
				if (player.controllers.joystickCount > 0 && !isJoystickSelected)
				{
					currentJoystickId = player.controllers.Joysticks[0].id;
					assignedControllerButtons[0].SetInteractible(false, false);
				}
				if (num >= 0)
				{
					foreach (GUIButton assignedControllerButton2 in assignedControllerButtons)
					{
						if (assignedControllerButton2.buttonInfo.intData == num)
						{
							SetUISelection(assignedControllerButton2.gameObject);
							break;
						}
					}
				}
			}
			else if (player.controllers.joystickCount > 0 && !isJoystickSelected)
			{
				currentJoystickId = player.controllers.Joysticks[0].id;
			}
			if (isJoystickSelected && player.controllers.joystickCount > 0 && currentPlayerId == 0)
			{
				references.removeControllerButton.interactable = true;
				references.controllerNameLabel.text = currentJoystick.name;
				if (currentJoystick.axisCount > 0)
				{
					references.calibrateControllerButton.interactable = true;
				}
			}
			int joystickCount = player.controllers.joystickCount;
			int joystickCount2 = ReInput.controllers.joystickCount;
			int num2 = GetMaxControllersPerPlayer();
			bool flag = num2 == 0;
			if (joystickCount2 > 0 && joystickCount < joystickCount2 && (num2 == 1 || flag || joystickCount < num2))
			{
				UITools.SetInteractable(references.assignControllerButton, true, false);
			}
		}

		private void RedrawMapCategoriesGroup(bool playTransitions)
		{
			if (showMapCategories)
			{
				for (int i = 0; i < mapCategoryButtons.Count; i++)
				{
					bool state = currentMapCategoryId != mapCategoryButtons[i].buttonInfo.intData;
					mapCategoryButtons[i].SetInteractible(state, playTransitions);
				}
			}
		}

		private void RedrawInputGrid(bool listsChanged)
		{
			if (listsChanged)
			{
				RefreshInputGridStructure();
			}
			PopulateInputFields();
			if (listsChanged)
			{
				ResetInputGridScrollBar();
			}
		}

		private void ForceRefresh()
		{
			if (windowManager.isWindowOpen)
			{
				CloseAllWindows();
			}
			else
			{
				Redraw(false, false);
			}
		}

		private void CreateInputCategoryRow(ref int rowCount, InputCategory category)
		{
			CreateLabel(category.descriptiveName, references.inputGridActionColumn, new Vector2(0f, (float)(rowCount * _inputRowHeight) * -1f));
			rowCount++;
		}

		private GUILabel CreateLabel(string labelText, Transform parent, Vector2 offset)
		{
			return CreateLabel(prefabs.inputGridLabel, labelText, parent, offset);
		}

		private GUILabel CreateLabel(GameObject prefab, string labelText, Transform parent, Vector2 offset)
		{
			GameObject gameObject = InstantiateGUIObject(prefab, parent, offset);
			Text componentInSelfOrChildren = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
			if (componentInSelfOrChildren == null)
			{
				Debug.LogError("Rewired Control Mapper: Label prefab is missing Text component!");
				return null;
			}
			string text = Localization.Translate(labelText).text;
			componentInSelfOrChildren.text = ((text != null) ? Localization.Translate(labelText).text : labelText);
			return new GUILabel(gameObject);
		}

		private GUIButton CreateButton(string labelText, Transform parent, Vector2 offset)
		{
			GUIButton gUIButton = new GUIButton(InstantiateGUIObject(prefabs.button, parent, offset));
			gUIButton.SetLabel(labelText);
			return gUIButton;
		}

		private GUIButton CreateFitButton(string labelText, Transform parent, Vector2 offset)
		{
			GUIButton gUIButton = new GUIButton(InstantiateGUIObject(prefabs.fitButton, parent, offset));
			gUIButton.SetLabel(labelText);
			return gUIButton;
		}

		private GUIInputField CreateInputField(Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			GUIInputField gUIInputField = CreateInputField(parent, offset);
			gUIInputField.SetLabel(string.Empty);
			gUIInputField.SetFieldInfoData(actionId, axisRange, controllerType, fieldIndex);
			gUIInputField.SetOnClickCallback(inputFieldActivatedDelegate);
			gUIInputField.fieldInfo.OnSelectedEvent += OnUIElementSelected;
			return gUIInputField;
		}

		private GUIInputField CreateInputField(Transform parent, Vector2 offset)
		{
			return new GUIInputField(InstantiateGUIObject(prefabs.inputGridFieldButton, parent, offset));
		}

		private GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			GUIToggle gUIToggle = CreateToggle(prefab, parent, offset);
			gUIToggle.SetToggleInfoData(actionId, axisRange, controllerType, fieldIndex);
			gUIToggle.SetOnSubmitCallback(inputFieldInvertToggleStateChangedDelegate);
			gUIToggle.toggleInfo.OnSelectedEvent += OnUIElementSelected;
			return gUIToggle;
		}

		private GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset)
		{
			return new GUIToggle(InstantiateGUIObject(prefab, parent, offset));
		}

		private GameObject InstantiateGUIObject(GameObject prefab, Transform parent, Vector2 offset)
		{
			if (prefab == null)
			{
				Debug.LogError("Rewired Control Mapper: Prefab is null!");
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			return InitializeNewGUIGameObject(gameObject, parent, offset);
		}

		private GameObject CreateNewGUIObject(string name, Transform parent, Vector2 offset)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = name;
			gameObject.AddComponent<RectTransform>();
			return InitializeNewGUIGameObject(gameObject, parent, offset);
		}

		private GameObject InitializeNewGUIGameObject(GameObject gameObject, Transform parent, Vector2 offset)
		{
			if (gameObject == null)
			{
				Debug.LogError("Rewired Control Mapper: GameObject is null!");
				return null;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			if (component == null)
			{
				Debug.LogError("Rewired Control Mapper: GameObject does not have a RectTransform component!");
				return gameObject;
			}
			if (parent != null)
			{
				component.SetParent(parent, false);
			}
			component.anchoredPosition = offset;
			return gameObject;
		}

		private GameObject CreateNewColumnGroup(string name, Transform parent, int maxWidth)
		{
			GameObject gameObject = CreateNewGUIObject(name, parent, Vector2.zero);
			inputGrid.AddGroup(gameObject);
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			if (maxWidth >= 0)
			{
				layoutElement.preferredWidth = maxWidth;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(1f, 0f);
			return gameObject;
		}

		private Window OpenWindow(bool closeOthers)
		{
			return OpenWindow(string.Empty, closeOthers);
		}

		private Window OpenWindow(string name, bool closeOthers)
		{
			if (closeOthers)
			{
				windowManager.CancelAll();
			}
			Window window = windowManager.OpenWindow(name, _defaultWindowWidth, _defaultWindowHeight);
			if (window == null)
			{
				return null;
			}
			ChildWindowOpened();
			return window;
		}

		private Window OpenWindow(GameObject windowPrefab, bool closeOthers)
		{
			return OpenWindow(windowPrefab, string.Empty, closeOthers);
		}

		private Window OpenWindow(GameObject windowPrefab, string name, bool closeOthers)
		{
			if (closeOthers)
			{
				windowManager.CancelAll();
			}
			Window window = windowManager.OpenWindow(windowPrefab, name);
			if (window == null)
			{
				return null;
			}
			ChildWindowOpened();
			return window;
		}

		private void OpenModal(string title, string message, string confirmText, Action<int> confirmAction, string cancelText, Action<int> cancelAction, bool closeOthers)
		{
			Window window = OpenWindow(closeOthers);
			if (!(window == null))
			{
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, title);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), message);
				UnityAction unityAction = delegate
				{
					OnWindowCancel(window.id);
				};
				window.cancelCallback = unityAction;
				window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, confirmText, delegate
				{
					OnRestoreDefaultsConfirmed(window.id);
				}, unityAction, false);
				window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, cancelText, unityAction, unityAction, true);
				windowManager.Focus(window);
			}
		}

		private void CloseWindow(int windowId)
		{
			if (windowManager.isWindowOpen)
			{
				windowManager.CloseWindow(windowId);
				ChildWindowClosed();
			}
		}

		private void CloseTopWindow()
		{
			if (windowManager.isWindowOpen)
			{
				windowManager.CloseTop();
				ChildWindowClosed();
			}
		}

		private void CloseAllWindows()
		{
			if (windowManager.isWindowOpen)
			{
				windowManager.CancelAll();
				ChildWindowClosed();
			}
		}

		private void ChildWindowOpened()
		{
			if (windowManager.isWindowOpen)
			{
				SetIsFocused(false);
			}
		}

		private void ChildWindowClosed()
		{
			if (!windowManager.isWindowOpen)
			{
				SetIsFocused(true);
			}
		}

		private bool HasElementAssignmentConflicts(Player player, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (player == null || mapping == null)
			{
				return false;
			}
			ElementAssignmentConflictCheck conflictCheck;
			if (!CreateConflictCheck(mapping, assignment, out conflictCheck))
			{
				return false;
			}
			if (skipOtherPlayers)
			{
				if (ReInput.players.SystemPlayer.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck))
				{
					return true;
				}
				if (player.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck))
				{
					return true;
				}
				return false;
			}
			return ReInput.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck);
		}

		private bool IsBlockingAssignmentConflict(InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			ElementAssignmentConflictCheck conflictCheck;
			if (!CreateConflictCheck(mapping, assignment, out conflictCheck))
			{
				return false;
			}
			if (skipOtherPlayers)
			{
				foreach (ElementAssignmentConflictInfo item in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!item.isUserAssignable)
					{
						return true;
					}
				}
				foreach (ElementAssignmentConflictInfo item2 in currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!item2.isUserAssignable)
					{
						return true;
					}
				}
			}
			else
			{
				foreach (ElementAssignmentConflictInfo item3 in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!item3.isUserAssignable)
					{
						return true;
					}
				}
			}
			return false;
		}

		private IEnumerable<ElementAssignmentConflictInfo> ElementAssignmentConflicts(Player player, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			ElementAssignmentConflictCheck conflictCheck;
			if (player == null || mapping == null || !CreateConflictCheck(mapping, assignment, out conflictCheck))
			{
				yield break;
			}
			if (skipOtherPlayers)
			{
				foreach (ElementAssignmentConflictInfo conflict in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!conflict.isUserAssignable)
					{
						yield return conflict;
					}
				}
				{
					foreach (ElementAssignmentConflictInfo conflict2 in player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
					{
						if (!conflict2.isUserAssignable)
						{
							yield return conflict2;
						}
					}
					yield break;
				}
			}
			foreach (ElementAssignmentConflictInfo conflict3 in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				if (!conflict3.isUserAssignable)
				{
					yield return conflict3;
				}
			}
		}

		private bool CreateConflictCheck(InputMapping mapping, ElementAssignment assignment, out ElementAssignmentConflictCheck conflictCheck)
		{
			if (mapping == null || currentPlayer == null)
			{
				conflictCheck = default(ElementAssignmentConflictCheck);
				return false;
			}
			conflictCheck = assignment.ToElementAssignmentConflictCheck();
			conflictCheck.playerId = currentPlayer.id;
			conflictCheck.controllerType = mapping.controllerType;
			conflictCheck.controllerId = mapping.controllerId;
			conflictCheck.controllerMapId = mapping.map.id;
			conflictCheck.controllerMapCategoryId = mapping.map.categoryId;
			if (mapping.aem != null)
			{
				conflictCheck.elementMapId = mapping.aem.id;
			}
			return true;
		}

		private void PollKeyboardForAssignment(out ControllerPollingInfo pollingInfo, out bool modifierKeyPressed, out ModifierKeyFlags modifierFlags, out string label)
		{
			pollingInfo = default(ControllerPollingInfo);
			label = string.Empty;
			modifierKeyPressed = false;
			modifierFlags = ModifierKeyFlags.None;
			int num = 0;
			ControllerPollingInfo controllerPollingInfo = default(ControllerPollingInfo);
			ControllerPollingInfo controllerPollingInfo2 = default(ControllerPollingInfo);
			ModifierKeyFlags modifierKeyFlags = ModifierKeyFlags.None;
			foreach (ControllerPollingInfo item in ReInput.controllers.Keyboard.PollForAllKeys())
			{
				KeyCode keyboardKey = item.keyboardKey;
				if (keyboardKey == KeyCode.AltGr)
				{
					continue;
				}
				if (Keyboard.IsModifierKey(item.keyboardKey))
				{
					if (num == 0)
					{
						controllerPollingInfo2 = item;
					}
					modifierKeyFlags |= Keyboard.KeyCodeToModifierKeyFlags(keyboardKey);
					num++;
				}
				else if (controllerPollingInfo.keyboardKey == KeyCode.None)
				{
					controllerPollingInfo = item;
				}
			}
			if (controllerPollingInfo.keyboardKey != KeyCode.None)
			{
				if (ReInput.controllers.Keyboard.GetKeyDown(controllerPollingInfo.keyboardKey))
				{
					if (num == 0)
					{
						pollingInfo = controllerPollingInfo;
						return;
					}
					pollingInfo = controllerPollingInfo;
					modifierFlags = modifierKeyFlags;
				}
			}
			else
			{
				if (num <= 0)
				{
					return;
				}
				modifierKeyPressed = true;
				if (num == 1)
				{
					if (ReInput.controllers.Keyboard.GetKeyTimePressed(controllerPollingInfo2.keyboardKey) > 1f)
					{
						pollingInfo = controllerPollingInfo2;
					}
					else
					{
						label = Keyboard.GetKeyName(controllerPollingInfo2.keyboardKey);
					}
				}
				else
				{
					label = Keyboard.ModifierKeyFlagsToString(modifierKeyFlags);
				}
			}
		}

		private void StartAxisCalibration(int axisIndex)
		{
			if (currentPlayer != null && currentPlayer.controllers.joystickCount != 0)
			{
				Joystick joystick = currentJoystick;
				if (axisIndex >= 0 && axisIndex < joystick.axisCount)
				{
					pendingAxisCalibration = new AxisCalibrator(joystick, axisIndex);
					ShowCalibrateAxisStep1Window();
				}
			}
		}

		private void EndAxisCalibration()
		{
			if (pendingAxisCalibration != null)
			{
				pendingAxisCalibration.Commit();
				pendingAxisCalibration = null;
			}
		}

		private void SetUISelection(GameObject selection)
		{
			if (!(EventSystem.current == null))
			{
				EventSystem.current.SetSelectedGameObject(selection);
			}
		}

		private void RestoreLastUISelection()
		{
			if (lastUISelection == null || !lastUISelection.activeInHierarchy)
			{
				SetDefaultUISelection();
			}
			else
			{
				SetUISelection(lastUISelection);
			}
		}

		private void SetDefaultUISelection()
		{
			if (isOpen)
			{
				if (references.defaultSelection == null)
				{
					SetUISelection(null);
				}
				else
				{
					SetUISelection(references.defaultSelection.gameObject);
				}
			}
		}

		private void SelectDefaultMapCategory(bool redraw)
		{
			currentMapCategoryId = GetDefaultMapCategoryId();
			OnMapCategorySelected(currentMapCategoryId, redraw);
			if (!showMapCategories)
			{
				return;
			}
			for (int i = 0; i < _mappingSets.Length; i++)
			{
				InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(_mappingSets[i].mapCategoryId);
				if (mapCategory != null)
				{
					currentMapCategoryId = _mappingSets[i].mapCategoryId;
					break;
				}
			}
			if (currentMapCategoryId >= 0)
			{
				for (int j = 0; j < _mappingSets.Length; j++)
				{
					bool state = ((_mappingSets[j].mapCategoryId != currentMapCategoryId) ? true : false);
					mapCategoryButtons[j].SetInteractible(state, false);
				}
			}
		}

		private void CheckUISelection()
		{
			if (isFocused && currentUISelection == null)
			{
				RestoreLastUISelection();
			}
		}

		private void OnUIElementSelected(GameObject selectedObject)
		{
			lastUISelection = selectedObject;
		}

		private void SetIsFocused(bool state)
		{
			references.mainCanvasGroup.interactable = state;
			if (state)
			{
				Redraw(false, false);
				RestoreLastUISelection();
				blockInputOnFocusEndTime = Time.unscaledTime + 0.1f;
			}
		}

		public void Toggle()
		{
			if (isOpen)
			{
				Close(true);
			}
			else
			{
				Open();
			}
		}

		public void Open()
		{
			Open(false);
		}

		private void Open(bool force)
		{
			if (!initialized)
			{
				Initialize();
			}
			if (initialized && (force || !isOpen))
			{
				Clear();
				canvas.SetActive(true);
				OnPlayerSelected(0, false);
				SelectDefaultMapCategory(false);
				SetDefaultUISelection();
				Redraw(true, false);
				if (this.ScreenOpenedEvent != null)
				{
					this.ScreenOpenedEvent();
				}
			}
		}

		public void Close(bool save)
		{
			if (initialized && isOpen)
			{
				if (save && ReInput.userDataStore != null)
				{
					ReInput.userDataStore.Save();
				}
				Clear();
				canvas.SetActive(false);
				SetUISelection(null);
				if (this.ScreenClosedEvent != null)
				{
					this.ScreenClosedEvent();
				}
			}
		}

		private void Clear()
		{
			windowManager.CancelAll();
			lastUISelection = null;
			pendingInputMapping = null;
			pendingAxisCalibration = null;
		}

		private void ClearCompletely()
		{
			ClearSpawnedObjects();
			ClearAllVars();
		}

		private void ClearSpawnedObjects()
		{
			windowManager.ClearCompletely();
			inputGrid.ClearAll();
			foreach (GUIButton playerButton in playerButtons)
			{
				UnityEngine.Object.Destroy(playerButton.gameObject);
			}
			playerButtons.Clear();
			foreach (GUIButton mapCategoryButton in mapCategoryButtons)
			{
				UnityEngine.Object.Destroy(mapCategoryButton.gameObject);
			}
			mapCategoryButtons.Clear();
			foreach (GUIButton assignedControllerButton in assignedControllerButtons)
			{
				UnityEngine.Object.Destroy(assignedControllerButton.gameObject);
			}
			assignedControllerButtons.Clear();
			if (assignedControllerButtonsPlaceholder != null)
			{
				UnityEngine.Object.Destroy(assignedControllerButtonsPlaceholder.gameObject);
				assignedControllerButtonsPlaceholder = null;
			}
		}

		private void ClearVarsOnPlayerChange()
		{
			currentJoystickId = -1;
		}

		private void ClearVarsOnJoystickChange()
		{
			currentJoystickId = -1;
		}

		private void ClearAllVars()
		{
			initialized = false;
			Instance = null;
			playerCount = 0;
			inputGrid = null;
			windowManager = null;
			currentPlayerId = -1;
			currentMapCategoryId = -1;
			playerButtons = null;
			mapCategoryButtons = null;
			canvas = null;
			lastUISelection = null;
			currentJoystickId = -1;
			pendingInputMapping = null;
			pendingAxisCalibration = null;
			inputFieldActivatedDelegate = null;
			inputFieldInvertToggleStateChangedDelegate = null;
		}

		public void Reset()
		{
			if (initialized)
			{
				ClearCompletely();
				Initialize();
				if (isOpen)
				{
					Open(true);
				}
			}
		}

		private void SetActionAxisInverted(bool state, ControllerType controllerType, int actionElementMapId)
		{
			if (currentPlayer == null)
			{
				return;
			}
			ControllerMapWithAxes controllerMapWithAxes = GetControllerMap(controllerType) as ControllerMapWithAxes;
			if (controllerMapWithAxes != null)
			{
				ActionElementMap elementMap = controllerMapWithAxes.GetElementMap(actionElementMapId);
				if (elementMap != null)
				{
					elementMap.invert = state;
				}
			}
		}

		private ControllerMap GetControllerMap(ControllerType type)
		{
			if (currentPlayer == null)
			{
				return null;
			}
			int controllerId = 0;
			switch (type)
			{
			case ControllerType.Joystick:
				if (currentPlayer.controllers.joystickCount > 0)
				{
					controllerId = currentJoystick.id;
					break;
				}
				return null;
			default:
				throw new NotImplementedException();
			case ControllerType.Keyboard:
			case ControllerType.Mouse:
				break;
			}
			return currentPlayer.controllers.maps.GetFirstMapInCategory(type, controllerId, currentMapCategoryId);
		}

		private ControllerMap GetControllerMapOrCreateNew(ControllerType controllerType, int controllerId, int layoutId)
		{
			ControllerMap controllerMap = GetControllerMap(controllerType);
			if (controllerMap == null)
			{
				currentPlayer.controllers.maps.AddEmptyMap(controllerType, controllerId, currentMapCategoryId, layoutId);
				controllerMap = currentPlayer.controllers.maps.GetMap(controllerType, controllerId, currentMapCategoryId, layoutId);
			}
			return controllerMap;
		}

		private int CountIEnumerable<T>(IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				return 0;
			}
			IEnumerator<T> enumerator = enumerable.GetEnumerator();
			if (enumerator == null)
			{
				return 0;
			}
			int num = 0;
			while (enumerator.MoveNext())
			{
				num++;
			}
			return num;
		}

		private int GetDefaultMapCategoryId()
		{
			if (_mappingSets.Length == 0)
			{
				return 0;
			}
			for (int i = 0; i < _mappingSets.Length; i++)
			{
				if (ReInput.mapping.GetMapCategory(_mappingSets[i].mapCategoryId) != null)
				{
					return _mappingSets[i].mapCategoryId;
				}
			}
			return 0;
		}

		private void SubscribeFixedUISelectionEvents()
		{
			if (references.fixedSelectableUIElements == null)
			{
				return;
			}
			GameObject[] fixedSelectableUIElements = references.fixedSelectableUIElements;
			foreach (GameObject gameObject in fixedSelectableUIElements)
			{
				UIElementInfo component = UnityTools.GetComponent<UIElementInfo>(gameObject);
				if (!(component == null))
				{
					component.OnSelectedEvent += OnUIElementSelected;
				}
			}
		}

		private void SubscribeMenuControlInputEvents()
		{
			SubscribeRewiredInputEventAllPlayers(_screenToggleAction, OnScreenToggleActionPressed);
			SubscribeRewiredInputEventAllPlayers(_screenOpenAction, OnScreenOpenActionPressed);
			SubscribeRewiredInputEventAllPlayers(_screenCloseAction, OnScreenCloseActionPressed);
			SubscribeRewiredInputEventAllPlayers(_universalCancelAction, OnUniversalCancelActionPressed);
		}

		private void UnsubscribeMenuControlInputEvents()
		{
			UnsubscribeRewiredInputEventAllPlayers(_screenToggleAction, OnScreenToggleActionPressed);
			UnsubscribeRewiredInputEventAllPlayers(_screenOpenAction, OnScreenOpenActionPressed);
			UnsubscribeRewiredInputEventAllPlayers(_screenCloseAction, OnScreenCloseActionPressed);
			UnsubscribeRewiredInputEventAllPlayers(_universalCancelAction, OnUniversalCancelActionPressed);
		}

		private void SubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
		{
			if (actionId < 0 || callback == null)
			{
				return;
			}
			if (ReInput.mapping.GetAction(actionId) == null)
			{
				Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
				return;
			}
			foreach (Player allPlayer in ReInput.players.AllPlayers)
			{
				allPlayer.AddInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionId);
			}
		}

		private void UnsubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
		{
			if (actionId < 0 || callback == null || !ReInput.isReady)
			{
				return;
			}
			if (ReInput.mapping.GetAction(actionId) == null)
			{
				Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
				return;
			}
			foreach (Player allPlayer in ReInput.players.AllPlayers)
			{
				allPlayer.RemoveInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionId);
			}
		}

		private int GetMaxControllersPerPlayer()
		{
			if (_rewiredInputManager.userData.ConfigVars.autoAssignJoysticks)
			{
				return _rewiredInputManager.userData.ConfigVars.maxJoysticksPerPlayer;
			}
			return _maxControllersPerPlayer;
		}

		private bool ShowAssignedControllers()
		{
			if (!_showControllers)
			{
				return false;
			}
			if (_showAssignedControllers)
			{
				return true;
			}
			if (GetMaxControllersPerPlayer() != 1)
			{
				return true;
			}
			return false;
		}

		private void InspectorPropertyChanged()
		{
		}

		private void AssignController(Player player, int controllerId)
		{
			if (player == null || player.controllers.ContainsController(ControllerType.Joystick, controllerId))
			{
				return;
			}
			if (GetMaxControllersPerPlayer() == 1)
			{
				RemoveAllControllers(player);
				ClearVarsOnJoystickChange();
			}
			foreach (Player player2 in ReInput.players.Players)
			{
				if (player2 != player)
				{
					RemoveController(player2, controllerId);
				}
			}
			player.controllers.AddController(ControllerType.Joystick, controllerId, false);
			PlayerManager.ControllerRemapped((currentPlayerId != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne, true, controllerId);
			if (ReInput.userDataStore != null)
			{
				ReInput.userDataStore.LoadControllerData(player.id, ControllerType.Joystick, controllerId);
			}
		}

		private void RemoveAllControllers(Player player)
		{
			if (player != null)
			{
				IList<Joystick> joysticks = player.controllers.Joysticks;
				for (int num = joysticks.Count - 1; num >= 0; num--)
				{
					RemoveController(player, joysticks[num].id);
				}
			}
		}

		private void RemoveController(Player player, int controllerId)
		{
			if (player != null && player.controllers.ContainsController(ControllerType.Joystick, controllerId))
			{
				if (ReInput.userDataStore != null)
				{
					ReInput.userDataStore.SaveControllerData(player.id, ControllerType.Joystick, controllerId);
				}
				player.controllers.RemoveController(ControllerType.Joystick, controllerId);
				PlayerManager.ControllerRemapped((currentPlayerId != 0) ? PlayerId.PlayerTwo : PlayerId.PlayerOne, false, 0);
			}
		}

		public static void ApplyTheme(ThemedElement.ElementInfo[] elementInfo)
		{
			if (!(Instance == null) && !(Instance._themeSettings == null) && Instance._useThemeSettings)
			{
				Instance._themeSettings.Apply(elementInfo);
			}
		}

		public static LanguageData GetLanguage()
		{
			if (Instance == null)
			{
				return null;
			}
			return Instance._language;
		}
	}
}
