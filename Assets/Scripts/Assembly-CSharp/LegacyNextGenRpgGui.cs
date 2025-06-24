using UnityEngine;

public class LegacyNextGenRpgGui : MonoBehaviour
{
	private static int TEXT_OUTLINE_WIDTH = 1;

	public GUISkin guiSkin;

	public AudioSource audioChoice;

	public AudioSource audioSelect;

	public Texture ringBase;

	public Texture ringTop;

	public Texture ringBottom;

	public NextGenRingPieces ringNormal;

	public NextGenRingPieces ringHover;

	private int _currentChoice;

	private Rect[] _ringeRects;

	private Rect[] _choicesTextRects;

	private bool _dialogue;

	private bool _showWindow;

	private string _text;

	private string[] _choices;

	private void Awake()
	{
		Dialoguer.Initialize();
	}

	private void Start()
	{
		addDialoguerEvents();
		_dialogue = false;
	}

	private void Update()
	{
		if (_showWindow && Input.GetMouseButtonDown(0))
		{
			if (_choices != null)
			{
				audioSelect.Play();
			}
			Dialoguer.ContinueDialogue(_currentChoice);
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
		_dialogue = false;
		_showWindow = false;
	}

	private void onDialogueInstantlyEndedHandler()
	{
		_dialogue = false;
		_showWindow = false;
	}

	private void onDialogueTextPhaseHandler(DialoguerTextData data)
	{
		_currentChoice = 0;
		if (data.choices != null)
		{
			_choices = new string[6];
			for (int i = 0; i < 6; i++)
			{
				if (data.choices.Length > i && data.choices[i] != null)
				{
					_choices[i] = data.choices[i];
					_currentChoice = i;
				}
			}
		}
		else
		{
			_choices = null;
		}
		_text = data.text;
		if (data.name != null && data.name != string.Empty)
		{
			_text = data.name + ": " + _text;
		}
		_showWindow = true;
	}

	private void onDialogueWindowCloseHandler()
	{
		_showWindow = false;
	}

	private void onDialoguerMessageEvent(string message, string metadata)
	{
	}

	private void OnGUI()
	{
		if (_dialogue && _showWindow)
		{
			GUI.skin = guiSkin;
			int num = 260;
			Rect rect = new Rect((float)Screen.width * 0.5f - 300f, Screen.height - num, 600f, 80f);
			GUIStyle gUIStyle = new GUIStyle("label");
			gUIStyle.alignment = TextAnchor.MiddleCenter;
			drawText(_text, rect, gUIStyle);
			if (_choices != null)
			{
				drawChoiceRing();
			}
		}
	}

	private void drawText(string text, Rect rect)
	{
		GUIStyle style = new GUIStyle("label");
		drawText(text, rect, style);
	}

	private void drawText(string text, Rect rect, GUIStyle style)
	{
		GUI.color = Color.black;
		for (int i = 0; i < TEXT_OUTLINE_WIDTH; i++)
		{
			for (int j = 0; j < TEXT_OUTLINE_WIDTH; j++)
			{
				GUI.Label(new Rect(rect.x + (float)(i + 1), rect.y + (float)(j + 1), rect.width, rect.height), text, style);
				GUI.Label(new Rect(rect.x - (float)(i + 1), rect.y - (float)(j + 1), rect.width, rect.height), text, style);
				GUI.Label(new Rect(rect.x + (float)(i + 1), rect.y - (float)(j + 1), rect.width, rect.height), text, style);
				GUI.Label(new Rect(rect.x - (float)(i + 1), rect.y + (float)(j + 1), rect.width, rect.height), text, style);
			}
		}
		GUI.color = GUI.contentColor;
		GUI.Label(rect, text, style);
	}

	private void drawChoiceRing()
	{
		Rect position = new Rect((float)Screen.width * 0.5f - 128f, Screen.height - 128 - 50, 256f, 128f);
		if (_ringeRects == null)
		{
			_ringeRects = new Rect[6];
			_ringeRects[0] = new Rect(position.center.x, position.y - 40f, (float)Screen.width * 0.5f, position.height * 0.3333f + 40f);
			_ringeRects[1] = new Rect(position.center.x, position.y + position.height * 0.3333f, (float)Screen.width * 0.5f, position.height * 0.3333f);
			_ringeRects[2] = new Rect(position.center.x, position.y + position.height * 0.3333f * 2f, (float)Screen.width * 0.5f, position.height * 0.3333f + 40f);
			_ringeRects[3] = new Rect(0f, position.y - 40f, (float)Screen.width * 0.5f, position.height * 0.3333f + 40f);
			_ringeRects[4] = new Rect(0f, position.y + position.height * 0.3333f, (float)Screen.width * 0.5f, position.height * 0.3333f);
			_ringeRects[5] = new Rect(0f, position.y + position.height * 0.3333f * 2f, (float)Screen.width * 0.5f, position.height * 0.3333f + 40f);
		}
		if (_choicesTextRects == null)
		{
			_choicesTextRects = new Rect[6];
			_choicesTextRects[0] = new Rect(position.center.x + position.width * 0.5f - 10f, position.y, (float)Screen.width * 0.5f - position.width * 0.5f + 10f, position.height * 0.3333f);
			_choicesTextRects[1] = new Rect(position.center.x + position.width * 0.5f + 10f, position.y + position.height * 0.3333f - 5f, (float)Screen.width * 0.5f - position.width * 0.5f - 10f, position.height * 0.3333f);
			_choicesTextRects[2] = new Rect(position.center.x + position.width * 0.5f, position.y + position.height * 0.3333f * 2f, (float)Screen.width * 0.5f - position.width * 0.5f, position.height * 0.3333f);
			_choicesTextRects[3] = new Rect(0f, position.y, (float)Screen.width * 0.5f - position.width * 0.5f + 10f, position.height * 0.3333f);
			_choicesTextRects[4] = new Rect(0f, position.y + position.height * 0.3333f - 5f, (float)Screen.width * 0.5f - position.width * 0.5f - 10f, position.height * 0.3333f);
			_choicesTextRects[5] = new Rect(0f, position.y + position.height * 0.3333f * 2f, (float)Screen.width * 0.5f - position.width * 0.5f, position.height * 0.3333f);
		}
		GUI.DrawTexture(position, ringBase);
		for (int i = 0; i < 6; i++)
		{
			if (_choices[i] != null && _choices[i] != string.Empty)
			{
				if (_currentChoice != i && _ringeRects[i].Contains(new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y)))
				{
					_currentChoice = i;
					audioChoice.PlayOneShot(audioChoice.clip);
				}
				if (_currentChoice == i)
				{
					GUI.DrawTexture(position, ringHover.getPieces()[i]);
				}
				else
				{
					GUI.DrawTexture(position, ringNormal.getPieces()[i]);
				}
				GUIStyle gUIStyle = new GUIStyle("label");
				if (i > 2)
				{
					gUIStyle.alignment = TextAnchor.MiddleRight;
				}
				else
				{
					gUIStyle.alignment = TextAnchor.MiddleLeft;
				}
				drawText(_choices[i], _choicesTextRects[i], gUIStyle);
			}
		}
	}
}
