using UnityEngine;

public class LevelScoringData
{
	public enum Grade
	{
		DMinus = 0,
		D = 1,
		DPlus = 2,
		CMinus = 3,
		C = 4,
		CPlus = 5,
		BMinus = 6,
		B = 7,
		BPlus = 8,
		AMinus = 9,
		A = 10,
		APlus = 11,
		S = 12,
		P = 13
	}

	public float time;

	public float goalTime;

	public int numTimesHit;

	public int numParries;

	public int superMeterUsed;

	public int coinsCollected;

	public Level.Mode difficulty;

	public bool pacifistRun;

	public bool useCoinsInsteadOfSuperMeter;

	public Grade CalculateGrade()
	{
		if (pacifistRun)
		{
			return Grade.P;
		}
		ScoringEditorData scoringProperties = Cuphead.Current.ScoringProperties;
		float num = Mathf.Clamp(100f - 100f * (time - goalTime) / (goalTime * (scoringProperties.bestTimeMultiplierForNoScore - 1f)), 0f, 100f);
		float num2 = Mathf.Clamp(100f - 100f * ((float)numTimesHit / scoringProperties.hitsForNoScore), 0f, 100f);
		float num3 = 100f * Mathf.Min(numParries, scoringProperties.parriesForHighestGrade + scoringProperties.bonusParries) / scoringProperties.parriesForHighestGrade;
		float num4 = 100f * Mathf.Min(superMeterUsed, scoringProperties.superMeterUsageForHighestGrade + scoringProperties.bonusSuperMeterUsage) / scoringProperties.superMeterUsageForHighestGrade;
		if (useCoinsInsteadOfSuperMeter)
		{
			num4 = 100f * ((float)coinsCollected / 5f);
		}
		float num5 = num * scoringProperties.timeWeight + num2 * scoringProperties.hitsWeight + num3 * scoringProperties.parriesWeight + num4 * scoringProperties.superMeterUsageWeight;
		ScoringEditorData.GradingCurveEntry[] array = ((difficulty == Level.Mode.Easy) ? scoringProperties.easyGradingCurve : ((difficulty != Level.Mode.Normal) ? scoringProperties.hardGradingCurve : scoringProperties.mediumGradingCurve));
		Grade result = Grade.DMinus;
		ScoringEditorData.GradingCurveEntry[] array2 = array;
		foreach (ScoringEditorData.GradingCurveEntry gradingCurveEntry in array2)
		{
			result = gradingCurveEntry.grade;
			if (num5 < gradingCurveEntry.upperLimit)
			{
				break;
			}
		}
		return result;
	}
}
