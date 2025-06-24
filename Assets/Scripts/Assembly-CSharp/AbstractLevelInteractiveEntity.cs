using System;
using UnityEngine;

public abstract class AbstractLevelInteractiveEntity : AbstractPausableComponent
{
	public enum Interactor
	{
		Cuphead = 0,
		Mugman = 1,
		Either = 2,
		Both = 3
	}

	protected enum State
	{
		Inactive = 0,
		Ready = 1,
		Activated = 2
	}

	public Interactor interactor = Interactor.Either;

	public Vector2 interactionPoint;

	public float interactionDistance = 100f;

	public AbstractUIInteractionDialogue.Properties dialogueProperties;

	public Vector2 dialogueOffset;

	public bool once = true;

	public bool hasTarget = true;

	protected LevelUIInteractionDialogue dialogue;

	private bool lastInteractable;

	protected State state { get; set; }

	protected AbstractPlayerController playerActivating { get; private set; }

	public event Action OnActivateEvent;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		Check();
		if (state == State.Activated)
		{
			return;
		}
		switch (interactor)
		{
		default:
			if (PlayerWithinDistance(PlayerId.PlayerOne) && PlayerManager.GetPlayer(PlayerId.PlayerOne).input.actions.GetButtonDown(13) && !PlayerIsDashing(PlayerId.PlayerOne))
			{
				Activate(PlayerManager.GetPlayer(PlayerId.PlayerOne));
			}
			break;
		case Interactor.Mugman:
			if (PlayerWithinDistance(PlayerId.PlayerTwo) && PlayerManager.GetPlayer(PlayerId.PlayerTwo).input.actions.GetButtonDown(13) && !PlayerIsDashing(PlayerId.PlayerTwo))
			{
				Activate(PlayerManager.GetPlayer(PlayerId.PlayerTwo));
			}
			break;
		case Interactor.Either:
			if (PlayerWithinDistance(PlayerId.PlayerOne) || PlayerWithinDistance(PlayerId.PlayerTwo))
			{
				if (PlayerManager.GetPlayer(PlayerId.PlayerOne).input.actions.GetButtonDown(13) && PlayerWithinDistance(PlayerId.PlayerOne) && !PlayerIsDashing(PlayerId.PlayerOne))
				{
					Activate(PlayerManager.GetPlayer(PlayerId.PlayerOne));
				}
				else if (!(PlayerManager.GetPlayer(PlayerId.PlayerTwo) == null) && PlayerManager.GetPlayer(PlayerId.PlayerTwo).input.actions.GetButtonDown(13) && PlayerWithinDistance(PlayerId.PlayerTwo) && !PlayerIsDashing(PlayerId.PlayerTwo))
				{
					Activate(PlayerManager.GetPlayer(PlayerId.PlayerTwo));
				}
			}
			break;
		case Interactor.Both:
			if (!(PlayerManager.GetPlayer(PlayerId.PlayerOne) == null) && !(PlayerManager.GetPlayer(PlayerId.PlayerTwo) == null) && PlayerWithinDistance(PlayerId.PlayerOne) && PlayerWithinDistance(PlayerId.PlayerTwo))
			{
				if (PlayerManager.GetPlayer(PlayerId.PlayerOne).input.actions.GetButtonDown(13) && PlayerWithinDistance(PlayerId.PlayerOne) && PlayerManager.GetPlayer(PlayerId.PlayerTwo).input.actions.GetButton(13) && PlayerWithinDistance(PlayerId.PlayerTwo) && !PlayerIsDashing(PlayerId.PlayerOne) && !PlayerIsDashing(PlayerId.PlayerTwo))
				{
					Activate(PlayerManager.GetPlayer(PlayerId.PlayerOne));
				}
				else if (PlayerManager.GetPlayer(PlayerId.PlayerTwo).input.actions.GetButtonDown(13) && PlayerWithinDistance(PlayerId.PlayerTwo) && PlayerManager.GetPlayer(PlayerId.PlayerOne).input.actions.GetButton(13) && PlayerWithinDistance(PlayerId.PlayerOne) && !PlayerIsDashing(PlayerId.PlayerOne) && !PlayerIsDashing(PlayerId.PlayerTwo))
				{
					Activate(PlayerManager.GetPlayer(PlayerId.PlayerTwo));
				}
			}
			break;
		}
	}

	protected bool AbleToActivate()
	{
		switch (interactor)
		{
		default:
			if (PlayerWithinDistance(PlayerId.PlayerOne))
			{
				return true;
			}
			return false;
		case Interactor.Mugman:
			if (PlayerWithinDistance(PlayerId.PlayerTwo))
			{
				return true;
			}
			return false;
		case Interactor.Either:
			if (PlayerWithinDistance(PlayerId.PlayerOne) || PlayerWithinDistance(PlayerId.PlayerTwo))
			{
				return true;
			}
			return false;
		case Interactor.Both:
			if (PlayerWithinDistance(PlayerId.PlayerOne) && PlayerWithinDistance(PlayerId.PlayerTwo))
			{
				return true;
			}
			return false;
		}
	}

	protected bool PlayerWithinDistance(PlayerId id)
	{
		if (PlayerManager.GetPlayer(id) == null)
		{
			return false;
		}
		Vector2 a = (Vector2)base.transform.position + interactionPoint;
		Vector2 b = PlayerManager.GetPlayer(id).transform.position;
		return Vector2.Distance(a, b) <= interactionDistance;
	}

	protected bool PlayerIsDashing(PlayerId id)
	{
		if (PlayerManager.GetPlayer(id) == null)
		{
			return false;
		}
		if (PlayerManager.GetPlayer(id).GetComponent<LevelPlayerMotor>() != null)
		{
			LevelPlayerController levelPlayerController = (LevelPlayerController)PlayerManager.GetPlayer(id);
			return levelPlayerController.motor.Dashing;
		}
		return false;
	}

	protected virtual void Check()
	{
		bool flag = AbleToActivate();
		if (flag != lastInteractable)
		{
			if (flag)
			{
				if (PlayerWithinDistance(PlayerId.PlayerOne))
				{
					Show(PlayerId.PlayerOne);
				}
				if (PlayerWithinDistance(PlayerId.PlayerTwo) && PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
				{
					Show(PlayerId.PlayerTwo);
				}
			}
			else
			{
				if (!PlayerWithinDistance(PlayerId.PlayerOne))
				{
					Hide(PlayerId.PlayerOne);
				}
				if (!PlayerWithinDistance(PlayerId.PlayerTwo) && PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
				{
					Hide(PlayerId.PlayerTwo);
				}
			}
		}
		lastInteractable = flag;
	}

	private void Activate(AbstractPlayerController player)
	{
		if (!(dialogue == null))
		{
			playerActivating = player;
			dialogue.Close();
			dialogue = null;
			state = State.Activated;
			if (this.OnActivateEvent != null)
			{
				this.OnActivateEvent();
			}
			Activate();
		}
	}

	protected virtual void Activate()
	{
	}

	protected virtual void Show(PlayerId playerId)
	{
		state = State.Ready;
		dialogueProperties.text = string.Empty;
		dialogue = LevelUIInteractionDialogue.Create(dialogueProperties, PlayerManager.GetPlayer(playerId).input, dialogueOffset, 0f, LevelUIInteractionDialogue.TailPosition.Bottom, hasTarget);
	}

	protected virtual void Hide(PlayerId playerId)
	{
		if (!(dialogue == null))
		{
			dialogue.Close();
			dialogue = null;
			state = State.Inactive;
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + dialogueOffset, Mathf.Min(5f, interactionDistance));
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + dialogueOffset, Mathf.Min(6f, interactionDistance + 1f));
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + interactionPoint, interactionDistance);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + interactionPoint, interactionDistance + 1f);
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		if (Application.isPlaying)
		{
			switch (interactor)
			{
			case Interactor.Cuphead:
				DrawGizmoLineToPlayer(PlayerId.PlayerOne, PlayerWithinDistance(PlayerId.PlayerOne));
				break;
			case Interactor.Mugman:
				DrawGizmoLineToPlayer(PlayerId.PlayerTwo, PlayerWithinDistance(PlayerId.PlayerTwo));
				break;
			case Interactor.Either:
				DrawGizmoLineToPlayer(PlayerId.PlayerOne, PlayerWithinDistance(PlayerId.PlayerOne));
				DrawGizmoLineToPlayer(PlayerId.PlayerTwo, PlayerWithinDistance(PlayerId.PlayerTwo));
				break;
			case Interactor.Both:
				DrawGizmoLineToPlayer(PlayerId.PlayerOne, PlayerWithinDistance(PlayerId.PlayerOne) && PlayerWithinDistance(PlayerId.PlayerTwo));
				DrawGizmoLineToPlayer(PlayerId.PlayerTwo, PlayerWithinDistance(PlayerId.PlayerOne) && PlayerWithinDistance(PlayerId.PlayerTwo));
				break;
			}
		}
	}

	private void DrawGizmoLineToPlayer(PlayerId id, bool valid)
	{
		if (!(PlayerManager.GetPlayer(id) == null))
		{
			Gizmos.color = ((!valid) ? Color.red : Color.green);
			Gizmos.DrawLine((Vector2)base.transform.position + interactionPoint, PlayerManager.GetPlayer(id).transform.position);
		}
	}
}
