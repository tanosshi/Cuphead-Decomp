using System;
using System.Collections.Generic;
using DialoguerEditor;
using UnityEngine;

namespace DialoguerCore
{
	public class DialoguerUtils
	{
		private static Dictionary<VariableEditorScopes, string> scopeStrings = new Dictionary<VariableEditorScopes, string>
		{
			{
				VariableEditorScopes.Global,
				"g"
			},
			{
				VariableEditorScopes.Local,
				"l"
			}
		};

		private static Dictionary<VariableEditorTypes, string> typeStrings = new Dictionary<VariableEditorTypes, string>
		{
			{
				VariableEditorTypes.Boolean,
				"b"
			},
			{
				VariableEditorTypes.Float,
				"f"
			},
			{
				VariableEditorTypes.String,
				"s"
			}
		};

		public static string insertTextPhaseStringVariables(string input)
		{
			int dialogueId = 0;
			string input2 = input;
			input2 = substituteStringVariable(input2, VariableEditorScopes.Global, VariableEditorTypes.Boolean, dialogueId);
			input2 = substituteStringVariable(input2, VariableEditorScopes.Global, VariableEditorTypes.Float, dialogueId);
			return substituteStringVariable(input2, VariableEditorScopes.Global, VariableEditorTypes.String, dialogueId);
		}

		private static string substituteStringVariable(string input, VariableEditorScopes scope, VariableEditorTypes type, int dialogueId)
		{
			string text = string.Empty;
			string[] separator = new string[1] { "<" + scopeStrings[scope] + typeStrings[type] + ">" };
			string[] separator2 = new string[1] { "</" + scopeStrings[scope] + typeStrings[type] + ">" };
			string[] array = input.Split(separator, StringSplitOptions.None);
			if (array.Length < 2)
			{
				return input;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(separator2, StringSplitOptions.None);
				int result;
				if (int.TryParse(array2[0], out result))
				{
					switch (scope)
					{
					case VariableEditorScopes.Global:
						switch (type)
						{
						case VariableEditorTypes.Boolean:
							array2[0] = Dialoguer.GetGlobalBoolean(result).ToString();
							break;
						case VariableEditorTypes.Float:
							array2[0] = Dialoguer.GetGlobalFloat(result).ToString();
							break;
						case VariableEditorTypes.String:
							array2[0] = Dialoguer.GetGlobalString(result);
							break;
						}
						break;
					case VariableEditorScopes.Local:
						Debug.Log("Local Variable string substitutions not yet supported");
						switch (type)
						{
						}
						break;
					}
				}
				text += string.Join(string.Empty, array2);
			}
			return text;
		}
	}
}
