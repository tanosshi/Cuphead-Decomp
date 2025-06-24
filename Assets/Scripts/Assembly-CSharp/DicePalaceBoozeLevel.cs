using System.Collections;
using UnityEngine;

public class DicePalaceBoozeLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private Transform[] lamps;

	[SerializeField]
	private DicePalaceBoozeLevelDecanter decanter;

	[SerializeField]
	private DicePalaceBoozeLevelMartini martini;

	[SerializeField]
	private DicePalaceBoozeLevelTumbler tumbler;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceBooze properties;

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

	public override DicePalaceLevels CurrentDicePalaceLevel
	{
		get
		{
			return DicePalaceLevels.DicePalaceBooze;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceBooze;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_booze;
		}
	}

	protected override void Start()
	{
		base.Start();
		decanter.LevelInit(properties);
		martini.LevelInit(properties);
		tumbler.LevelInit(properties);
		Transform[] array = lamps;
		foreach (Transform lamp in array)
		{
			StartCoroutine(lamps_cr(lamp));
		}
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalaceboozePattern_cr());
	}

	private IEnumerator dicepalaceboozePattern_cr()
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
		Debug.LogWarning("No pattern programmed for " + properties.CurrentState.NextPattern);
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	private IEnumerator lamps_cr(Transform lamp)
	{
		float t = 0f;
		float time = 0f;
		float maxSpeed = 0f;
		float speed = maxSpeed;
		while (true)
		{
			t = 0f;
			maxSpeed = Random.Range(5f, 15f);
			speed = maxSpeed;
			while (!CupheadLevelCamera.Current.isShaking)
			{
				yield return null;
			}
			bool movingRight = Rand.Bool();
			while (speed > 0f)
			{
				t = ((!movingRight) ? (t - (float)CupheadTime.Delta) : (t + (float)CupheadTime.Delta));
				float phase = Mathf.Sin(t);
				lamp.localRotation = Quaternion.Euler(new Vector3(0f, 0f, phase * speed));
				speed -= 0.05f;
				yield return null;
			}
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.DicePalaceBooze.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
