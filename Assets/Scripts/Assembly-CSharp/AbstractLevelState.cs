using UnityEngine;

public abstract class AbstractLevelState<PATTERN, STATE_NAMES>
{
	public readonly float healthTrigger;

	public readonly PATTERN[] patterns;

	public readonly STATE_NAMES stateName;

	private int patternIndex;

	public PATTERN NextPattern
	{
		get
		{
			patternIndex++;
			if (patternIndex >= patterns.Length)
			{
				patternIndex = 0;
			}
			return patterns[patternIndex];
		}
	}

	public PATTERN CurrentPattern
	{
		get
		{
			return patterns[patternIndex];
		}
	}

	public AbstractLevelState(float healthTrigger, PATTERN[][] patterns, STATE_NAMES stateName)
	{
		this.healthTrigger = Mathf.Clamp(healthTrigger, 0f, 1f);
		this.patterns = patterns[Random.Range(0, patterns.Length)];
		patternIndex = Random.Range(0, this.patterns.Length);
		this.stateName = stateName;
	}
}
