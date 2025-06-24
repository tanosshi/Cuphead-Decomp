namespace DialoguerCore
{
	public class DialoguerEventManager
	{
		public delegate void StartedHandler();

		public delegate void EndedHandler();

		public delegate void SuddenlyEndedHandler();

		public delegate void TextPhaseHandler(DialoguerTextData data);

		public delegate void WindowCloseHandler();

		public delegate void WaitStartHandler();

		public delegate void WaitCompleteHandler();

		public delegate void MessageEventHandler(string message, string metadata);

		public static event StartedHandler onStarted;

		public static event EndedHandler onEnded;

		public static event SuddenlyEndedHandler onSuddenlyEnded;

		public static event TextPhaseHandler onTextPhase;

		public static event WindowCloseHandler onWindowClose;

		public static event WaitStartHandler onWaitStart;

		public static event WaitCompleteHandler onWaitComplete;

		public static event MessageEventHandler onMessageEvent;

		public static void dispatchOnStarted()
		{
			if (DialoguerEventManager.onStarted != null)
			{
				DialoguerEventManager.onStarted();
			}
		}

		public static void dispatchOnEnded()
		{
			if (DialoguerEventManager.onEnded != null)
			{
				DialoguerEventManager.onEnded();
			}
		}

		public static void dispatchOnSuddenlyEnded()
		{
			if (DialoguerEventManager.onSuddenlyEnded != null)
			{
				DialoguerEventManager.onSuddenlyEnded();
			}
		}

		public static void dispatchOnTextPhase(DialoguerTextData data)
		{
			if (DialoguerEventManager.onTextPhase != null)
			{
				DialoguerEventManager.onTextPhase(data);
			}
		}

		public static void dispatchOnWindowClose()
		{
			if (DialoguerEventManager.onWindowClose != null)
			{
				DialoguerEventManager.onWindowClose();
			}
		}

		public static void dispatchOnWaitStart()
		{
			if (DialoguerEventManager.onWaitStart != null)
			{
				DialoguerEventManager.onWaitStart();
			}
		}

		public static void dispatchOnWaitComplete()
		{
			if (DialoguerEventManager.onWaitComplete != null)
			{
				DialoguerEventManager.onWaitComplete();
			}
		}

		public static void dispatchOnMessageEvent(string message, string metadata)
		{
			if (DialoguerEventManager.onMessageEvent != null)
			{
				DialoguerEventManager.onMessageEvent(message, metadata);
			}
		}
	}
}
