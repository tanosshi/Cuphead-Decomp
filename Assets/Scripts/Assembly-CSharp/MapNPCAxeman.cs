using UnityEngine;

public class MapNPCAxeman : MonoBehaviour
{
	public Vector3 positionAfterWorld1;

	[SerializeField]
	private int dialoguerVariableID = 3;

	private void Start()
	{
		if (PlayerData.Data.CheckLevelsCompleted(Level.world1BossLevels))
		{
			Dialoguer.SetGlobalFloat(dialoguerVariableID, 1f);
			base.transform.position = positionAfterWorld1;
		}
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(positionAfterWorld1, 0.5f);
	}
}
