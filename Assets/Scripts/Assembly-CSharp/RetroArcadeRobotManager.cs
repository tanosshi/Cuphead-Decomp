using System.Collections;
using UnityEngine;

public class RetroArcadeRobotManager : LevelProperties.RetroArcade.Entity
{
	private const float BIG_ROBOT_SPACING = 160f;

	[SerializeField]
	private RetroArcadeBigRobot bigRobotPrefab;

	[SerializeField]
	private RetroArcadeBonusRobot bonusRobotPrefab;

	private LevelProperties.RetroArcade.Robots p;

	private int numDied;

	private int phase;

	private int numRobotsToKill;

	public void StartRobots()
	{
		p = base.properties.CurrentState.robots;
		phase = 0;
		StartCoroutine(bonus_cr());
		StartNewPhase();
	}

	public void StartNewPhase()
	{
		numDied = 0;
		string[] array = p.robotWaves[phase].Split(',');
		numRobotsToKill = array.Length;
		for (int i = 0; i < numRobotsToKill; i++)
		{
			int result;
			int.TryParse(array[i], out result);
			if (result > 0 && result <= p.robotsXPositions.Length)
			{
				string[] array2 = p.robotColorPattern[phase].Split(',');
				string[] orbiterPattern = array2[i].Split('-');
				float xPos = p.robotsXPositions[result - 1];
				bigRobotPrefab.Create(xPos, p, (float)i / 3f, this, orbiterPattern);
			}
		}
	}

	private IEnumerator bonus_cr()
	{
		for (int i = 0; i < p.bonusCount; i++)
		{
			yield return CupheadTime.WaitForSeconds(this, p.bonusDelay.RandomFloat());
			bonusRobotPrefab.Create((!Rand.Bool()) ? RetroArcadeBonusRobot.Direction.Right : RetroArcadeBonusRobot.Direction.Left, p);
		}
	}

	public void OnRobotGroupDie()
	{
		numDied++;
		if (numDied >= numRobotsToKill)
		{
			if (phase >= p.robotWaves.Length - 1)
			{
				base.properties.DealDamageToNextNamedState();
				StopAllCoroutines();
			}
			else
			{
				phase++;
				StartNewPhase();
			}
		}
	}
}
