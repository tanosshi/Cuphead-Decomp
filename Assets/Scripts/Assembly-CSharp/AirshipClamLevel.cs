using System.Collections;
using UnityEngine;

public class AirshipClamLevel : Level
{
	[SerializeField]
	private AirshipClamLevelClam clam;

	private bool attacking;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	[Multiline]
	private string _bossQuote;

	private LevelProperties.AirshipClam properties;

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
			return Levels.AirshipClam;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_airship_clam;
		}
	}

	protected override void Start()
	{
		base.Start();
		clam.LevelInit(properties);
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(airshipclamPattern_cr());
	}

	private IEnumerator airshipclamPattern_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			yield return StartCoroutine(nextPattern_cr());
			yield return null;
		}
	}

	private IEnumerator nextPattern_cr()
	{
		LevelProperties.AirshipClam.Pattern p = properties.CurrentState.NextPattern;
		switch (p)
		{
		case LevelProperties.AirshipClam.Pattern.Spit:
			StartCoroutine(spit_cr());
			break;
		case LevelProperties.AirshipClam.Pattern.Barnacles:
			StartCoroutine(barnacles_cr());
			break;
		default:
			Debug.LogWarning("No pattern programmed for " + p);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			break;
		}
	}

	private IEnumerator spit_cr()
	{
		if (!attacking)
		{
			clam.OnSpitStart(EndAttack);
			attacking = true;
		}
		while (attacking)
		{
			yield return null;
		}
	}

	private IEnumerator barnacles_cr()
	{
		if (!attacking)
		{
			clam.OnBarnaclesStart(EndAttack);
			attacking = true;
		}
		while (attacking)
		{
			yield return null;
		}
	}

	private void EndAttack()
	{
		attacking = false;
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.AirshipClam.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
