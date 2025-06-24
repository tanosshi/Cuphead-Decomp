using UnityEngine;

namespace DialoguerCore
{
	public class EmptyPhase : AbstractDialoguePhase
	{
		public EmptyPhase()
			: base(null)
		{
			Debug.LogWarning("Something went wrong, phase is EmptyPhase");
		}

		public override string ToString()
		{
			return "Empty Phase\nEmpty Phases should not be generated, something went wrong.\n";
		}
	}
}
