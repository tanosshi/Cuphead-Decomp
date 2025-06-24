using UnityEngine;

public class MapPlayerAnimationController : AbstractMapPlayerComponent
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	public enum State
	{
		Idle = 0,
		Walk = 1
	}

	[SerializeField]
	private SpriteRenderer Cuphead;

	[SerializeField]
	private SpriteRenderer Mugman;

	[SerializeField]
	private MapPlayerDust dustEffect;

	private MapSpritePlaySound current;

	private Trilean2 axis;

	private bool onBridge;

	private float directionRotation;

	public Direction direction { get; private set; }

	public State state { get; private set; }

	public SpriteRenderer renderer { get; set; }

	public void Init(MapPlayerPose pose)
	{
		Cuphead.enabled = false;
		Mugman.enabled = false;
		PlayerId id = base.player.id;
		if (id == PlayerId.PlayerOne || id != PlayerId.PlayerTwo)
		{
			renderer = Cuphead;
			base.animator.SetInteger("Player", 0);
		}
		else
		{
			renderer = Mugman;
			base.animator.SetInteger("Player", 1);
		}
		renderer.enabled = true;
		switch (pose)
		{
		case MapPlayerPose.Default:
			state = State.Idle;
			break;
		case MapPlayerPose.Joined:
		case MapPlayerPose.Won:
			base.animator.Play("Jump");
			break;
		default:
			Debug.LogWarning(string.Concat("Pose '", pose, "' not yet configured"));
			break;
		}
		SetProperties();
	}

	protected override void Update()
	{
		base.Update();
		if (base.player.state == MapPlayerController.State.Stationary)
		{
			SetStationary();
			return;
		}
		if (!MapPlayerController.CanMove())
		{
			SetStationary();
			return;
		}
		Vector2 vector = new Vector2(base.player.input.actions.GetAxis(CupheadButton.MoveHorizontal.ToString()), base.player.input.actions.GetAxis(CupheadButton.MoveVertical.ToString()));
		state = ((vector.magnitude > 0.3f) ? State.Walk : State.Idle);
		SetProperties();
	}

	private void SetStationary()
	{
		state = State.Idle;
		axis.x = 0;
		axis.y = 0;
		SetProperties();
	}

	public void CompleteJump()
	{
		base.animator.SetTrigger("OnJumpComplete");
	}

	private void SetProperties()
	{
		if (state == State.Walk)
		{
			axis.x = base.player.input.GetAxisInt(PlayerInput.Axis.X);
			axis.y = base.player.input.GetAxisInt(PlayerInput.Axis.Y);
			if ((int)axis.x == -1)
			{
				renderer.transform.SetScale(-1f);
			}
			else
			{
				renderer.transform.SetScale(1f);
			}
		}
		base.animator.SetInteger("X", axis.x);
		base.animator.SetInteger("Y", axis.y);
		base.animator.SetInteger("Speed", (state != State.Idle) ? 1 : 0);
		SetDirectionRotation();
	}

	private void SetDirectionRotation()
	{
		if ((int)axis.x == 1 && (int)axis.y == 1)
		{
			directionRotation = -45f;
		}
		else if ((int)axis.x == 1 && (int)axis.y == 0)
		{
			directionRotation = -90f;
		}
		else if ((int)axis.x == 1 && (int)axis.y == -1)
		{
			directionRotation = -135f;
		}
		else if ((int)axis.x == 0 && (int)axis.y == 1)
		{
			directionRotation = 0f;
		}
		else if ((int)axis.x == 0 && (int)axis.y == 0)
		{
			directionRotation = 0f;
		}
		else if ((int)axis.x == 0 && (int)axis.y == -1)
		{
			directionRotation = -180f;
		}
		else if ((int)axis.x == -1 && (int)axis.y == 1)
		{
			directionRotation = 45f;
		}
		else if ((int)axis.x == -1 && (int)axis.y == 0)
		{
			directionRotation = 90f;
		}
		else if ((int)axis.x == -1 && (int)axis.y == -1)
		{
			directionRotation = 135f;
		}
	}

	private void WalkStepLeft()
	{
		if (renderer == Cuphead)
		{
			if (current != null)
			{
				current.PlaySoundRight(true);
			}
			else
			{
				AudioManager.Play("player_map_walk_one_p1");
			}
		}
		else if (current != null)
		{
			current.PlaySoundRight(false);
		}
		else
		{
			AudioManager.Play("player_map_walk_one_p2");
		}
		dustEffect.Create(base.transform.position, directionRotation, true, renderer.sortingOrder);
	}

	private void WalkStepRight()
	{
		if (renderer == Cuphead)
		{
			if (current != null)
			{
				current.PlaySoundRight(true);
			}
			else
			{
				AudioManager.Play("player_map_walk_one_p1");
			}
		}
		else if (current != null)
		{
			current.PlaySoundRight(false);
		}
		else
		{
			AudioManager.Play("player_map_walk_two_p2");
		}
		dustEffect.Create(base.transform.position, directionRotation, false, renderer.sortingOrder);
	}

	protected override void OnTriggerEnter2D(Collider2D collider)
	{
		base.OnTriggerEnter2D(collider);
		if ((bool)collider.GetComponent<MapSpritePlaySound>())
		{
			current = collider.GetComponent<MapSpritePlaySound>();
		}
	}

	protected override void OnTriggerExit2D(Collider2D collider)
	{
		base.OnTriggerExit2D(collider);
		if ((bool)collider.GetComponent<MapSpritePlaySound>())
		{
			current = null;
		}
	}
}
