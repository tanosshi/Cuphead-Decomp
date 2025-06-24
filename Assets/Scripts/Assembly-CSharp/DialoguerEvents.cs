public class DialoguerEvents
{
	public delegate void StartedHandler();

	public delegate void EndedHandler();

	public delegate void SuddenlyEndedHandler();

	public delegate void TextPhaseHandler(DialoguerTextData data);

	public delegate void WindowCloseHandler();

	public delegate void WaitStartHandler();

	public delegate void WaitCompleteHandler();

	public delegate void MessageEventHandler(string message, string metadata);

	public event StartedHandler onStarted;

	public event EndedHandler onEnded;

	public event SuddenlyEndedHandler onInstantlyEnded;

	public event TextPhaseHandler onTextPhase;

	public event WindowCloseHandler onWindowClose;

	public event WaitStartHandler onWaitStart;

	public event WaitCompleteHandler onWaitComplete;

	public event MessageEventHandler onMessageEvent;

	public void ClearAll()
	{
		this.onStarted = null;
		this.onEnded = null;
		this.onTextPhase = null;
		this.onWindowClose = null;
		this.onWaitStart = null;
		this.onWaitComplete = null;
		this.onMessageEvent = null;
	}

	public void handler_onStarted()
	{
		if (this.onStarted != null)
		{
			this.onStarted();
		}
	}

	public void handler_onEnded()
	{
		if (this.onEnded != null)
		{
			this.onEnded();
		}
	}

	public void handler_SuddenlyEnded()
	{
		if (this.onInstantlyEnded != null)
		{
			this.onInstantlyEnded();
		}
	}

	public void handler_TextPhase(DialoguerTextData data)
	{
		if (this.onTextPhase != null)
		{
			this.onTextPhase(data);
		}
	}

	public void handler_WindowClose()
	{
		if (this.onWindowClose != null)
		{
			this.onWindowClose();
		}
	}

	public void handler_WaitStart()
	{
		if (this.onWaitStart != null)
		{
			this.onWaitStart();
		}
	}

	public void handler_WaitComplete()
	{
		if (this.onWaitComplete != null)
		{
			this.onWaitComplete();
		}
	}

	public void handler_MessageEvent(string message, string metadata)
	{
		if (this.onMessageEvent != null)
		{
			this.onMessageEvent(message, metadata);
		}
	}
}
