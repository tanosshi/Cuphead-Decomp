using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class InputBehaviorWindow : Window
	{
		private class InputBehaviorInfo
		{
			private InputBehavior _inputBehavior;

			private UIControlSet _controlSet;

			private Dictionary<int, PropertyType> idToProperty;

			private InputBehavior copyOfOriginal;

			public InputBehavior inputBehavior
			{
				get
				{
					return _inputBehavior;
				}
			}

			public UIControlSet controlSet
			{
				get
				{
					return _controlSet;
				}
			}

			public InputBehaviorInfo(InputBehavior inputBehavior, UIControlSet controlSet, Dictionary<int, PropertyType> idToProperty)
			{
				_inputBehavior = inputBehavior;
				_controlSet = controlSet;
				this.idToProperty = idToProperty;
				copyOfOriginal = new InputBehavior(inputBehavior);
			}

			public void RestorePreviousData()
			{
				_inputBehavior.ImportData(copyOfOriginal);
			}

			public void RestoreDefaultData()
			{
				_inputBehavior.Reset();
				RefreshControls();
			}

			public void RestoreData(PropertyType propertyType, int controlId)
			{
				switch (propertyType)
				{
				case PropertyType.JoystickAxisSensitivity:
				{
					float joystickAxisSensitivity = copyOfOriginal.joystickAxisSensitivity;
					_inputBehavior.joystickAxisSensitivity = joystickAxisSensitivity;
					UISliderControl control2 = _controlSet.GetControl<UISliderControl>(controlId);
					if (control2 != null)
					{
						control2.slider.value = joystickAxisSensitivity;
					}
					break;
				}
				case PropertyType.MouseXYAxisSensitivity:
				{
					float mouseXYAxisSensitivity = copyOfOriginal.mouseXYAxisSensitivity;
					_inputBehavior.mouseXYAxisSensitivity = mouseXYAxisSensitivity;
					UISliderControl control = _controlSet.GetControl<UISliderControl>(controlId);
					if (control != null)
					{
						control.slider.value = mouseXYAxisSensitivity;
					}
					break;
				}
				}
			}

			public void RefreshControls()
			{
				if (_controlSet == null || idToProperty == null)
				{
					return;
				}
				foreach (KeyValuePair<int, PropertyType> item in idToProperty)
				{
					UISliderControl control = _controlSet.GetControl<UISliderControl>(item.Key);
					if (!(control == null))
					{
						switch (item.Value)
						{
						case PropertyType.JoystickAxisSensitivity:
							control.slider.value = _inputBehavior.joystickAxisSensitivity;
							break;
						case PropertyType.MouseXYAxisSensitivity:
							control.slider.value = _inputBehavior.mouseXYAxisSensitivity;
							break;
						}
					}
				}
			}
		}

		public enum ButtonIdentifier
		{
			Done = 0,
			Cancel = 1,
			Default = 2
		}

		private enum PropertyType
		{
			JoystickAxisSensitivity = 0,
			MouseXYAxisSensitivity = 1
		}

		private const float minSensitivity = 0.1f;

		[SerializeField]
		private RectTransform spawnTransform;

		[SerializeField]
		private Button doneButton;

		[SerializeField]
		private Button cancelButton;

		[SerializeField]
		private Button defaultButton;

		[SerializeField]
		private Text doneButtonLabel;

		[SerializeField]
		private Text cancelButtonLabel;

		[SerializeField]
		private Text defaultButtonLabel;

		[SerializeField]
		private GameObject uiControlSetPrefab;

		[SerializeField]
		private GameObject uiSliderControlPrefab;

		private List<InputBehaviorInfo> inputBehaviorInfo;

		private Dictionary<int, Action<int>> buttonCallbacks;

		private int playerId;

		public override void Initialize(int id, Func<int, bool> isFocusedCallback)
		{
			if (spawnTransform == null || doneButton == null || cancelButton == null || defaultButton == null || uiControlSetPrefab == null || uiSliderControlPrefab == null || doneButtonLabel == null || cancelButtonLabel == null || defaultButtonLabel == null)
			{
				Debug.LogError("Rewired Control Mapper: All inspector values must be assigned!");
				return;
			}
			inputBehaviorInfo = new List<InputBehaviorInfo>();
			buttonCallbacks = new Dictionary<int, Action<int>>();
			doneButtonLabel.text = ControlMapper.GetLanguage().done;
			cancelButtonLabel.text = ControlMapper.GetLanguage().cancel;
			defaultButtonLabel.text = ControlMapper.GetLanguage().default_;
			base.Initialize(id, isFocusedCallback);
		}

		public void SetData(int playerId, ControlMapper.InputBehaviorSettings[] data)
		{
			if (!base.initialized)
			{
				return;
			}
			this.playerId = playerId;
			foreach (ControlMapper.InputBehaviorSettings inputBehaviorSettings in data)
			{
				if (inputBehaviorSettings == null || !inputBehaviorSettings.isValid)
				{
					continue;
				}
				InputBehavior inputBehavior = GetInputBehavior(inputBehaviorSettings.inputBehaviorId);
				if (inputBehavior != null)
				{
					UIControlSet uIControlSet = CreateControlSet();
					Dictionary<int, PropertyType> dictionary = new Dictionary<int, PropertyType>();
					string customEntry = ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.labelLanguageKey);
					if (!string.IsNullOrEmpty(customEntry))
					{
						uIControlSet.SetTitle(customEntry);
					}
					else
					{
						uIControlSet.SetTitle(inputBehavior.name);
					}
					if (inputBehaviorSettings.showJoystickAxisSensitivity)
					{
						UISliderControl uISliderControl = CreateSlider(uIControlSet, inputBehavior.id, null, ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.joystickAxisSensitivityLabelLanguageKey), inputBehaviorSettings.joystickAxisSensitivityIcon, inputBehaviorSettings.joystickAxisSensitivityMin, inputBehaviorSettings.joystickAxisSensitivityMax, JoystickAxisSensitivityValueChanged, JoystickAxisSensitivityCanceled);
						uISliderControl.slider.value = Mathf.Clamp(inputBehavior.joystickAxisSensitivity, inputBehaviorSettings.joystickAxisSensitivityMin, inputBehaviorSettings.joystickAxisSensitivityMax);
						dictionary.Add(uISliderControl.id, PropertyType.JoystickAxisSensitivity);
					}
					if (inputBehaviorSettings.showMouseXYAxisSensitivity)
					{
						UISliderControl uISliderControl2 = CreateSlider(uIControlSet, inputBehavior.id, null, ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.mouseXYAxisSensitivityLabelLanguageKey), inputBehaviorSettings.mouseXYAxisSensitivityIcon, inputBehaviorSettings.mouseXYAxisSensitivityMin, inputBehaviorSettings.mouseXYAxisSensitivityMax, MouseXYAxisSensitivityValueChanged, MouseXYAxisSensitivityCanceled);
						uISliderControl2.slider.value = Mathf.Clamp(inputBehavior.mouseXYAxisSensitivity, inputBehaviorSettings.mouseXYAxisSensitivityMin, inputBehaviorSettings.mouseXYAxisSensitivityMax);
						dictionary.Add(uISliderControl2.id, PropertyType.MouseXYAxisSensitivity);
					}
					inputBehaviorInfo.Add(new InputBehaviorInfo(inputBehavior, uIControlSet, dictionary));
				}
			}
			base.defaultUIElement = doneButton.gameObject;
		}

		public void SetButtonCallback(ButtonIdentifier buttonIdentifier, Action<int> callback)
		{
			if (base.initialized && callback != null)
			{
				if (buttonCallbacks.ContainsKey((int)buttonIdentifier))
				{
					buttonCallbacks[(int)buttonIdentifier] = callback;
				}
				else
				{
					buttonCallbacks.Add((int)buttonIdentifier, callback);
				}
			}
		}

		public override void Cancel()
		{
			if (!base.initialized)
			{
				return;
			}
			foreach (InputBehaviorInfo item in inputBehaviorInfo)
			{
				item.RestorePreviousData();
			}
			Action<int> value;
			if (!buttonCallbacks.TryGetValue(1, out value))
			{
				if (cancelCallback != null)
				{
					cancelCallback();
				}
			}
			else
			{
				value(base.id);
			}
		}

		public void OnDone()
		{
			Action<int> value;
			if (base.initialized && buttonCallbacks.TryGetValue(0, out value))
			{
				value(base.id);
			}
		}

		public void OnCancel()
		{
			Cancel();
		}

		public void OnRestoreDefault()
		{
			if (!base.initialized)
			{
				return;
			}
			foreach (InputBehaviorInfo item in inputBehaviorInfo)
			{
				item.RestoreDefaultData();
			}
		}

		private void JoystickAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value)
		{
			GetInputBehavior(inputBehaviorId).joystickAxisSensitivity = value;
		}

		private void MouseXYAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value)
		{
			GetInputBehavior(inputBehaviorId).mouseXYAxisSensitivity = value;
		}

		private void JoystickAxisSensitivityCanceled(int inputBehaviorId, int controlId)
		{
			InputBehaviorInfo inputBehaviorInfo = GetInputBehaviorInfo(inputBehaviorId);
			if (inputBehaviorInfo != null)
			{
				inputBehaviorInfo.RestoreData(PropertyType.JoystickAxisSensitivity, controlId);
			}
		}

		private void MouseXYAxisSensitivityCanceled(int inputBehaviorId, int controlId)
		{
			InputBehaviorInfo inputBehaviorInfo = GetInputBehaviorInfo(inputBehaviorId);
			if (inputBehaviorInfo != null)
			{
				inputBehaviorInfo.RestoreData(PropertyType.MouseXYAxisSensitivity, controlId);
			}
		}

		public override void TakeInputFocus()
		{
			base.TakeInputFocus();
		}

		private UIControlSet CreateControlSet()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(uiControlSetPrefab);
			gameObject.transform.SetParent(spawnTransform, false);
			return gameObject.GetComponent<UIControlSet>();
		}

		private UISliderControl CreateSlider(UIControlSet set, int inputBehaviorId, string defaultTitle, string overrideTitle, Sprite icon, float minValue, float maxValue, Action<int, int, float> valueChangedCallback, Action<int, int> cancelCallback)
		{
			UISliderControl uISliderControl = set.CreateSlider(uiSliderControlPrefab, icon, minValue, maxValue, delegate(int cId, float value)
			{
				valueChangedCallback(inputBehaviorId, cId, value);
			}, delegate(int cId)
			{
				cancelCallback(inputBehaviorId, cId);
			});
			string text = ((!string.IsNullOrEmpty(overrideTitle)) ? overrideTitle : defaultTitle);
			if (!string.IsNullOrEmpty(text))
			{
				uISliderControl.showTitle = true;
				uISliderControl.title.text = text;
			}
			else
			{
				uISliderControl.showTitle = false;
			}
			uISliderControl.showIcon = icon != null;
			return uISliderControl;
		}

		private InputBehavior GetInputBehavior(int id)
		{
			return ReInput.mapping.GetInputBehavior(playerId, id);
		}

		private InputBehaviorInfo GetInputBehaviorInfo(int inputBehaviorId)
		{
			int count = inputBehaviorInfo.Count;
			for (int i = 0; i < count; i++)
			{
				if (inputBehaviorInfo[i].inputBehavior.id == inputBehaviorId)
				{
					return inputBehaviorInfo[i];
				}
			}
			return null;
		}
	}
}
