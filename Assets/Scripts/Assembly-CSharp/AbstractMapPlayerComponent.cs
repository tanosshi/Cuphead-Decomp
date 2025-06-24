using UnityEngine;

public abstract class AbstractMapPlayerComponent : AbstractPausableComponent
{
	public MapPlayerController player { get; private set; }

	public PlayerInput input { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		player = GetComponent<MapPlayerController>();
		input = GetComponent<PlayerInput>();
		RegisterEvents();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		UnregisterEvents();
	}

	private void RegisterEvents()
	{
		player.LadderEnterEvent += OnLadderEnter;
		player.LadderEnterCompleteEvent += OnLadderEnterComplete;
		player.LadderExitEvent += OnLadderExit;
		player.LadderExitCompleteEvent += OnLadderExitComplete;
	}

	private void UnregisterEvents()
	{
		player.LadderEnterEvent -= OnLadderEnter;
		player.LadderEnterCompleteEvent -= OnLadderEnterComplete;
		player.LadderExitEvent -= OnLadderExit;
		player.LadderExitCompleteEvent -= OnLadderExitComplete;
	}

	protected virtual void OnLadderEnter(Vector2 point, MapPlayerLadderObject ladder, MapLadder.Location location)
	{
	}

	protected virtual void OnLadderExit(Vector2 point, Vector2 exit, MapLadder.Location location)
	{
	}

	protected virtual void OnLadderEnterComplete()
	{
	}

	protected virtual void OnLadderExitComplete()
	{
	}
}
