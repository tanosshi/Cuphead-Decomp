using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsoleData : ScriptableObject
{
	[Serializable]
	public class Command
	{
		[Serializable]
		public class Argument
		{
			public enum Type
			{
				Int = 0,
				Float = 1,
				Bool = 2,
				String = 3
			}

			public Type type;

			public string name = "argName";
		}

		public string command = "new.command";

		public KeyCode key;

		public string rewiredAction = string.Empty;

		public List<Argument> arguments = new List<Argument>();

		public string help = string.Empty;

		public string code = string.Empty;

		public bool closeConsole;
	}

	public static string PATH = "TC_DebugConsole/tc_debug_console_data";

	public int index;

	public List<Command> commands = new List<Command>();

	public Command Current
	{
		get
		{
			CleanIndex();
			return commands[index];
		}
	}

	public void PrepareForSave()
	{
	}

	private void CleanIndex()
	{
		if (index >= commands.Count)
		{
			index = commands.Count - 1;
		}
	}
}
