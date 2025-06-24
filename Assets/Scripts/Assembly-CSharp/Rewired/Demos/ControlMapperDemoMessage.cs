using System.Collections;
using Rewired.UI.ControlMapper;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class ControlMapperDemoMessage : MonoBehaviour
	{
		public ControlMapper controlMapper;

		public Selectable defaultSelectable;

		private void Awake()
		{
			if (controlMapper != null)
			{
				controlMapper.ScreenClosedEvent += OnControlMapperClosed;
				controlMapper.ScreenOpenedEvent += OnControlMapperOpened;
			}
		}

		private void Start()
		{
			SelectDefault();
		}

		private void OnControlMapperClosed()
		{
			base.gameObject.SetActive(true);
			StartCoroutine(SelectDefaultDeferred());
		}

		private void OnControlMapperOpened()
		{
			base.gameObject.SetActive(false);
		}

		private void SelectDefault()
		{
			if (!(EventSystem.current == null) && defaultSelectable != null)
			{
				EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);
			}
		}

		private IEnumerator SelectDefaultDeferred()
		{
			yield return null;
			SelectDefault();
		}
	}
}
