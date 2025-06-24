using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	[RequireComponent(typeof(CanvasGroup))]
	public class Window : MonoBehaviour
	{
		public class Timer
		{
			private bool _started;

			private float end;

			public bool started
			{
				get
				{
					return _started;
				}
			}

			public bool finished
			{
				get
				{
					if (!started)
					{
						return false;
					}
					if (Time.realtimeSinceStartup < end)
					{
						return false;
					}
					_started = false;
					return true;
				}
			}

			public float remaining
			{
				get
				{
					if (!_started)
					{
						return 0f;
					}
					return end - Time.realtimeSinceStartup;
				}
			}

			public void Start(float length)
			{
				end = Time.realtimeSinceStartup + length;
				_started = true;
			}

			public void Stop()
			{
				_started = false;
			}
		}

		public Image backgroundImage;

		public GameObject content;

		private bool _initialized;

		private int _id = -1;

		private RectTransform _rectTransform;

		private Text _titleText;

		private List<Text> _contentText;

		private GameObject _defaultUIElement;

		private Action<int> _updateCallback;

		private Func<int, bool> _isFocusedCallback;

		private Timer _timer;

		private CanvasGroup _canvasGroup;

		public UnityAction cancelCallback;

		public bool hasFocus
		{
			get
			{
				return _isFocusedCallback != null && _isFocusedCallback(_id);
			}
		}

		public int id
		{
			get
			{
				return _id;
			}
		}

		public RectTransform rectTransform
		{
			get
			{
				if (_rectTransform == null)
				{
					_rectTransform = base.gameObject.GetComponent<RectTransform>();
				}
				return _rectTransform;
			}
		}

		public Text titleText
		{
			get
			{
				return _titleText;
			}
		}

		public List<Text> contentText
		{
			get
			{
				return _contentText;
			}
		}

		public GameObject defaultUIElement
		{
			get
			{
				return _defaultUIElement;
			}
			set
			{
				_defaultUIElement = value;
			}
		}

		public Action<int> updateCallback
		{
			get
			{
				return _updateCallback;
			}
			set
			{
				_updateCallback = value;
			}
		}

		public Timer timer
		{
			get
			{
				return _timer;
			}
		}

		public int width
		{
			get
			{
				return (int)rectTransform.sizeDelta.x;
			}
			set
			{
				Vector2 sizeDelta = rectTransform.sizeDelta;
				sizeDelta.x = value;
				rectTransform.sizeDelta = sizeDelta;
			}
		}

		public int height
		{
			get
			{
				return (int)rectTransform.sizeDelta.y;
			}
			set
			{
				Vector2 sizeDelta = rectTransform.sizeDelta;
				sizeDelta.y = value;
				rectTransform.sizeDelta = sizeDelta;
			}
		}

		protected bool initialized
		{
			get
			{
				return _initialized;
			}
		}

		private void OnEnable()
		{
			StartCoroutine("OnEnableAsync");
		}

		protected virtual void Update()
		{
			if (_initialized && hasFocus && _updateCallback != null)
			{
				_updateCallback(_id);
			}
		}

		public virtual void Initialize(int id, Func<int, bool> isFocusedCallback)
		{
			if (_initialized)
			{
				Debug.LogError("Window is already initialized!");
				return;
			}
			_id = id;
			_isFocusedCallback = isFocusedCallback;
			_timer = new Timer();
			_contentText = new List<Text>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_initialized = true;
		}

		public void SetSize(int width, int height)
		{
			rectTransform.sizeDelta = new Vector2(width, height);
		}

		public void CreateTitleText(GameObject prefab, Vector2 offset)
		{
			CreateText(prefab, ref _titleText, "Title Text", UIPivot.TopCenter, UIAnchor.TopHStretch, offset);
		}

		public void CreateTitleText(GameObject prefab, Vector2 offset, string text)
		{
			CreateTitleText(prefab, offset);
			SetTitleText(text);
		}

		public void AddContentText(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset)
		{
			Text textComponent = null;
			CreateText(prefab, ref textComponent, "Content Text", pivot, anchor, offset);
			_contentText.Add(textComponent);
		}

		public void AddContentText(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string text)
		{
			AddContentText(prefab, pivot, anchor, offset);
			SetContentText(text, _contentText.Count - 1);
		}

		public void AddContentImage(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset)
		{
			CreateImage(prefab, "Image", pivot, anchor, offset);
		}

		public void AddContentImage(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string text)
		{
			AddContentImage(prefab, pivot, anchor, offset);
		}

		public void CreateButton(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string buttonText, UnityAction confirmCallback, UnityAction cancelCallback, bool setDefault)
		{
			if (prefab == null)
			{
				return;
			}
			ButtonInfo buttonInfo;
			GameObject gameObject = CreateButton(prefab, "Button", anchor, pivot, offset, out buttonInfo);
			if (!(gameObject == null))
			{
				Button component = gameObject.GetComponent<Button>();
				if (confirmCallback != null)
				{
					component.onClick.AddListener(confirmCallback);
				}
				CustomButton customButton = component as CustomButton;
				if (cancelCallback != null && customButton != null)
				{
					customButton.CancelEvent += cancelCallback;
				}
				if (buttonInfo.text != null)
				{
					buttonInfo.text.text = buttonText;
				}
				if (setDefault)
				{
					_defaultUIElement = gameObject;
				}
			}
		}

		public string GetTitleText(string text)
		{
			if (_titleText == null)
			{
				return string.Empty;
			}
			return _titleText.text;
		}

		public void SetTitleText(string text)
		{
			if (!(_titleText == null))
			{
				_titleText.text = text;
			}
		}

		public string GetContentText(int index)
		{
			if (_contentText == null || _contentText.Count <= index || _contentText[index] == null)
			{
				return string.Empty;
			}
			return _contentText[index].text;
		}

		public float GetContentTextHeight(int index)
		{
			if (_contentText == null || _contentText.Count <= index || _contentText[index] == null)
			{
				return 0f;
			}
			return _contentText[index].rectTransform.sizeDelta.y;
		}

		public void SetContentText(string text, int index)
		{
			if (_contentText != null && _contentText.Count > index && !(_contentText[index] == null))
			{
				_contentText[index].text = text;
			}
		}

		public void SetUpdateCallback(Action<int> callback)
		{
			updateCallback = callback;
		}

		public virtual void TakeInputFocus()
		{
			if (!(EventSystem.current == null))
			{
				EventSystem.current.SetSelectedGameObject(_defaultUIElement);
				Enable();
			}
		}

		public virtual void Enable()
		{
			_canvasGroup.interactable = true;
		}

		public virtual void Disable()
		{
			_canvasGroup.interactable = false;
		}

		public virtual void Cancel()
		{
			if (initialized && cancelCallback != null)
			{
				cancelCallback();
			}
		}

		private void CreateText(GameObject prefab, ref Text textComponent, string name, UIPivot pivot, UIAnchor anchor, Vector2 offset)
		{
			if (prefab == null || content == null)
			{
				return;
			}
			if (textComponent != null)
			{
				Debug.LogError("Window already has " + name + "!");
				return;
			}
			GameObject gameObject = UITools.InstantiateGUIObject<Text>(prefab, content.transform, name, pivot, anchor.min, anchor.max, offset);
			if (!(gameObject == null))
			{
				textComponent = gameObject.GetComponent<Text>();
			}
		}

		private void CreateImage(GameObject prefab, string name, UIPivot pivot, UIAnchor anchor, Vector2 offset)
		{
			if (!(prefab == null) && !(content == null))
			{
				UITools.InstantiateGUIObject<Image>(prefab, content.transform, name, pivot, anchor.min, anchor.max, offset);
			}
		}

		private GameObject CreateButton(GameObject prefab, string name, UIAnchor anchor, UIPivot pivot, Vector2 offset, out ButtonInfo buttonInfo)
		{
			buttonInfo = null;
			if (prefab == null)
			{
				return null;
			}
			GameObject gameObject = UITools.InstantiateGUIObject<ButtonInfo>(prefab, content.transform, name, pivot, anchor.min, anchor.max, offset);
			if (gameObject == null)
			{
				return null;
			}
			buttonInfo = gameObject.GetComponent<ButtonInfo>();
			Button component = gameObject.GetComponent<Button>();
			if (component == null)
			{
				Debug.Log("Button prefab is missing Button component!");
				return null;
			}
			if (buttonInfo == null)
			{
				Debug.Log("Button prefab is missing ButtonInfo component!");
				return null;
			}
			return gameObject;
		}

		private IEnumerator OnEnableAsync()
		{
			yield return 1;
			if (!(EventSystem.current == null))
			{
				if (defaultUIElement != null)
				{
					EventSystem.current.SetSelectedGameObject(defaultUIElement);
				}
				else
				{
					EventSystem.current.SetSelectedGameObject(null);
				}
			}
		}
	}
}
