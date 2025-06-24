using System.Collections;
using UnityEngine;

public class ShmupTutorialLevel : Level
{
	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	public Animator canvasAnimator;

	public float waitForAnimationTime;

	private LevelProperties.ShmupTutorial properties;

	public override Sprite BossPortrait
	{
		get
		{
			return _bossPortrait;
		}
	}

	public override string BossQuote
	{
		get
		{
			return _bossQuote;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.ShmupTutorial;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_shmup_tutorial;
		}
	}

	protected override void Start()
	{
		base.Start();
		canvasAnimator.SetTrigger("StartAnimation");
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(shmuptutorialPattern_cr());
	}

	private IEnumerator shmuptutorialPattern_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			yield return StartCoroutine(nextPattern_cr());
			yield return null;
		}
	}

	private IEnumerator shmupTutorialStartAnimation_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, waitForAnimationTime);
		canvasAnimator.SetTrigger("StartAnimation");
	}

	private IEnumerator nextPattern_cr()
	{
		Debug.LogWarning("No pattern programmed for " + properties.CurrentState.NextPattern);
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.ShmupTutorial.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
