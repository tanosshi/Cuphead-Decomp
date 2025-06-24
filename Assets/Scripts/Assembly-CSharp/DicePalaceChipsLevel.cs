using System.Collections;
using UnityEngine;

public class DicePalaceChipsLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private GameObject background;

	[SerializeField]
	private DicePalaceChipsLevelChips chips;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalaceChips properties;

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
			return DicePalaceLevels.DicePalaceChips;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalaceChips;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_chips;
		}
	}

	protected override void Start()
	{
		base.Start();
		chips.LevelInit(properties);
		StartCoroutine(CupheadLevelCamera.Current.rotate_camera());
		StartCoroutine(rotate_background_cr());
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalacechipsPattern_cr());
	}

	private IEnumerator dicepalacechipsPattern_cr()
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

	private IEnumerator rotate_background_cr()
	{
		float time = 1.5f;
		float t = 0f;
		while (true)
		{
			t += (float)CupheadTime.Delta;
			float phase = Mathf.Sin(t / time);
			background.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, phase * 1f));
			yield return null;
		}
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.DicePalaceChips.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
