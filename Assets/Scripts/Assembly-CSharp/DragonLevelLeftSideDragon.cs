using System.Collections;
using UnityEngine;

public class DragonLevelLeftSideDragon : LevelProperties.Dragon.Entity
{
	public enum State
	{
		UnSpawned = 0,
		Fire = 1,
		Transition = 2,
		ThreeHeads = 3,
		Dead = 4
	}

	private enum HeadPicked
	{
		ATop = 0,
		ABottom = 1,
		CTop = 2,
		CBottom = 3,
		None = 4
	}

	private const float FRAME_RATE = 1f / 24f;

	private HeadPicked headPicked;

	private const int MAIN_LAYER = 0;

	private const int TONGUE_LAYER = 1;

	private const int FIRE_LAYER = 2;

	private const float introTime = 1.3f;

	private const float mainX = -350f;

	[SerializeField]
	private Collider2D damageBox;

	[SerializeField]
	private DragonLevelSpire spire;

	[SerializeField]
	private DragonLevelRain rain;

	[SerializeField]
	private DragonLevelBackgroundChange[] backgrounds;

	[SerializeField]
	private GameObject[] backgroundsToHide;

	[SerializeField]
	private DragonLevelFire fire;

	[SerializeField]
	private Transform fireMarcherRoot;

	[SerializeField]
	private DragonLevelFireMarcher[] fireMarcherPrefabs;

	[SerializeField]
	private DragonLevelFireMarcher fireMarcherLeaderPrefab;

	[SerializeField]
	private Transform topHead;

	[SerializeField]
	private Transform bottomHead;

	[SerializeField]
	private Transform middleHead;

	[SerializeField]
	private DragonLevelPotion horizontalPotionPrefab;

	[SerializeField]
	private DragonLevelPotion verticalPotionPrefab;

	[SerializeField]
	private DragonLevelPotion bothPotionPrefab;

	[SerializeField]
	private Transform spittle;

	private DragonLevelFireMarcher lastFireMarcher;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private bool dead;

	private bool torch;

	private int potionTypeIndex;

	private int potionTypeMainIndex;

	private int attackCountIndex;

	private int attackCountMainIndex;

	private int shotPositionIndex;

	private int shotPositionMainIndex;

	private int AttackFrames;

	private int Counter;

	private string animationString;

