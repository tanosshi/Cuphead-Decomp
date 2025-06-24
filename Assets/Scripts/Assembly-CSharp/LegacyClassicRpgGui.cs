using System.Collections;
using UnityEngine;

public class LegacyClassicRpgGui : MonoBehaviour
{
	public GUISkin skin;

	public Texture2D diagonalLines;

	public AudioSource audioText;

	public AudioSource audioTextEnd;

	public AudioSource audioGood;

	public AudioSource audioBad;

	private bool _dialogue;

	private bool _ending;

	private bool _showDialogueBox;

	private bool _usingPositionRect;

	private Rect _positionRect = new Rect(0f, 0f, 0f, 0f);

	private string _windowTargetText = string.Empty;

	private string _windowCurrentText = string.Empty;

	private string _nameText = string.Empty;

	private bool _isBranchedText;

	private string[] _branchedTextChoices;

	private int _currentChoice;

	private string _theme;

	private float _windowTweenValue;

	private bool _windowReady;

	private float _nameTweenValue;

	private int _textFrames = int.MaxValue;

	private void Awake()
	{
		Dialoguer.Initialize();
	}

	private void Start()
	{
		addDialoguerEvents();
		_showDialogueBox = false;
	}

	private void Update()
	{
		if (!_dialogue)
		{
			return;
		}
		if (_windowReady)
		{
			calculateText();
		}
		if (!_dialogue || _ending)
		{
			return;
		}
		if (!_isBranchedText)
		{
			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
			{
				if (_windowCurrentText == _windowTargetText)
				{
					Dialoguer.ContinueDialogue(0);
					return;
				}
				_windowCurrentText = _windowTargetText;
				audioTextEnd.Play();
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			_currentChoice = (int)Mathf.Repeat(_currentChoice + 1, _branchedTextChoices.Length);
			audioText.Play();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			_currentChoice = (int)Mathf.Repeat(_currentChoice - 1, _branchedTextChoices.Length);
			audioText.Play();
		}
		if (Input.GetMouseButtonDown(0) && _windowCurrentText != _windowTargetText)
		{
			_windowCurrentText = _windowTargetText;
		}
		if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
		{
			if (_windowCurrentText == _windowTargetText)
			{
				Dialoguer.ContinueDialogue(_currentChoice);
				return;
			}
			_windowCurrentText = _windowTargetText;
			audioTextEnd.Play();
		}
	}

	public void addDialoguerEvents()
	{
		Dialoguer.events.onStarted += onDialogueStartedHandler;
		Dialoguer.events.onEnded += onDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded += onDialogueInstantlyEndedHandler;
		Dialoguer.events.onTextPhase += onDialogueTextPhaseHandler;
		Dialoguer.events.onWindowClose += onDialogueWindowCloseHandler;
		Dialoguer.events.onMessageEvent += onDialoguerMessageEvent;
	}

	private void onDialogueStartedHandler()
	{
		_dialogue = true;
	}

	private void onDialogueEndedHandler()
	{
		_ending = true;
		audioTextEnd.Play();
	}

	private void onDialogueInstantlyEndedHandler()
	{
		_dialogue = false;
		_showDialogueBox = false;
		resetWindowSize();
	}

	private void onDialogueTextPhaseHandler(DialoguerTextData data)
	{
		_usingPositionRect = data.usingPositionRect;
		_positionRect = data.rect;
		_windowCurrentText = string.Empty;
		_windowTargetText = data.text;
		_nameText = data.name;
		_showDialogueBox = true;
		_isBranchedText = data.windowType == DialoguerTextPhaseType.BranchedText;
		_branchedTextChoices = data.choices;
		_currentChoice = 0;
		if (data.theme != _theme)
		{
			resetWindowSize();
		}
		_theme = data.theme;
		startWindowTweenIn();
	}

	private void onDialogueWindowCloseHandler()
	{
		startWindowTweenOut();
	}

	private void onDialoguerMessageEvent(string message, string metadata)
	{
		if (message == "playOldRpgSound")
		{
			playOldRpgSound(metadata);
		}
	}

	private void OnGUI()
	{
		if (!_showDialogueBox)
		{
			return;
		}
		GUI.skin = skin;
		GUI.depth = 10;
		float x = (_usingPositionRect ? _positionRect.x : ((float)Screen.width * 0.5f));
		float y = (_usingPositionRect ? _positionRect.y : ((float)(Screen.height - 100)));
		float num = (_usingPositionRect ? _positionRect.width : 512f);
		float num2 = (_usingPositionRect ? _positionRect.height : 190f);
		Rect rect = centerRect(new Rect(x, y, num * _windowTweenValue, num2 * _windowTweenValue));
		rect.width = Mathf.Clamp(rect.width, 32f, 2000f);
		rect.height = Mathf.Clamp(rect.height, 32f, 2000f);
		if (_theme == "good")
		{
			drawDialogueBox(rect, new Color(0.2f, 0.8f, 0.4f));
		}
		else if (_theme == "bad")
		{
			drawDialogueBox(rect, new Color(0.8f, 0.2f, 0.2f));
		}
		else
		{
			drawDialogueBox(rect);
		}
		if (_nameText != string.Empty)
		{
			Rect rect2 = new Rect(rect.x, rect.y - 60f, 150f * _windowTweenValue, 50f * _windowTweenValue);
			rect2.width = Mathf.Clamp(rect2.width, 32f, 2000f);
			rect2.height = Mathf.Clamp(rect2.height, 32f, 2000f);
			drawDialogueBox(rect2);
			drawShadowedText(new Rect(rect2.x + 15f * _windowTweenValue - 5f * (1f - _windowTweenValue), rect2.y + 5f * _windowTweenValue - 10f * (1f - _windowTweenValue), rect2.width - 30f * _windowTweenValue, rect2.height - 5f * _windowTweenValue), _nameText);
		}
		Rect rect3 = new Rect(rect.x + 20f * _windowTweenValue, rect.y + 10f * _windowTweenValue, rect.width - 40f * _windowTweenValue, rect.height - 20f * _windowTweenValue);
		drawShadowedText(rect3, _windowCurrentText);
		if (!_isBranchedText || !(_windowCurrentText == _windowTargetText) || _branchedTextChoices == null)
		{
			return;
		}
		for (int i = 0; i < _branchedTextChoices.Length; i++)
		{
			float y2 = rect.yMax - (float)(38 * _branchedTextChoices.Length - 38 * i) - 20f;
			Rect rect4 = new Rect(rect.x + 60f, y2, rect.width - 80f, 38f);
			drawShadowedText(rect4, _branchedTextChoices[i]);
			if (rect4.Contains(new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y)))
			{
				if (_currentChoice != i)
				{
					audioText.Play();
					_currentChoice = i;
				}
				if (Input.GetMouseButtonDown(0))
				{
					Dialoguer.ContinueDialogue(_currentChoice);
					break;
				}
			}
			if (_currentChoice == i)
			{
				GUI.Box(new Rect(rect4.x - 64f, rect4.y, 64f, 64f), string.Empty, GUI.skin.GetStyle("box_cursor"));
			}
		}
	}

