using DialoguerCore;

public class Dialoguer
{
	public static bool ready { get; private set; }

	public static DialoguerEvents events { get; private set; }

	public static void Initialize()
	{
		if (!ready)
		{
			events = new DialoguerEvents();
			DialoguerDataManager.Initialize();
			DialoguerEventManager.onStarted += events.handler_onStarted;
			DialoguerEventManager.onEnded += events.handler_onEnded;
			DialoguerEventManager.onSuddenlyEnded += events.handler_SuddenlyEnded;
			DialoguerEventManager.onTextPhase += events.handler_TextPhase;
			DialoguerEventManager.onWindowClose += events.handler_WindowClose;
			DialoguerEventManager.onWaitStart += events.handler_WaitStart;
			DialoguerEventManager.onWaitComplete += events.handler_WaitComplete;
			DialoguerEventManager.onMessageEvent += events.handler_MessageEvent;
			ready = true;
		}
	}

	public static void StartDialogue(DialoguerDialogues dialogue)
	{
		DialoguerDialogueManager.startDialogue((int)dialogue);
	}

	public static void StartDialogue(DialoguerDialogues dialogue, DialoguerCallback callback)
	{
		DialoguerDialogueManager.startDialogueWithCallback((int)dialogue, callback);
	}

	public static void StartDialogue(int dialogueId)
	{
		DialoguerDialogueManager.startDialogue(dialogueId);
	}

	public static void StartDialogue(int dialogueId, DialoguerCallback callback)
	{
		DialoguerDialogueManager.startDialogueWithCallback(dialogueId, callback);
	}

	public static void ContinueDialogue(int choice)
	{
		DialoguerDialogueManager.continueDialogue(choice);
	}

	public static void ContinueDialogue()
	{
		DialoguerDialogueManager.continueDialogue(0);
	}

	public static void EndDialogue()
	{
		DialoguerDialogueManager.endDialogue();
	}

	public static void SetGlobalBoolean(int booleanId, bool booleanValue)
	{
		DialoguerDataManager.SetGlobalBoolean(booleanId, booleanValue);
	}

	public static bool GetGlobalBoolean(int booleanId)
	{
		return DialoguerDataManager.GetGlobalBoolean(booleanId);
	}

	public static void SetGlobalFloat(int floatId, float floatValue)
	{
		DialoguerDataManager.SetGlobalFloat(floatId, floatValue);
	}

	public static float GetGlobalFloat(int floatId)
	{
		return DialoguerDataManager.GetGlobalFloat(floatId);
	}

	public static void SetGlobalString(int stringId, string stringValue)
	{
		DialoguerDataManager.SetGlobalString(stringId, stringValue);
	}

	public static string GetGlobalString(int stringId)
	{
		return DialoguerDataManager.GetGlobalString(stringId);
	}

	public static string GetGlobalVariablesState()
	{
		return DialoguerDataManager.GetGlobalVariablesState();
	}

	public static void SetGlobalVariablesState(string globalVariablesXml)
	{
		DialoguerDataManager.LoadGlobalVariablesState(globalVariablesXml);
	}
}
