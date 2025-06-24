using System;
using System.Collections;
using UnityEngine;

public class VeggiesLevel : Level
{
	private enum CurrentBoss
	{
		Potato = 0,
		Onion = 1,
		Beet = 2,
		Peas = 3,
		Carrot = 4
	}

	[Serializable]
	public class Prefabs
	{
		public VeggiesLevelPotato potato;

		public VeggiesLevelOnion onion;

		public VeggiesLevelBeet beet;

		public VeggiesLevelPeas peas;

		public VeggiesLevelCarrot carrot;
	}

	[Space(10f)]
	[SerializeField]
	private Prefabs prefabs;

	private VeggiesLevelPotato potato;

	[Header("Boss Info")]
	[SerializeField]
	private Sprite _bossPortraitPotato;

	[SerializeField]
	private Sprite _bossPortraitOnion;

	[SerializeField]
	private Sprite _bossPortraitCarrot;

	[SerializeField]
	private string _bossQuotePotato;

	[SerializeField]
	private string _bossQuoteOnion;

	[SerializeField]
	private string _bossQuoteCarrot;

	private CurrentBoss currentBoss;

	private LevelProperties.Veggies properties;

	public override Sprite BossPortrait
	{
		get
		{
			switch (currentBoss)
			{
			case CurrentBoss.Potato:
				return _bossPortraitPotato;
			case CurrentBoss.Onion:
				return _bossPortraitOnion;
			case CurrentBoss.Carrot:
				return _bossPortraitCarrot;
			default:
				Debug.LogWarning(string.Concat("Boss '", currentBoss, "' not configured!"));
				return _bossPortraitPotato;
			}
		}
	}

	public override string BossQuote
	{
		get
		{
			switch (currentBoss)
			{
			case CurrentBoss.Potato:
				return _bossQuotePotato;
			case CurrentBoss.Onion:
				return _bossQuoteOnion;
			case CurrentBoss.Carrot:
				return _bossQuoteCarrot;
			default:
				Debug.LogWarning(string.Concat("Boss '", currentBoss, "' not configured!"));
				return "QuoteNone";
			}
		}
	}

	public override Levels CurrentLevel
	{
		get
		{
			return Levels.Veggies;
		}
	}

	public override Scenes CurrentScene
	{
		get
		{
			return Scenes.scene_level_veggies;
		}
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(potatoStart_cr());
		properties.OnBossDamaged -= base.timeline.DealDamage;
		base.timeline = new Timeline();
		base.timeline.health = 0f;
		base.timeline.health += properties.CurrentState.potato.hp;
		if (base.mode != Mode.Easy)
		{
			base.timeline.health += properties.CurrentState.onion.hp;
		}
		base.timeline.health += properties.CurrentState.carrot.hp;
		if (base.mode != Mode.Easy)
		{
			base.timeline.AddEventAtHealth("Onion", base.timeline.GetHealthOfLastEvent() + properties.CurrentState.potato.hp);
		}
		base.timeline.AddEventAtHealth("Carrot", base.timeline.GetHealthOfLastEvent() + ((base.mode != Mode.Easy) ? properties.CurrentState.onion.hp : properties.CurrentState.potato.hp));
	}

	protected override void OnLevelStart()
	{
		StartCoroutine(veggiesPattern_cr());
	}

	private IEnumerator veggiesPattern_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		yield return StartCoroutine(potato_cr());
		if (base.mode != Mode.Easy)
		{
			yield return StartCoroutine(onion_cr());
		}
		yield return StartCoroutine(carrot_cr());
		yield return StartCoroutine(win_cr());
	}

	private IEnumerator potatoStart_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		potato = prefabs.potato.InstantiatePrefab<VeggiesLevelPotato>();
		potato.OnDamageTakenEvent += base.timeline.DealDamage;
	}

	private IEnumerator potato_cr()
	{
		currentBoss = CurrentBoss.Potato;
		potato.LevelInitWithGroup(properties.CurrentState.potato);
		while (potato.state != VeggiesLevelPotato.State.Complete)
		{
			yield return null;
		}
	}

	private IEnumerator onion_cr()
	{
		currentBoss = CurrentBoss.Onion;
		yield return CupheadTime.WaitForSeconds(this, 1f);
		VeggiesLevelOnion v = prefabs.onion.InstantiatePrefab<VeggiesLevelOnion>();
		v.LevelInitWithGroup(properties.CurrentState.onion);
		v.OnDamageTakenEvent += base.timeline.DealDamage;
		v.OnHappyLeave += OnionHappyLeave;
		while (v.state != VeggiesLevelOnion.State.Complete)
		{
			yield return null;
		}
	}

	private void OnionHappyLeave()
	{
		base.timeline.DealDamage(properties.CurrentState.onion.hp);
	}

	private IEnumerator beet_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2f);
		VeggiesLevelBeet v = prefabs.beet.InstantiatePrefab<VeggiesLevelBeet>();
		v.LevelInitWithGroup(properties.CurrentState.beet);
		v.OnDamageTakenEvent += base.timeline.DealDamage;
		while (v.state != VeggiesLevelBeet.State.Complete)
		{
			yield return null;
		}
	}

	private IEnumerator peas_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2f);
		VeggiesLevelPeas v = prefabs.peas.InstantiatePrefab<VeggiesLevelPeas>();
		v.LevelInitWithGroup(properties.CurrentState.peas);
		v.OnDamageTakenEvent += base.timeline.DealDamage;
		while (v.state != VeggiesLevelPeas.State.Complete)
		{
			yield return null;
		}
	}

	private IEnumerator carrot_cr()
	{
		currentBoss = CurrentBoss.Carrot;
		VeggiesLevelCarrot v = prefabs.carrot.InstantiatePrefab<VeggiesLevelCarrot>();
		v.LevelInit(properties);
		v.OnDamageTakenEvent += base.timeline.DealDamage;
		while (v.state != VeggiesLevelCarrot.State.Complete)
		{
			yield return null;
		}
	}

	private IEnumerator win_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		properties.WinInstantly();
	}

	protected override void PartialInit()
	{
		properties = LevelProperties.Veggies.GetMode(base.mode);
		properties.OnStateChange += base.zHack_OnStateChanged;
		properties.OnBossDeath += base.zHack_OnWin;
		base.timeline = properties.CreateTimeline(base.mode);
		goalTimes = properties.goalTimes;
		properties.OnBossDamaged += base.timeline.DealDamage;
		base.PartialInit();
	}
}
