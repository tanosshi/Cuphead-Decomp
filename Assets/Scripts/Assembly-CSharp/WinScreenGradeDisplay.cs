using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenGradeDisplay : AbstractMonoBehaviour
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private Text topGradeLabel;

	[SerializeField]
	private Text topGradeValue;

	[SerializeField]
	private string[] grades;

	[SerializeField]
	private Animator circle;

	[SerializeField]
	private Animator recordBanner;

	[SerializeField]
	private GameObject[] recordEnglish;

	[SerializeField]
	private GameObject[] recordOther;

	[SerializeField]
	private Animator gollyBanner;

	[SerializeField]
	private GameObject[] gollyEnglish;

	[SerializeField]
	private GameObject[] gollyOther;

	[SerializeField]
	private SpriteRenderer tryRegular;

	[SerializeField]
	private SpriteRenderer tryExpert;

	private const float COUNTER_TIME = 0.02f;

	private CupheadInput.AnyPlayerInput input;

	public LevelScoringData.Grade Grade { get; set; }

	public Level.Mode Difficulty { get; set; }

	public bool Celebration { get; private set; }

	public bool FinishedGrading { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		input = new CupheadInput.AnyPlayerInput();
	}

	protected override void Start()
	{
		base.Start();
		Celebration = false;
		if (Level.PreviouslyWon)
		{
			topGradeValue.text = " " + grades[(int)Level.PreviousGrade];
		}
	}

	public void Show()
	{
		StartCoroutine(grade_tally_up_cr());
	}

	private IEnumerator grade_tally_up_cr()
	{
		bool isTallying = true;
		float t = 0f;
		int counter = 0;
		text.text = grades[grades.Length - 1].Substring(0, 1) + " ";
		while (counter <= (int)Grade && isTallying && counter < (int)Grade)
		{
			AudioManager.Play("win_score_tick");
			counter++;
			text.text = grades[counter].Substring(0, 1) + " ";
			while (t < 0.02f)
			{
				if (input.GetButtonDown(CupheadButton.Accept))
				{
					isTallying = false;
					break;
				}
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			t = 0f;
		}
		AudioManager.Play("win_grade_chalk");
		circle.SetTrigger("Circle");
		text.GetComponent<Animator>().SetTrigger("MakeBig");
		text.text = grades[(int)Grade];
		if (counter == grades.Length - 1)
		{
			text.color = ColorUtils.HexToColor("FCC93D");
		}
		LevelScoringData.Grade PerfectGrade = ((Difficulty != Level.Mode.Hard) ? LevelScoringData.Grade.APlus : LevelScoringData.Grade.S);
		if (Grade == PerfectGrade)
		{
			StartCoroutine(fade_text_cr());
			yield return CupheadTime.WaitForSeconds(this, 0.16f);
			gollyBanner.SetTrigger("OnBanner");
			Celebration = true;
			LanguageUpdate();
			yield return gollyBanner.WaitForAnimationToEnd(this, "Golly");
		}
		else if (Grade > Level.PreviousGrade || !Level.PreviouslyWon)
		{
			StartCoroutine(fade_text_cr());
			yield return CupheadTime.WaitForSeconds(this, 0.16f);
			recordBanner.SetTrigger("OnBanner");
			Celebration = true;
			LanguageUpdate();
			yield return recordBanner.WaitForAnimationToEnd(this, "Record");
		}
		FinishedGrading = true;
		yield return null;
	}

	private void LanguageUpdate()
	{
		bool flag = Localization.language == Localization.Languages.English;
		for (int i = 0; i < recordEnglish.Length; i++)
		{
			recordEnglish[i].SetActive(flag);
		}
		for (int j = 0; j < gollyEnglish.Length; j++)
		{
			gollyEnglish[j].SetActive(flag);
		}
		for (int k = 0; k < recordOther.Length; k++)
		{
			recordOther[k].SetActive(!flag);
		}
		for (int l = 0; l < gollyOther.Length; l++)
		{
			gollyOther[l].SetActive(!flag);
		}
	}

	private IEnumerator fade_text_cr()
	{
		float t = 0f;
		float fadeTime = 0.29f;
		Color topGradeLabelColor = topGradeLabel.color;
		Color topGradeValColor = topGradeValue.color;
		while (t < fadeTime)
		{
			t += (float)CupheadTime.Delta;
			topGradeLabel.color = new Color(topGradeLabelColor.r, topGradeLabelColor.g, topGradeLabelColor.b, 1f - t / fadeTime);
			topGradeValue.color = new Color(topGradeValColor.r, topGradeValColor.g, topGradeValColor.b, 1f - t / fadeTime);
			if (tryExpert.gameObject.activeSelf)
			{
				SpriteRenderer[] componentsInChildren = tryExpert.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer in componentsInChildren)
				{
					spriteRenderer.color = new Color(1f, 1f, 1f, 1f - t / fadeTime);
				}
			}
			if (tryRegular.gameObject.activeSelf)
			{
				SpriteRenderer[] componentsInChildren2 = tryRegular.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer2 in componentsInChildren2)
				{
					spriteRenderer2.color = new Color(1f, 1f, 1f, 1f - t / fadeTime);
				}
			}
			yield return null;
		}
		yield return null;
	}
}
