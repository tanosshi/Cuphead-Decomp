using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelNewPlayerGUI : AbstractMonoBehaviour
{
	[SerializeField]
	private Image background;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private LocalizationHelper localizationHelper;

	private CanvasGroup canvasGroup;

	public static LevelNewPlayerGUI Current { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		base.gameObject.SetActive(false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	public void Init()
	{
		base.gameObject.SetActive(true);
		if (OnlineManager.Instance.Interface.SupportsMultipleUsers && OnlineManager.Instance.Interface.GetUser(PlayerId.PlayerTwo) != null)
		{
			localizationHelper.ApplyTranslation(Localization.Find("PlayerTwoJoinedWithUser"), new LocalizationHelper.LocalizationSubtext[1]
			{
				new LocalizationHelper.LocalizationSubtext("USERNAME", OnlineManager.Instance.Interface.GetUser(PlayerId.PlayerTwo).Name, true)
			});
		}
		StartCoroutine(tweenIn_cr());
		StartCoroutine(text_cr());
	}

	protected IEnumerator tweenIn_cr()
	{
		float t = 0f;
		PauseManager.Pause();
		while (t < 0.2f)
		{
			float val = t / 0.2f;
			canvasGroup.alpha = Mathf.Lerp(0f, 1f, val);
			t += Time.deltaTime;
			yield return null;
		}
		canvasGroup.alpha = 1f;
		yield return new WaitForSeconds(2f);
		StartCoroutine(tweenOut_cr());
	}

	protected IEnumerator tweenOut_cr()
	{
		float t = 0f;
		while (t < 0.2f)
		{
			float val = t / 0.2f;
			canvasGroup.alpha = Mathf.Lerp(1f, 0f, val);
			t += Time.deltaTime;
			yield return null;
		}
		canvasGroup.alpha = 0f;
		while (InterruptingPrompt.IsInterrupting())
		{
			yield return null;
		}
		PauseManager.Unpause();
		base.gameObject.SetActive(false);
	}

	private IEnumerator text_cr()
	{
		while (true)
		{
			yield return StartCoroutine(textScale_cr(0.9f, 1.1f, 0.5f));
			yield return StartCoroutine(textScale_cr(1.1f, 0.9f, 0.5f));
		}
	}

	protected IEnumerator textScale_cr(float start, float end, float time)
	{
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			text.transform.localScale = Vector3.one * EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, val);
			t += Time.deltaTime;
			yield return null;
		}
		text.transform.localScale = Vector3.one * end;
		yield return null;
	}
}
