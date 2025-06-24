using System;

public class ScoringEditorData : AbstractMonoBehaviour
{
	[Serializable]
	public class GradingCurveEntry
	{
		public LevelScoringData.Grade grade;

		public float upperLimit;
	}

	public float bestTimeMultiplierForNoScore;

	public float hitsForNoScore;

	public float parriesForHighestGrade;

	public float bonusParries;

	public float superMeterUsageForHighestGrade;

	public float bonusSuperMeterUsage;

	public GradingCurveEntry[] easyGradingCurve;

	public GradingCurveEntry[] mediumGradingCurve;

	public GradingCurveEntry[] hardGradingCurve;

	public float timeWeight;

	public float hitsWeight;

	public float parriesWeight;

	public float superMeterUsageWeight;
}
