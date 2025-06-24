using System.Collections;
using UnityEngine;

public class DialogueInteractionPoint : SpeechInteractionPoint
{
	[SerializeField]
	protected SpeechBubble speechBubble;

	[SerializeField]
	public DialoguerDialogues dialogueInteraction;

	private int kettleStateIndex;

	[SerializeField]
	private Vector2 speechBubblePosition;

	public Vector2 playerOneDialoguePosition;

	public Vector2 playerTwoDialoguePosition;

	public Animator animatorOnStart;

	public string animationTriggerOnStart;

	public string animationOnStartTextName;

	public Animator animatorOnEnd;

	public string animationTriggerOnEnd;

	public string animationOnGiveBackInput;

	public string animationOnGiveBackInputAtEnd;

	private Coroutine cutsceneCoroutine;

	[HideInInspector]
	public bool conversationIsActive;

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = new Color(44f / 51f, 44f / 51f, 44f / 51f);
		Vector3 position = base.transform.position;
		position.x += speechBubblePosition.x;
		position.y += speechBubblePosition.y;
		Gizmos.DrawWireSphere(position, interactionDistance * 0.5f);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(playerOneDialoguePosition, 10f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(playerTwoDialoguePosition, 10f);
	}

	protected override void Start()
	{
		base.Start();
		Dialoguer.events.onEnded += OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded += OnDialogueEndedHandler;
		PlayerManager.OnPlayerJoinedEvent += OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent += OnPlayerLeave;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Dialoguer.events.onEnded -= OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded -= OnDialogueEndedHandler;
		PlayerManager.OnPlayerJoinedEvent -= OnPlayerJoined;
		PlayerManager.OnPlayerLeaveEvent -= OnPlayerLeave;
	}

	private void OnDialogueEndedHandler()
	{
		if (AbleToActivate())
		{
			Show(PlayerId.PlayerOne);
		}
	}

	protected override void Activate()
	{
		if (speechBubble.displayState == SpeechBubble.DisplayState.Hidden)
		{
			Vector3 position = base.transform.position;
			position.x += speechBubblePosition.x;
			position.y += speechBubblePosition.y;
			speechBubble.basePosition = position;
			if (cutsceneCoroutine != null)
			{
				StopCoroutine(cutsceneCoroutine);
			}
			cutsceneCoroutine = StartCoroutine(CutScene_cr());
		}
	}

	private void OnPlayerJoined(PlayerId player)
	{
		if (player == PlayerId.PlayerTwo)
		{
			AbstractPlayerController player2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			player2.OnReviveEvent += OnRevive;
		}
	}

	private IEnumerator move_cr(AbstractPlayerController player, float xPosition)
	{
		if (player == null)
		{
			yield break;
		}
		yield return null;
		while (!player.gameObject.activeSelf)
		{
			yield return null;
		}
		LevelPlayerMotor playerMotor = null;
		LevelPlayerWeaponManager playerWeaponManager = null;
		if (!player)
		{
			yield break;
		}
		playerMotor = player.GetComponent<LevelPlayerMotor>();
		playerWeaponManager = player.GetComponent<LevelPlayerWeaponManager>();
		if ((bool)playerWeaponManager)
		{
			playerWeaponManager.DisableInput();
		}
		if ((bool)playerMotor)
		{
			while (playerMotor.Dashing)
			{
				yield return null;
			}
			playerMotor.DisableInput();
			yield return playerMotor.StartCoroutine(playerMotor.MoveToX_cr(xPosition));
		}
		DialoguerEvents.EndedHandler afterDialogue = null;
		afterDialogue = delegate
		{
			Dialoguer.events.onEnded -= afterDialogue;
			StartCoroutine(ReactivateInputsCoroutine(playerMotor, null, playerWeaponManager, null, animatorOnEnd));
		};
		Dialoguer.events.onEnded += afterDialogue;
	}

	private void OnPlayerLeave(PlayerId player)
	{
		if (player != PlayerId.PlayerTwo)
		{
		}
	}

	public void OnRevive(Vector3 pos)
	{
		if (conversationIsActive)
		{
			AbstractPlayerController player = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			StartCoroutine(move_cr(player, playerTwoDialoguePosition.x));
		}
	}

	private IEnumerator CutScene_cr()
	{
		if (speechBubble.displayState != SpeechBubble.DisplayState.Hidden)
		{
			yield break;
		}
		Coroutine playerOneMove = null;
		Coroutine playerTwoMove = null;
		conversationIsActive = true;
		AbstractPlayerController playerOne = PlayerManager.GetPlayer(PlayerId.PlayerOne);
		AbstractPlayerController playerTwo = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
		playerOneMove = StartCoroutine(move_cr(playerOne, playerOneDialoguePosition.x));
		playerTwoMove = StartCoroutine(move_cr(playerTwo, playerTwoDialoguePosition.x));
		yield return playerOneMove;
		yield return playerTwoMove;
		if (animatorOnStart != null)
		{
			animatorOnStart.SetTrigger(animationTriggerOnStart);
			while (!animatorOnStart.GetCurrentAnimatorStateInfo(0).IsName(animationOnStartTextName))
			{
				yield return null;
			}
		}
		Dialoguer.StartDialogue(dialogueInteraction);
		DialoguerEvents.EndedHandler afterDialogue = null;
		afterDialogue = delegate
		{
			Dialoguer.events.onEnded -= afterDialogue;
			if (animatorOnEnd != null)
			{
				animatorOnEnd.SetTrigger(animationTriggerOnEnd);
			}
			playerOne = PlayerManager.GetPlayer(PlayerId.PlayerOne);
			playerTwo = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			LevelPlayerMotor playerTwoMotor = null;
			LevelPlayerWeaponManager playerTwoWeaponManager = null;
			if (playerTwo != null)
			{
				playerTwoMotor = playerTwo.GetComponent<LevelPlayerMotor>();
				playerTwoWeaponManager = playerTwo.GetComponent<LevelPlayerWeaponManager>();
			}
			conversationIsActive = false;
			StartCoroutine(ReactivateInputsCoroutine(playerOne.GetComponent<LevelPlayerMotor>(), playerTwoMotor, playerOne.GetComponent<LevelPlayerWeaponManager>(), playerTwoWeaponManager, animatorOnEnd));
		};
		Dialoguer.events.onEnded += afterDialogue;
	}

	protected virtual IEnumerator ReactivateInputsCoroutine(LevelPlayerMotor playerOneMotor, LevelPlayerMotor playerTwoMotor, LevelPlayerWeaponManager playerOneWeaponManager, LevelPlayerWeaponManager playerTwoWeaponManager, Animator animator)
	{
		if (animator != null)
		{
			if (animationOnGiveBackInputAtEnd != null && animationOnGiveBackInputAtEnd != string.Empty)
			{
				while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationOnGiveBackInput) && (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationOnGiveBackInputAtEnd) || !((double)animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99)))
				{
					yield return null;
				}
			}
			else
			{
				while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationOnGiveBackInput))
				{
					yield return null;
				}
			}
		}
		playerOneMotor.ClearBufferedInput();
		playerOneMotor.EnableInput();
		playerOneWeaponManager.EnableInput();
		if ((bool)playerTwoMotor)
		{
			playerTwoMotor.ClearBufferedInput();
			playerTwoMotor.EnableInput();
		}
		if ((bool)playerTwoWeaponManager)
		{
			playerTwoWeaponManager.EnableInput();
		}
	}
}
