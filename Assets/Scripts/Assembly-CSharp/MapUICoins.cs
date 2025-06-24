using UnityEngine;
using UnityEngine.UI;

public class MapUICoins : MonoBehaviour
{
	[SerializeField]
	private PlayerId playerId;

	[SerializeField]
	private Image coinImage;

	[SerializeField]
	private Image currencyNbImage;

	[SerializeField]
	private Sprite[] coinSprites;

	[SerializeField]
	private Transform doubleDigitCoinTransform;

	private Vector3 singleDigitCoinPosition;

	private int previousCurrency = -1;

	private void Start()
	{
		singleDigitCoinPosition = coinImage.transform.localPosition;
	}

	private void Update()
	{
		if (playerId == PlayerId.PlayerTwo)
		{
			if (!PlayerManager.Multiplayer)
			{
				coinImage.enabled = false;
				currencyNbImage.enabled = false;
				return;
			}
			coinImage.enabled = true;
			currencyNbImage.enabled = true;
		}
		int currency = PlayerData.Data.GetCurrency(playerId);
		if (currency != previousCurrency)
		{
			previousCurrency = currency;
			currencyNbImage.sprite = coinSprites[currency];
			if (currency > 9)
			{
				coinImage.transform.localPosition = doubleDigitCoinTransform.localPosition;
			}
			else
			{
				coinImage.transform.localPosition = singleDigitCoinPosition;
			}
		}
	}
}
