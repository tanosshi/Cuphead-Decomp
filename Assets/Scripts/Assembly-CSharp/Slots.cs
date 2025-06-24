using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Slots
{
	public enum Mode
	{
		Snake = 0,
		Tiger = 1,
		Bison = 2,
		Oni = 3
	}

	private const float DELAY = 0.2f;

	[SerializeField]
	private FrogsLevelMorphedSlot left;

	[SerializeField]
	private FrogsLevelMorphedSlot mid;

	[SerializeField]
	private FrogsLevelMorphedSlot right;

	private MonoBehaviour parent;

	public void Init(MonoBehaviour parent)
	{
		this.parent = parent;
	}

	public void Spin()
	{
		parent.StartCoroutine(spin_cr());
	}

	private IEnumerator spin_cr()
	{
		left.StartSpin();
		yield return CupheadTime.WaitForSeconds(parent, 0.2f);
		mid.StartSpin();
		yield return CupheadTime.WaitForSeconds(parent, 0.2f);
		right.StartSpin();
	}

	public void Stop(Mode mode)
	{
		parent.StartCoroutine(stop_cr(mode));
	}

	private IEnumerator stop_cr(Mode mode)
	{
		left.StopSpin(mode);
		yield return CupheadTime.WaitForSeconds(parent, 0.2f);
		mid.StopSpin(mode);
		yield return CupheadTime.WaitForSeconds(parent, 0.2f);
		right.StopSpin(mode);
	}

	public void StartFlash()
	{
		parent.StartCoroutine(startFlash_cr());
	}

	private IEnumerator startFlash_cr()
	{
		left.Flash();
		yield return CupheadTime.WaitForSeconds(parent, 0.2f);
		mid.Flash();
		yield return CupheadTime.WaitForSeconds(parent, 0.2f);
		right.Flash();
	}
}
