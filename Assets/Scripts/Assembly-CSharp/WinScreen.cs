using System.Collections;
using TMPro;
using UnityEngine;

public class WinScreen : AbstractMonoBehaviour
{
	private const float BOB_FRAME_TIME = 1f / 24f;

	[Header("Delays")]
	[SerializeField]
	private float introDelay = 10f;

	[SerializeField]
	private float talliesDelay = 0.5f;

	[SerializeField]
	private float gradeDelay = 0.7f;

	[SerializeField]
	private float advanceDelay = 10f;

	[SerializeField]
	private WinScreenTicker timeTicker;

	[SerializeField]
	private WinScreenTicker hitsTicker;

	[SerializeField]
	private WinScreenTicker parriesTicker;

	[SerializeField]
	private WinScreenTicker superMeterTicker;

	[SerializeField]
	private WinScreenTicker difficultyTicker;

	[SerializeField]
	private LocalizationHelper spiritStockLabelLocalizationHelper;

	[SerializeField]
	private WinScreenGradeDisplay gradeDisplay;

	[SerializeField]
	private GameObject continuePrompt;

	[Header("Try Text")]
	[SerializeField]
	private GameObject tryRegular;

	[SerializeField]
	private TMP_Text tryRegularText;

	[SerializeField]
	private GameObject tryExpert;

	[SerializeField]
	private TMP_Text tryExpertText;

	[Header("Background")]
	[SerializeField]
	private Transform Background;

	[Header("DifferentLayouts")]
	[SerializeField]
	private GameObject OnePlayerCuphead;

	[SerializeField]
	private Transform OnePlayerUIRoot;

	[SerializeField]
	private Animator OnePlayerTitle;

	[Space(10f)]
	[SerializeField]
	private GameObject TwoPlayerCupheadMugman;

	[SerializeField]
	private Transform TwoPlayerUIRoot;

	[SerializeField]
	private Animator TwoPlayerTitle;

	[Space(10f)]
	[SerializeField]
	private Canvas results;

	private CupheadInput.AnyPlayerInput input;

	private const float BG_NORMAL_SPEED = 50f;

	private const float BG_FAST_SPEED = 150f;

	protected override void Awake()
	{
		base.Awake();
		OnePlayerCuphead.SetActive(false);
		TwoPlayerCupheadMugman.SetActive(false);
		Cuphead.Init();
		if (PlayerManager.Multiplayer)
		{
			TwoPlayerCupheadMugman.SetActive(true);
			results.transform.position = TwoPlayerUIRoot.transform.position;
			TwoPlayerTitle.SetBool("pickedA", Rand.Bool());
		}
		else
		{
			OnePlayerCuphead.SetActive(true);
			results.transform.position = OnePlayerUIRoot.transform.position;
			OnePlayerTitle.SetBool("pickedA", Rand.Bool());
		}
		StartCoroutine(main_cr());
		continuePrompt.SetActive(false);
		input = new CupheadInput.AnyPlayerInput();
		StartCoroutine(rotate_bg_cr());
	}

	private IEnumerator main_cr()
	{
		LevelScoringData data = Level.ScoringData;
		if (data.difficulty == Level.Mode.Easy && Level.PreviousDifficulty == Level.Mode.Easy && Level.PreviousLevelType == Level.Type.Battle && !Level.IsDicePalace && !Level.IsDicePalaceMain && Level.PreviousLevel != Levels.Devil)
		{
			Localization.Translation translation = Localization.Translate("ResultsTryRegular");
			if (translation.image == null)
			{
				tryRegular.GetComponent<SpriteRenderer>().enabled = false;
				tryRegularText.text = translation.text;
			}
			else
			{
				tryRegularText.enabled = false;
			}
			tryRegular.SetActive(true);
		}
		timeTicker.TargetValue = (int)data.time;
		timeTicker.MaxValue = (int)data.goalTime;
		hitsTicker.TargetValue = ((data.numTimesHit < 3) ? (3 - data.numTimesHit) : 0);
		hitsTicker.MaxValue = 3;
		parriesTicker.TargetValue = Mathf.Min(data.numParries, (int)Cuphead.Current.ScoringProperties.parriesForHighestGrade);
		parriesTicker.MaxValue = (int)Cuphead.Current.ScoringProperties.parriesForHighestGrade;
		superMeterTicker.TargetValue = Mathf.Min(data.superMeterUsed, (int)Cuphead.Current.ScoringProperties.superMeterUsageForHighestGrade);
		superMeterTicker.MaxValue = (int)Cuphead.Current.ScoringProperties.superMeterUsageForHighestGrade;
		if (data.useCoinsInsteadOfSuperMeter)
		{
			superMeterTicker.TargetValue = data.coinsCollected;
			superMeterTicker.MaxValue = 5;
			spiritStockLabelLocalizationHelper.ApplyTranslation(Localization.Find("ResultsMenuCoins"));
		}
		difficultyTicker.TargetValue = ((data.difficulty != Level.Mode.Easy) ? ((data.difficulty == Level.Mode.Normal) ? 1 : 2) : 0);
		gradeDisplay.Grade = Level.Grade;
		gradeDisplay.Difficulty = data.difficulty;
		yield return new WaitForSeconds(introDelay);
		WinScreenTicker[] tickers = new WinScreenTicker[5] { timeTicker, hitsTicker, parriesTicker, superMeterTicker, difficultyTicker };
		WinScreenTicker[] array = tickers;
		foreach (WinScreenTicker ticker in array)
		{
			ticker.StartCounting();
			while (!ticker.FinishedCounting)
			{
				yield return null;
			}
			if (ticker.TargetValue != 0)
			{
				yield return new WaitForSeconds(talliesDelay);
			}
		}
		InterruptingPrompt.SetCanInterrupt(true);
		float timer = 0f;
		while (timer < gradeDelay && !input.GetAnyButtonDown())
		{
			if (!InterruptingPrompt.IsInterrupting())
			{
				timer += Time.deltaTime;
			}
			yield return null;
		}
		gradeDisplay.Show();
		while (!gradeDisplay.FinishedGrading)
		{
			yield return null;
		}
		timer = 0f;
		continuePrompt.SetActive(true);
		while (timer < advanceDelay && !input.GetAnyButtonDown())
		{
			if (!InterruptingPrompt.IsInterrupting())
			{
				timer += Time.deltaTime;
			}
			yield return null;
		}
		if (Level.PreviousLevel == Levels.Devil)
		{
			Cutscene.Load(Scenes.scene_title, Scenes.scene_cutscene_outro, SceneLoader.Transition.Iris, SceneLoader.Transition.Fade);
		}
		else
		{
			SceneLoader.LoadLastMap();
		}
	}

	private IEnumerator rotate_bg_cr()
	{
		float frameTime = 0f;
		float normalTime = 0f;
		float speed = 50f;
		while (true)
		{
			frameTime += (float)CupheadTime.Delta;
			while (frameTime > 1f / 24f)
			{
				frameTime -= 1f / 24f;
				Background.Rotate(0f, 0f, speed * (float)CupheadTime.Delta);
				yield return null;
			}
			if (gradeDisplay.Celebration && speed < 150f)
			{
				normalTime += (float)CupheadTime.Delta;
				speed = Mathf.Lerp(50f, 150f, normalTime / 0.5f);
			}
			yield return null;
		}
	}
}
