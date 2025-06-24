using UnityEngine;

namespace DialoguerCore
{
	public class DialoguerDialogueManager
	{
		private static AbstractDialoguePhase currentPhase;

		private static DialoguerDialogue dialogue;

		private static DialoguerCallback onEndCallback;

		public static void startDialogueWithCallback(int dialogueId, DialoguerCallback callback)
		{
			onEndCallback = callback;
			startDialogue(dialogueId);
		}

		public static void startDialogue(int dialogueId)
		{
			if (dialogue != null)
			{
				DialoguerEventManager.dispatchOnSuddenlyEnded();
			}
			DialoguerEventManager.dispatchOnStarted();
			dialogue = DialoguerDataManager.GetDialogueById(dialogueId);
			dialogue.Reset();
			setupPhase(dialogue.startPhaseId);
		}

		public static void continueDialogue(int outId)
		{
			if (currentPhase != null)
			{
				currentPhase.Continue(outId);
			}
		}

		public static void endDialogue()
		{
			if (dialogue != null)
			{
				if (onEndCallback != null)
				{
					onEndCallback();
				}
				DialoguerEventManager.dispatchOnWindowClose();
				DialoguerEventManager.dispatchOnEnded();
				dialogue.Reset();
				reset();
			}
		}

		private static void setupPhase(int nextPhaseId)
		{
			if (dialogue == null)
			{
				return;
			}
			AbstractDialoguePhase abstractDialoguePhase = dialogue.phases[nextPhaseId];
			if (abstractDialoguePhase is EndPhase)
			{
				endDialogue();
				return;
			}
			if (currentPhase != null)
			{
				currentPhase.resetEvents();
			}
			abstractDialoguePhase.onPhaseComplete += phaseComplete;
			if (abstractDialoguePhase is TextPhase || abstractDialoguePhase is BranchedTextPhase)
			{
				DialoguerEventManager.dispatchOnTextPhase((abstractDialoguePhase as TextPhase).data);
			}
			currentPhase = abstractDialoguePhase;
			abstractDialoguePhase.Start(dialogue.localVariables);
		}

		private static void phaseComplete(int nextPhaseId)
		{
			setupPhase(nextPhaseId);
		}

		private static bool isWindowed(AbstractDialoguePhase phase)
		{
			if (phase is TextPhase || phase is BranchedTextPhase)
			{
				Debug.Log("Phase is: " + phase.GetType().ToString());
				return true;
			}
			return false;
		}

		private static void reset()
		{
			currentPhase = null;
			dialogue = null;
			onEndCallback = null;
		}
	}
}
