using System;
using UnityEngine;

public class MapCoin : MonoBehaviour
{
	[SerializeField]
	private MapNPCCoinMoneyman mapNPCCoinMoneyman;

	public string coinID = Guid.NewGuid().ToString();

	private void Start()
	{
		if (PlayerData.Data.coinManager.GetCoinCollected(coinID))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!PlayerData.Data.coinManager.GetCoinCollected(coinID))
		{
			PlayerData.Data.coinManager.SetCoinValue(coinID, true, PlayerId.Any);
			PlayerData.Data.AddCurrency(PlayerId.PlayerOne, 1);
			PlayerData.Data.AddCurrency(PlayerId.PlayerTwo, 1);
			PlayerData.SaveCurrentFile();
			MapEventNotification.Current.ShowEvent(MapEventNotification.Type.Coin);
			if (mapNPCCoinMoneyman != null)
			{
				mapNPCCoinMoneyman.UpdateCoins();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
