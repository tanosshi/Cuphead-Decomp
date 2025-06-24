using UnityEngine;

public class DiceGateLevelKingDice : MonoBehaviour
{
	public void SetDisappearBool()
	{
		PlayerData.Data.CurrentMapData.hasKingDiceDisappeared = true;
		PlayerData.SaveCurrentFile();
	}

	private void SoundKingDiceExitAnim()
	{
		AudioManager.Play("dicegate_kingdice_exit");
	}
}
