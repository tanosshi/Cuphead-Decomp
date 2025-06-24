using System;

namespace DialoguerEditor
{
	[Serializable]
	public class DialogueEditorVariableObject
	{
		public string name;

		public string variable;

		public int id;

		public DialogueEditorVariableObject()
		{
			name = string.Empty;
			variable = string.Empty;
		}
	}
}
