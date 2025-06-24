using System;

[Serializable]
public struct CoinPositionAndID
{
	public string CoinID;

	public float xPos;

	public CoinPositionAndID(string id, float pos)
	{
		CoinID = id;
		xPos = pos;
	}
}
