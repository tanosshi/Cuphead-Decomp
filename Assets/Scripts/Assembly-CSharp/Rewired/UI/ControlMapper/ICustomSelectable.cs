using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Rewired.UI.ControlMapper
{
	public interface ICustomSelectable : ICancelHandler, IEventSystemHandler
	{
		Sprite disabledHighlightedSprite { get; set; }

		Color disabledHighlightedColor { get; set; }

		string disabledHighlightedTrigger { get; set; }

		bool autoNavUp { get; set; }

		bool autoNavDown { get; set; }

		bool autoNavLeft { get; set; }

		bool autoNavRight { get; set; }

		event UnityAction CancelEvent;
	}
}
