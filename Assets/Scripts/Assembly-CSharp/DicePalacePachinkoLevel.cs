using System.Collections;
using UnityEngine;

public class DicePalacePachinkoLevel : AbstractDicePalaceLevel
{
	[SerializeField]
	private Transform[] starDiscs;

	[SerializeField]
	private DicePalacePachinkoLevelPipes pipes;

	[SerializeField]
	private DicePalacePachinkoLevelPachinko pachinko;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortrait;

	[SerializeField]
	private string _bossQuote;

	private LevelProperties.DicePalacePachinko properties;

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
			return DicePalaceLevels.DicePalacePachinko;
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.DicePalacePachinko;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_dice_palace_pachinko;
		}
	}

	protected override void Start()
	{
		base.Start();
		pipes.LevelInit(properties);
		pachinko.LevelInit(properties);
		Transform[] array = starDiscs;
		foreach (Transform disc in array)
		{
			StartCoroutine(star_disc_cr(disc));
		}
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(dicepalacepachinkoPattern_cr());
	}

	private IEnumerator star_disc_cr(Transform disc)
	{
		bool fadingOut = Rand.Bool();
		while (true)
		{
			float fadeTime = Random.Range(0.1f, 0.3f);
			float holdTime = Random.Range(0.1f, 0.3f);
			yield return CupheadTime.WaitForSeconds(this, holdTime);
			if (fadingOut)
			{
				float t = 0f;
				while (t < fadeTime)
				{
					disc.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - t / fadeTime);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
			}
			else
			{
				float t2 = 0f;
				while (t2 < fadeTime)
				{
					disc.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, t2 / fadeTime);
					t2 += (float)CupheadTime.Delta;
					yield return null;
				}
			}
			fadingOut = !fadingOut;
			yield return null;
		}
	}

	private IEnumerator dicepalacepachinkoPattern_cr()
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

	protected override void PartialInit()
	{
		properties = LevelProperties.DicePalacePachinko.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
