using Rewired;
using UnityEngine;

public class PlayerInput : AbstractMonoBehaviour
{
	public enum Axis
	{
		X = 0,
		Y = 1
	}

	private AbstractPlayerController player;

	public PlayerId playerId { get; private set; }

	public bool IsDead
	{
		get
		{
			if (player != null)
			{
				return player.IsDead;
			}
			return false;
		}
	}

	public Player actions { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		player = GetComponent<AbstractPlayerController>();
	}

	public void Init(PlayerId playerId)
	{
		this.playerId = playerId;
		actions = PlayerManager.GetPlayerInput(playerId);
	}

	public override void StopAllCoroutines()
	{
	}

	public int GetAxisInt(Axis axis, bool crampedDiagonal = false, bool duckMod = false)
	{
		if (DebugConsole.IsVisible)
		{
			return 0;
		}
		Vector2 vector = new Vector2(actions.GetAxis(CupheadButton.MoveHorizontal.ToString()), actions.GetAxis(CupheadButton.MoveVertical.ToString()));
		float magnitude = vector.magnitude;
		float num = ((!crampedDiagonal) ? 0.38268f : 0.5f);
		if (magnitude < 0.375f)
		{
			return 0;
		}
		float num2 = ((axis != Axis.X) ? vector.y : vector.x) / magnitude;
		if (num2 > num)
		{
			return 1;
		}
		if (num2 < ((!duckMod) ? (0f - num) : (-0.705f)))
		{
			return -1;
		}
		return 0;
	}

	public float GetAxis(Axis axis)
	{
		if (DebugConsole.IsVisible)
		{
			return 0f;
		}
		if (axis == Axis.X)
		{
			return actions.GetAxis(CupheadButton.MoveHorizontal.ToString());
		}
		return actions.GetAxis(CupheadButton.MoveVertical.ToString());
	}
}
