using System.Collections.Generic;
using UnityEngine;

namespace DialoguerEditor
{
	public class DialogueEditorPhaseTemplates
	{
		public static DialogueEditorPhaseObject newTextPhase(int id)
		{
			DialogueEditorPhaseObject dialogueEditorPhaseObject = new DialogueEditorPhaseObject();
			dialogueEditorPhaseObject.id = id;
			dialogueEditorPhaseObject.type = DialogueEditorPhaseTypes.TextPhase;
			dialogueEditorPhaseObject.position = Vector2.zero;
			dialogueEditorPhaseObject.advanced = false;
			dialogueEditorPhaseObject.metadata = string.Empty;
			dialogueEditorPhaseObject.name = string.Empty;
			dialogueEditorPhaseObject.portrait = string.Empty;
			dialogueEditorPhaseObject.audio = string.Empty;
			dialogueEditorPhaseObject.audioDelay = 0f;
			dialogueEditorPhaseObject.rect = new Rect(0f, 0f, 0f, 0f);
			dialogueEditorPhaseObject.newWindow = false;
			dialogueEditorPhaseObject.outs = new List<int>();
			dialogueEditorPhaseObject.outs.Add(-1);
			return dialogueEditorPhaseObject;
		}

		public static DialogueEditorPhaseObject newBranchedTextPhase(int id)
		{
			DialogueEditorPhaseObject dialogueEditorPhaseObject = new DialogueEditorPhaseObject();
			dialogueEditorPhaseObject.id = id;
			dialogueEditorPhaseObject.type = DialogueEditorPhaseTypes.BranchedTextPhase;
			dialogueEditorPhaseObject.position = Vector2.zero;
			dialogueEditorPhaseObject.advanced = false;
			dialogueEditorPhaseObject.metadata = string.Empty;
			dialogueEditorPhaseObject.name = string.Empty;
			dialogueEditorPhaseObject.portrait = string.Empty;
			dialogueEditorPhaseObject.audio = string.Empty;
			dialogueEditorPhaseObject.audioDelay = 0f;
			dialogueEditorPhaseObject.rect = new Rect(0f, 0f, 0f, 0f);
			dialogueEditorPhaseObject.newWindow = false;
			dialogueEditorPhaseObject.outs = new List<int>();
			dialogueEditorPhaseObject.outs.Add(-1);
			dialogueEditorPhaseObject.outs.Add(-1);
			dialogueEditorPhaseObject.choices = new List<string>();
			dialogueEditorPhaseObject.choices.Add(string.Empty);
			dialogueEditorPhaseObject.choices.Add(string.Empty);
			return dialogueEditorPhaseObject;
		}

		public static DialogueEditorPhaseObject newWaitPhase(int id)
		{
			DialogueEditorPhaseObject dialogueEditorPhaseObject = new DialogueEditorPhaseObject();
			dialogueEditorPhaseObject.id = id;
			dialogueEditorPhaseObject.type = DialogueEditorPhaseTypes.WaitPhase;
			dialogueEditorPhaseObject.position = Vector2.zero;
			dialogueEditorPhaseObject.advanced = false;
			dialogueEditorPhaseObject.metadata = string.Empty;
			dialogueEditorPhaseObject.outs = new List<int>();
			dialogueEditorPhaseObject.outs.Add(-1);
			return dialogueEditorPhaseObject;
		}

		public static DialogueEditorPhaseObject newSetVariablePhase(int id)
		{
			DialogueEditorPhaseObject dialogueEditorPhaseObject = new DialogueEditorPhaseObject();
			dialogueEditorPhaseObject.id = id;
			dialogueEditorPhaseObject.type = DialogueEditorPhaseTypes.SetVariablePhase;
			dialogueEditorPhaseObject.position = Vector2.zero;
			dialogueEditorPhaseObject.advanced = false;
			dialogueEditorPhaseObject.metadata = string.Empty;
			dialogueEditorPhaseObject.outs = new List<int>();
			dialogueEditorPhaseObject.outs.Add(-1);
			dialogueEditorPhaseObject.variableScope = VariableEditorScopes.Local;
			dialogueEditorPhaseObject.variableType = VariableEditorTypes.Boolean;
			dialogueEditorPhaseObject.variableSetEquation = VariableEditorSetEquation.Equals;
			dialogueEditorPhaseObject.variableScrollPosition = default(Vector2);
			dialogueEditorPhaseObject.variableId = 0;
			dialogueEditorPhaseObject.variableSetValue = string.Empty;
			return dialogueEditorPhaseObject;
		}

		public static DialogueEditorPhaseObject newConditionalPhase(int id)
		{
			DialogueEditorPhaseObject dialogueEditorPhaseObject = new DialogueEditorPhaseObject();
			dialogueEditorPhaseObject.id = id;
			dialogueEditorPhaseObject.type = DialogueEditorPhaseTypes.ConditionalPhase;
			dialogueEditorPhaseObject.position = Vector2.zero;
			dialogueEditorPhaseObject.advanced = false;
			dialogueEditorPhaseObject.metadata = string.Empty;
			dialogueEditorPhaseObject.outs = new List<int>();
			dialogueEditorPhaseObject.outs.Add(-1);
			dialogueEditorPhaseObject.outs.Add(-1);
			return dialogueEditorPhaseObject;
		}

		public static DialogueEditorPhaseObject newSendMessagePhase(int id)
		{
			DialogueEditorPhaseObject dialogueEditorPhaseObject = new DialogueEditorPhaseObject();
			dialogueEditorPhaseObject.id = id;
			dialogueEditorPhaseObject.type = DialogueEditorPhaseTypes.SendMessagePhase;
			dialogueEditorPhaseObject.position = Vector2.zero;
			dialogueEditorPhaseObject.advanced = false;
			dialogueEditorPhaseObject.metadata = string.Empty;
			dialogueEditorPhaseObject.outs = new List<int>();
			dialogueEditorPhaseObject.outs.Add(-1);
			dialogueEditorPhaseObject.messageName = string.Empty;
			return dialogueEditorPhaseObject;
		}

		public static DialogueEditorPhaseObject newEndPhase(int id)
		{
			DialogueEditorPhaseObject dialogueEditorPhaseObject = new DialogueEditorPhaseObject();
			dialogueEditorPhaseObject.id = id;
			dialogueEditorPhaseObject.type = DialogueEditorPhaseTypes.EndPhase;
			dialogueEditorPhaseObject.position = Vector2.zero;
			return dialogueEditorPhaseObject;
		}
	}
}
