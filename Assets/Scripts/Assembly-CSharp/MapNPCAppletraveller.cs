using System;
using UnityEngine;

public class MapNPCAppletraveller : MonoBehaviour
{
	private enum AppletravellerState
	{
		idle = 0,
		wave = 1,
		wait = 2
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private int dialoguerVariableID = 6;

	[SerializeField]
	private string coinID1 = Guid.NewGuid().ToString();

	[SerializeField]
	private string coinID2 = Guid.NewGuid().ToString();

	[SerializeField]
	private string coinID3 = Guid.NewGuid().ToString();

	[SerializeField]
	private float radiusStartWaving = 50f;

	[SerializeField]
	private float radiusStopWaving = 10f;

	private float squareRadiusStartWaving;

	private float squareRadiusStopWaving;

	private AppletravellerState state;

	private void Start()
	{
		squareRadiusStartWaving = radiusStartWaving * radiusStartWaving;
		squareRadiusStopWaving = radiusStopWaving * radiusStopWaving;
		AddDialoguerEvents();
	}

	private void OnDestroy()
	{
		RemoveDialoguerEvents();
	}

	private void Update()
	{
		if ((state == AppletravellerState.idle && (base.transform.position - Map.Current.players[0].transform.position).sqrMagnitude < squareRadiusStartWaving) || (PlayerManager.Multiplayer && (base.transform.position - Map.Current.players[1].transform.position).sqrMagnitude < squareRadiusStartWaving))
		{
			state = AppletravellerState.wave;
			animator.SetTrigger("wave");
		}
		else if (state == AppletravellerState.wave)
		{
			if ((base.transform.position - Map.Current.players[0].transform.position).sqrMagnitude > squareRadiusStartWaving && (!PlayerManager.Multiplayer || (base.transform.position - Map.Current.players[1].transform.position).sqrMagnitude > squareRadiusStartWaving))
			{
				state = AppletravellerState.idle;
				animator.SetTrigger("next");
			}
			if ((base.transform.position - Map.Current.players[0].transform.position).sqrMagnitude < squareRadiusStopWaving || (PlayerManager.Multiplayer && (base.transform.position - Map.Current.players[1].transform.position).sqrMagnitude < squareRadiusStopWaving))
			{
				state = AppletravellerState.wait;
				animator.SetTrigger("next");
			}
		}
		else if ((base.transform.position - Map.Current.players[0].transform.position).sqrMagnitude > squareRadiusStartWaving && (!PlayerManager.Multiplayer || (base.transform.position - Map.Current.players[1].transform.position).sqrMagnitude > squareRadiusStartWaving))
		{
			state = AppletravellerState.idle;
		}
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, radiusStartWaving);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, radiusStopWaving);
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
		if (message == "MacCoin" && !PlayerData.Data.coinManager.GetCoinCollected(coinID1))
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 1f);
			PlayerData.Data.coinManager.SetCoinValue(coinID1, true, PlayerId.Any);
			PlayerData.Data.coinManager.SetCoinValue(coinID2, true, PlayerId.Any);
			PlayerData.Data.coinManager.SetCoinValue(coinID3, true, PlayerId.Any);
			PlayerData.Data.AddCurrency(PlayerId.PlayerOne, 3);
			PlayerData.Data.AddCurrency(PlayerId.PlayerTwo, 3);
			PlayerData.SaveCurrentFile();
			MapEventNotification.Current.ShowEvent(MapEventNotification.Type.ThreeCoins);
		}
	}
}
