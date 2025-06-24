using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class InputRow : MonoBehaviour
	{
		public Text label;

		private int rowIndex;

		private Action<int, ButtonInfo> inputFieldActivatedCallback;

		public ButtonInfo[] buttons { get; private set; }

		public void Initialize(int rowIndex, string label, Action<int, ButtonInfo> inputFieldActivatedCallback)
		{
			this.rowIndex = rowIndex;
			this.label.text = label;
			this.inputFieldActivatedCallback = inputFieldActivatedCallback;
			buttons = base.transform.GetComponentsInChildren<ButtonInfo>(true);
		}

		public void OnButtonActivated(ButtonInfo buttonInfo)
		{
			if (inputFieldActivatedCallback != null)
			{
				inputFieldActivatedCallback(rowIndex, buttonInfo);
			}
		}
	}
}
