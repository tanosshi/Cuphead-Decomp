using UnityEngine;

public class MapNPCProfessional : MonoBehaviour
{
	[SerializeField]
	private int dialoguerVariableID = 20;

	private void Start()
	{
		AddDialoguerEvents();
		if (!(Dialoguer.GetGlobalFloat(dialoguerVariableID) < 3f))
		{
			return;
		}
		int num = PlayerData.Data.CountLevelsHaveMinGrade(Level.world1BossLevels, LevelScoringData.Grade.AMinus);
		num += PlayerData.Data.CountLevelsHaveMinGrade(Level.world2BossLevels, LevelScoringData.Grade.AMinus);
		num += PlayerData.Data.CountLevelsHaveMinGrade(Level.world3BossLevels, LevelScoringData.Grade.AMinus);
		num += PlayerData.Data.CountLevelsHaveMinGrade(Level.world4BossLevels, LevelScoringData.Grade.AMinus);
		num += PlayerData.Data.CountLevelsHaveMinGrade(Level.platformingLevels, LevelScoringData.Grade.AMinus);
		if (num >= 15)
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 3f);
			PlayerData.SaveCurrentFile();
		}
		else if (num >= 10)
		{
			if (Dialoguer.GetGlobalFloat(dialoguerVariableID) < 2f)
			{
				Dialoguer.SetGlobalFloat(dialoguerVariableID, 2f);
				PlayerData.SaveCurrentFile();
			}
		}
		else if (num >= 5 && Dialoguer.GetGlobalFloat(dialoguerVariableID) < 1f)
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 1f);
			PlayerData.SaveCurrentFile();
		}
	}

	private void OnDestroy()
	{
		RemoveDialoguerEvents();
	}

	public void AddDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
	}

	public void RemoveDialoguerEvents()
	{
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (message == "RetroColorUnlock")
		{
			MapEventNotification.Current.ShowTooltipEvent(TooltipEvent.Professional);
			PlayerData.Data.unlocked2Strip = true;
			PlayerData.SaveCurrentFile();
			MapUI.Current.Refresh();
		}
	}
}
