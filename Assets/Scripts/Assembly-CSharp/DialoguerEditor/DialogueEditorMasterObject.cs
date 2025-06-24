using System;
using System.Collections.Generic;
using DialoguerCore;
using UnityEngine;

namespace DialoguerEditor
{
	[Serializable]
	public class DialogueEditorMasterObject
	{
		private int __currentDialogueId;

		public bool generateEnum = true;

		public List<DialogueEditorDialogueObject> dialogues;

		public DialogueEditorGlobalVariablesContainer globals;

		public DialogueEditorThemesContainer themes;

		public Vector2 selectorScrollPosition;

		public int count
		{
			get
			{
				return dialogues.Count;
			}
		}

		public int currentDialogueId
		{
			get
			{
				return __currentDialogueId;
			}
			set
			{
				__currentDialogueId = Mathf.Clamp(value, 0, count - 1);
			}
		}

		public DialogueEditorMasterObject()
		{
			dialogues = new List<DialogueEditorDialogueObject>();
			globals = new DialogueEditorGlobalVariablesContainer();
			themes = new DialogueEditorThemesContainer();
			selectorScrollPosition = Vector2.zero;
			currentDialogueId = -1;
		}

		public void addDialogue(int count)
		{
			for (int i = 0; i < count; i++)
			{
				int num = dialogues.Count;
				dialogues.Add(new DialogueEditorDialogueObject());
				dialogues[num].id = num;
				currentDialogueId = dialogues[num].id;
			}
		}

		public void removeDialogue(int removeCount)
		{
			if (count >= 1)
			{
				for (int i = 0; i < removeCount; i++)
				{
					int index = dialogues.Count - 1;
					dialogues.RemoveAt(index);
				}
				currentDialogueId = currentDialogueId;
			}
		}

		public string[] getThemeNames()
		{
			return getThemeNames(false);
		}

		public string[] getThemeNames(bool includeId)
		{
			string[] array = new string[themes.themes.Count];
			for (int i = 0; i < themes.themes.Count; i++)
			{
				array[i] = string.Empty;
				string[] array2;
				if (includeId)
				{
					int num;
					(array2 = array)[num = i] = array2[num] + themes.themes[i].id + " ";
				}
				int num2;
				(array2 = array)[num2 = i] = array2[num2] + themes.themes[i].name;
			}
			return array;
		}

