using UnityEngine;

public class MapNPCTurtle : MapDialogueInteraction
{
	[SerializeField]
	private BoxCollider2D colliderB;

	[SerializeField]
	private int dialoguerVariableID = 19;

	protected override void Start()
	{
		base.Start();
		Dialoguer.events.onEnded += OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded += OnDialogueEndedHandler;
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
		if (Dialoguer.GetGlobalFloat(dialoguerVariableID) < 2f)
		{
			if (PlayerData.Data.CheckLevelsHaveMinGrade(Level.platformingLevels, LevelScoringData.Grade.P))
			{
				Dialoguer.SetGlobalFloat(dialoguerVariableID, 2f);
				PlayerData.SaveCurrentFile();
			}
			else if (Dialoguer.GetGlobalFloat(dialoguerVariableID) < 1f && PlayerData.Data.CountLevelsHaveMinGrade(Level.platformingLevels, LevelScoringData.Grade.P) > 1)
			{
				Dialoguer.SetGlobalFloat(dialoguerVariableID, 1f);
				PlayerData.SaveCurrentFile();
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Dialoguer.events.onEnded -= OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded -= OnDialogueEndedHandler;
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (message == "Pacifist")
		{
			MapEventNotification.Current.ShowTooltipEvent(TooltipEvent.Turtle);
			PlayerData.Data.unlockedBlackAndWhite = true;
			PlayerData.SaveCurrentFile();
			MapUI.Current.Refresh();
		}
	}

	protected override void Activate(MapPlayerController player)
	{
		if (dialogues[(int)player.id].transform.localScale.x == 1f)
		{
			base.Activate(player);
			if (colliderB.OverlapPoint(player.transform.position))
			{
				base.animator.SetTrigger("turn_b");
			}
			else
			{
				base.animator.SetTrigger("turn_a");
			}
		}
	}

	private void OnDialogueEndedHandler()
	{
		base.animator.SetTrigger("return");
	}
}
