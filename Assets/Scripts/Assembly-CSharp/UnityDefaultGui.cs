using UnityEngine;

public class UnityDefaultGui : MonoBehaviour
{
	public const float HEIGHT = 200f;

	public const float WIDTH = 500f;

	private bool _showing;

	private bool _windowShowing;

	private bool _selectionClicked;

	private string _windowText = string.Empty;

	private string[] _choices;

	private Color _guiColor;

	private void Awake()
	{
		Dialoguer.Initialize();
	}

	private void Start()
	{
		addDialoguerEvents();
	}

	private void OnGUI()
	{
		if (!_showing || !_windowShowing)
		{
			return;
		}
		GUI.color = _guiColor;
		GUI.depth = 10;
		Rect rect = new Rect((float)Screen.width * 0.5f - 250f, (float)Screen.height - 200f - 100f, 500f, 200f);
		Rect position = new Rect(rect.x, rect.y, rect.width, rect.height - (float)(45 * _choices.Length));
		GUI.Box(position, string.Empty);
		GUI.color = GUI.contentColor;
		GUI.Label(new Rect(position.x + 10f, position.y + 10f, position.width - 20f, position.height - 20f), _windowText);
		if (_selectionClicked)
		{
			return;
		}
		for (int i = 0; i < _choices.Length; i++)
		{
			Rect position2 = new Rect(rect.x, rect.yMax - (float)(45 * (_choices.Length - i)) + 5f, rect.width, 40f);
			if (GUI.Button(position2, _choices[i]))
			{
				_selectionClicked = true;
				Dialoguer.ContinueDialogue(i);
			}
		}
		GUI.color = GUI.contentColor;
	}

	public void addDialoguerEvents()
	{
		Dialoguer.events.onStarted += onStartedHandler;
		Dialoguer.events.onEnded += onEndedHandler;
		Dialoguer.events.onInstantlyEnded += onInstantlyEndedHandler;
		Dialoguer.events.onTextPhase += onTextPhaseHandler;
		Dialoguer.events.onWindowClose += onWindowCloseHandler;
	}

	private void onStartedHandler()
	{
		_showing = true;
	}

	private void onEndedHandler()
	{
		_showing = false;
	}

	private void onInstantlyEndedHandler()
	{
		_showing = true;
		_windowShowing = false;
		_selectionClicked = false;
	}

	private void onTextPhaseHandler(DialoguerTextData data)
	{
		_guiColor = GUI.contentColor;
		_windowText = data.text;
		if (data.windowType == DialoguerTextPhaseType.Text)
		{
			_choices = new string[1] { "Continue" };
		}
		else
		{
			_choices = data.choices;
		}
		switch (data.theme)
		{
		case "bad":
			_guiColor = Color.red;
			break;
		case "good":
			_guiColor = Color.green;
			break;
		default:
			_guiColor = GUI.contentColor;
			break;
		}
		_windowShowing = true;
		_selectionClicked = false;
	}

	private void onWindowCloseHandler()
	{
		_windowShowing = false;
		_selectionClicked = false;
	}
}
