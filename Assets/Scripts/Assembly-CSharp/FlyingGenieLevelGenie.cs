using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingGenieLevelGenie : LevelProperties.FlyingGenie.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Transform = 2,
		Treasure = 3,
		Disappear = 4,
		Dead = 5,
		Coffin = 6
	}

	private const float MUMMY_SPAWN_OFFSET = 200f;

	[SerializeField]
	private Transform hieroBG;

	[SerializeField]
	private Transform brickBG;

	[SerializeField]
	private ScrollingSprite hiero;

	[SerializeField]
	private ScrollingSprite brick;

	[SerializeField]
	private Transform sawMask;

	[SerializeField]
	private GameObject casket;

	[SerializeField]
	private FlyingGenieLevelMeditateFX meditateEffect;

	[SerializeField]
	private BasicProjectile skullPrefab;

	[SerializeField]
	private FlyingGenieLevelBouncer bouncerPrefab;

	[SerializeField]
	private FlyingGenieLevelObelisk obeliskPrefab;

	[SerializeField]
	private FlyingGenieLevelSphinx sphinxPrefab;

	[Space(10f)]
	[SerializeField]
	private FlyingGenieLevelGem gemPrefab;

	[Space(10f)]
	[SerializeField]
	private FlyingGenieLevelGoop goop;

	[SerializeField]
	private FlyingGenieLevelMummy mummyClassic;

	[SerializeField]
	private FlyingGenieLevelMummy mummyChomper;

	[SerializeField]
	private FlyingGenieLevelMummy mummyChaser;

	[SerializeField]
	private FlyingGenieLevelSword swordPrefab;

	[SerializeField]
	private FlyingGenieLevelGenieTransform genieTransformed;

	[Space(10f)]
	[SerializeField]
	private Effect puffEffect;

	[SerializeField]
	private Transform puffRoot;

	[SerializeField]
	private Transform skullRoot;

	[SerializeField]
	private SpriteRenderer carpet;

	[SerializeField]
	private Transform morphRoot;

	[SerializeField]
	private Transform treasureRoot;

	private List<int> treasureAttacks;

	private List<FlyingGenieLevelObelisk> obelisks;

	private FlyingGenieLevelMeditateFX meditate;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	private bool attackLooping;

	private bool stillShooting;

	private bool smallGemTimerUp;

	private bool bigGemTimerUp;

	private int skullCounter;

	private int treasureCounter;

	private Vector3 casketStartPos;

	private Coroutine patternCoroutine;

	private FlyingGenieLevelMeditateFX meditateP1;

	private FlyingGenieLevelMeditateFX meditateP2;

	private Color defaultColor;

	private string[] swordPinkPattern;

	private int swordPinkIndex;

	private string[] gemPinkPattern;

	private int gemPinkIndex;

	private string[] sphinxPinkPattern;

	private int sphinxPinkIndex;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		defaultColor = GetComponent<SpriteRenderer>().color;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(intro_cr());
		casketStartPos = casket.transform.position;
		GetComponent<Collider2D>().enabled = false;
		hiero.speed = base.properties.CurrentState.obelisk.obeliskMovementSpeed;
		brick.speed = base.properties.CurrentState.obelisk.obeliskMovementSpeed;
	}

	public override void LevelInit(LevelProperties.FlyingGenie properties)
	{
		base.LevelInit(properties);
		treasureAttacks = new List<int> { 0, 1, 2 };
		swordPinkPattern = properties.CurrentState.swords.swordPinkString.Split(',');
		swordPinkIndex = Random.Range(0, swordPinkPattern.Length);
		gemPinkPattern = properties.CurrentState.gems.gemPinkString.Split(',');
		gemPinkIndex = Random.Range(0, gemPinkPattern.Length);
		sphinxPinkPattern = properties.CurrentState.sphinx.scarabPinkString.Split(',');
		sphinxPinkIndex = Random.Range(0, sphinxPinkPattern.Length);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private IEnumerator intro_cr()
	{
		state = State.Intro;
		yield return CupheadTime.WaitForSeconds(this, 1.3f);
		base.animator.SetTrigger("Continue");
		GenieIntroSFX();
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.main.introHesitate);
		yield return base.animator.WaitForAnimationToEnd(this, "Intro_End");
		state = State.Idle;
		StartTreasure();
		StartCoroutine(skull_attack_cr());
		yield return null;
	}

	private void SpawnPuff()
	{
		StartCoroutine(handle_puff_cr(puffEffect.Create(puffRoot.position)));
	}

	private void StartCarpet()
	{
		base.animator.Play("Idle_Carpet");
		StartCoroutine(handle_carpet_fadein());
	}

	private void EndCarpet()
	{
		StartCoroutine(handle_carpet_fadeout());
	}

	private IEnumerator handle_puff_cr(Effect puff)
	{
		yield return puff.animator.WaitForAnimationToEnd(this, "Start");
		SpriteRenderer puffRenderer = puff.GetComponent<SpriteRenderer>();
		while (puff.transform.position.x > -740f)
		{
			puff.transform.position -= Vector3.right * 100f * CupheadTime.Delta;
			if (puff.transform.position.x < -540f)
			{
				Color color = puffRenderer.color;
				color.a -= 1f * (float)CupheadTime.Delta;
				puffRenderer.color = color;
			}
			yield return null;
		}
		Object.Destroy(puff.gameObject);
		yield return null;
	}

	private IEnumerator handle_carpet_fadein()
	{
		carpet.color = new Color(1f, 1f, 1f, 0f);
		float t = 0f;
		float time = 2f;
		while (t < time)
		{
			carpet.color = new Color(1f, 1f, 1f, t / time);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		carpet.color = new Color(1f, 1f, 1f, 1f);
		yield return null;
	}

	private IEnumerator handle_carpet_fadeout()
	{
		carpet.color = new Color(1f, 1f, 1f, 1f);
		float t = 0f;
		float time = 2f;
		while (t < time)
		{
			carpet.color = new Color(1f, 1f, 1f, 1f - t / time);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		carpet.color = new Color(1f, 1f, 1f, 0f);
		yield return null;
	}

	public void HitTrigger()
	{
		attackLooping = false;
	}

	public void StartTreasure()
	{
		state = State.Treasure;
		skullCounter = 0;
		base.animator.SetBool("OnTreasure", true);
		attackLooping = true;
		int num = Random.Range(0, treasureAttacks.Count);
		treasureCounter = treasureAttacks[num];
		switch (treasureCounter)
		{
		case 0:
			StartSwords();
			break;
		case 1:
			StartGems();
			break;
		case 2:
			StartSphinx();
			break;
		default:
			Debug.LogError("The counter is messed up: " + treasureCounter);
			break;
		}
		treasureAttacks.Remove(num);
	}

	private IEnumerator skull_attack_cr()
	{
		while (true)
		{
			if (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Chest_Idle") && skullCounter < base.properties.CurrentState.skull.skullCount)
			{
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.skull.skullDelayRange);
				base.animator.SetTrigger("OnSkull");
				AudioManager.Play("genie_skull_release");
				emitAudioFromObject.Add("genie_skull_release");
				yield return base.animator.WaitForAnimationToEnd(this, "Chest_Skull_Attack");
				skullCounter++;
			}
			yield return null;
		}
	}

	private void SpawnSkull()
	{
		skullPrefab.Create(skullRoot.transform.position, 0f, 0f - base.properties.CurrentState.skull.skullSpeed);
		AudioManager.Play("genie_skull_release_projectile");
		emitAudioFromObject.Add("genie_skull_release_projectile");
	}

	private void DisableCarpet()
	{
		base.animator.Play("Off");
	}

	private void EnableCarpet()
	{
		base.animator.Play("Idle_Carpet");
	}

	private void EnableChestIdle()
	{
		base.animator.Play("Idle_Carpet_Chest");
	}

	public void StartSwords()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(swords_cr());
	}

	private IEnumerator swords_cr()
	{
		attackLooping = true;
		stillShooting = true;
		yield return base.animator.WaitForAnimationToEnd(this, "Chest_Intro");
		LevelProperties.FlyingGenie.Swords p = base.properties.CurrentState.swords;
		int positionIndex = Random.Range(0, p.patternPositionStrings.Length);
		string[] positionPattern = p.patternPositionStrings[positionIndex].Split(',');
		Vector3 endPosition = Vector3.zero;
		float location = 0f;
		while (attackLooping)
		{
			positionPattern = p.patternPositionStrings[positionIndex].Split(',');
			for (int i = 0; i < positionPattern.Length; i++)
			{
				string[] coordinates = positionPattern[i].Split('-');
				for (int j = 0; j < coordinates.Length; j++)
				{
					float.TryParse(coordinates[j], out location);
					if (j % 2 == 0)
					{
						endPosition.x = -640f + location;
					}
					else
					{
						endPosition.y = 360f - location;
					}
				}
				SpawnSwords(endPosition);
				if (!attackLooping)
				{
					break;
				}
				yield return CupheadTime.WaitForSeconds(this, p.spawnDelay);
			}
			if (!attackLooping)
			{
				break;
			}
			yield return CupheadTime.WaitForSeconds(this, p.repeatDelay);
			positionIndex = (positionIndex + 1) % p.patternPositionStrings.Length;
			yield return null;
		}
		stillShooting = false;
		base.animator.SetBool("OnTreasure", false);
		yield return base.animator.WaitForAnimationToEnd(this, "Chest_Outro");
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
		yield return null;
	}

	private void SpawnSwords(Vector3 pos)
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		FlyingGenieLevelSword flyingGenieLevelSword = Object.Instantiate(swordPrefab);
		flyingGenieLevelSword.Init(treasureRoot.position, pos, base.properties.CurrentState.swords, next);
		flyingGenieLevelSword.SetParryable(swordPinkPattern[swordPinkIndex][0] == 'P');
		swordPinkIndex = (swordPinkIndex + 1) % swordPinkPattern.Length;
	}

	public void StartGems()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(gems_cr());
	}

	private IEnumerator gems_cr()
	{
		attackLooping = true;
		yield return base.animator.WaitForAnimationToEnd(this, "Chest_Intro");
		AudioManager.Play("genie_chest_jewel_escape");
		emitAudioFromObject.Add("genie_chest_jewel_escape");
		AudioManager.PlayLoop("genie_chest_magic_loop");
		emitAudioFromObject.Add("genie_chest_magic_loop");
		while (attackLooping)
		{
			smallGemTimerUp = false;
			bigGemTimerUp = false;
			StartCoroutine(big_gems_cr());
			StartCoroutine(small_gems_cr());
			while (!smallGemTimerUp && !bigGemTimerUp && attackLooping)
			{
				yield return null;
			}
			if (attackLooping)
			{
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.gems.repeatDelay);
			}
			yield return null;
		}
		base.animator.SetBool("OnTreasure", false);
		yield return base.animator.WaitForAnimationToStart(this, "Chest_Outro");
		AudioManager.Stop("genie_chest_magic_loop");
		AudioManager.Play("genie_chest_magic_loop_end");
		emitAudioFromObject.Add("genie_chest_magic_loop_end");
		yield return base.animator.WaitForAnimationToEnd(this, "Chest_Outro");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.gems.hesitate);
		state = State.Idle;
		yield return null;
	}

	private IEnumerator small_gems_cr()
	{
		LevelProperties.FlyingGenie.Gems p = base.properties.CurrentState.gems;
		smallGemTimerUp = false;
		int mainOffsetIndex = Random.Range(0, p.gemSmallAimOffset.Length);
		string[] smallOffsetString = p.gemSmallAimOffset[mainOffsetIndex].Split(',');
		int offsetIndex = Random.Range(0, smallOffsetString.Length);
		float offset = 0f;
		StartCoroutine(small_gem_timer_cr());
		while (!smallGemTimerUp && attackLooping)
		{
			smallOffsetString = p.gemSmallAimOffset[mainOffsetIndex].Split(',');
			float.TryParse(smallOffsetString[offsetIndex], out offset);
			AbstractPlayerController player = PlayerManager.GetNext();
			gemPrefab.Create(treasureRoot.position, player, offset, p.gemSmallSpeed, gemPinkPattern[gemPinkIndex][0] == 'P', false);
			gemPinkIndex = (gemPinkIndex + 1) % gemPinkPattern.Length;
			yield return CupheadTime.WaitForSeconds(this, p.gemSmallDelayRange.RandomFloat());
			if (offsetIndex < smallOffsetString.Length - 1)
			{
				offsetIndex++;
				continue;
			}
			mainOffsetIndex = (mainOffsetIndex + 1) % p.gemSmallAimOffset.Length;
			offsetIndex = 0;
		}
		yield return null;
	}

	private IEnumerator small_gem_timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.gems.gemSmallAttackDuration);
		smallGemTimerUp = true;
	}

	private IEnumerator big_gems_cr()
	{
		LevelProperties.FlyingGenie.Gems p = base.properties.CurrentState.gems;
		bigGemTimerUp = false;
		int mainOffsetIndex = Random.Range(0, p.gemBigAimOffset.Length);
		string[] bigOffsetString = p.gemBigAimOffset[mainOffsetIndex].Split(',');
		int offsetIndex = Random.Range(0, bigOffsetString.Length);
		float offset = 0f;
		StartCoroutine(big_gems_timer_cr());
		while (!bigGemTimerUp && attackLooping)
		{
			float.TryParse(bigOffsetString[offsetIndex], out offset);
			AbstractPlayerController player = PlayerManager.GetNext();
			gemPrefab.Create(treasureRoot.position, player, offset, p.gemBigSpeed, false, true);
			yield return CupheadTime.WaitForSeconds(this, p.gemBigDelayRange.RandomFloat());
			if (offsetIndex < bigOffsetString.Length - 1)
			{
				offsetIndex++;
			}
			else
			{
				mainOffsetIndex = (mainOffsetIndex + 1) % p.gemBigAimOffset.Length;
				offsetIndex = 0;
			}
			yield return null;
		}
		yield return null;
	}

	private IEnumerator big_gems_timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.gems.gemBigAttackDuration);
		bigGemTimerUp = true;
	}

	public void StartSphinx()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(sphinx_cr());
	}

	private IEnumerator sphinx_cr()
	{
		attackLooping = true;
		yield return base.animator.WaitForAnimationToEnd(this, "Chest_Intro");
		AudioManager.Play("genie_chest_jewel_escape");
		emitAudioFromObject.Add("genie_chest_jewel_escape");
		AudioManager.PlayLoop("genie_chest_magic_loop_nojingle");
		emitAudioFromObject.Add("genie_chest_magic_loop_nojingle");
		LevelProperties.FlyingGenie.Sphinx p = base.properties.CurrentState.sphinx;
		Vector3 endPos = Vector3.zero;
		int mainCountIndex = Random.Range(0, p.sphinxCount.Length);
		string[] sphinxCountPattern = p.sphinxCount[mainCountIndex].Split(',');
		int countIndex = Random.Range(0, sphinxCountPattern.Length);
		int mainXIndex = Random.Range(0, p.sphinxAimX.Length);
		string[] sphinxPosXPattern = p.sphinxAimX[mainXIndex].Split(',');
		int posXIndex = Random.Range(0, sphinxPosXPattern.Length);
		int mainYIndex = Random.Range(0, p.sphinxAimY.Length);
		string[] sphinxPosYPattern = p.sphinxAimY[mainYIndex].Split(',');
		int posYIndex = Random.Range(0, sphinxPosYPattern.Length);
		float sphinxCount = 0f;
		float posX = 0f;
		float posY = 0f;
		while (attackLooping)
		{
			sphinxCountPattern = p.sphinxCount[mainCountIndex].Split(',');
			sphinxPosXPattern = p.sphinxAimX[mainXIndex].Split(',');
			sphinxPosYPattern = p.sphinxAimY[mainYIndex].Split(',');
			float.TryParse(sphinxCountPattern[countIndex], out sphinxCount);
			for (int i = 0; (float)i < sphinxCount; i++)
			{
				float.TryParse(sphinxPosXPattern[posXIndex], out posX);
				float.TryParse(sphinxPosXPattern[posYIndex], out posY);
				endPos.x = -640f + posX;
				endPos.y = 360f - posY;
				SpawnSphinx(endPos);
				yield return CupheadTime.WaitForSeconds(this, p.sphinxMainDelay);
				if (posXIndex < p.sphinxAimX.Length - 1)
				{
					posXIndex++;
				}
				else
				{
					mainXIndex = (mainXIndex + 1) % p.sphinxAimX.Length;
					posXIndex = 0;
				}
				if (posYIndex < p.sphinxAimY.Length - 1)
				{
					posYIndex++;
				}
				else
				{
					mainYIndex = (mainYIndex + 1) % p.sphinxAimY.Length;
					posYIndex = 0;
				}
				if (!attackLooping)
				{
					break;
				}
			}
			if (attackLooping)
			{
				yield return CupheadTime.WaitForSeconds(this, p.repeatDelay);
			}
			if (countIndex < p.sphinxCount.Length - 1)
			{
				countIndex++;
				continue;
			}
			mainCountIndex = (mainCountIndex + 1) % p.sphinxCount.Length;
			countIndex = 0;
		}
		base.animator.SetBool("OnTreasure", false);
		yield return base.animator.WaitForAnimationToStart(this, "Chest_Outro");
		AudioManager.Stop("genie_chest_magic_loop_nojingle");
		AudioManager.Play("genie_chest_magic_loop_nojingle_end");
		emitAudioFromObject.Add("genie_chest_magic_loop_nojingle_end");
		yield return base.animator.WaitForAnimationToEnd(this, "Chest_Outro");
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
		yield return null;
	}

	private void SpawnSphinx(Vector3 targetPos)
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		FlyingGenieLevelSphinx flyingGenieLevelSphinx = Object.Instantiate(sphinxPrefab);
		flyingGenieLevelSphinx.Init(treasureRoot.position, targetPos, base.properties.CurrentState.sphinx, next, sphinxPinkPattern, sphinxPinkIndex);
		sphinxPinkIndex = (sphinxPinkIndex + (int)base.properties.CurrentState.sphinx.sphinxSpawnNum) % sphinxPinkPattern.Length;
	}

	public void StartCoffin()
	{
		state = State.Coffin;
		base.animator.SetBool("OnDisappear", true);
		StartCoroutine(coffin_cr());
	}

	private IEnumerator coffin_cr()
	{
		attackLooping = true;
		LevelProperties.FlyingGenie.Coffin p = base.properties.CurrentState.coffin;
		int mainPosIndex = Random.Range(0, p.mummyAppearString.Length);
		string[] coffinPosPattern = p.mummyAppearString[mainPosIndex].Split(',');
		int posIndex = Random.Range(0, coffinPosPattern.Length);
		int mainAngleIndex = Random.Range(0, p.mummyGenieDirection.Length);
		string[] coffinAnglePattern = p.mummyGenieDirection[mainAngleIndex].Split(',');
		int angleIndex = Random.Range(0, coffinAnglePattern.Length);
		int mainTypeIndex = Random.Range(0, p.mummyTypeString.Length);
		string[] coffinTypePattern = p.mummyTypeString[mainTypeIndex].Split(',');
		int typeIndex = Random.Range(0, coffinTypePattern.Length);
		Vector3 pos = Vector3.zero;
		float position = 0f;
		float angle = 0f;
		int sortingOrder = 0;
		yield return CupheadTime.WaitForSeconds(this, 1f);
		AudioManager.Play("genie_sarcophagus_enter");
		emitAudioFromObject.Add("genie_sarcophagus_enter");
		while (casket.transform.position.y > -30f)
		{
			casket.transform.AddPosition(0f, -800f * (float)CupheadTime.Delta);
			yield return null;
		}
		CupheadLevelCamera.Current.Shake(10f, 0.4f);
		casket.GetComponent<Animator>().SetTrigger("StartCasket");
		yield return casket.GetComponent<Animator>().WaitForAnimationToEnd(this, "Open_Start");
		goop.ActivateGoop();
		while (attackLooping)
		{
			coffinPosPattern = p.mummyAppearString[mainPosIndex].Split(',');
			coffinTypePattern = p.mummyTypeString[mainTypeIndex].Split(',');
			coffinAnglePattern = p.mummyGenieDirection[mainAngleIndex].Split(',');
			float.TryParse(coffinPosPattern[posIndex], out position);
			float.TryParse(coffinAnglePattern[angleIndex], out angle);
			pos = new Vector3(casket.transform.position.x + 200f, position, 0f);
			if (coffinTypePattern[typeIndex][0] == 'A')
			{
				mummyClassic.Create(pos, 0f - p.mummyASpeed, 0f - angle, p, FlyingGenieLevelMummy.MummyType.Classic, p.mummyGenieHP, sortingOrder);
			}
			else if (coffinTypePattern[typeIndex][0] == 'B')
			{
				mummyChomper.Create(pos, 0f - p.mummyBSpeed, 0f - angle, p, FlyingGenieLevelMummy.MummyType.Chomper, p.mummyGenieHP, sortingOrder);
			}
			else if (coffinTypePattern[typeIndex][0] == 'C')
			{
				mummyChaser.Create(pos, 0f - p.mummyCSpeed, 0f - angle, p, FlyingGenieLevelMummy.MummyType.Grabby, p.mummyGenieHP, sortingOrder);
			}
			yield return CupheadTime.WaitForSeconds(this, p.mummyGenieDelay);
			if (posIndex < coffinPosPattern.Length - 1)
			{
				posIndex++;
			}
			else
			{
				mainPosIndex = (mainPosIndex + 1) % p.mummyAppearString.Length;
				posIndex = 0;
			}
			if (typeIndex < coffinTypePattern.Length - 1)
			{
				typeIndex++;
			}
			else
			{
				mainTypeIndex = (mainTypeIndex + 1) % p.mummyTypeString.Length;
				typeIndex = 0;
			}
			if (angleIndex < coffinAnglePattern.Length - 1)
			{
				angleIndex++;
			}
			else
			{
				mainAngleIndex = (mainAngleIndex + 1) % p.mummyGenieDirection.Length;
				angleIndex = 0;
			}
			sortingOrder += 2;
		}
		goop.StartDeath();
		casket.GetComponent<Animator>().SetTrigger("OnClose");
		AudioManager.Play("genie_sarcophagus_exit");
		emitAudioFromObject.Add("genie_sarcophagus_exit");
		yield return casket.GetComponent<Animator>().WaitForAnimationToEnd(this, "Close");
		while (casket.transform.position.x < 1140f)
		{
			casket.transform.AddPosition(200f * (float)CupheadTime.Delta);
			yield return null;
		}
		casket.transform.position = casketStartPos;
		casket.GetComponent<Animator>().SetTrigger("EndCasket");
		FadeIntoIdle();
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
		yield return null;
	}

	public void StartObelisk()
	{
		state = State.Disappear;
		base.animator.SetBool("OnDisappear", true);
		StartCoroutine(obelisk_cr());
		StartCoroutine(genie_laugh_sound_cr());
	}

	private IEnumerator obelisk_cr()
	{
		LevelProperties.FlyingGenie.Obelisk p = base.properties.CurrentState.obelisk;
		attackLooping = true;
		Vector3 startPos = Vector3.zero;
		startPos.x = 1340f;
		startPos.y = 360f;
		float t = 0f;
		float time = 1f;
		float angle = 0f;
		bool firstPillar = true;
		obelisks = new List<FlyingGenieLevelObelisk>();
		int obelisksListIndex = 0;
		int obeliskPoolSize = 6;
		int obeliskCounter = 0;
		int mainObeliskIndex = Random.Range(0, p.obeliskGeniePos.Length);
		string[] blockOrderPattern = p.obeliskGeniePos[mainObeliskIndex].Split(',');
		int obeliskIndex = Random.Range(0, blockOrderPattern.Length);
		int mainBouncerIndex = Random.Range(0, p.bouncerAngleString.Length);
		string[] bouncerPattern = p.bouncerAngleString[mainBouncerIndex].Split(',');
		int bouncerIndex = Random.Range(0, bouncerPattern.Length);
		for (int i = 0; i < obeliskPoolSize; i++)
		{
			FlyingGenieLevelObelisk flyingGenieLevelObelisk = Object.Instantiate(obeliskPrefab);
			flyingGenieLevelObelisk.Init(startPos, p, this, i == 0);
			obelisks.Add(flyingGenieLevelObelisk);
		}
		yield return base.animator.WaitForAnimationToStart(this, "Genie_Meditate");
		sawMask.gameObject.SetActive(true);
		while (t < time)
		{
			Vector3 pos1 = hieroBG.position;
			Vector3 pos2 = brickBG.position;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInBounce, 0f, 1f, t / time);
			pos1.y = Mathf.Lerp(hieroBG.position.y, 340f, val);
			pos2.y = Mathf.Lerp(brickBG.position.y, -320f, val);
			hieroBG.position = pos1;
			brickBG.position = pos2;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		while (obeliskCounter < p.obeliskCount)
		{
			float.TryParse(bouncerPattern[bouncerIndex], out angle);
			string[] headLocations = blockOrderPattern[obeliskIndex].Split('-');
			obelisks[obelisksListIndex].ActivateObelisk(headLocations);
			if (p.bounceShotOn)
			{
				if (!firstPillar)
				{
					int index = ((obelisksListIndex > 0) ? (obelisksListIndex - 1) : (obelisks.Count - 1));
					SpawnBouncer(obelisks[obelisksListIndex], obelisks[index], angle);
				}
				else
				{
					float num = Vector3.Distance(obelisks[obelisksListIndex + 1].transform.position, base.transform.position);
					obelisks[obelisksListIndex].SetColliders((obelisks[obelisksListIndex + 1].transform.position.x + Mathf.Abs(num / 2f)) / 2f, base.transform.position.x - num / 2f);
					firstPillar = false;
				}
			}
			obelisksListIndex = (obelisksListIndex + 1) % obelisks.Count;
			yield return null;
			yield return CupheadTime.WaitForSeconds(this, p.obeliskAppearDelay);
			if (obeliskIndex < blockOrderPattern.Length - 1)
			{
				obeliskIndex++;
			}
			else
			{
				mainObeliskIndex = (mainObeliskIndex + 1) % p.obeliskGeniePos.Length;
				obeliskIndex = 0;
			}
			if (bouncerIndex < bouncerPattern.Length - 1)
			{
				bouncerIndex++;
			}
			else
			{
				mainBouncerIndex = (mainBouncerIndex + 1) % p.bouncerAngleString.Length;
				bouncerIndex = 0;
			}
			blockOrderPattern = p.obeliskGeniePos[mainObeliskIndex].Split(',');
			bouncerPattern = p.bouncerAngleString[mainBouncerIndex].Split(',');
			obeliskCounter++;
			yield return null;
		}
		foreach (FlyingGenieLevelObelisk obelisk in obelisks)
		{
			if (obelisk.isOn)
			{
				while (obelisk.transform.position.x > -640f)
				{
					yield return null;
				}
			}
		}
		AudioManager.Stop("genie_pillar_main_loop");
		AudioManager.Stop("genie_pillar_destructable_loop");
		sawMask.gameObject.SetActive(false);
		StartCoroutine(delete_obelisks_cr(obelisks));
		state = State.Idle;
		StartCoffin();
		yield return null;
	}

	private IEnumerator delete_obelisks_cr(List<FlyingGenieLevelObelisk> obelisks)
	{
		float t = 0f;
		float time = 2f;
		foreach (FlyingGenieLevelObelisk obelisk in obelisks)
		{
			if (obelisk.isOn)
			{
				while (obelisk.transform.position.x > -740f)
				{
					yield return null;
				}
			}
			Object.Destroy(obelisk.gameObject);
			yield return null;
		}
		while (t < time)
		{
			Vector3 pos1 = hieroBG.position;
			Vector3 pos2 = brickBG.position;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeOutBounce, 0f, 1f, t / time);
			pos1.y = Mathf.Lerp(hieroBG.position.y, 460f, val);
			pos2.y = Mathf.Lerp(brickBG.position.y, -460f, val);
			hieroBG.position = pos1;
			brickBG.position = pos2;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private void SpawnBouncer(FlyingGenieLevelObelisk currentObelisk, FlyingGenieLevelObelisk lastObelisk, float angle)
	{
		float num = Vector3.Distance(lastObelisk.transform.position, currentObelisk.transform.position);
		Vector3 position = lastObelisk.transform.position;
		position.x = currentObelisk.transform.position.x - num / 2f;
		float num2 = 150f;
		float num3 = 180f - (90f + angle / 2f);
		currentObelisk.SetColliders((lastObelisk.transform.position.x + Mathf.Abs(num / 2f)) / 2f, currentObelisk.transform.position.x - num / 2f);
		position.y = Random.Range((float)Level.Current.Ceiling - num2, (float)Level.Current.Ground + num2);
		FlyingGenieLevelBouncer flyingGenieLevelBouncer = Object.Instantiate(bouncerPrefab).Init(position, base.properties.CurrentState.obelisk, 0f - num3);
		flyingGenieLevelBouncer.transform.parent = currentObelisk.transform;
	}

	public void DoDamage(float damage)
	{
		base.properties.DealDamage(damage);
	}

	private void FadeIntoIdle()
	{
		StartCoroutine(handle_fade_in_idle());
	}

	private IEnumerator handle_fade_in_idle()
	{
		GetComponent<SpriteRenderer>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.a, 0f);
		carpet.color = new Color(1f, 1f, 1f, 0f);
		float t = 0f;
		float time = 0.3f;
		base.animator.Play("To_Phase_2");
		while (t < time)
		{
			carpet.color = new Color(1f, 1f, 1f, t / time);
			GetComponent<SpriteRenderer>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.a, t / time);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		GetComponent<SpriteRenderer>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.a, 1f);
		carpet.color = new Color(1f, 1f, 1f, 1f);
		GetComponent<Collider2D>().enabled = true;
		yield return null;
	}

	public void StartPhase2()
	{
		StartCoroutine(start_phase_2_cr());
	}

	private IEnumerator start_phase_2_cr()
	{
		genieTransformed.animator.Play("Genie_Head_Roll");
		yield return new WaitForEndOfFrame();
		genieTransformed.StartMarionette(base.transform.position, meditateP1, meditateP2);
		Object.Destroy(base.gameObject);
		yield return null;
	}

	private void CreateMeditateFX()
	{
		PlanePlayerController player = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
		PlanePlayerController player2 = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
		if (player != null)
		{
			meditateP1 = Object.Instantiate(meditateEffect);
			meditateP1.transform.position = player.transform.position;
			meditateP1.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
			meditateP1.transform.parent = player.transform;
		}
		if (player2 != null)
		{
			meditateP2 = Object.Instantiate(meditateEffect);
			meditateP2.transform.position = player2.transform.position;
			meditateP2.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
			meditateP2.transform.parent = player2.transform;
		}
	}

	private void GenieIntroSFX()
	{
		AudioManager.Play("genie_entrance");
		emitAudioFromObject.Add("genie_entrance");
	}

	private void SoundGenieVoiceIntro()
	{
		AudioManager.Play("genie_voice_intro_intake");
		emitAudioFromObject.Add("genie_voice_intro_intake");
	}

	private void SoundGenieVoiceEffort()
	{
		AudioManager.Play("genie_voice_effort");
		emitAudioFromObject.Add("genie_voice_effort");
	}

	private void SoundGenieVoiceLaugh()
	{
		AudioManager.Play("genie_voice_laugh");
		emitAudioFromObject.Add("genie_voice_laugh");
	}

	private void SoundGenieVoiceLure()
	{
		AudioManager.Play("genie_voice_lure");
		emitAudioFromObject.Add("genie_voice_lure");
	}

	private void SoundGenieVoiceMeditate()
	{
		AudioManager.Play("genie_voice_meditate");
		emitAudioFromObject.Add("genie_voice_meditate");
	}

	private void SoundGenieChestOpen()
	{
		AudioManager.Play("genie_chest_attack_open");
		emitAudioFromObject.Add("genie_chest_attack_open");
	}

	private IEnumerator genie_laugh_sound_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 6f);
		AudioManager.Play("genie_voice_laugh_reverb");
	}
}
