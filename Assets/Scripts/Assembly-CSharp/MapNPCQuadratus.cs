using System.Collections;
using UnityEngine;

public class MapNPCQuadratus : AbstractMapInteractiveEntity
{
	[SerializeField]
	private SpriteRenderer entitySpriteRenderer;

	[SerializeField]
	private SpriteRenderer rippleEffectSpriteRenderer;

	[SerializeField]
	private int dialoguerVariableID = 15;

	[SerializeField]
	private int dialoguerScholarVariableID = 11;

	protected override void Start()
	{
		base.Start();
		AddDialoguerEvents();
		if (entitySpriteRenderer == null)
		{
			entitySpriteRenderer = GetComponent<SpriteRenderer>();
		}
		MapDialogueInteraction component = GetComponent<MapDialogueInteraction>();
		int num = PlayerData.Data.DeathCount(PlayerId.Any);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		RemoveDialoguerEvents();
	}

	public void AddDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
		Dialoguer.events.onEnded += OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded += OnDialogueEndedHandler;
	}

	public void RemoveDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
		Dialoguer.events.onEnded -= OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded -= OnDialogueEndedHandler;
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (!(message == "QuadratusGift"))
		{
		}
	}

	protected override void Activate()
	{
	}

	protected override MapUIInteractionDialogue Show(PlayerInput player)
	{
		Debug.Log("Show");
		Dialoguer.SetGlobalFloat(dialoguerScholarVariableID, 3f);
		PlayerData.SaveCurrentFile();
		StartCoroutine(tween_cr(entitySpriteRenderer.color.a, 0.65f, EaseUtils.EaseType.easeInOutCubic, 0.5f));
		return null;
	}

	public override void Hide(MapUIInteractionDialogue dialogue)
	{
		if (Map.Current.CurrentState == Map.State.Ready && Map.Current.players[0].state == MapPlayerController.State.Walking)
		{
			Debug.Log("Hide");
			StartCoroutine(tween_cr(entitySpriteRenderer.color.a, 0f, EaseUtils.EaseType.easeInOutCubic, 0.5f));
		}
	}

	private void OnDialogueEndedHandler()
	{
	}

	private IEnumerator tween_cr(float start, float end, EaseUtils.EaseType ease, float time)
	{
		if (start != end)
		{
			float t = 0f;
			Color currentColor = Color.white;
			currentColor.a = start;
			entitySpriteRenderer.color = currentColor;
			rippleEffectSpriteRenderer.color = currentColor;
			while (t < time)
			{
				float val = EaseUtils.Ease(ease, start, end, t / time);
				currentColor.a = val;
				entitySpriteRenderer.color = currentColor;
				rippleEffectSpriteRenderer.color = currentColor;
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			currentColor.a = end;
			entitySpriteRenderer.color = currentColor;
			rippleEffectSpriteRenderer.color = currentColor;
			yield return null;
		}
	}
}