	private int layer;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		state = State.UnSpawned;
		headPicked = HeadPicked.None;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = damageBox.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		middleHead.GetComponent<Collider2D>().enabled = false;
		Collider2D[] components = GetComponents<Collider2D>();
		foreach (Collider2D collider2D in components)
		{
			collider2D.enabled = false;
		}
		fire.SetColliderEnabled(false);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!dead)
		{
			base.properties.DealDamage(info.damage);
			if (base.properties.CurrentHealth <= 0f && state != State.Dead)
			{
				state = State.Dead;
				StartDeath();
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public override void LevelInit(LevelProperties.Dragon properties)
	{
		base.LevelInit(properties);
		potionTypeMainIndex = Random.Range(0, properties.CurrentState.potions.potionTypeString.Length);
		potionTypeIndex = Random.Range(0, properties.CurrentState.potions.potionTypeString[potionTypeMainIndex].Split(',').Length);
		shotPositionMainIndex = Random.Range(0, properties.CurrentState.potions.shotPositionString.Length);
		shotPositionIndex = Random.Range(0, properties.CurrentState.potions.shotPositionString[shotPositionMainIndex].Split(',').Length);
		attackCountMainIndex = Random.Range(0, properties.CurrentState.potions.attackCount.Length);
		attackCountIndex = Random.Range(0, properties.CurrentState.potions.attackCount[attackCountMainIndex].Split(',').Length);
		AttackFrames = 36 - (properties.CurrentState.blowtorch.warningDurationOne + properties.CurrentState.blowtorch.warningDurationTwo);
	}

	public void StartIntro()
	{
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		AudioManager.Play("level_dragon_left_dragon_intro");
		emitAudioFromObject.Add("level_dragon_left_dragon_intro");
		float xPos = base.transform.position.x;
		yield return TweenPositionX(xPos, -350f, 1.3f, EaseUtils.EaseType.easeInSine);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro", 0);
		Collider2D[] components = GetComponents<Collider2D>();
		foreach (Collider2D collider2D in components)
		{
			collider2D.enabled = true;
		}
		StartCoroutine(fire_cr());
		StartFireMarchers();
	}

	private void TongueIntroSFX()
	{
		AudioManager.Play("level_dragon_left_dragon_tongue_intro");
		emitAudioFromObject.Add("level_dragon_left_dragon_tongue_intro");
	}

	private IEnumerator fire_cr()
	{
		state = State.Fire;
		int patternIndex = 0;
		fire.transform.parent = null;
		while (true)
		{
			string[] pattern = base.properties.CurrentState.fireAndSmoke.PatternString.Split(',');
			switch (pattern[patternIndex % pattern.Length])
			{
			case "F":
				AudioManager.Play("level_dragon_left_dragon_fire_start");
				emitAudioFromObject.Add("level_dragon_left_dragon_fire_start");
				base.animator.SetTrigger("StartFire");
				yield return base.animator.WaitForAnimationToStart(this, "Fire_Loop", 2);
				AudioManager.Play("level_dragon_left_dragon_fire_loop");
				emitAudioFromObject.Add("level_dragon_left_dragon_fire_loop");
				fire.SetColliderEnabled(true);
				yield return CupheadTime.WaitForSeconds(this, Random.Range(0.25f, 1.75f));
				AudioManager.Play("level_dragon_left_dragon_fire_end");
				emitAudioFromObject.Add("level_dragon_left_dragon_fire_end");
				fire.SetColliderEnabled(false);
				break;
			case "S":
				AudioManager.Play("level_dragon_left_dragon_smoke_start");
				emitAudioFromObject.Add("level_dragon_left_dragon_smoke_start");
				base.animator.SetTrigger("StartSmoke");
				yield return base.animator.WaitForAnimationToStart(this, "Smoke_Loop", 2);
				AudioManager.Play("level_dragon_left_dragon_smoke_loop");
				emitAudioFromObject.Add("level_dragon_left_dragon_smoke_loop");
				yield return CupheadTime.WaitForSeconds(this, Random.Range(0.66f, 1.32f));
				AudioManager.Play("level_dragon_left_dragon_smoke_end");
				emitAudioFromObject.Add("level_dragon_left_dragon_smoke_end");
				break;
			}
			base.animator.SetTrigger("Continue");
			yield return base.animator.WaitForAnimationToStart(this, "Idle", 2);
			yield return CupheadTime.WaitForSeconds(this, 0.25f);
			patternIndex++;
		}
	}

	private void StartFireMarchers()
	{
		StartCoroutine(spawnFireMarchers_cr());
		StartCoroutine(fireMarchersJump_cr());
	}

	private IEnumerator spawnFireMarchers_cr()
	{
		fireMarcherLeaderPrefab.Create(fireMarcherRoot, base.properties.CurrentState.fireMarchers);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.fireMarchers.spawnDelay);
			lastFireMarcher = fireMarcherPrefabs.RandomChoice().Create(fireMarcherRoot, base.properties.CurrentState.fireMarchers);
			yield return null;
		}
	}

	private IEnumerator fireMarchersJump_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.fireMarchers.jumpDelay.RandomFloat());
			DragonLevelFireMarcher[] fireMarchers = Object.FindObjectsOfType<DragonLevelFireMarcher>();
			fireMarchers.Shuffle();
			DragonLevelFireMarcher[] array = fireMarchers;
			foreach (DragonLevelFireMarcher dragonLevelFireMarcher in array)
			{
				if (dragonLevelFireMarcher.CanJump())
				{
					dragonLevelFireMarcher.StartJump(PlayerManager.GetNext());
					break;
				}
			}
			yield return null;
		}
	}

	public void StartThreeHeads()
	{
		StopAllCoroutines();
		state = State.Transition;
		fire.gameObject.SetActive(false);
		StartCoroutine(three_heads_cr());
	}

	private IEnumerator three_heads_cr()
	{
		base.animator.SetTrigger("StartThree");
		GetComponent<LevelBossDeathExploder>().StartExplosion();
		while (lastFireMarcher != null)
		{
			yield return null;
		}
		base.animator.SetTrigger("FoldTongue");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro_Reverse", 1);
		base.animator.SetTrigger("ToThree");
		yield return base.animator.WaitForAnimationToStart(this, "Three_Intro");
		GetComponent<LevelBossDeathExploder>().StopExplosions();
		AudioManager.Play("level_dragon_three_dragon_intro");
		emitAudioFromObject.Add("level_dragon_three_dragon_intro");
		state = State.ThreeHeads;
		DragonLevelBackgroundChange[] array = backgrounds;
		foreach (DragonLevelBackgroundChange dragonLevelBackgroundChange in array)
		{
			dragonLevelBackgroundChange.StartChange();
		}
		for (int j = 0; j < backgroundsToHide.Length; j++)
		{
			backgroundsToHide[j].SetActive(false);
		}
		spire.StartChange();
		rain.StartRain();
		StartCoroutine(potion_cr());
		StartCoroutine(blow_torch_cr());
		yield return null;
	}

	private void ActivateHeadLayers()
	{
		base.animator.SetTrigger("StartHeads");
	}

	private void SpawnPotion(int type)
	{
		spittle.gameObject.SetActive(false);
		Vector3 vector = Vector3.zero;
		DragonLevelPotion original = horizontalPotionPrefab;
		LevelProperties.Dragon.Potions potions = base.properties.CurrentState.potions;
		string[] array = potions.potionTypeString[potionTypeMainIndex].Split(',');
		if (array[potionTypeIndex][0] == 'H')
		{
			original = horizontalPotionPrefab;
		}
		else if (array[potionTypeIndex][0] == 'V')
		{
			original = verticalPotionPrefab;
		}
		else if (array[potionTypeIndex][0] == 'X')
		{
			original = bothPotionPrefab;
		}
		switch (type)
		{
		case 1:
		case 3:
			vector = topHead.position;
			break;
		case 2:
		case 4:
			vector = bottomHead.position;
			break;
		}
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector3 zero = Vector3.zero;
		zero = ((!(next.transform.position.x > base.transform.position.x)) ? ((Vector3)MathUtils.AngleToDirection(90f)) : (next.transform.position - vector));
		DragonLevelPotion dragonLevelPotion = Object.Instantiate(original);
		dragonLevelPotion.Init(vector, base.properties.CurrentState.potions.potionHP, MathUtils.DirectionToAngle(zero), base.properties.CurrentState.potions);
		if (potionTypeIndex < array.Length - 1)
		{
			potionTypeIndex++;
		}
		else
		{
			potionTypeMainIndex = (potionTypeMainIndex + 1) % potions.potionTypeString.Length;
			potionTypeIndex = 0;
		}
		spittle.gameObject.SetActive(true);
		spittle.position = vector;
	}

	private IEnumerator potion_cr()
	{
		LevelProperties.Dragon.Potions p = base.properties.CurrentState.potions;
		string[] attackCountString = p.attackCount[attackCountMainIndex].Split(',');
		string[] shotPositionString = p.shotPositionString[shotPositionMainIndex].Split(',');
		int attackCount = 0;
		while (true)
		{
			attackCountString = p.attackCount[attackCountMainIndex].Split(',');
			int.TryParse(attackCountString[attackCountIndex], out attackCount);
			for (int i = 0; i < attackCount; i++)
			{
				shotPositionString = p.shotPositionString[shotPositionMainIndex].Split(',');
				string[] pickedDragon = shotPositionString[shotPositionIndex].Split(':');
				string[] array = pickedDragon;
				foreach (string picked in array)
				{
					while (torch)
					{
						yield return null;
					}
					if (shotPositionString[shotPositionIndex][0] == 'T')
					{
						animationString = "High_Attack";
					}
					else if (shotPositionString[shotPositionIndex][0] == 'B')
					{
						animationString = "Low_Attack";
					}
					if (picked == "A")
					{
						layer = 5;
					}
					else if (picked == "C")
					{
						layer = 6;
					}
				}
				if (layer == 5 && animationString == "High_Attack")
				{
					headPicked = HeadPicked.CTop;
				}
				else if (layer == 6 && animationString == "High_Attack")
				{
					headPicked = HeadPicked.ATop;
				}
				else if (layer == 5 && animationString == "Low_Attack")
				{
					headPicked = HeadPicked.CBottom;
				}
				else if (layer == 6 && animationString == "Low_Attack")
				{
					headPicked = HeadPicked.ABottom;
				}
				yield return base.animator.WaitForAnimationToEnd(this, animationString, layer);
				if (shotPositionIndex < shotPositionString.Length - 1)
				{
					shotPositionIndex++;
				}
				else
				{
					shotPositionMainIndex = (shotPositionMainIndex + 1) % p.shotPositionString.Length;
					shotPositionIndex = 0;
				}
				yield return CupheadTime.WaitForSeconds(this, p.repeatDelay);
			}
			base.animator.SetInteger("HeadAttack", 0);
			if (attackCountIndex < attackCountString.Length - 1)
			{
				attackCountIndex++;
			}
			else
			{
				attackCountMainIndex = (attackCountMainIndex + 1) % p.attackCount.Length;
				attackCountIndex = 0;
			}
			yield return CupheadTime.WaitForSeconds(this, p.attackMainDelay);
			yield return null;
		}
	}

	private void PotionAttack(HeadPicked picked)
	{
		if (picked == headPicked)
		{
			AudioManager.Play("level_dragon_three_dragon_head_attack");
			emitAudioFromObject.Add("level_dragon_three_dragon_head_attack");
			base.animator.Play(animationString, layer);
			headPicked = HeadPicked.None;
		}
	}

	private IEnumerator blow_torch_cr()
	{
		LevelProperties.Dragon.Blowtorch p = base.properties.CurrentState.blowtorch;
		string[] delayPattern = p.attackDelayString.GetRandom().Split(',');
		middleHead.SetScale(middleHead.transform.localScale.x, p.fireSize, 1f);
		float delay = 0f;
		int delayCountIndex = Random.Range(0, delayPattern.Length);
		while (true)
		{
			float.TryParse(delayPattern[delayCountIndex], out delay);
			yield return CupheadTime.WaitForSeconds(this, delay);
			delayCountIndex = (delayCountIndex + 1) % delayPattern.Length;
			torch = true;
			yield return base.animator.WaitForAnimationToEnd(this, "Dragon_Head_Idle_Loop", 3);
			AudioManager.Play("level_dragon_torch_warning_1_start");
			emitAudioFromObject.Add("level_dragon_torch_warning_1_start");
			base.animator.Play("Torch_Warning_One", 4);
			yield return base.animator.WaitForAnimationToEnd(this, "Torch_End", 4);
			torch = false;
			yield return null;
		}
	}

	private void ActivateTorch()
	{
		middleHead.GetComponent<Animator>().SetBool("TorchOn", true);
		AudioManager.Play("level_dragon_three_dragon_head_b_torch_attack_burst");
		emitAudioFromObject.Add("level_dragon_three_dragon_head_b_torch_attack_burst");
		AudioManager.PlayLoop("level_dragon_three_dragon_head_b_torch_attack_loop");
		emitAudioFromObject.Add("level_dragon_three_dragon_head_b_torch_attack_loop");
	}

	private void DeactivateTorch()
	{
		middleHead.GetComponent<Animator>().SetBool("TorchOn", false);
		AudioManager.Stop("level_dragon_three_dragon_head_b_torch_attack_loop");
	}

	private void Torch1Counter()
	{
		if (Counter >= base.properties.CurrentState.blowtorch.warningDurationOne)
		{
			Counter = 0;
			AudioManager.Play("level_dragon_three_dragon_head_b_torch_continue_one");
			emitAudioFromObject.Add("level_dragon_three_dragon_head_b_torch_continue_one");
			base.animator.Play("Torch_Continue", 4);
		}
		else
		{
			Counter++;
		}
	}

	private void Attack1Counter()
	{
		if (Counter >= AttackFrames / 2 + AttackFrames % 2)
		{
			Counter = 0;
			AudioManager.Play("level_dragon_three_dragon_head_b_torch_warning_2_start");
			emitAudioFromObject.Add("level_dragon_three_dragon_head_b_torch_warning_2_start");
			base.animator.Play("Torch_Warning_Two", 4);
		}
		else
		{
			Counter++;
		}
	}

	private void Torch2Counter()
	{
		if (Counter >= base.properties.CurrentState.blowtorch.warningDurationTwo)
		{
			Counter = 0;
			AudioManager.Play("level_dragon_three_dragon_head_b_torch_continue_one");
			emitAudioFromObject.Add("level_dragon_three_dragon_head_b_torch_continue_one");
			base.animator.Play("Torch_Continue_Two", 4);
		}
		else
		{
			Counter++;
		}
	}

	private void Attack2Counter()
	{
		if (Counter >= AttackFrames / 2)
		{
			Counter = 0;
			AudioManager.Play("level_dragon_three_dragon_head_b_torch_end");
			emitAudioFromObject.Add("level_dragon_three_dragon_head_b_torch_end");
			base.animator.Play("Torch_End", 4);
		}
		else
		{
			Counter++;
		}
	}

	private void StartDeath()
	{
		AudioManager.Play("level_dragon_three_dragon_death");
		emitAudioFromObject.Add("level_dragon_three_dragon_death");
		StopAllCoroutines();
		middleHead.gameObject.SetActive(false);
		base.animator.SetTrigger("Continue");
		if (Level.Current.mode == Level.Mode.Easy)
		{
			base.animator.SetTrigger("DeadEASY");
		}
		else
		{
			base.animator.SetTrigger("Dead");
		}
	}
}
