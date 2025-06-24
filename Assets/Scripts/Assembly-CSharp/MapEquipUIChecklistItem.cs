using UnityEngine;
using UnityEngine.UI;

public class MapEquipUIChecklistItem : AbstractMonoBehaviour
{
	[Header("Text")]
	public Text descriptionText;

	public Text leaderDotText;

	public Text gradeText;

	public Text timeText;

	[Header("Images")]
	public Image checkBox;

	public Image checkMark;

	public Image checkMarkHard;

	private readonly string[] grades = new string[14]
	{
		"D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-",
		"A", "A+", "S", "P"
	};

	private readonly string unknown = "?????";

	private readonly string dots = ". . . . . . . . . . . . . . . . . . . . . . .";

	private readonly string extraDots = ". . ";

	public bool isDicePalaceMiniBoss;

	private float dotsPadding = 5f;

	private float lineWidth
	{
		get
		{
			return descriptionText.rectTransform.sizeDelta.x;
		}
	}

	protected override void Start()
	{
		base.Start();
	}

	public bool EnableCheckbox(bool enabled)
	{
		if (checkBox != null)
		{
			bool result = enabled;
			checkBox.enabled = result;
			return result;
		}
		return false;
	}

	public void SetDescription(Levels selectedLevel, string levelName, bool isFinale)
	{
		PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(selectedLevel);
		descriptionText.text = ((!levelData.played) ? unknown : levelName);
		if (!isDicePalaceMiniBoss)
		{
			SetLeaderDots(levelName, isFinale);
		}
		if (levelData.played)
		{
			if (!isDicePalaceMiniBoss && levelData.completed)
			{
				gradeText.text = grades[(int)levelData.grade];
				timeText.text = SecondsToMinutes(levelData.bestTime);
				Level.Mode mode = ((!PlayerData.Data.IsHardModeAvailable) ? Level.Mode.Normal : Level.Mode.Hard);
				if (levelData.difficultyBeaten == Level.Mode.Normal && checkBox != null && checkBox.enabled)
				{
					checkMark.enabled = true;
					checkMarkHard.enabled = false;
				}
				if (levelData.difficultyBeaten == Level.Mode.Hard && checkBox != null && checkBox.enabled)
				{
					checkMark.enabled = false;
					checkMarkHard.enabled = true;
				}
			}
		}
		else
		{
			ClearDescription(isFinale);
		}
	}

	private string SecondsToMinutes(float seconds)
	{
		if (seconds == float.MaxValue)
		{
			return "6:66";
		}
		int num = (int)seconds / 60;
		int num2 = (int)seconds % 60;
		return string.Format("{0}:{1:00}", num, num2);
	}

	public void ClearDescription(bool isFinale)
	{
		if (!isDicePalaceMiniBoss)
		{
			gradeText.text = "?";
			timeText.text = "?";
			descriptionText.text = unknown;
			if (isFinale)
			{
				SetLeaderDots(unknown, isFinale);
			}
			else
			{
				SetLeaderDots(unknown, isFinale);
			}
		}
		else
		{
			descriptionText.text = unknown;
		}
		if (checkMark != null)
		{
			checkMark.enabled = false;
		}
		if (checkMarkHard != null)
		{
			checkMarkHard.enabled = false;
		}
	}

	private void SetLeaderDots(string name, bool isFinale)
	{
		leaderDotText.text = dots;
		float num = lineWidth - descriptionText.preferredWidth - dotsPadding;
		int num2 = 100000;
		while (leaderDotText.text.Length > 0 && leaderDotText.preferredWidth > num && num2 > 0)
		{
			num2--;
			leaderDotText.text = leaderDotText.text.Substring(0, leaderDotText.text.Length - 1);
		}
	}
}
