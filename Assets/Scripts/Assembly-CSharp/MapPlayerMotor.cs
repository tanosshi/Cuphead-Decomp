using System.Collections;
using UnityEngine;

public class MapPlayerMotor : AbstractMapPlayerComponent
{
	private const float SPEED = 2.5f;

	private const float DIAGONAL_FALLOFF = 0.75f;

	private const float FALLOFF_SPEED = 100f;

	public const float INPUT_THRESHOLD = 0.3f;

	private Vector2 axis;

	public const float LADDER_ENTER_TIME = 0.3f;

	public const float LADDER_EXIT_TIME = 0.3f;

	public const float LADDER_EXIT_JUMP = 0.2f;

	public Vector2 velocity { get; private set; }

	protected override void Update()
	{
		base.Update();
		if (!MapPlayerController.CanMove() || base.player.state == MapPlayerController.State.Stationary)
		{
			velocity = Vector2.zero;
			axis = Vector2.zero;
			base.rigidbody2D.velocity = Vector2.zero;
		}
		else if (PauseManager.state != PauseManager.State.Paused)
		{
			HandleInput();
			switch (base.player.state)
			{
			case MapPlayerController.State.Walking:
				MoveWalking();
				break;
			case MapPlayerController.State.Ladder:
				MoveLadder();
				break;
			}
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		MapPlayerController.State state = base.player.state;
		if (state == MapPlayerController.State.Ladder)
		{
			ClampPositionLadder();
		}
	}

	public override void OnPause()
	{
		base.OnPause();
		base.rigidbody2D.velocity = Vector2.zero;
	}

	private void HandleInput()
	{
		if (!base.player.EquipMenuOpen)
		{
			axis = new Vector2(base.input.GetAxisInt(PlayerInput.Axis.X), base.input.GetAxisInt(PlayerInput.Axis.Y));
			float magnitude = axis.magnitude;
			if (magnitude < 0.0001f)
			{
				axis = Vector2.zero;
			}
			else
			{
				axis /= magnitude;
			}
		}
	}

	private void MoveWalking()
	{
		velocity = Vector2.Lerp(velocity, new Vector2(axis.x * 2.5f, axis.y * 2.5f), (float)CupheadTime.Delta * 100f);
		base.rigidbody2D.velocity = velocity;
	}

	private void MoveLadder()
	{
		velocity = new Vector2(0f, axis.y * 2.5f);
		base.rigidbody2D.velocity = velocity;
	}

	private void ClampPositionLadder()
	{
		MapPlayerLadderObject current = base.player.ladderManager.Current;
		MapPlayerController.State state = base.player.state;
		if (state == MapPlayerController.State.Ladder)
		{
			Transform obj = base.transform;
			float y = base.transform.position.y;
			Vector2 bottom = current.bottom;
			float y2 = bottom.y;
			Vector2 top = current.top;
			obj.SetPosition(null, Mathf.Clamp(y, y2, top.y));
		}
	}

	protected override void OnLadderEnter(Vector2 point, MapPlayerLadderObject ladder, MapLadder.Location location)
	{
		base.OnLadderEnter(point, ladder, location);
		StartCoroutine(onLadderStart_cr(point, location));
	}

	protected override void OnLadderExit(Vector2 point, Vector2 exit, MapLadder.Location location)
	{
		base.OnLadderExit(point, exit, location);
		StartCoroutine(onLadderEnd_cr(exit, location));
	}

	private IEnumerator onLadderStart_cr(Vector2 endPos, MapLadder.Location location)
	{
		location = ((location == MapLadder.Location.Top) ? MapLadder.Location.Bottom : MapLadder.Location.Top);
		yield return StartCoroutine(ladder_cr(base.transform.position, endPos, location));
		base.player.LadderEnterComplete();
	}

	private IEnumerator onLadderEnd_cr(Vector2 endPos, MapLadder.Location location)
	{
		yield return StartCoroutine(ladder_cr(base.transform.position, endPos, location));
		base.player.LadderExitComplete();
	}

	private IEnumerator ladder_cr(Vector2 startPos, Vector2 endPos, MapLadder.Location location)
	{
		Vector2 centerPos = new Vector2(Mathf.Lerp(startPos.x, endPos.x, 0.5f), (location != MapLadder.Location.Top) ? (startPos.y + 0.2f) : (endPos.y + 0.2f));
		float t = 0f;
		float time = 0.15f;
		while (t < time)
		{
			Vector2 newPos = Vector2.Lerp(t: EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 0f, 1f, t / time), a: startPos, b: centerPos);
			base.transform.SetPosition(newPos.x, newPos.y);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		t = 0f;
		while (t < time)
		{
			Vector2 newPos2 = Vector2.Lerp(t: EaseUtils.Ease(EaseUtils.EaseType.easeInSine, 0f, 1f, t / time), a: centerPos, b: endPos);
			base.transform.SetPosition(newPos2.x, newPos2.y);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
	}
}
