using System;
using System.Collections;
using RektTransform;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LevelGameOverGUI : AbstractMonoBehaviour
{
	private enum State
	{
		Init = 0,
		Ready = 1,
		Exiting = 2
	}

	[Serializable]
	public class TimelineObjects
	{
		public RectTransform timeline;

		public RectTransform line;

		[Header("Players")]
		public Image cuphead;

		public Image mugman;

		[Header("Positions")]
		public Transform start;

		public Transform end;

		private LevelGameOverGUI gui;

		public void Setup(LevelGameOverGUI gui, Level.Timeline properties)
		{
			int num = 0;
			foreach (Level.Timeline.Event @event in properties.events)
			{
				RectTransform rectTransform = UnityEngine.Object.Instantiate(line);
				rectTransform.SetParent(line.parent, false);
				rectTransform.SetAsFirstSibling();
				rectTransform.name = "Line " + num++;
				Vector3 localPosition = Vector3.Lerp(end.localPosition, start.localPosition, @event.percentage);
				localPosition.y -= 7f;
				rectTransform.localPosition = localPosition;
			}
			line.gameObject.SetActive(false);
			gui.StartCoroutine(timelineIcon_cr(cuphead, properties.cuphead / properties.health));
			if (!PlayerManager.Multiplayer)
			{
				mugman.gameObject.SetActive(false);
			}
			else
			{
				gui.StartCoroutine(timelineIcon_cr(mugman, properties.mugman / properties.health));
			}
		}

		private IEnumerator timelineIcon_cr(Image icon, float percent)
		{
			Color startColor = new Color(1f, 1f, 1f, 0f);
			Color endColor = new Color(1f, 1f, 1f, 1f);
			float t = 0f;
			Vector3 endPosition = Vector3.Lerp(start.localPosition, end.localPosition, percent);
			icon.rectTransform.localPosition = start.localPosition;
			while (t < 2f)
			{
				float val = t / 2f;
				Vector3 newPosition = Vector3.Lerp(start.localPosition, endPosition, EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 0f, 1f, val));
				icon.rectTransform.localPosition = newPosition;
				icon.color = Color.Lerp(startColor, endColor, val * 8f);
				t += Time.deltaTime;
				yield return null;
			}
			icon.rectTransform.localPosition = endPosition;
		}
	}

	public static LevelGameOverGUI Current;

	[SerializeField]
	private Image youDiedText;

	[Space(10f)]
	[SerializeField]
	private CanvasGroup cardCanvasGroup;

	[Space(10f)]
	[SerializeField]
	private CanvasGroup helpCanvasGroup;

	[Space(10f)]
	[SerializeField]
	private Image bossPortraitImage;

	[SerializeField]
	private Text bossQuoteText;

	[SerializeField]
	private LocalizationHelper bossQuoteLocalization;

	[Space(10f)]
	[SerializeField]
	private Text[] menuItems;

	[SerializeField]
	private TimelineObjects timeline;

	private State state;

	private CupheadInput.AnyPlayerInput input;

	private CanvasGroup canvasGroup;

	private int selection;

	public static Color COLOR_SELECTED { get; private set; }

	public static Color COLOR_INACTIVE { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		canvasGroup = GetComponent<CanvasGroup>();
		base.gameObject.SetActive(false);
		input = new CupheadInput.AnyPlayerInput();
		cardCanvasGroup.alpha = 0f;
		helpCanvasGroup.alpha = 0f;
		ignoreGlobalTime = true;
		timeLayer = CupheadTime.Layer.UI;
		COLOR_SELECTED = menuItems[0].color;
		COLOR_INACTIVE = menuItems[menuItems.Length - 1].color;
		state = State.Init;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Current = null;
	}

	protected override void Update()
	{
		base.Update();
		if (state == State.Ready)
		{
			if (getButtonDown(CupheadButton.Accept))
			{
				Select();
				state = State.Exiting;
			}
			if (getButtonDown(CupheadButton.MenuDown))
			{
				selection++;
			}
			if (getButtonDown(CupheadButton.MenuUp))
			{
				selection--;
			}
			selection = Mathf.Clamp(selection, 0, menuItems.Length - 1);
			UpdateSelection();
		}
	}

	private bool getButtonDown(CupheadButton button)
	{
		return input.GetButtonDown(button);
	}

	private void UpdateSelection()
	{
		for (int i = 0; i < menuItems.Length; i++)
		{
			Text text = menuItems[i];
			if (i == selection)
			{
				text.color = COLOR_SELECTED;
			}
			else
			{
				text.color = COLOR_INACTIVE;
			}
		}
	}

	private void Select()
	{
		AudioManager.SnapshotReset(SceneLoader.SceneName, 2f);
		AudioManager.ChangeBGMPitch(1f, 2f);
		switch (selection)
		{
		default:
			Retry();
			AudioManager.Play("level_menu_card_down");
			break;
		case 1:
			ExitToMap();
			AudioManager.Play("level_menu_card_down");
			break;
		case 2:
			QuitGame();
			AudioManager.Play("level_menu_card_down");
			break;
		}
	}

	private void Retry()
	{
		if (Level.IsDicePalaceMain || Level.IsDicePalace)
		{
			DicePalaceMainLevelGameInfo.CleanUpRetry();
		}
		SceneLoader.ReloadLevel();
	}

	private void ExitToMap()
	{
		SceneLoader.LoadLastMap();
	}

	private void QuitGame()
	{
		PlayerManager.ResetPlayers();
		SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Fade, SceneLoader.Transition.Iris);
	}

	private void SetAlpha(float value)
	{
		canvasGroup.alpha = value;
	}

	private void SetTextAlpha(float value)
	{
		Color color = youDiedText.color;
		color.a = value;
		youDiedText.color = color;
	}

	private void SetCardValue(float value)
	{
		cardCanvasGroup.alpha = value;
		helpCanvasGroup.alpha = value;
		cardCanvasGroup.transform.SetLocalEulerAngles(null, null, Mathf.Lerp(30f, 4f, value));
	}

	public void In()
	{
		base.gameObject.SetActive(true);
		bossPortraitImage.sprite = Level.Current.BossPortrait;
		if (bossQuoteLocalization == null)
		{
			bossQuoteText.text = "\"" + Level.Current.BossQuote + "\"";
		}
		else
		{
			bossQuoteLocalization.ApplyTranslation(Localization.Find(Level.Current.BossQuote));
		}
		if (bossPortraitImage.sprite != null)
		{
			bossPortraitImage.rectTransform.SetSize(bossPortraitImage.sprite.texture.width, bossPortraitImage.sprite.texture.height);
		}
		StartCoroutine(in_cr());
	}

	private IEnumerator in_cr()
	{
		AudioManager.Play("level_menu_card_up");
		yield return TweenValue(0f, 1f, 0.05f, EaseUtils.EaseType.linear, SetAlpha);
		yield return new WaitForSeconds(1f);
		PlayerDeathEffect[] array = UnityEngine.Object.FindObjectsOfType<PlayerDeathEffect>();
		foreach (PlayerDeathEffect playerDeathEffect in array)
		{
			playerDeathEffect.GameOverUnpause();
		}
		PlanePlayerDeathPart[] array2 = UnityEngine.Object.FindObjectsOfType<PlanePlayerDeathPart>();
		foreach (PlanePlayerDeathPart planePlayerDeathPart in array2)
		{
			planePlayerDeathPart.GameOverUnpause();
		}
		yield return TweenValue(1f, 0f, 0.25f, EaseUtils.EaseType.linear, SetTextAlpha);
		yield return new WaitForSeconds(0.3f);
		AudioManager.Play("player_die_vinylscratch");
		AudioManager.HandleSnapshot(AudioManager.Snapshots.Death.ToString(), 4f);
		AudioManager.ChangeBGMPitch(0.7f, 6f);
		CupheadLevelCamera.Current.StartBlur();
		timeline.Setup(this, Level.Current.timeline);
		TweenValue(0f, 1f, 0.3f, EaseUtils.EaseType.easeOutCubic, SetCardValue);
		state = State.Ready;
		yield return null;
	}
}
