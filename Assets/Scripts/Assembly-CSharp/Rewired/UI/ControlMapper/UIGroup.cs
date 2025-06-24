using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class UIGroup : MonoBehaviour
	{
		[SerializeField]
		private Text _label;

		[SerializeField]
		private Transform _content;

		public string labelText
		{
			get
			{
				return (!(_label != null)) ? string.Empty : _label.text;
			}
			set
			{
				if (!(_label == null))
				{
					_label.text = value;
				}
			}
		}

		public Transform content
		{
			get
			{
				return _content;
			}
		}

		public void SetLabelActive(bool state)
		{
			if (!(_label == null))
			{
				_label.gameObject.SetActive(state);
			}
		}
	}
}
