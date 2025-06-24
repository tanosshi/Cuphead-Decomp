using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialoguerEditor
{
	[Serializable]
	public class DialogueEditorDialogueObject
	{
		public int id;

		public string name;

		public int startPage = -1;

		public Vector2 scrollPosition;

		public List<DialogueEditorPhaseObject> phases;

		public DialogueEditorVariablesContainer floats;

		public DialogueEditorVariablesContainer strings;

		public DialogueEditorVariablesContainer booleans;

		public DialogueEditorDialogueObject()
		{
			name = string.Empty;
			phases = new List<DialogueEditorPhaseObject>();
			floats = new DialogueEditorVariablesContainer();
			strings = new DialogueEditorVariablesContainer();
			booleans = new DialogueEditorVariablesContainer();
		}

		public void addPhase(DialogueEditorPhaseTypes phaseType, Vector2 newPhasePosition)
		{
			switch (phaseType)
			{
			case DialogueEditorPhaseTypes.TextPhase:
				phases.Add(DialogueEditorPhaseTemplates.newTextPhase(phases.Count));
				break;
			case DialogueEditorPhaseTypes.BranchedTextPhase:
				phases.Add(DialogueEditorPhaseTemplates.newBranchedTextPhase(phases.Count));
				break;
			case DialogueEditorPhaseTypes.WaitPhase:
				phases.Add(DialogueEditorPhaseTemplates.newWaitPhase(phases.Count));
				break;
			case DialogueEditorPhaseTypes.SetVariablePhase:
				phases.Add(DialogueEditorPhaseTemplates.newSetVariablePhase(phases.Count));
				break;
			case DialogueEditorPhaseTypes.ConditionalPhase:
				phases.Add(DialogueEditorPhaseTemplates.newConditionalPhase(phases.Count));
				break;
			case DialogueEditorPhaseTypes.SendMessagePhase:
				phases.Add(DialogueEditorPhaseTemplates.newSendMessagePhase(phases.Count));
				break;
			case DialogueEditorPhaseTypes.EndPhase:
				phases.Add(DialogueEditorPhaseTemplates.newEndPhase(phases.Count));
				break;
			}
			phases[phases.Count - 1].position = newPhasePosition;
		}

		public void removePhase(int phaseId)
		{
			for (int i = 0; i < phases.Count; i++)
			{
				DialogueEditorPhaseObject dialogueEditorPhaseObject = phases[i];
				for (int j = 0; j < dialogueEditorPhaseObject.outs.Count; j++)
				{
					if (dialogueEditorPhaseObject.outs[j] >= 0 && dialogueEditorPhaseObject.outs[j] > phaseId)
					{
						dialogueEditorPhaseObject.outs[j]--;
					}
					else if (dialogueEditorPhaseObject.outs[j] >= 0 && dialogueEditorPhaseObject.outs[j] == phaseId)
					{
						dialogueEditorPhaseObject.outs[j] = -1;
					}
				}
				if (startPage >= 0 && startPage == phaseId)
				{
					startPage = -1;
				}
				if (i > phaseId)
				{
					dialogueEditorPhaseObject.id--;
				}
			}
			phases.RemoveAt(phaseId);
		}
	}
}
