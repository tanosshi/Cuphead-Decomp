using UnityEngine;

public class PirateLevelBoatSail : AbstractMonoBehaviour
{
	[Space(10f)]
	[Range(1f, 20f)]
	[SerializeField]
	private int regularLoopsMin = 3;

	[Range(1f, 20f)]
	[SerializeField]
	private int regularLoopsMax = 5;

	[Space(10f)]
	[Range(1f, 20f)]
	[SerializeField]
	private int fastLoopsMin = 5;

	[Range(1f, 20f)]
	[SerializeField]
	private int fastLoopsMax = 9;

	private int reg;

	private int fast;

	private int regTarget = 4;

	private int fastTarget = 7;

	private new Animator animator;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
	}

	private void RegularEnded()
	{
		if (reg >= regTarget)
		{
			StartFast();
		}
		else
		{
			reg++;
		}
	}

	private void FastEnded()
	{
		if (fast >= fastTarget)
		{
			StartReg();
		}
		else
		{
			fast++;
		}
	}

	private void StartReg()
	{
		regTarget = Random.Range(regularLoopsMin, regularLoopsMax + 1);
		reg = 0;
		animator.SetBool("Fast", false);
	}

	private void StartFast()
	{
		fastTarget = Random.Range(fastLoopsMin, fastLoopsMax + 1);
		fast = 0;
		animator.SetBool("Fast", true);
	}
}
