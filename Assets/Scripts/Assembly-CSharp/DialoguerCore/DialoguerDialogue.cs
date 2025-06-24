using System.Collections.Generic;

namespace DialoguerCore
{
	public class DialoguerDialogue
	{
		public readonly string name;

		public readonly int startPhaseId;

		public readonly List<AbstractDialoguePhase> phases;

		private readonly DialoguerVariables _originalLocalVariables;

		public DialoguerVariables localVariables;

		public DialoguerDialogue(string name, int startPhaseId, DialoguerVariables localVariables, List<AbstractDialoguePhase> phases)
		{
			this.name = name;
			this.startPhaseId = startPhaseId;
			this.phases = phases;
			_originalLocalVariables = localVariables;
		}

		public void Reset()
		{
			localVariables = _originalLocalVariables.Clone();
		}

		public override string ToString()
		{
			string text = "Dialogue: " + name + "\n-";
			text = text + "\nLocal Booleans: " + _originalLocalVariables.booleans.Count;
			text = text + "\nLocal Floats: " + _originalLocalVariables.floats.Count;
			text = text + "\nLocal Strings: " + _originalLocalVariables.strings.Count;
			text += "\n";
			for (int i = 0; i < phases.Count; i++)
			{
				string text2 = text;
				text = text2 + "\nPhase " + i + ": " + phases[i].ToString();
			}
			return text;
		}
	}
}
