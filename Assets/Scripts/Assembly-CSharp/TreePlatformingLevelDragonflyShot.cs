using UnityEngine;

public class TreePlatformingLevelDragonflyShot : PlatformingLevelPathMovementEnemy
{
	private bool firstTime = true;

	private int index;

	public bool isActivated { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		isActivated = false;
	}

	protected override void Die()
	{
		Deactivate();
	}

	public void Activate()
	{
		GetComponent<Collider2D>().enabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
		isActivated = true;
		PrepareShot();
	}

	public void Deactivate()
	{
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		isActivated = false;
		ResetStartingCondition();
	}

	private void PrepareShot()
	{
		if (Rand.Bool())
		{
			startPosition = 0f;
			_direction = Direction.Forward;
		}
		else
		{
			startPosition = 1f;
			_direction = Direction.Back;
		}
		StartFromCustom();
	}
}
