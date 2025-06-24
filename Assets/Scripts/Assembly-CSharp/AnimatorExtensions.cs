using System.Collections;
using UnityEngine;

public static class AnimatorExtensions
{
	public static Coroutine WaitForAnimationToStart(this Animator animator, MonoBehaviour parent, string animationName, bool waitForEndOfFrame = false)
	{
		return parent.StartCoroutine(waitForAnimStart_cr(parent, animator, 0, animationName, waitForEndOfFrame));
	}

	public static Coroutine WaitForAnimationToStart(this Animator animator, MonoBehaviour parent, string animationName, int layer, bool waitForEndOfFrame = false)
	{
		return parent.StartCoroutine(waitForAnimStart_cr(parent, animator, layer, animationName, waitForEndOfFrame));
	}

	private static IEnumerator waitForAnimStart_cr(MonoBehaviour parent, Animator animator, int layer, string animationName, bool waitForEndOfFrame)
	{
		int target = Animator.StringToHash(animator.GetLayerName(layer) + "." + animationName);
		while (animator.GetCurrentAnimatorStateInfo(layer).fullPathHash != target)
		{
			if (waitForEndOfFrame)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return null;
			}
		}
	}

	public static Coroutine WaitForAnimationToEnd(this Animator animator, MonoBehaviour parent, bool waitForEndOfFrame = false)
	{
		return parent.StartCoroutine(waitForAnimEnd_cr(parent, animator, 0, waitForEndOfFrame));
	}

	private static IEnumerator waitForAnimEnd_cr(MonoBehaviour parent, Animator animator, int layer, bool waitForEndOfFrame)
	{
		int current = animator.GetCurrentAnimatorStateInfo(layer).fullPathHash;
		while (current == animator.GetCurrentAnimatorStateInfo(layer).fullPathHash)
		{
			if (waitForEndOfFrame)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return null;
			}
		}
	}

	public static Coroutine WaitForAnimationToEnd(this Animator animator, MonoBehaviour parent, string name, bool waitForEndOfFrame = false)
	{
		return parent.StartCoroutine(waitForNamedAnimEnd_cr(parent, animator, name, 0, waitForEndOfFrame));
	}

	public static Coroutine WaitForAnimationToEnd(this Animator animator, MonoBehaviour parent, string name, int layer, bool waitForEndOfFrame = false)
	{
		return parent.StartCoroutine(waitForNamedAnimEnd_cr(parent, animator, name, layer, waitForEndOfFrame));
	}

	private static IEnumerator waitForNamedAnimEnd_cr(MonoBehaviour parent, Animator animator, string name, int layer, bool waitForEndOfFrame)
	{
		yield return parent.StartCoroutine(waitForAnimStart_cr(parent, animator, layer, name, waitForEndOfFrame));
		int target = Animator.StringToHash(animator.GetLayerName(layer) + "." + name);
		while (animator.GetCurrentAnimatorStateInfo(layer).fullPathHash == target)
		{
			if (waitForEndOfFrame)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return null;
			}
		}
	}
}
