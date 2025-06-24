using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class UIControl : MonoBehaviour
	{
		public Text title;

		private int _id;

		private bool _showTitle;

		private static int _uidCounter;

		public int id
		{
			get
			{
				return _id;
			}
		}

		public bool showTitle
		{
			get
			{
				return _showTitle;
			}
			set
			{
				if (!(title == null))
				{
					title.gameObject.SetActive(value);
					_showTitle = value;
				}
			}
		}

		private void Awake()
		{
			_id = GetNextUid();
		}

		public virtual void SetCancelCallback(Action cancelCallback)
		{
		}

		private static int GetNextUid()
		{
			if (_uidCounter == int.MaxValue)
			{
				_uidCounter = 0;
			}
			int uidCounter = _uidCounter;
			_uidCounter++;
			return uidCounter;
		}
	}
}
