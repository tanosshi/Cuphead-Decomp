using System.Collections.Generic;
using UnityEngine;

namespace DialoguerCore
{
	public class TextPhase : AbstractDialoguePhase
	{
		public readonly DialoguerTextData data;

		public TextPhase(string text, string themeName, bool newWindow, string name, string portrait, string metadata, string audio, float audioDelay, Rect rect, List<int> outs, List<string> choices, int dialogueID, int nodeID)
			: base(outs)
		{
			data = new DialoguerTextData(text, themeName, newWindow, name, portrait, metadata, audio, audioDelay, rect, choices, dialogueID, nodeID);
		}

		protected override void onStart()
		{
		}

		public override void Continue(int nextPhaseId)
		{
			DialoguerTextData dialoguerTextData = data;
			if (dialoguerTextData.newWindow)
			{
				DialoguerEventManager.dispatchOnWindowClose();
			}
			base.Continue(nextPhaseId);
			base.state = PhaseState.Complete;
		}

		public override string ToString()
		{
			return "Text Phase" + data.ToString() + "\nOut: " + outs[0] + "\n";
		}
	}
}
