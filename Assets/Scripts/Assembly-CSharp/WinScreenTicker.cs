using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenTicker : AbstractMonoBehaviour
{
	public enum TickerType
	{
		Time = 0,
		Health = 1,
		Score = 2,
		Stars = 3
	}

	public TickerType tickerType;

	[SerializeField]
	private Animator[] stars;

	[SerializeField]
	private Text[] leaderDots;

	[SerializeField]
	private Text label;

	[SerializeField]
	private Text valueText;

	private float value;

	private bool startedCounting;

	private const float TIME_COUNTER_TIME = 0.03f;

	private const float USUAL_COUNTER_TIME = 0.07f;

	private const float STAR_COUNTER_TIME = 0.5f;

	private CupheadInput.AnyPlayerInput input;

	public int TargetValue { get; set; }

	public int MaxValue { get; set; }

	public bool FinishedCounting { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		input = new CupheadInput.AnyPlayerInput();
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(select_type_cr());
	}

	private IEnumerator select_type_cr()
	{
		switch (tickerType)
		{
		case TickerType.Time:
			StartCoroutine(time_tally_up_cr());
			break;
		case TickerType.Health:
			StartCoroutine(health_tally_up_cr());
			break;
		case TickerType.Stars:
			StartCoroutine(stars_tally_up_cr());
			break;
		case TickerType.Score:
			StartCoroutine(score_tally_up_cr());
			break;
		}
		yield return null;
	}

	private IEnumerator health_tally_up_cr()
	{
		bool isTallying = true;
		float t = 0f;
		int counter = 0;
		valueText.text = counter + " " + MaxValue;
		while (!startedCounting)
		{
			yield return null;
		}
		while (counter < TargetValue && isTallying && counter < TargetValue)
		{
			while (t < 0.03f)
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
			if (isTallying)
			{
				AudioManager.Play("win_score_tick");
				counter++;
				valueText.text = counter + " " + MaxValue;
			}
		}
		valueText.text = TargetValue + " " + MaxValue;
		valueText.GetComponent<Animator>().SetTrigger("MakeBigTally");
		if (TargetValue == MaxValue)
		{
			AudioManager.Play("win_score_tick");
			valueText.color = ColorUtils.HexToColor("FCC93D");
		}
		yield return null;
		FinishedCounting = true;
	}

	private IEnumerator score_tally_up_cr()
	{
		bool isTallying = true;
		float t = 0f;
		int counter = 0;
		valueText.text = counter + " " + MaxValue;
		if (leaderDots.Length > 0)
		{
			leaderDots[0].enabled = true;
			leaderDots[1].enabled = false;
		}
		while (!startedCounting)
		{
			yield return null;
		}
		while (counter <= TargetValue && isTallying && counter < TargetValue)
		{
			while (t < 0.03f)
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
			if (isTallying)
			{
				AudioManager.Play("win_score_tick");
				counter++;
				if (leaderDots.Length > 0 && counter > 9)
				{
					leaderDots[0].enabled = false;
					leaderDots[1].enabled = true;
				}
				valueText.text = counter + " " + MaxValue;
			}
			yield return null;
		}
		valueText.text = TargetValue + " " + MaxValue;
		valueText.GetComponent<Animator>().SetTrigger("MakeBigTally");
		if (TargetValue == MaxValue)
		{
			AudioManager.Play("win_score_tick");
			valueText.color = ColorUtils.HexToColor("FCC93D");
		}
		yield return null;
		FinishedCounting = true;
	}

	private IEnumerator time_tally_up_cr()
	{
		bool isTallying = true;
		float t = 0f;
		int minutesMax = MaxValue / 60;
		int secondsMax = MaxValue % 60;
		int minutesTarget = TargetValue / 60;
		int secondsTarget = TargetValue % 60;
		int secondCounter = 0;
		int minuteCounter = 0;
		valueText.text = "00 00";
		while (!startedCounting)
		{
			yield return null;
		}
		AudioManager.PlayLoop("win_time_ticker_loop");
		while (isTallying)
		{
			if (secondCounter < 60)
			{
				secondCounter++;
			}
			else
			{
				minuteCounter++;
				secondCounter = 0;
			}
			string displayedMinutes = ((minuteCounter > 9) ? minuteCounter.ToString() : ("0" + minuteCounter));
			string displayedSeconds = ((secondCounter > 9) ? secondCounter.ToString() : ("0" + secondCounter));
			valueText.text = displayedMinutes + " " + displayedSeconds;
			if (minuteCounter >= minutesTarget && secondCounter >= secondsTarget)
			{
				isTallying = false;
				break;
			}
			while (t < 0.03f)
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
		AudioManager.Stop("win_time_ticker_loop");
		AudioManager.Play("win_time_ticker_loop_end");
		string minutes = ((minutesTarget > 9) ? minutesTarget.ToString() : ("0" + minutesTarget));
		string seconds = ((secondsTarget > 9) ? secondsTarget.ToString() : ("0" + secondsTarget));
		valueText.text = minutes + " " + seconds;
		if (minutesTarget == minutesMax)
		{
			if (secondsTarget <= secondsMax)
			{
				AudioManager.Play("win_score_tick");
				valueText.color = ColorUtils.HexToColor("FCC93D");
			}
		}
		else if (minutesTarget < minutesMax)
		{
			AudioManager.Play("win_score_tick");
			valueText.color = ColorUtils.HexToColor("FCC93D");
		}
		valueText.GetComponent<Animator>().SetTrigger("MakeBigTally");
		FinishedCounting = true;
		yield return null;
	}

	private IEnumerator stars_tally_up_cr()
	{
		int startVal = 0;
		if (TargetValue == 2)
		{
			leaderDots[0].enabled = false;
			leaderDots[1].enabled = true;
			stars[0].gameObject.SetActive(true);
		}
		else
		{
			leaderDots[0].enabled = true;
			leaderDots[1].enabled = false;
			stars[0].gameObject.SetActive(false);
			startVal = 1;
		}
		YieldInstruction time = new WaitForSeconds(0.5f);
		while (!startedCounting)
		{
			yield return null;
		}
		for (int i = startVal; i < TargetValue + 1 + startVal; i++)
		{
			if (input.GetButtonDown(CupheadButton.Accept))
			{
				break;
			}
			stars[i].SetTrigger("OnAppear");
			AudioManager.Play("win_skill_lvl");
			yield return time;
		}
		FinishedCounting = true;
		yield return null;
	}

	public void StartCounting()
	{
		startedCounting = true;
	}
}
