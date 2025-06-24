using System;
using System.Collections.Generic;

namespace DialoguerEditor
{
	[Serializable]
	public class DialogueEditorVariablesContainer
	{
		public List<DialogueEditorVariableObject> variables;

		public int selection;

		public DialogueEditorVariablesContainer()
		{
			selection = 0;
			variables = new List<DialogueEditorVariableObject>();
		}

		public void addVariable()
		{
			int count = variables.Count;
			variables.Add(new DialogueEditorVariableObject());
			variables[count].id = count;
			selection = variables.Count - 1;
		}

		public void removeVariable()
		{
			if (variables.Count >= 1)
			{
				variables.RemoveAt(variables.Count - 1);
				if (selection > variables.Count - 1)
				{
					selection = variables.Count - 1;
				}
			}
		}
	}
}