	private void drawDialogueBox(Rect rect)
	{
		drawDialogueBox(rect, new Color(0.1764706f, 37f / 85f, 1f));
	}

	private void drawDialogueBox(Rect rect, Color color)
	{
		GUI.color = color;
		GUI.Box(rect, string.Empty, GUI.skin.GetStyle("box_background"));
		GUI.color = GUI.contentColor;
		GUI.color = new Color(0f, 0f, 0f, 0.25f);
		Rect position = new Rect(rect.x + 7f, rect.y + 7f, rect.width - 14f, rect.height - 14f);
		GUI.DrawTextureWithTexCoords(position, diagonalLines, new Rect(0f, 0f, position.width / (float)diagonalLines.width, position.height / (float)diagonalLines.height));
		GUI.color = GUI.contentColor;
		GUI.depth = 20;
		GUI.Box(rect, string.Empty, GUI.skin.GetStyle("box_border"));
		GUI.depth = 10;
	}

	private void drawShadowedText(Rect rect, string text)
	{
		GUI.color = new Color(0f, 0f, 0f, 0.5f);
		GUI.Label(new Rect(rect.x + 1f, rect.y + 2f, rect.width, rect.height), text);
		GUI.color = GUI.contentColor;
		GUI.Label(rect, text);
	}

	private void playOldRpgSound(string metadata)
	{
		if (metadata == "good")
		{
			audioGood.Play();
		}
		else if (metadata == "bad")
		{
			audioBad.Play();
		}
	}

	private void resetWindowSize()
	{
		_windowTweenValue = 0f;
		_windowReady = false;
	}

	private void startWindowTweenIn()
	{
		_showDialogueBox = true;
		DialogueriTween.ValueTo(base.gameObject, new Hashtable
		{
			{ "from", _windowTweenValue },
			{ "to", 1 },
			{ "onupdatetarget", base.gameObject },
			{ "onupdate", "updateWindowTweenValue" },
			{ "oncompletetarget", base.gameObject },
			{ "oncomplete", "windowInComplete" },
			{ "time", 0.5f },
			{
				"easetype",
				DialogueriTween.EaseType.easeOutBack
			}
		});
	}

	private void startWindowTweenOut()
	{
		_windowReady = false;
		DialogueriTween.ValueTo(base.gameObject, new Hashtable
		{
			{ "from", _windowTweenValue },
			{ "to", 0 },
			{ "onupdatetarget", base.gameObject },
			{ "onupdate", "updateWindowTweenValue" },
			{ "oncompletetarget", base.gameObject },
			{ "oncomplete", "windowOutComplete" },
			{ "time", 0.5f },
			{
				"easetype",
				DialogueriTween.EaseType.easeInBack
			}
		});
	}

	private void updateWindowTweenValue(float newValue)
	{
		_windowTweenValue = newValue;
	}

	private void windowInComplete()
	{
		_windowReady = true;
	}

	private void windowOutComplete()
	{
		_showDialogueBox = false;
		resetWindowSize();
		if (_ending)
		{
			_dialogue = false;
			_ending = false;
		}
	}

	private Rect centerRect(Rect rect)
	{
		return new Rect(rect.x - rect.width * 0.5f, rect.y - rect.height * 0.5f, rect.width, rect.height);
	}

	private void calculateText()
	{
		if (_windowTargetText == string.Empty || _windowCurrentText == _windowTargetText)
		{
			return;
		}
		int num = 2;
		if (_textFrames < num)
		{
			_textFrames++;
			return;
		}
		_textFrames = 0;
		int num2 = 1;
		if (_windowCurrentText != _windowTargetText)
		{
			for (int i = 0; i < num2; i++)
			{
				if (_windowTargetText.Length <= _windowCurrentText.Length)
				{
					break;
				}
				_windowCurrentText += _windowTargetText[_windowCurrentText.Length];
			}
		}
		audioText.Play();
	}
}
