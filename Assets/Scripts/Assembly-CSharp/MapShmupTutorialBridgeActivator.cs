using System.Collections;
using UnityEngine;

public class MapShmupTutorialBridgeActivator : MonoBehaviour
{
	[SerializeField]
	private MapLevelDependentObstacle blueprintObstacle;

	[SerializeField]
	private float DoTransitionDelay;

	[SerializeField]
	private int dialoguerVariableID = 5;

	private void Start()
	{
		if (!PlayerData.Data.IsFlyingTutorialCompleted && Level.PreviousLevel == Levels.ShmupTutorial)
		{
			PlayerData.Data.IsFlyingTutorialCompleted = true;
			blueprintObstacle.OnConditionNotMet();
			StartCoroutine(DoTransition());
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 1f);
			PlayerData.SaveCurrentFile();
		}
		else if (!PlayerData.Data.IsFlyingTutorialCompleted)
		{
			blueprintObstacle.OnConditionNotMet();
		}
		else
		{
			blueprintObstacle.OnConditionAlreadyMet();
		}
	}

	private IEnumerator DoTransition()
	{
		yield return CupheadTime.WaitForSeconds(this, DoTransitionDelay);
		blueprintObstacle.DoTransition();
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		blueprintObstacle.OnConditionAlreadyMet();
	}
}
