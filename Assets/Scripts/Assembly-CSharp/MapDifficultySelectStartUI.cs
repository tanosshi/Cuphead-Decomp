using UnityEngine;
using UnityEngine.UI;

public class MapDifficultySelectStartUI : AbstractMapSceneStartUI
{
	[SerializeField]
	private Image inAnimated;

	[SerializeField]
	private Sprite[] inSprites;

	[SerializeField]
	private Image[] separatorsAnimated;

	[SerializeField]
	private Sprite[] separatorsSprites;

	[SerializeField]
	private RectTransform cursor;

	[Header("Options")]
	[SerializeField]
	private RectTransform easy;

	[SerializeField]
	private RectTransform normal;

	[SerializeField]
	private RectTransform normalSeparator;

	[SerializeField]
	private RectTransform hard;

	[SerializeField]
	private RectTransform hardSeparator;

	[SerializeField]
	private RectTransform box;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color unselectedColor;

	[Header("Stage")]
	[SerializeField]
	private LocalizationHelper bossImage;

	[Header("Animation")]
	[SerializeField]
	private new Animator animator;

	private Text[] difficulyTexts;

	private Level.Mode[] options;

	private int index = 1;

	private float cursorY;

	public static MapDifficultySelectStartUI Current { get; protected set; }

	public static Level.Mode Mode { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		switch (Level.CurrentMode)
		{
		case Level.Mode.Easy:
			index = 0;
			break;
		case Level.Mode.Normal:
			index = 1;
			break;
		case Level.Mode.Hard:
			index = 2;
			break;
		default:
			Debug.LogWarning(string.Concat("Mode '", Level.CurrentMode, "' not yet configured!"));
			break;
		}
		options = new Level.Mode[3]
		{
			Level.Mode.Easy,
			Level.Mode.Normal,
			Level.Mode.Hard
		};
		if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_4)
		{
			if (!PlayerData.Data.IsHardModeAvailable)
			{
				options = new Level.Mode[1] { Level.Mode.Normal };
				hard.gameObject.SetActive(false);
				hardSeparator.gameObject.SetActive(false);
			}
			else
			{
				options = new Level.Mode[2]
				{
					Level.Mode.Normal,
					Level.Mode.Hard
				};
			}
			index = Mathf.Max(0, index - 1);
			easy.gameObject.SetActive(false);
			normalSeparator.gameObject.SetActive(false);
		}
		else
		{
			if (!PlayerData.Data.IsHardModeAvailable)
			{
				options = new Level.Mode[2]
				{
					Level.Mode.Easy,
					Level.Mode.Normal
				};
			}
			hard.gameObject.SetActive(PlayerData.Data.IsHardModeAvailable);
			hardSeparator.gameObject.SetActive(PlayerData.Data.IsHardModeAvailable);
		}
		difficulyTexts = new Text[3];
		difficulyTexts[0] = easy.GetComponent<Text>();
		difficulyTexts[1] = normal.GetComponent<Text>();
		difficulyTexts[2] = hard.GetComponent<Text>();
	}

	public new void In(MapPlayerController playerController)
	{
		base.In(playerController);
		if (animator != null)
		{
			animator.SetTrigger("ZoomIn");
			AudioManager.Play("world_map_level_menu_open");
		}
		inAnimated.sprite = inSprites[Random.Range(0, inSprites.Length)];
		for (int i = 0; i < separatorsAnimated.Length; i++)
		{
			separatorsAnimated[i].sprite = separatorsSprites[Random.Range(0, separatorsSprites.Length)];
		}
		TranslationElement translationElement = Localization.Find(level + "Selection");
		if (bossImage != null && translationElement != null)
		{
			bossImage.ApplyTranslation(translationElement);
		}
		for (int j = 0; j < difficulyTexts.Length; j++)
		{
			difficulyTexts[j].color = unselectedColor;
		}
		difficulyTexts[(int)Level.CurrentMode].color = selectedColor;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Update()
	{
		base.Update();
		UpdateCursor();
		if (base.CurrentState == State.Active)
		{
			CheckInput();
		}
	}

	private void CheckInput()
	{
		if (base.Able)
		{
			if (GetButtonDown(CupheadButton.MenuLeft))
			{
				Next(-1);
			}
			if (GetButtonDown(CupheadButton.MenuRight))
			{
				Next(1);
			}
			if (GetButtonDown(CupheadButton.Cancel))
			{
				Out();
			}
			if (GetButtonDown(CupheadButton.Accept))
			{
				LoadLevel();
			}
		}
	}

	private void Next(int direction)
	{
		if ((index != options.Length - 1 && direction != -1) || (index != 0 && direction != 1))
		{
			AudioManager.Play("world_map_level_difficulty_hover");
		}
		index = Mathf.Clamp(index + direction, 0, options.Length - 1);
		Level.SetCurrentMode(options[index]);
		UpdateCursor();
		for (int i = 0; i < difficulyTexts.Length; i++)
		{
			difficulyTexts[i].color = unselectedColor;
		}
		difficulyTexts[(int)Level.CurrentMode].color = selectedColor;
	}

	private void UpdateCursor()
	{
		Vector3 position = cursor.transform.position;
		position.y = normal.position.y;
		Level.Mode mode = Level.CurrentMode;
		if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_4 && mode == Level.Mode.Easy)
		{
			mode = Level.Mode.Normal;
		}
		switch (mode)
		{
		case Level.Mode.Easy:
			position.x = easy.position.x;
			cursor.sizeDelta = new Vector2(easy.sizeDelta.x + 30f, easy.sizeDelta.y + 20f);
			break;
		case Level.Mode.Normal:
			position.x = normal.position.x;
			cursor.sizeDelta = new Vector2(normal.sizeDelta.x + 30f, normal.sizeDelta.y + 20f);
			break;
		case Level.Mode.Hard:
			position.x = hard.position.x;
			cursor.sizeDelta = new Vector2(hard.sizeDelta.x + 30f, hard.sizeDelta.y + 20f);
			break;
		default:
			Debug.LogWarning(string.Concat("Mode '", Level.CurrentMode, "' not yet configured!"));
			break;
		}
		cursor.transform.position = position;
	}
}
