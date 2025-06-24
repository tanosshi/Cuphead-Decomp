using System.Collections.Generic;

namespace DialoguerCore
{
	public class SendMessagePhase : AbstractDialoguePhase
	{
		public readonly string message;

		public readonly string metadata;

		public SendMessagePhase(string message, string metadata, List<int> outs)
			: base(outs)
		{
			this.message = message;
			this.metadata = metadata;
		}

		protected override void onStart()
		{
			DialoguerEventManager.dispatchOnMessageEvent(message, metadata);
			base.state = PhaseState.Complete;
		}

		public override string ToString()
		{
			return "Send Message Phase\nMessage: " + message + "\nMetadata: " + metadata + "\n";
		}
	}
}
