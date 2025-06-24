using System.Collections;
using UnityEngine;

public class MapDialogueInteraction : AbstractMapInteractiveEntity
{
	[SerializeField]
	private SpeechBubble speechBubblePrefab;

	[SerializeField]
	private Vector2 speechBubblePosition;

	[SerializeField]
	private Vector2 panCameraToPosition;

	[SerializeField]
	private int maxLines = -1;

	[SerializeField]
	private bool tailOnTheLeft;

	protected SpeechBubble speechBubble;

	public DialoguerDialogues dialogueInteraction;

	private Coroutine cutsceneCoroutine;

	protected override void Start()
	{
		base.Start();
		if (speechBubble == null)
		{
			Vector3 position = base.transform.position;
			position.x += speechBubblePosition.x;
			position.y += speechBubblePosition.y;
			speechBubble = SpeechBubble.Instance;
			if (speechBubble == null)
			{
				speechBubble = Object.Instantiate(speechBubblePrefab.gameObject, position, Quaternion.identity, MapUI.Current.sceneCanvas.transform).GetComponent<SpeechBubble>();
			}
		}
		Dialoguer.events.onEnded += OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded += OnDialogueEndedHandler;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Dialoguer.events.onEnded -= OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded -= OnDialogueEndedHandler;
	}

	private void OnDialogueEndedHandler()
	{
		Check();
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Vector3 position = base.transform.position;
		position.x += speechBubblePosition.x;
		position.y += speechBubblePosition.y;
		Gizmos.DrawWireSphere(position, interactionDistance * 0.5f);
		position = base.transform.position;
		position.x += panCameraToPosition.x;
		position.y += panCameraToPosition.y;
		Gizmos.DrawWireSphere(position, interactionDistance * 0.5f);
	}

	protected override void Activate(MapPlayerController player)
	{
		if (!(dialogues[(int)player.id] != null) || dialogues[(int)player.id].transform.localScale.x != 1f)
		{
			return;
		}
		base.Activate(player);
		if (speechBubble.displayState == SpeechBubble.DisplayState.Hidden)
		{
			Vector3 position = base.transform.position;
			position.x += speechBubblePosition.x;
			position.y += speechBubblePosition.y;
			speechBubble.basePosition = position;
			position = base.transform.position;
			position.x += panCameraToPosition.x;
			position.y += panCameraToPosition.y;
			speechBubble.panPosition = position;
			speechBubble.maxLines = maxLines;
			speechBubble.tailOnTheLeft = tailOnTheLeft;
			if (cutsceneCoroutine != null)
			{
				StopCoroutine(cutsceneCoroutine);
			}
			cutsceneCoroutine = StartCoroutine(CutScene_cr());
		}
	}

	private IEnumerator CutScene_cr()
	{
		if (speechBubble.displayState != SpeechBubble.DisplayState.Hidden)
		{
			yield break;
		}
		for (int i = 0; i < Map.Current.players.Length; i++)
		{
			if (!(Map.Current.players[i] == null))
			{
				Map.Current.players[i].Disable();
			}
		}
		Coroutine playerOneMove = null;
		Coroutine playerTwoMove = null;
		yield return null;
		Dialoguer.StartDialogue(dialogueInteraction);
		DialoguerEvents.EndedHandler afterDialogue = null;
		afterDialogue = delegate
		{
			Dialoguer.events.onEnded -= afterDialogue;
			StartCoroutine(reactivate_input_cr());
		};
		Dialoguer.events.onEnded += afterDialogue;
	}

	private IEnumerator reactivate_input_cr()
	{
		if (CupheadMapCamera.Current != null)
		{
			CupheadMapCamera.Current.SetActiveCollider(false);
		}
		while (CupheadMapCamera.Current != null && CupheadMapCamera.Current.IsCameraFarFromPlayer())
		{
			yield return null;
		}
		if (CupheadMapCamera.Current != null)
		{
			CupheadMapCamera.Current.SetActiveCollider(true);
		}
		for (int i = 0; i < Map.Current.players.Length; i++)
		{
			if (!(Map.Current.players[i] == null))
			{
				Map.Current.players[i].Enable();
			}
		}
	}
}
