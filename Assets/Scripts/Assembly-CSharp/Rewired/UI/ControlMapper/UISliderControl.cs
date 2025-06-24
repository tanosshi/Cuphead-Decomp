using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class UISliderControl : UIControl
	{
		public Image iconImage;

		public Slider slider;

		private bool _showIcon;

		private bool _showSlider;

		public bool showIcon
		{
			get
			{
				return _showIcon;
			}
			set
			{
				if (!(iconImage == null))
				{
					iconImage.gameObject.SetActive(value);
					_showIcon = value;
				}
			}
		}

		public bool showSlider
		{
			get
			{
				return _showSlider;
			}
			set
			{
				if (!(slider == null))
				{
					slider.gameObject.SetActive(value);
					_showSlider = value;
				}
			}
		}

		public override void SetCancelCallback(Action cancelCallback)
		{
			base.SetCancelCallback(cancelCallback);
			if (cancelCallback == null || slider == null)
			{
				return;
			}
			if (slider is ICustomSelectable)
			{
				(slider as ICustomSelectable).CancelEvent += delegate
				{
					cancelCallback();
				};
				return;
			}
			EventTrigger eventTrigger = slider.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = slider.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.callback = new EventTrigger.TriggerEvent();
			entry.eventID = EventTriggerType.Cancel;
			entry.callback.AddListener(delegate
			{
				cancelCallback();
			});
			if (eventTrigger.triggers == null)
			{
				eventTrigger.triggers = new List<EventTrigger.Entry>();
			}
			eventTrigger.triggers.Add(entry);
		}
	}
}
