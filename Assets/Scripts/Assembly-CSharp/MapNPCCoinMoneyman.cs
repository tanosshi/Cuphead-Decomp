using UnityEngine;

public class MapNPCCoinMoneyman : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float idleDurationMin;

	[SerializeField]
	private float idleDurationMax;

	private float durationBeforeNext;

	private float durationBeforeBlink;

	private bool waiting = true;

	[SerializeField]
	private string[] hiddenCoinIds;

	[SerializeField]
	private int dialoguerVariableID = 4;

	private void Start()
	{
		UpdateCoins();
		LookAroundFinished();
	}

	public void UpdateCoins()
	{
		for (int i = 0; i < hiddenCoinIds.Length; i++)
		{
			if (!PlayerData.Data.coinManager.GetCoinCollected(hiddenCoinIds[i]))
			{
				return;
			}
		}
		Dialoguer.SetGlobalFloat(dialoguerVariableID, 1f);
		PlayerData.SaveCurrentFile();
	}

	private void Update()
	{
		if (!waiting)
		{
			durationBeforeNext -= CupheadTime.Delta;
			durationBeforeBlink -= CupheadTime.Delta;
			if (durationBeforeBlink <= 0f)
			{
				durationBeforeBlink = float.PositiveInfinity;
				animator.SetTrigger("blink");
			}
			if (durationBeforeNext <= 0f)
			{
				waiting = true;
				animator.SetTrigger("next");
			}
		}
	}

	private void LookAroundFinished()
	{
		durationBeforeNext = Random.Range(idleDurationMin, idleDurationMax);
		durationBeforeBlink = Random.Range(0f, durationBeforeNext);
		waiting = false;
	}
}
