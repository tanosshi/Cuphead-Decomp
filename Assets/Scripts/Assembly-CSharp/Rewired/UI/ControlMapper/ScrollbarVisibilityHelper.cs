using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class ScrollbarVisibilityHelper : MonoBehaviour
	{
		public ScrollRect scrollRect;

		private bool onlySendMessage;

		private ScrollbarVisibilityHelper target;

		private Scrollbar hScrollBar
		{
			get
			{
				return (!(scrollRect != null)) ? null : scrollRect.horizontalScrollbar;
			}
		}

		private Scrollbar vScrollBar
		{
			get
			{
				return (!(scrollRect != null)) ? null : scrollRect.verticalScrollbar;
			}
		}

		private void Awake()
		{
			if (scrollRect != null)
			{
				target = scrollRect.gameObject.AddComponent<ScrollbarVisibilityHelper>();
				target.onlySendMessage = true;
				target.target = this;
			}
		}

		private void OnRectTransformDimensionsChange()
		{
			if (onlySendMessage)
			{
				if (target != null)
				{
					target.ScrollRectTransformDimensionsChanged();
				}
			}
			else
			{
				EvaluateScrollbar();
			}
		}

		private void ScrollRectTransformDimensionsChanged()
		{
			OnRectTransformDimensionsChange();
		}

		private void EvaluateScrollbar()
		{
			if (!(scrollRect == null) && (!(vScrollBar == null) || !(hScrollBar == null)) && base.gameObject.activeInHierarchy)
			{
				Rect rect = scrollRect.content.rect;
				Rect rect2 = (scrollRect.transform as RectTransform).rect;
				if (vScrollBar != null)
				{
					bool value = !(rect.height <= rect2.height);
					SetActiveDeferred(vScrollBar.gameObject, value);
				}
				if (hScrollBar != null)
				{
					bool value2 = !(rect.width <= rect2.width);
					SetActiveDeferred(hScrollBar.gameObject, value2);
				}
			}
		}

		private void SetActiveDeferred(GameObject obj, bool value)
		{
			StopAllCoroutines();
			StartCoroutine(SetActiveCoroutine(obj, value));
		}

		private IEnumerator SetActiveCoroutine(GameObject obj, bool value)
		{
			yield return null;
			if (obj != null)
			{
				obj.SetActive(value);
			}
		}
	}
}
