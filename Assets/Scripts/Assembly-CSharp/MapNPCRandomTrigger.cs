using UnityEngine;

public class MapNPCRandomTrigger : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private int triggerMinFrequency = 3;

	[SerializeField]
	private int triggerMaxFrequency = 5;

	public string trigger = "blink";

	private int loopToWait;

	private void Start()
	{
		loopToWait = Random.Range(triggerMinFrequency, triggerMaxFrequency + 1);
	}

	private void Looped()
	{
		loopToWait--;
		if (loopToWait <= 0)
		{
			loopToWait = Random.Range(triggerMinFrequency, triggerMaxFrequency + 1);
			Trigger();
		}
	}

	private void Trigger()
	{
		animator.SetTrigger(trigger);
	}
}
