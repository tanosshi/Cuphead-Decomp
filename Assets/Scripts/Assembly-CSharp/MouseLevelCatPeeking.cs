using System.Collections;
using UnityEngine;

public class MouseLevelCatPeeking : MonoBehaviour
{
	private const string CatPeekParameterName = "Peek";

	private const string IsRightParameterName = "IsRight";

	[SerializeField]
	private Animator catAnimator;

	[SerializeField]
	private MinMax catDelay;

	[SerializeField]
	private MinMax catRotationRange;

	[Range(0f, 1f)]
	[SerializeField]
	private float peek1Threshold;

	[Range(0f, 1f)]
	[SerializeField]
	private float peek2Threshold;

	private bool isPhase2;

	private IEnumerator peekRoutine;

	public float Peek1Threshold
	{
		get
		{
			return peek1Threshold;
		}
	}

	public float Peek2Threshold
	{
		get
		{
			return peek2Threshold;
		}
	}

	public bool IsPhase2
	{
		get
		{
			return isPhase2;
		}
		set
		{
			isPhase2 = value;
			catAnimator.SetBool("IsPhase2", value);
		}
	}

	public void StartPeeking()
	{
		peekRoutine = catPeeking_cr();
		StartCoroutine(peekRoutine);
	}

	public void StopPeeking()
	{
		StopCoroutine(peekRoutine);
	}

	private IEnumerator catPeeking_cr()
	{
		Transform catTransform = base.transform;
		while (true)
		{
			bool isRight = Rand.Bool();
			catAnimator.SetBool("IsRight", isRight);
			catTransform.eulerAngles = Vector3.forward * catRotationRange.RandomFloat();
			catAnimator.SetTrigger("Peek");
			yield return null;
			yield return catAnimator.WaitForAnimationToEnd(this, true);
			yield return CupheadTime.WaitForSeconds(this, catDelay.RandomFloat());
		}
	}
}
