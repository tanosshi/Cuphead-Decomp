using UnityEngine;

public class ShopScenePig : AbstractMonoBehaviour
{
	private const int CLOCK_LOOPS_MIN = 20;

	private const int CLOCK_LOOPS_MAX = 35;

	private int idleLoopsMax = 35;

	private int idleLoops;

	private void OnIdleLoop()
	{
		idleLoops++;
		if (idleLoops >= idleLoopsMax)
		{
			base.animator.SetTrigger("OnClock");
			idleLoopsMax = Random.Range(20, 35);
			idleLoops = 0;
		}
	}

	public void OnStart()
	{
		AudioManager.Play("shop_pig_welcome");
		base.animator.Play("Welcome");
	}

	public void OnPurchase()
	{
		AudioManager.Play("shop_pig_nod");
		base.animator.Play("Nod");
	}

	public void OnExit()
	{
		AudioManager.Play("shop_pig_bye");
		base.animator.Play("Bye");
	}
}
