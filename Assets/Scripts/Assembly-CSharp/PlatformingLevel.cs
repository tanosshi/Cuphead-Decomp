using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformingLevel : Level
{
	public enum Theme
	{
		Forest = 0
	}

	public const float TIMELINE_LENGTH = 100f;

	public List<CoinPositionAndID> LevelCoinsIDs = new List<CoinPositionAndID>();

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private Sprite _bossPortraitAlt;

	[SerializeField]
	private string _bossQuote;

	[SerializeField]
	private string _bossQuoteAlt;

	[SerializeField]
	private float goalTime;

	private Levels _currentLevel;

	private Scenes _currentScene;

	public bool useAltQuote;

	public new static PlatformingLevel Current { get; private set; }

	public override Levels CurrentLevel
	{
		get
		{
			return _currentLevel;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return _currentScene;
		}
	}

	public override Sprite BossPortrait
	{
		get
		{
			return (!useAltQuote) ? _bossPortrait : _bossPortraitAlt;
		}
	}

	public override string BossQuote
	{
		get
		{
			return (!useAltQuote) ? _bossQuote : _bossQuoteAlt;
		}
	}

	protected override void Awake()
	{
		_currentLevel = SceneLoader.CurrentLevel;
		_currentScene = EnumUtils.Parse<Scenes>(LevelProperties.GetLevelScene(_currentLevel));
		goalTimes = new GoalTimes(goalTime, goalTime, goalTime);
		Level.OverrideDifficulty = true;
		base.mode = Mode.Normal;
		base.Awake();
		Current = this;
	}

	protected override void Start()
	{
		base.Start();
		LevelCoinsIDs = LevelCoinsIDs.OrderBy((CoinPositionAndID c) => c.xPos).ToList();
	}

	protected override void OnLevelStart()
	{
		base.OnLevelStart();
		base.timeline = new Timeline();
		base.timeline.health = 100f;
		StartCoroutine(checkPosition_cr());
		Level.ScoringData.pacifistRun = true;
		PlatformingLevelExit.OnWinStartEvent += OnWinStart;
		PlatformingLevelExit.OnWinCompleteEvent += OnWinComplete;
	}

	private void OnWinStart()
	{
		CupheadLevelCamera.Current.MoveRightCollider();
	}

	private void OnWinComplete()
	{
		LevelCoin.OnLevelComplete();
		Level.ScoringData.coinsCollected = PlayerData.Data.GetNumCoinsCollectedInLevel(CurrentLevel);
		Level.ScoringData.useCoinsInsteadOfSuperMeter = true;
		zHack_OnWin();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Reset()
	{
		base.Reset();
		type = Type.Platforming;
		bounds.bottom = 500;
		camera.moveX = true;
		camera.moveY = true;
		camera.mode = CupheadLevelCamera.Mode.Platforming;
		camera.colliders = true;
		camera.bounds.rightEnabled = false;
		camera.bounds.topEnabled = false;
	}

	private IEnumerator checkPosition_cr()
	{
		while (true)
		{
			AbstractPlayerController[] array = players;
			foreach (AbstractPlayerController abstractPlayerController in array)
			{
				if (abstractPlayerController != null && !abstractPlayerController.IsDead && base.LevelType == Type.Platforming && camera.mode == CupheadLevelCamera.Mode.Path)
				{
					float value = camera.path.GetClosestNormalizedPoint(abstractPlayerController.center, abstractPlayerController.center, true, true) * 100f;
					base.timeline.SetPlayerDamage(abstractPlayerController.id, value);
				}
			}
			yield return null;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.2f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		if (camera.mode == CupheadLevelCamera.Mode.Path)
		{
			camera.path.DrawGizmos(a, base.baseTransform.position);
		}
	}
}
