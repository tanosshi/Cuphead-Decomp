using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : AbstractPausableComponent
{
	public enum Mode
	{
		Text = 0,
		ListChoice = 1
	}

	public enum DisplayState
	{
		Hidden = 0,
		FadeIn = 1,
		Showing = 2,
		FadeOut = 3
	}

	private const string ARROW_PADDING = "    ";

	private const float DEFAULT_TIME = 2f;

	private const float FADE_TIME = 0.07f;

	private const float END_TIME = 0.25f;

	private const float ARROW_WAIT_TIME = 0.125f;

	private const float MAX_RANDOM_OFFSET = 0.05f;

	private const int MAX_CHOICES_PER_COLUMN = 4;

	private const int COLUMN_PADDING = 55;

	private const int COLUMN_SPACING = 45;

	private const int DEFAULT_PADDING = 30;

	private const int SMALL_PADDING = 20;

	private const float TAIL_POSITION_X = -73f;

	public static SpeechBubble Instance;

	[SerializeField]
	private TextMeshProUGUI mainText;

	[SerializeField]
	private TextMeshProUGUI columnTwoText;

	[SerializeField]
	private HorizontalLayoutGroup layout;

	[SerializeField]
	private Image tail;

	[SerializeField]
	private List<Sprite> tailVariants;

	[SerializeField]
	private Image arrow;

	[SerializeField]
	private Image cursor;

	[SerializeField]
	private RectTransform cursorRoot;

	[SerializeField]
	private List<Sprite> arrowVariants;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private List<RectTransform> bullets;

	private float maxWidth = 558f;

	public Vector2 basePosition;

	public Vector2 panPosition;

	private List<string> listItems;

	private int currentChoiceIndex;

	private int choicesPerColumn;

	public int maxLines = -1;

	public bool tailOnTheLeft;

	private CupheadInput.AnyPlayerInput input;

	public bool waitForRealease;

	public bool waitForFade;

	private Coroutine showCoroutine;

	[SerializeField]
	private LayoutElement textLayoutElement;

	private bool waiting;

	[HideInInspector]
	public bool preventQuit;

	public Mode mode { get; private set; }

	public DisplayState displayState { get; private set; }

	protected override void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
		panPosition = base.transform.position;
		base.Awake();
		Dialoguer.Initialize();
	}

	protected override void Start()
	{
		base.Start();
		canvasGroup.alpha = 0f;
		basePosition = base.rectTransform.position;
		input = new CupheadInput.AnyPlayerInput();
		AddDialoguerEvents();
	}

	protected override void Update()
	{
		base.Update();
		if ((!(MapEventNotification.Current == null) && MapEventNotification.Current.showing) || waiting || waitForFade)
		{
			return;
		}
		if (input.GetButtonUp(CupheadButton.Accept))
		{
			waitForRealease = false;
		}
		if (!waitForRealease && (input.GetButtonDown(CupheadButton.Accept) || input.GetButtonDown(CupheadButton.Cancel)))
		{
			if (currentChoiceIndex >= 0)
			{
				Dialoguer.ContinueDialogue(currentChoiceIndex);
			}
			else
			{
				Dialoguer.ContinueDialogue();
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		RemoveDialoguerEvents();
		Instance = null;
	}

	public void Show(string text)
	{
		if (showCoroutine != null)
		{
			StopCoroutine(showCoroutine);
		}
		showCoroutine = StartCoroutine(show_cr(Mode.Text, StringVariantGenerator.Instance.Generate(GetNormalizedText(Localization.Translate(text).SanitizedText())) + "    ", null));
	}

	public void Show(List<string> listItems)
	{
		if (showCoroutine != null)
		{
			StopCoroutine(showCoroutine);
		}
		showCoroutine = StartCoroutine(show_cr(Mode.ListChoice, null, listItems));
	}

	public void Dismiss()
	{
		StartCoroutine(dismiss_cr(preventQuit));
	}

	protected virtual string GetNormalizedText(string text)
	{
		string text2 = mainText.text;
		text = text.Replace("{DEATHS}", "<size=15> </size><font=\"CupheadVogue-Bold SDF\"><b><size=36><line-height=1>" + PlayerData.Data.DeathCount(PlayerId.Any) + "</size></b></font><size=15> </size>");
		string text3 = string.Empty;
		text = text.Replace("\n", " ");
		mainText.text = text;
		mainText.CalculateLayoutInputHorizontal();
		string text4 = string.Empty;
		int num = 10000;
		int num2 = 0;
		while (mainText.text.Length > 0 && num > 0)
		{
			num--;
			while (mainText.text.Length > 0 && mainText.preferredWidth > maxWidth && num > 0)
			{
				num--;
				text4 = mainText.text.Substring(mainText.text.Length - 1, 1) + text4;
				mainText.text = mainText.text.Substring(0, mainText.text.Length - 1);
				mainText.CalculateLayoutInputHorizontal();
			}
			int num3 = mainText.text.LastIndexOf(" ");
			if (num3 == -1 || string.IsNullOrEmpty(text4))
			{
				text3 += mainText.text;
			}
			else
			{
				text4 = mainText.text.Substring(num3 + 1) + text4;
				text3 = text3 + mainText.text.Substring(0, num3) + "\n";
			}
			mainText.text = text4;
			mainText.CalculateLayoutInputHorizontal();
			text4 = string.Empty;
			num2++;
		}
		if (num == 0)
		{
			Debug.LogError("THE WHILES ARE DEAD, BAD CODE !!!");
		}
		if (maxLines != -1 && num2 > maxLines)
		{
			text3 = text3.Replace("\n", " ");
			mainText.enableAutoSizing = true;
			textLayoutElement.enabled = true;
			layout.padding.left = 20;
			layout.padding.right = 20;
			layout.padding.bottom = 20;
			layout.padding.top = 20;
		}
		else
		{
			mainText.enableAutoSizing = false;
			textLayoutElement.enabled = false;
		}
		mainText.text = text2;
		return text3;
	}

	private IEnumerator show_cr(Mode mode, string text, List<string> listItems)
	{
		waitForFade = true;
		if (displayState != DisplayState.Hidden)
		{
			yield return StartCoroutine(dismiss_cr(false));
		}
		if (mode == Mode.Text)
		{
			layout.padding.left = 30;
			layout.padding.right = 30;
			layout.padding.bottom = 30;
			layout.padding.top = 30;
			layout.spacing = 0f;
			mainText.text = text;
			columnTwoText.text = string.Empty;
			foreach (RectTransform bullet in bullets)
			{
				bullet.gameObject.SetActive(false);
			}
			currentChoiceIndex = -1;
		}
		else
		{
			string leftColumn = string.Empty;
			this.listItems = listItems;
			choicesPerColumn = 4;
			if (listItems.Count > 4)
			{
				if (listItems.Count % 2 == 0)
				{
					choicesPerColumn = listItems.Count / 2;
				}
				else
				{
					choicesPerColumn = listItems.Count / 2 + 1;
				}
			}
			for (int i = 0; i < choicesPerColumn && i < listItems.Count; i++)
			{
				leftColumn = ((i >= choicesPerColumn - 1 || i >= listItems.Count - 1) ? (leftColumn + Localization.Translate(listItems[i]).SanitizedText()) : (leftColumn + Localization.Translate(listItems[i]).SanitizedText() + "\n"));
			}
			string rightColumn = string.Empty;
			if (listItems.Count > choicesPerColumn)
			{
				for (int j = choicesPerColumn; j < choicesPerColumn * 2; j++)
				{
					if (j < listItems.Count)
					{
						rightColumn = ((j >= choicesPerColumn * 2 - 1) ? (rightColumn + Localization.Translate(listItems[j]).SanitizedText()) : (rightColumn + Localization.Translate(listItems[j]).SanitizedText() + "\n"));
					}
					else if (j < choicesPerColumn * 2 - 1)
					{
						rightColumn += "\n";
					}
				}
			}
			mainText.text = StringVariantGenerator.Instance.Generate(leftColumn);
			columnTwoText.text = StringVariantGenerator.Instance.Generate(rightColumn);
			currentChoiceIndex = 0;
			layout.padding.left = 55;
			layout.spacing = ((listItems.Count > choicesPerColumn) ? 45 : 0);
			yield return null;
			bullets.Shuffle();
			for (int k = 0; k < bullets.Count; k++)
			{
				if (k < listItems.Count)
				{
					bullets[k].gameObject.SetActive(true);
					bullets[k].position = getCursorPos(k);
				}
				else
				{
					bullets[k].gameObject.SetActive(false);
				}
			}
		}
		if (tailOnTheLeft)
		{
			tail.rectTransform.anchorMin = Vector2.zero;
			tail.rectTransform.anchorMax = Vector2.zero;
			tail.rectTransform.anchoredPosition = new Vector2(73f, tail.rectTransform.anchoredPosition.y);
		}
		else
		{
			tail.rectTransform.anchorMin = new Vector2(1f, 0f);
			tail.rectTransform.anchorMax = new Vector2(1f, 0f);
			tail.rectTransform.anchoredPosition = new Vector2(-73f, tail.rectTransform.anchoredPosition.y);
		}
		arrow.color = new Color(1f, 1f, 1f, 0f);
		cursor.color = new Color(1f, 1f, 1f, 0f);
		float maxOffset = 0.05f;
		if (CupheadLevelCamera.Current != null)
		{
			maxOffset *= 100f;
		}
		base.rectTransform.position = basePosition + new Vector2(Random.Range(0f - maxOffset, maxOffset), Random.Range(0f - maxOffset, maxOffset)) * base.rectTransform.localScale.x;
		tail.sprite = tailVariants.RandomChoice();
		arrow.sprite = arrowVariants.RandomChoice();
		cursor.sprite = arrowVariants.RandomChoice();
		base.animator.Play("Idle", 0, Random.Range(0f, 1f));
		base.animator.Play("Idle", 1, Random.Range(0f, 1f));
		displayState = DisplayState.FadeIn;
		yield return StartCoroutine(fade_cr(canvasGroup.alpha, 1f));
		yield return CupheadTime.WaitForSeconds(this, 0.125f);
		displayState = DisplayState.Showing;
		showCoroutine = null;
		Color colorHidden = new Color(1f, 1f, 1f, 0f);
		Color colorShown = new Color(1f, 1f, 1f, 1f);
		if (mode == Mode.Text)
		{
			arrow.color = ((!waiting) ? colorShown : colorHidden);
		}
		else
		{
			cursor.color = ((!waiting) ? colorShown : colorHidden);
			while (displayState == DisplayState.Showing)
			{
				if (waiting)
				{
					yield return null;
					continue;
				}
				cursor.color = colorShown;
				if (input.GetButtonDown(CupheadButton.MenuDown) && currentChoiceIndex != choicesPerColumn - 1 && currentChoiceIndex < listItems.Count - 1)
				{
					currentChoiceIndex++;
					base.animator.SetTrigger("MoveDown");
				}
				else if (input.GetButtonDown(CupheadButton.MenuDown) && currentChoiceIndex >= choicesPerColumn && currentChoiceIndex == listItems.Count - 1 && listItems.Count < choicesPerColumn * 2)
				{
					currentChoiceIndex -= choicesPerColumn - 1;
					base.animator.SetTrigger("MoveDown");
				}
				if (input.GetButtonDown(CupheadButton.MenuUp) && currentChoiceIndex != choicesPerColumn && currentChoiceIndex > 0)
				{
					currentChoiceIndex--;
					base.animator.SetTrigger("MoveUp");
				}
				if (input.GetButtonDown(CupheadButton.MenuRight) && currentChoiceIndex < choicesPerColumn && listItems.Count > choicesPerColumn)
				{
					currentChoiceIndex = Mathf.Min(currentChoiceIndex + choicesPerColumn, listItems.Count - 1);
					base.animator.SetTrigger("MoveRight");
				}
				if (input.GetButtonDown(CupheadButton.MenuLeft) && currentChoiceIndex >= choicesPerColumn)
				{
					currentChoiceIndex -= choicesPerColumn;
					base.animator.SetTrigger("MoveLeft");
				}
				cursorRoot.position = getCursorPos(currentChoiceIndex);
				for (int l = 0; l < listItems.Count && l < bullets.Count; l++)
				{
					bullets[l].gameObject.SetActive(l != currentChoiceIndex);
				}
				yield return null;
			}
		}
		waitForFade = false;
	}

	private IEnumerator dismiss_cr(bool watchPreventQuit)
	{
		if (displayState == DisplayState.Hidden)
		{
			yield break;
		}
		while (displayState == DisplayState.FadeIn)
		{
			yield return null;
		}
		if (watchPreventQuit)
		{
			while (preventQuit)
			{
				yield return null;
			}
		}
		displayState = DisplayState.FadeOut;
		yield return StartCoroutine(fade_cr(canvasGroup.alpha, 0f));
		displayState = DisplayState.Hidden;
	}

	private IEnumerator fade_cr(float startOpacity, float endOpacity)
	{
		if (endOpacity == 0f)
		{
			canvasGroup.alpha = endOpacity;
			yield break;
		}
		yield return null;
		float t = 0f;
		while (t < 0.07f)
		{
			yield return null;
			t += (float)CupheadTime.Delta;
			canvasGroup.alpha = Mathf.Lerp(startOpacity, endOpacity, t / 0.07f);
		}
		canvasGroup.alpha = endOpacity;
	}

	private Vector2 getCursorPos(int choiceIndex)
	{
		int num = 0;
		TMP_Text tMP_Text = null;
		if (choiceIndex < choicesPerColumn)
		{
			num = choiceIndex;
			tMP_Text = mainText;
		}
		else
		{
			num = choiceIndex - choicesPerColumn;
			tMP_Text = columnTwoText;
		}
		return tMP_Text.rectTransform.position;
	}

	private void setOpacity(float opacity)
	{
	}

	public void AddDialoguerEvents()
	{
		Dialoguer.events.onStarted += OnDialogueStartedHandler;
		Dialoguer.events.onEnded += OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded += OnDialogueInstantlyEndedHandler;
		Dialoguer.events.onTextPhase += OnDialogueTextPhaseHandler;
		Dialoguer.events.onWindowClose += OnDialogueWindowCloseHandler;
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
	}

	public void RemoveDialoguerEvents()
	{
		Dialoguer.events.onStarted -= OnDialogueStartedHandler;
		Dialoguer.events.onEnded -= OnDialogueEndedHandler;
		Dialoguer.events.onInstantlyEnded -= OnDialogueInstantlyEndedHandler;
		Dialoguer.events.onTextPhase -= OnDialogueTextPhaseHandler;
		Dialoguer.events.onWindowClose -= OnDialogueWindowCloseHandler;
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
	}

	private void OnDialogueStartedHandler()
	{
		if (Map.Current != null)
		{
			Map.Current.CurrentState = Map.State.Event;
		}
		if (CupheadMapCamera.Current != null)
		{
			CupheadMapCamera.Current.MoveToPosition(panPosition, 0.75f, 1f);
		}
		if (MapUIVignetteDialogue.Current != null)
		{
			MapUIVignetteDialogue.Current.FadeIn();
		}
	}

	private void OnDialogueEndedHandler()
	{
		Dismiss();
	}

	private void OnDialogueInstantlyEndedHandler()
	{
		Dismiss();
	}

	private void OnDialogueTextPhaseHandler(DialoguerTextData data)
	{
		if (data.choices == null)
		{
			Show(data.text);
		}
		else if (data.choices.Length > 0)
		{
			Show(new List<string>(data.choices));
		}
	}

	private void OnDialogueWindowCloseHandler()
	{
		Dismiss();
		if (MapUIVignetteDialogue.Current != null)
		{
			MapUIVignetteDialogue.Current.FadeOut();
		}
		if (Map.Current != null)
		{
			Map.Current.CurrentState = Map.State.Ready;
		}
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (message == "Wait")
		{
			StartCoroutine(wait_cr(float.Parse(metadata)));
		}
	}

	private IEnumerator wait_cr(float waitDuration)
	{
		waiting = true;
		arrow.color = new Color(1f, 1f, 1f, 0f);
		while (waitDuration > 0f)
		{
			yield return null;
			waitDuration -= (float)CupheadTime.Delta;
		}
		waiting = false;
		arrow.color = new Color(1f, 1f, 1f, 1f);
	}
}
