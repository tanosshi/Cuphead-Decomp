using UnityEngine;

public class FlowerLevelFlowerAnimator : AbstractPausableComponent
{
	private const int MIN_IDLE_LOOPS = 2;

	private const int MAX_IDLE_LOOPS = 4;

	private int patchChance = 25;

	private int bothChance = 15;

	private int loops;

	private int max = 2;

	private new Animator animator;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
	}

	private void OnIdleEnd()
	{
		if (loops >= max)
		{
			OnBlink();
		}
		else
		{
			loops++;
		}
	}

	private void OnBlink()
	{
		animator.SetTrigger("OnBlink");
		max = Random.Range(2, 5);
		loops = 0;
	}
}
