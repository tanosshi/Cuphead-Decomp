using System.Collections;
using UnityEngine;

public class MouseLevelSawBladeSide : AbstractPausableComponent
{
	public enum State
	{
		Init = 0,
		Pattern = 1,
		FullAttack = 2
	}

	[SerializeField]
	private MouseLevelSawBlade[] sawBlades;

	private LevelProperties.Mouse properties;

	private string[] pattern;

	private int patternIndex;

	public State state { get; private set; }

	public void Begin(LevelProperties.Mouse properties)
	{
		this.properties = properties;
		MouseLevelSawBlade[] array = sawBlades;
		foreach (MouseLevelSawBlade mouseLevelSawBlade in array)
		{
			mouseLevelSawBlade.Begin(properties);
		}
		StartCoroutine(intro_cr());
	}

	public void Leave()
	{
		StopAllCoroutines();
		MouseLevelSawBlade[] array = sawBlades;
		foreach (MouseLevelSawBlade mouseLevelSawBlade in array)
		{
			mouseLevelSawBlade.Leave();
		}
	}

	public void SetPattern(string pattern)
	{
		this.pattern = pattern.Split(',');
		patternIndex = Random.Range(0, this.pattern.Length);
	}

	public void FullAttack()
	{
		StopAllCoroutines();
		StartCoroutine(fullAttack_cr());
	}

	private IEnumerator intro_cr()
	{
		while (sawBlades[0].state != MouseLevelSawBlade.State.Idle)
		{
			yield return null;
		}
		state = State.Pattern;
		StartCoroutine(pattern_cr());
	}

	private IEnumerator pattern_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.brokenCanSawBlades.delayBeforeNextSaw);
			int sawBladeIndex = 0;
			int.TryParse(pattern[patternIndex], out sawBladeIndex);
			sawBlades[sawBladeIndex - 1].Attack();
			patternIndex = (patternIndex + 1) % pattern.Length;
		}
	}

	private IEnumerator fullAttack_cr()
	{
		state = State.FullAttack;
		bool canFullAttack = false;
		while (!canFullAttack)
		{
			canFullAttack = true;
			MouseLevelSawBlade[] array = sawBlades;
			foreach (MouseLevelSawBlade mouseLevelSawBlade in array)
			{
				if (mouseLevelSawBlade.state != MouseLevelSawBlade.State.Idle)
				{
					canFullAttack = false;
				}
			}
			yield return null;
		}
		AudioManager.Play("level_mouse_buzzsaw_wall");
		MouseLevelSawBlade[] array2 = sawBlades;
		foreach (MouseLevelSawBlade mouseLevelSawBlade2 in array2)
		{
			mouseLevelSawBlade2.FullAttack();
		}
		while (sawBlades[0].state != MouseLevelSawBlade.State.Idle)
		{
			yield return null;
		}
		state = State.Pattern;
		StartCoroutine(pattern_cr());
	}
}
