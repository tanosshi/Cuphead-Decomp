using UnityEngine;

public class MapPlayerLadderManager : AbstractMapPlayerComponent
{
	public MapPlayerLadderObject Current { get; private set; }

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnLadderEnter(Vector2 point, MapPlayerLadderObject ladder, MapLadder.Location location)
	{
		base.OnLadderEnter(point, ladder, location);
		GetComponent<Collider2D>().enabled = false;
		Current = ladder;
	}

	protected override void OnLadderExitComplete()
	{
		base.OnLadderExitComplete();
		GetComponent<Collider2D>().enabled = true;
		Current = null;
	}
}
