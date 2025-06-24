using System.Collections;
using UnityEngine;

public class DicePalaceFlyingHorseLevelHorse : LevelProperties.DicePalaceFlyingHorse.Entity
{
	public enum MiniHorseType
	{
		One = 0,
		Two = 1,
		Three = 2
	}

	[SerializeField]
	private Transform bottomLine;

	[SerializeField]
	private Transform middleLine;

	[SerializeField]
	private Transform topLine;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private DicePalaceFlyingHorseLevelMiniHorse miniHorse1Prefab;

	[SerializeField]
	private DicePalaceFlyingHorseLevelMiniHorse miniHorse2Prefab;

	[SerializeField]
	private DicePalaceFlyingHorseLevelMiniHorse miniHorse3Prefab;

	[SerializeField]
	private DicePalaceFlyingHorseLevelPresent presentPrefab;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private int giftPosYMainIndex;

	private int giftPosYIndex;

	private int giftPosXMainIndex;

	private int giftPosXIndex;

	private int playerAimMaxCounter;

	private int playerAimCounter;

	private bool DieSFXPlaying;

	public MiniHorseType miniHorseType { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	public override void LevelInit(LevelProperties.DicePalaceFlyingHorse properties)
	{
		base.LevelInit(properties);
		Level.Current.OnLevelStartEvent += StartAttacks;
		Level.Current.OnWinEvent += Death;
		giftPosXMainIndex = Random.Range(0, properties.CurrentState.giftBombs.giftPositionStringX.Length);
		giftPosYMainIndex = Random.Range(0, properties.CurrentState.giftBombs.giftPositionStringY.Length);
		giftPosXIndex = Random.Range(0, properties.CurrentState.giftBombs.giftPositionStringX[giftPosXMainIndex].Split(',').Length);
		giftPosYIndex = Random.Range(0, properties.CurrentState.giftBombs.giftPositionStringY[giftPosYMainIndex].Split(',').Length);
		playerAimMaxCounter = properties.CurrentState.giftBombs.playerAimRange.RandomInt();
		StartCoroutine(intro_cr());
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
		yield return CupheadTime.WaitForSeconds(this, 2f);
		base.animator.SetTrigger("Continue");
		AudioManager.Play("dice_palace_flying_horse_intro");
		emitAudioFromObject.Add("dice_palace_flying_horse_intro");
		yield return null;
	}

	private void StartAttacks()
	{
		StartCoroutine(presents_cr());
		StartCoroutine(mini_horses_cr());
	}

	private void SpawnPresent()
	{
		StartCoroutine(spawn_present_cr());
	}

	private IEnumerator spawn_present_cr()
	{
		LevelProperties.DicePalaceFlyingHorse.GiftBombs p = base.properties.CurrentState.giftBombs;
		float positionX = 0f;
		float positionY = 0f;
		Vector3 endPos = Vector3.zero;
		AbstractPlayerController player = PlayerManager.GetNext();
		string[] giftPositionXPattern = p.giftPositionStringX[giftPosXMainIndex].Split(',');
		string[] giftPositionYPattern = p.giftPositionStringY[giftPosYMainIndex].Split(',');
		if (playerAimCounter >= playerAimMaxCounter)
		{
			endPos = player.transform.position;
			player = PlayerManager.GetNext();
			playerAimMaxCounter = p.playerAimRange.RandomInt();
			playerAimCounter = 0;
		}
		else
		{
			float.TryParse(giftPositionXPattern[giftPosXIndex], out positionX);
			float.TryParse(giftPositionYPattern[giftPosYIndex], out positionY);
			endPos.x = -640f + positionX;
			endPos.y = 360f - positionY;
			playerAimCounter++;
		}
		DicePalaceFlyingHorseLevelPresent present = Object.Instantiate(presentPrefab);
		present.Init(projectileRoot.position, endPos, base.properties.CurrentState.giftBombs);
		if (giftPosXIndex < giftPositionXPattern[giftPosXIndex].Length)
		{
			giftPosXIndex++;
		}
		else
		{
			giftPosXMainIndex = (giftPosXMainIndex + 1) % p.giftPositionStringX.Length;
			giftPosXIndex = 0;
		}
		if (giftPosYIndex < giftPositionYPattern[giftPosYIndex].Length)
		{
			giftPosYIndex++;
		}
		else
		{
			giftPosYMainIndex = (giftPosYMainIndex + 1) % p.giftPositionStringY.Length;
			giftPosYIndex = 0;
		}
		yield return null;
	}

	private IEnumerator presents_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.giftBombs.giftDelay);
			base.animator.SetTrigger("OnAttack");
			yield return base.animator.WaitForAnimationToStart(this, "Attack");
			AudioManager.Play("dice_palace_flying_horse_attack");
			emitAudioFromObject.Add("dice_palace_flying_horse_attack");
			yield return base.animator.WaitForAnimationToEnd(this, "Attack");
			yield return null;
		}
	}

	private void AttackVox()
	{
		AudioManager.Play("dice_palace_horse_vox");
		emitAudioFromObject.Add("dice_palace_horse_vox");
	}

	private void TrotSFX()
	{
		AudioManager.Play("dice_horse_trot");
		emitAudioFromObject.Add("dice_horse_trot");
	}

	private void DieSFX()
	{
		AudioManager.Play("dice_horse_death");
		emitAudioFromObject.Add("dice_horse_death");
	}

	private void SpawnMiniHorses(Vector3 startPos, DicePalaceFlyingHorseLevelMiniHorse prefab, MiniHorseType type, bool isPink, float threeProx, int lane)
	{
		LevelProperties.DicePalaceFlyingHorse.MiniHorses miniHorses = base.properties.CurrentState.miniHorses;
		AbstractPlayerController next = PlayerManager.GetNext();
		DicePalaceFlyingHorseLevelMiniHorse dicePalaceFlyingHorseLevelMiniHorse = Object.Instantiate(prefab);
		dicePalaceFlyingHorseLevelMiniHorse.Init(startPos, miniHorses.HP, base.properties.CurrentState.miniHorses, next, type, isPink, threeProx, lane);
	}

	private IEnumerator mini_horses_cr()
	{
		LevelProperties.DicePalaceFlyingHorse.MiniHorses p = base.properties.CurrentState.miniHorses;
		string[] typePattern = p.miniTypeString.GetRandom().Split(',');
		string[] delayPattern = p.delayString.GetRandom().Split(',');
		string[] pinkPattern = p.miniTwoPinkString.GetRandom().Split(',');
		string[] proxPattern = p.miniThreeProxString.GetRandom().Split(',');
		int typeIndex = Random.Range(0, typePattern.Length);
		int delayIndex = Random.Range(0, delayPattern.Length);
		int pinkIndex = Random.Range(0, pinkPattern.Length);
		int proxIndex = Random.Range(0, proxPattern.Length);
		int type = 0;
		int trackCounter = 0;
		int pinkCounter = 0;
		int maxPink = 0;
		int threeProximity = 0;
		float delay = 0f;
		bool isPink = false;
		int lane = 0;
		Vector3 position = base.transform.position;
		DicePalaceFlyingHorseLevelMiniHorse prefab = null;
		MiniHorseType getType = MiniHorseType.One;
		position.x = (float)Level.Current.Right + 100f;
		while (true)
		{
			int i;
			for (i = typeIndex; i < typePattern.Length; i++)
			{
				int.TryParse(typePattern[i], out type);
				float.TryParse(delayPattern[delayIndex], out delay);
				trackCounter++;
				if (trackCounter <= 1)
				{
					position.y = topLine.position.y;
					lane = 0;
				}
				else if (trackCounter == 2)
				{
					position.y = middleLine.position.y;
					lane = 1;
				}
				else if (trackCounter >= 3)
				{
					position.y = bottomLine.position.y;
					trackCounter = 0;
					lane = 2;
				}
				switch (type)
				{
				case 1:
					prefab = miniHorse1Prefab;
					getType = MiniHorseType.One;
					break;
				case 2:
					prefab = miniHorse2Prefab;
					getType = MiniHorseType.Two;
					if (pinkCounter == 0)
					{
						isPink = false;
						int.TryParse(pinkPattern[pinkIndex], out maxPink);
						pinkIndex %= pinkPattern.Length;
						pinkCounter++;
					}
					else if (pinkCounter >= maxPink)
					{
						isPink = true;
						pinkCounter = 0;
					}
					else
					{
						isPink = false;
						pinkCounter++;
					}
					break;
				case 3:
					prefab = miniHorse3Prefab;
					getType = MiniHorseType.Three;
					int.TryParse(proxPattern[proxIndex], out threeProximity);
					proxIndex = (proxIndex + 1) % proxPattern.Length;
					break;
				}
				SpawnMiniHorses(position, prefab, getType, isPink, threeProximity, lane);
				yield return CupheadTime.WaitForSeconds(this, delay);
				delayIndex = (delayIndex + 1) % delayPattern.Length;
				i %= typePattern.Length;
				typeIndex = 0;
			}
			yield return null;
		}
	}

	private void Death()
	{
		StopAllCoroutines();
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("OnDeath");
		AudioManager.PlayLoop("dice_palace_flying_horse_death_loop");
		emitAudioFromObject.Add("dice_palace_flying_horse_death_loop");
		DieSFX();
	}
}
