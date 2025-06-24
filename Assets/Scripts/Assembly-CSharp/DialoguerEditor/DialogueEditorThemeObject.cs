using System;

namespace DialoguerEditor
{
	[Serializable]
	public class DialogueEditorThemeObject
	{
		public int id;

		public string name;

		public string linkage;

		public DialogueEditorThemeObject()
		{
			name = string.Empty;
			linkage = string.Empty;
		}
	}
}
