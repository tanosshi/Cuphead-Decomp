using System;
using UnityEngine;

public class MapNPCJuggler : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private int dialoguerVariableID;

	[SerializeField]
	private string coinID = Guid.NewGuid().ToString();

	private void Start()
	{
		AddDialoguerEvents();
		if (Dialoguer.GetGlobalFloat(dialoguerVariableID) == 1f)
		{
			animator.SetTrigger("three");
			return;
		}
		int numParriesInRow = PlayerData.Data.GetNumParriesInRow(PlayerId.Any);
		if (numParriesInRow <= 1)
		{
			animator.SetTrigger("one");
		}
		else if (numParriesInRow == 2)
		{
			animator.SetTrigger("two");
		}
		else if (numParriesInRow == 3)
		{
			animator.SetTrigger("three");
		}
		else if (numParriesInRow > 3)
		{
			animator.SetTrigger("three");
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
		if (message == "JugglerCoin" && !PlayerData.Data.coinManager.GetCoinCollected(coinID))
		{
			PlayerData.Data.coinManager.SetCoinValue(coinID, true, PlayerId.Any);
			PlayerData.Data.AddCurrency(PlayerId.PlayerOne, 1);
			PlayerData.Data.AddCurrency(PlayerId.PlayerTwo, 1);
			PlayerData.SaveCurrentFile();
			MapEventNotification.Current.ShowEvent(MapEventNotification.Type.Coin);
		}
	}
}
