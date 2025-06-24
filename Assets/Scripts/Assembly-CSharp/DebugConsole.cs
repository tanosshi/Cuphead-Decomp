using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugConsole : AbstractMonoBehaviour
{
	private enum AnimationType
	{
		Alpha = 0,
		Tween = 1
	}

	private enum State
	{
		Hidden = 0,
		Visible = 1,
		Animating = 2
	}

	public class Arguments
	{
		public class Value
		{
			public readonly DebugConsoleData.Command.Argument.Type type;

			public readonly int intValue;

			public readonly float floatValue;

			public readonly bool boolValue;

			public readonly string stringValue;

			private Value(int intV, float floatV, bool boolV, string stringV, DebugConsoleData.Command.Argument.Type type)
			{
				this.type = type;
				intValue = intV;
				floatValue = floatV;
				boolValue = boolV;
				stringValue = stringV;
			}

			public static Value NewInt(int value)
			{
				return new Value(value, 0f, false, string.Empty, DebugConsoleData.Command.Argument.Type.Int);
			}

			public static Value NewFloat(float value)
			{
				return new Value(0, value, false, string.Empty, DebugConsoleData.Command.Argument.Type.Float);
			}

			public static Value NewBool(bool value)
			{
				return new Value(0, 0f, value, string.Empty, DebugConsoleData.Command.Argument.Type.Bool);
			}

			public static Value NewString(string value)
			{
				return new Value(0, 0f, false, value, DebugConsoleData.Command.Argument.Type.String);
			}
		}

		private static bool DEBUG;

		public readonly List<Value> values;

		public Arguments(DebugConsoleData.Command.Argument.Type[] types)
		{
			values = new List<Value>();
			for (int i = 0; i < types.Length; i++)
			{
				switch (types[i])
				{
				case DebugConsoleData.Command.Argument.Type.Int:
					values.Add(Value.NewInt(0));
					break;
				case DebugConsoleData.Command.Argument.Type.Float:
					values.Add(Value.NewFloat(0f));
					break;
				case DebugConsoleData.Command.Argument.Type.Bool:
					values.Add(Value.NewBool(false));
					break;
				case DebugConsoleData.Command.Argument.Type.String:
					values.Add(Value.NewString(string.Empty));
					break;
				}
			}
		}

		public Arguments(List<string> strings)
		{
			values = new List<Value>();
			for (int i = 0; i < strings.Count; i++)
			{
				int result = 0;
				bool flag = int.TryParse(strings[i], out result);
				float result2 = 0f;
				bool flag2 = float.TryParse(strings[i], out result2);
				bool result3 = false;
				if (bool.TryParse(strings[i], out result3))
				{
					if (DEBUG)
					{
						Debug.Log("BOOL: " + result3);
					}
					values.Add(Value.NewBool(result3));
				}
				else if (flag)
				{
					if (DEBUG)
					{
						Debug.Log("INT: " + result);
					}
					values.Add(Value.NewInt(result));
				}
				else if (flag2)
				{
					if (DEBUG)
					{
						Debug.Log("FLOAT: " + result2);
					}
					values.Add(Value.NewFloat(result2));
				}
				else
				{
					if (DEBUG)
					{
						Debug.Log("STRING: " + strings[i]);
					}
					values.Add(Value.NewString(strings[i]));
				}
			}
		}
	}

	private const string PATH = "TC_DebugConsole/DebugConsole";

	private const KeyCode TOGGLE_KEY = KeyCode.BackQuote;

	private const KeyCode FOCUS_KEY = KeyCode.Tab;

	private const float IN_TIME = 0.2f;

	private const float OUT_TIME = 0.2f;

	private const EaseUtils.EaseType EASE = EaseUtils.EaseType.easeOutCubic;

	private const string START_TEXT = "DEBUG CONSOLE\n--------------\n<color=\"#FFFFFF88\">type 'help' to view available commands</color>";

	private const string LINE = "--------------";

	private const string HELP_COMMAND = "help";

	private const string LOVE_COMMAND = "love";

	private static AnimationType ANIMATION_TYPE = AnimationType.Tween;

	private static DebugConsole _instance;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private CanvasGroup panel;

	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private Text body;

	[SerializeField]
	private ScrollRect scrollRect;

	private State state;

	private DebugConsoleData data;

	public static bool IsVisible
	{
		get
		{
			Init();
			return _instance.state != State.Hidden;
		}
	}

	public static void Init()
	{
		if (!(_instance != null))
		{
			_instance = (Object.Instantiate(Resources.Load("TC_DebugConsole/DebugConsole")) as GameObject).GetComponent<DebugConsole>();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (_instance == null)
		{
			_instance = this;
			base.gameObject.name = base.gameObject.name.Replace("(Clone)", string.Empty);
			Object.DontDestroyOnLoad(base.gameObject);
			CupheadEventSystem.Init();
			body.text = "DEBUG CONSOLE\n--------------\n<color=\"#FFFFFF88\">type 'help' to view available commands</color>";
			data = Resources.Load<DebugConsoleData>(DebugConsoleData.PATH);
			HideImmediate();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (Debug.isDebugBuild || Application.isEditor)
		{
			UpdateConsole();
		}
	}

	private void UpdateConsole()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			switch (state)
			{
			case State.Visible:
				Hide();
				break;
			case State.Hidden:
				Show();
				break;
			}
			DelayedCleanInput();
		}
		if (state == State.Visible)
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				DelayedFocusInput();
			}
			if (Input.GetKey(KeyCode.UpArrow))
			{
				float num = body.text.Split('\n').Length;
				scrollRect.verticalNormalizedPosition += 1f / num;
			}
			if (Input.GetKey(KeyCode.DownArrow))
			{
				float num2 = body.text.Split('\n').Length;
				scrollRect.verticalNormalizedPosition -= 1f / num2;
			}
			scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition, 0f, 1f);
			return;
		}
		for (int i = 0; i < data.commands.Count; i++)
		{
			bool flag = Input.GetKeyDown(data.commands[i].key);
			if (data.commands[i].rewiredAction != string.Empty)
			{
				Player playerInput = PlayerManager.GetPlayerInput(PlayerId.PlayerOne);
				Player playerInput2 = PlayerManager.GetPlayerInput(PlayerId.PlayerTwo);
				flag |= playerInput.GetButtonDown(data.commands[i].rewiredAction) || playerInput2.GetButtonDown(data.commands[i].rewiredAction);
			}
			if (flag)
			{
				DebugConsoleProperties.RunCommand(data.commands[i].command, DebugConsoleProperties.GetExpectedArgs(data.commands[i].command));
			}
		}
	}

	public void OnEditingEnd(string s)
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			Submit(s);
		}
	}

	private void Submit(string s)
	{
		scrollRect.verticalNormalizedPosition = 0f;
		Run(s);
		ClearInput();
		AdjustBodyHeight();
		DelayedFocusInput();
	}

	private void Run(string s)
	{
		if (inputField.text == string.Empty)
		{
			return;
		}
		Text text = body;
		text.text = text.text + "\n\n> " + inputField.text;
		List<string> list = new List<string>(s.Split(' '));
		string text2 = list[0];
		list.RemoveAt(0);
		Arguments arguments = new Arguments(list);
		if (text2 == "help")
		{
			PrintHelp();
			return;
		}
		foreach (DebugConsoleData.Command command in data.commands)
		{
			if (!(command.command == text2))
			{
				continue;
			}
			Arguments expectedArgs = DebugConsoleProperties.GetExpectedArgs(text2);
			if (!CheckArgCount(expectedArgs, arguments))
			{
				PrintError("ArgumentsOutOfRangeException: " + arguments.values.Count + " argument(s) supplied to command, " + expectedArgs.values.Count + " Expected");
			}
			else if (!CheckArgTypes(expectedArgs, arguments))
			{
				string text3 = string.Empty;
				for (int i = 0; i < expectedArgs.values.Count; i++)
				{
					text3 = text3 + expectedArgs.values[i].type.ToString().ToLower() + " ";
				}
				PrintError("InvalidTypeException: Invalid argument type(s) supplied to command. Expected: " + text3);
			}
			else
			{
				DebugConsoleProperties.RunCommand(text2, arguments);
				PrintLog("Valid command");
				if (command.closeConsole)
				{
					Hide();
				}
			}
			return;
		}
		if (text2 == "love")
		{
			PrintLove();
		}
		else
		{
			PrintError("Invalid Command");
		}
	}

	public static bool CheckArgCount(Arguments expected, Arguments input)
	{
		return expected.values.Count == input.values.Count;
	}

	public static bool CheckArgTypes(Arguments expected, Arguments input)
	{
		for (int i = 0; i < expected.values.Count; i++)
		{
			if (expected.values[i].type != input.values[i].type)
			{
				return false;
			}
		}
		return true;
	}

	private void CleanInput()
	{
		inputField.text = inputField.text.Replace("`", string.Empty);
		inputField.text = inputField.text.Replace("~", string.Empty);
	}

	private void DelayedCleanInput()
	{
		StartCoroutine(delayedCleanInput_cr());
	}

	private void ClearInput()
	{
		inputField.text = string.Empty;
	}

	private void FocusInput()
	{
		EventSystem.current.SetSelectedGameObject(inputField.gameObject);
	}

	private void DelayedFocusInput()
	{
		StartCoroutine(delayedFocusInput_cr());
	}

	public static void Break()
	{
		Init();
		_instance.body.text += "\n";
	}

	public static void Line()
	{
		Init();
		Print("--------------");
	}

	public static void Print(string s)
	{
		Init();
		Break();
		_instance.body.text += s;
	}

	public static void PrintColored(string s, Color c)
	{
		Init();
		Print(ColorString(s, c));
	}

	public static void PrintError(string s)
	{
		Init();
		PrintColored(s, Color.red);
	}

	public static void PrintWarning(string s)
	{
		Init();
		PrintColored(s, new Color(1f, 0.5f, 0.5f, 1f));
	}

	public static void PrintLog(string s)
	{
		Init();
		PrintColored(s, new Color(1f, 1f, 1f, 0.5f));
	}

	private static string ColorString(string s, Color c)
	{
		return "<color=\"#" + c.ToHex(true) + "\">" + s + "</color>";
	}

	public void PrintHelp()
	{
		Break();
		PrintLog("HELP");
		PrintLog("--------------");
		PrintLog("When using the Debug Console, consider the following:");
		PrintLog("1. To run a command, use the following format: command arg1 arg2 arg3");
		PrintLog("2. String arguments must contain NO spaces");
		PrintLog("3. Use the Up and Down arrows to quickly scroll the log window");
		PrintLog("--------------");
		for (int i = 0; i < data.commands.Count; i++)
		{
			DebugConsoleData.Command command = data.commands[i];
			if (i != 0)
			{
				Break();
			}
			string text = string.Empty;
			for (int j = 0; j < command.arguments.Count; j++)
			{
				text = text + ColorString(command.arguments[j].type.ToString().ToLower(), Color.cyan) + " " + command.arguments[j].name;
				if (j < command.arguments.Count - 1)
				{
					text += ", ";
				}
			}
			Print(command.command + "\t" + text);
			PrintLog(command.help);
		}
		PrintLog("--------------");
	}

	public void PrintLove()
	{
		PrintColored("\n,d88b.d88b,\n88888888888\n`Y8888888Y'\n  `Y888Y'\t\n    `Y'\t  ", Color.red);
	}

	public static void Show()
	{
		Init();
		_instance.StartCoroutine(_instance.show_cr());
	}

	public static void Hide()
	{
		Init();
		_instance.StartCoroutine(_instance.hide_cr());
	}

	private void ShowImmediate()
	{
		panel.interactable = true;
		panel.blocksRaycasts = true;
		panel.alpha = 1f;
		state = State.Visible;
	}

	private void HideImmediate()
	{
		panel.interactable = false;
		panel.blocksRaycasts = false;
		panel.alpha = 0f;
		state = State.Hidden;
	}

	private void AdjustBodyHeight()
	{
		int num = body.text.Split('\n').Length;
		int num2 = num * 16;
		Vector2 sizeDelta = body.rectTransform.sizeDelta;
		sizeDelta.y = num2 + 5;
		body.rectTransform.sizeDelta = sizeDelta;
	}

	private void SetAnimationValue(float val)
	{
		RectTransform rectTransform = panel.transform as RectTransform;
		int num = (int)rectTransform.sizeDelta.y + 10;
		switch (ANIMATION_TYPE)
		{
		case AnimationType.Alpha:
			rectTransform.anchoredPosition = new Vector2(0f, 0f);
			panel.alpha = val;
			break;
		case AnimationType.Tween:
			panel.alpha = 1f;
			val = (float)(-num) + val * (float)num;
			rectTransform.anchoredPosition = new Vector2(0f, val);
			break;
		}
	}

	private IEnumerator delayedCleanInput_cr()
	{
		yield return null;
		CleanInput();
	}

	private IEnumerator delayedFocusInput_cr()
	{
		yield return null;
		EventSystem.current.SetSelectedGameObject(base.gameObject);
		yield return null;
		FocusInput();
	}

	private IEnumerator show_cr()
	{
		panel.interactable = true;
		panel.blocksRaycasts = true;
		DelayedFocusInput();
		yield return null;
		ClearInput();
		yield return StartCoroutine(animate_cr(0f, 1f, 0.2f));
		state = State.Visible;
	}

	private IEnumerator hide_cr()
	{
		panel.interactable = false;
		panel.blocksRaycasts = false;
		yield return StartCoroutine(animate_cr(1f, 0f, 0.2f));
		state = State.Hidden;
	}

	private IEnumerator animate_cr(float start, float end, float time)
	{
		state = State.Animating;
		SetAnimationValue(start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			SetAnimationValue(EaseUtils.Ease(EaseUtils.EaseType.easeOutCubic, start, end, val));
			t += Time.deltaTime;
			yield return null;
		}
		SetAnimationValue(end);
	}
}
