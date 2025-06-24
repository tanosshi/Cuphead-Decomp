using UnityEngine;

public class MapNPCFishgirl : MonoBehaviour
{
	private void Start()
	{
		if (PlayerData.Data.CheckLevelsHaveMinDifficulty(new Levels[1] { Levels.Mausoleum }, Level.Mode.Easy))
		{
			Dialoguer.SetGlobalFloat(12, 1f);
		}
	}
}
