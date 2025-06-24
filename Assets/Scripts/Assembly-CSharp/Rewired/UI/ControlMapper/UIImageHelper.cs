using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	[RequireComponent(typeof(Image))]
	public class UIImageHelper : MonoBehaviour
	{
		[Serializable]
		private class State
		{
			[SerializeField]
			public Color color;

			public void Set(Image image)
			{
				if (!(image == null))
				{
					image.color = color;
				}
			}
		}

		[SerializeField]
		private State enabledState;

		[SerializeField]
		private State disabledState;

		private bool currentState;

		public void SetEnabledState(bool newState)
		{
			currentState = newState;
			State state = ((!newState) ? disabledState : enabledState);
			if (state != null)
			{
				Image component = base.gameObject.GetComponent<Image>();
				if (component == null)
				{
					Debug.LogError("Image is missing!");
				}
				else
				{
					state.Set(component);
				}
			}
		}

		public void SetEnabledStateColor(Color color)
		{
			enabledState.color = color;
		}

		public void SetDisabledStateColor(Color color)
		{
			disabledState.color = color;
		}

		public void Refresh()
		{
			State state = ((!currentState) ? disabledState : enabledState);
			Image component = base.gameObject.GetComponent<Image>();
			if (!(component == null))
			{
				state.Set(component);
			}
		}
	}
}
