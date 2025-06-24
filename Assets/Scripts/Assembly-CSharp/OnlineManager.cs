public class OnlineManager
{
	private static OnlineManager instance;

	public static OnlineManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new OnlineManager();
			}
			return instance;
		}
	}

	public OnlineInterface Interface { get; private set; }

	public void Init()
	{
		Interface = new DummyOnlineInterface();
		Interface.Init();
	}
}
