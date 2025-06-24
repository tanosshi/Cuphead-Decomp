using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialoguerEditor
{
	[Serializable]
	public class DialogueEditorPhaseObject
	{
		public int id;

		public DialogueEditorPhaseTypes type;

		public object paramaters;

		public string theme;

		public Vector2 position;

		public List<int> outs;

		public bool advanced;

		public string metadata;

		public string text;

		public string name;

		public string portrait;

		public string audio;

		public float audioDelay;

		public Rect rect;

		public bool newWindow;

		public List<string> choices;

		public DialogueEditorWaitTypes waitType;

		public float waitDuration;

		public VariableEditorScopes variableScope;

		public VariableEditorTypes variableType;

		public int variableId;

		public Vector2 variableScrollPosition;

		public VariableEditorSetEquation variableSetEquation;

		public string variableSetValue;

		public VariableEditorGetEquation variableGetEquation;

		public string variableGetValue;

		public string messageName;

		public DialogueEditorPhaseObject()
		{
			type = DialogueEditorPhaseTypes.EmptyPhase;
			position = Vector2.zero;
			text = string.Empty;
			outs = new List<int>();
			choices = new List<string>();
			waitType = DialogueEditorWaitTypes.Seconds;
		}

		public void addNewOut()
		{
			outs.Add(-1);
		}

		public void removeOut()
		{
			outs.RemoveAt(outs.Count - 1);
		}

		public void addNewChoice()
		{
			addNewOut();
			choices.Add(string.Empty);
		}

		public void removeChoice()
		{
			removeOut();
			choices.RemoveAt(choices.Count - 1);
		}
	}
}
