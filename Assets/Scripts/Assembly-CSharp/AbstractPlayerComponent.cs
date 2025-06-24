public class AbstractPlayerComponent : AbstractCollidableObject
{
	private AbstractPlayerController _basePlayer;

	public AbstractPlayerController basePlayer
	{
		get
		{
			if (_basePlayer == null)
			{
				_basePlayer = GetComponent<AbstractPlayerController>();
			}
			return _basePlayer;
		}
	}

	protected sealed override void Awake()
	{
		base.Awake();
		OnAwake();
	}

	protected virtual void OnAwake()
	{
	}

	public virtual void OnLevelStart()
	{
	}
}
