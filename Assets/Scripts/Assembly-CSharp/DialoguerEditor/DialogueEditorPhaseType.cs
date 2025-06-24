using System.Collections.Generic;
using UnityEngine;

namespace DialoguerEditor
{
	public class DialogueEditorPhaseType
	{
		public DialogueEditorPhaseTypes type;

		public string name;

		public string info;

		public Texture iconDark;

		public Texture iconLight;

		public DialogueEditorPhaseType(DialogueEditorPhaseTypes type, string name, string info, Texture iconDark, Texture iconLight)
		{
			this.type = type;
			this.name = name;
			this.info = info;
			this.iconDark = iconDark;
			this.iconLight = iconLight;
		}

		public static Dictionary<int, DialogueEditorPhaseType> getPhases()
		{
			Dictionary<int, DialogueEditorPhaseType> dictionary = new Dictionary<int, DialogueEditorPhaseType>();
			DialogueEditorPhaseType value = new DialogueEditorPhaseType(DialogueEditorPhaseTypes.TextPhase, "Text", "A simple text page with one out-path.", getDarkIcon("textPhase"), getLightIcon("textPhase"));
			dictionary.Add(0, value);
			value = new DialogueEditorPhaseType(DialogueEditorPhaseTypes.BranchedTextPhase, "Branched Text", "A text page with multiple, selectable out-paths.", getDarkIcon("branchedTextPhase"), getLightIcon("branchedTextPhase"));
			dictionary.Add(1, value);
			value = new DialogueEditorPhaseType(DialogueEditorPhaseTypes.WaitPhase, "Wait", "Wait X seconds before progressing.", getDarkIcon("waitPhase"), getLightIcon("waitPhase"));
			dictionary.Add(2, value);
			value = new DialogueEditorPhaseType(DialogueEditorPhaseTypes.SetVariablePhase, "Set Variable", "Set a local or global variable.", getDarkIcon("setVariablePhase"), getLightIcon("setVariablePhase"));
			dictionary.Add(3, value);
			value = new DialogueEditorPhaseType(DialogueEditorPhaseTypes.ConditionalPhase, "Condition", "Moves to an out-path based on a condition.", getDarkIcon("conditionalPhase"), getLightIcon("conditionalPhase"));
			dictionary.Add(4, value);
			value = new DialogueEditorPhaseType(DialogueEditorPhaseTypes.SendMessagePhase, "Message Event", "Dispatch an event which can be easily listened to and handled.", getDarkIcon("sendMessagePhase"), getLightIcon("sendMessagePhase"));
			dictionary.Add(5, value);
			value = new DialogueEditorPhaseType(DialogueEditorPhaseTypes.EndPhase, "End", "Ends the dialogue and calls the dialogue's callback.", getDarkIcon("endPhase"), getLightIcon("endPhase"));
			dictionary.Add(6, value);
			return dictionary;
		}

		private static Texture getDarkIcon(string icon)
		{
			string text = "Assets/Dialoguer/DialogueEditor/Textures/GUI/";
			text += "Dark/";
			text = text + "icon_" + icon + ".png";
			return null;
		}

		private static Texture getLightIcon(string icon)
		{
			string text = "Assets/Dialoguer/DialogueEditor/Textures/GUI/";
			text += "Light/";
			text = text + "icon_" + icon + ".png";
			return null;
		}
	}
}
