public class SharedMapGate : MapLevelDependentObstacle
{
	private bool previouslyWon;

	protected override bool ValidateSucess()
	{
		bool result = false;
		Levels[] levels = _levels;
		foreach (Levels levelID in levels)
		{
			PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(levelID);
			if (levelData.completed)
			{
				result = true;
				difficulty = levelData.difficultyBeaten;
				grade = levelData.grade;
				break;
			}
		}
		return result;
	}

	protected override bool ValidateCondition(Levels level)
	{
		bool result = false;
		if (Level.PreviousLevel != level && PlayerData.Data.GetLevelData(level).completed)
		{
			previouslyWon = true;
		}
		if (previouslyWon)
		{
			return false;
		}
		if (!Level.PreviouslyWon && Level.Won)
		{
			result = true;
		}
		if (ReactToGradeChange && Level.Grade > Level.PreviousGrade)
		{
			gradeChanged = true;
			result = true;
		}
		if (ReactToDifficultyChange && Level.Difficulty > Level.PreviousDifficulty)
		{
			difficultyChanged = true;
			result = true;
		}
		return result;
	}
}
