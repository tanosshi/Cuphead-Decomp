using UnityEngine;

namespace DialoguerEditor
{
	public class DialogueEditorDragPhaseObject
	{
		public int phaseId;

		public Vector2 mouseOffset;

		public DialogueEditorDragPhaseObject(int phaseId, Vector2 mouseOffset)
		{
			this.phaseId = phaseId;
			this.mouseOffset = mouseOffset;
		}
	}
}