		public DialoguerData getDialoguerData()
		{
			List<bool> list = new List<bool>();
			List<float> list2 = new List<float>();
			List<string> list3 = new List<string>();
			for (int i = 0; i < globals.booleans.variables.Count; i++)
			{
				bool result;
				if (!bool.TryParse(globals.booleans.variables[i].variable, out result))
				{
					Debug.LogWarning("Global Boolean " + i + " did not parse correctly, defaulting to false");
				}
				list.Add(result);
			}
			for (int j = 0; j < globals.floats.variables.Count; j++)
			{
				float result2;
				if (!float.TryParse(globals.floats.variables[j].variable, out result2))
				{
					Debug.LogWarning("Global Float " + j + " did not parse correctly, defaulting to 0");
				}
				list2.Add(result2);
			}
			for (int k = 0; k < globals.strings.variables.Count; k++)
			{
				list3.Add(globals.strings.variables[k].variable);
			}
			DialoguerGlobalVariables globalVariables = new DialoguerGlobalVariables(list, list2, list3);
			List<DialoguerDialogue> list4 = new List<DialoguerDialogue>();
			for (int l = 0; l < dialogues.Count; l++)
			{
				DialogueEditorDialogueObject dialogueEditorDialogueObject = dialogues[l];
				List<AbstractDialoguePhase> list5 = new List<AbstractDialoguePhase>();
				for (int m = 0; m < dialogueEditorDialogueObject.phases.Count; m++)
				{
					DialogueEditorPhaseObject dialogueEditorPhaseObject = dialogueEditorDialogueObject.phases[m];
					switch (dialogueEditorPhaseObject.type)
					{
					case DialogueEditorPhaseTypes.TextPhase:
						list5.Add(new TextPhase(dialogueEditorPhaseObject.text, dialogueEditorPhaseObject.theme, dialogueEditorPhaseObject.newWindow, dialogueEditorPhaseObject.name, dialogueEditorPhaseObject.portrait, dialogueEditorPhaseObject.metadata, dialogueEditorPhaseObject.audio, dialogueEditorPhaseObject.audioDelay, dialogueEditorPhaseObject.rect, dialogueEditorPhaseObject.outs, null, dialogueEditorDialogueObject.id, dialogueEditorPhaseObject.id));
						break;
					case DialogueEditorPhaseTypes.BranchedTextPhase:
						list5.Add(new BranchedTextPhase(dialogueEditorPhaseObject.text, dialogueEditorPhaseObject.choices, dialogueEditorPhaseObject.theme, dialogueEditorPhaseObject.newWindow, dialogueEditorPhaseObject.name, dialogueEditorPhaseObject.portrait, dialogueEditorPhaseObject.metadata, dialogueEditorPhaseObject.audio, dialogueEditorPhaseObject.audioDelay, dialogueEditorPhaseObject.rect, dialogueEditorPhaseObject.outs, dialogueEditorDialogueObject.id, dialogueEditorPhaseObject.id));
						break;
					case DialogueEditorPhaseTypes.WaitPhase:
						list5.Add(new WaitPhase(dialogueEditorPhaseObject.waitType, dialogueEditorPhaseObject.waitDuration, dialogueEditorPhaseObject.outs));
						break;
					case DialogueEditorPhaseTypes.SetVariablePhase:
						list5.Add(new SetVariablePhase(dialogueEditorPhaseObject.variableScope, dialogueEditorPhaseObject.variableType, dialogueEditorPhaseObject.variableId, dialogueEditorPhaseObject.variableSetEquation, dialogueEditorPhaseObject.variableSetValue, dialogueEditorPhaseObject.outs));
						break;
					case DialogueEditorPhaseTypes.ConditionalPhase:
						list5.Add(new ConditionalPhase(dialogueEditorPhaseObject.variableScope, dialogueEditorPhaseObject.variableType, dialogueEditorPhaseObject.variableId, dialogueEditorPhaseObject.variableGetEquation, dialogueEditorPhaseObject.variableGetValue, dialogueEditorPhaseObject.outs));
						break;
					case DialogueEditorPhaseTypes.SendMessagePhase:
						list5.Add(new SendMessagePhase(dialogueEditorPhaseObject.messageName, dialogueEditorPhaseObject.metadata, dialogueEditorPhaseObject.outs));
						break;
					case DialogueEditorPhaseTypes.EndPhase:
						list5.Add(new EndPhase());
						break;
					default:
						list5.Add(new EmptyPhase());
						break;
					}
				}
				List<bool> list6 = new List<bool>();
				for (int n = 0; n < dialogueEditorDialogueObject.booleans.variables.Count; n++)
				{
					bool result3;
					if (!bool.TryParse(dialogueEditorDialogueObject.booleans.variables[n].variable, out result3))
					{
						Debug.Log("Dialogue " + l + ": Boolean " + n + " not formatted correctly. Defaulting to false");
					}
					list6.Add(result3);
				}
				List<float> list7 = new List<float>();
				for (int num = 0; num < dialogueEditorDialogueObject.floats.variables.Count; num++)
				{
					float result4;
					if (!float.TryParse(dialogueEditorDialogueObject.floats.variables[num].variable, out result4))
					{
						Debug.Log("Dialogue " + l + ": Float " + num + " not formatted correctly. Defaulting to 0");
					}
					list7.Add(result4);
				}
				List<string> list8 = new List<string>();
				for (int num2 = 0; num2 < dialogueEditorDialogueObject.strings.variables.Count; num2++)
				{
					list8.Add(dialogueEditorDialogueObject.strings.variables[num2].variable);
				}
				DialoguerVariables localVariables = new DialoguerVariables(list6, list7, list8);
				DialoguerDialogue item = new DialoguerDialogue(dialogueEditorDialogueObject.name, dialogueEditorDialogueObject.startPage, localVariables, list5);
				list4.Add(item);
			}
			List<DialoguerTheme> list9 = new List<DialoguerTheme>();
			for (int num3 = 0; num3 < themes.themes.Count; num3++)
			{
				list9.Add(new DialoguerTheme(themes.themes[num3].name, themes.themes[num3].linkage));
			}
			return new DialoguerData(globalVariables, list4, list9);
		}
	}
}
