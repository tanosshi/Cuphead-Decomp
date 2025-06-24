using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownLevelClownHorse : LevelProperties.Clown.Entity
{
	public enum HorseType
	{
		Wave = 0,
		Drop = 1,
		Simple = 2
	}

	[SerializeField]
	private GameObject clownHorseBody;

	[SerializeField]
	private GameObject clownHorseHead;

	[SerializeField]
	private ClownLevelClownSwing clownSwing;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private ClownLevelHorseshoe regularHorseshoe;

	[SerializeField]
	private ClownLevelHorseshoe pinkHorseshoe;

	[SerializeField]
	private Effect spitFxPrefabA;

	[SerializeField]
	private Effect spitFxPrefabB;

	[SerializeField]
	private Transform spitFxRoot;

	private Vector3 pos;

	private Vector3 startPos;

	private Transform moveObject;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private string[] horseTypePattern;

	private string[] wavePositionPattern;

	private string[] wavePinkPattern;

	private string[] dropPositionPattern;

	private string[] dropBulletPositionPattern;

	private int positionIndex;

	private int typeIndex;

	private int pinkIndex;

	private int dropBulletIndex;

	private int horseTypeIndex;

	private int dropMainIndex;

	private int pinkMainIndex;

	private bool droppedClown;

	private bool ScreamSFXPlaying;

	public HorseType horseType { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = clownHorseHead.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		clownHorseHead.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (Level.Current.mode == Level.Mode.Easy)
		{
			Level.Current.OnLevelEndEvent += Dead;
		}
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

	public void StartCarouselHorse()
	{
		base.animator.SetTrigger("Start");
		LevelProperties.Clown.Horse horse = base.properties.CurrentState.horse;
		dropMainIndex = Random.Range(0, horse.DropBulletPositionString.Length);
		pinkMainIndex = Random.Range(0, horse.WavePinkString.Length);
		horseTypePattern = horse.HorseString.GetRandom().Split(',');
		horseTypeIndex = Random.Range(0, horseTypePattern.Length);
		wavePositionPattern = horse.WavePosString.GetRandom().Split(',');
		wavePinkPattern = horse.WavePinkString[pinkMainIndex].Split(',');
		dropPositionPattern = horse.DropHorsePositionString.GetRandom().Split(',');
		dropBulletPositionPattern = horse.DropBulletPositionString[dropMainIndex].Split(',');
		dropBulletIndex = Random.Range(0, dropBulletPositionPattern.Length);
		StopAllCoroutines();
		StartCoroutine(select_horse_cr());
	}

	private void BounceSFX()
	{
		AudioManager.Play("clown_horse_clown");
		emitAudioFromObject.Add("clown_horse_clown");
	}

	private IEnumerator select_horse_cr()
	{
		typeIndex = Random.Range(0, horseTypePattern.Length - 1);
		LevelProperties.Clown.Horse p = base.properties.CurrentState.horse;
		if (horseTypePattern[horseTypeIndex][0] == 'W')
		{
			base.animator.SetBool("IsWave", true);
			StartCoroutine(horse_cr(HorseType.Wave, wavePositionPattern, p.WaveATKRepeat));
		}
		else if (horseTypePattern[horseTypeIndex][0] == 'D')
		{
			base.animator.SetBool("IsWave", false);
			StartCoroutine(horse_cr(HorseType.Drop, dropPositionPattern, p.DropATKRepeat));
		}
		else
		{
			Debug.LogError("Horse Type Pattern is messed up!");
		}
		if (horseTypeIndex < horseTypePattern.Length - 1)
		{
			horseTypeIndex++;
		}
		else
		{
			horseTypeIndex = 0;
		}
		yield return null;
	}

	private IEnumerator horse_cr(HorseType horseType, string[] positionPattern, float ATKAmount)
	{
		bool isPink = false;
		float hesitate = 0f;
		float posOffset = 0f;
		float ATKcounter = 0f;
		float YSpeed = 0f;
		LevelProperties.Clown.Horse p = base.properties.CurrentState.horse;
		SelectStartPos();
		for (; ATKcounter < ATKAmount; ATKcounter += 1f)
		{
			float.TryParse(positionPattern[positionIndex], out posOffset);
			float getPos = 360f - posOffset;
			while (base.transform.position.y != getPos)
			{
				this.pos = base.transform.position;
				this.pos.y = Mathf.MoveTowards(base.transform.position.y, getPos, p.HorseSpeed * (float)CupheadTime.Delta);
				base.transform.position = this.pos;
				yield return null;
			}
			StartCoroutine(spit_fx_cr());
			switch (horseType)
			{
			case HorseType.Wave:
			{
				hesitate = p.WaveHesitate;
				Vector3 pos = projectileRoot.transform.position;
				YSpeed = ((!Rand.Bool()) ? p.WaveBulletWaveSpeed : (0f - p.WaveBulletWaveSpeed));
				SpitSFX();
				base.animator.SetBool("Spit", true);
				for (int k = 0; k < p.WaveBulletCount; k++)
				{
					wavePinkPattern = p.WavePinkString[pinkMainIndex].Split(',');
					if (wavePinkPattern[pinkIndex][0] == 'R')
					{
						isPink = false;
					}
					else if (wavePinkPattern[pinkIndex][0] == 'P')
					{
						isPink = true;
					}
					FireWaveBullets(k, isPink, YSpeed, pos);
					if (pinkIndex < wavePinkPattern.Length - 1)
					{
						pinkIndex++;
					}
					else
					{
						pinkMainIndex = (pinkMainIndex + 1) % p.WavePinkString.Length;
						pinkIndex = 0;
					}
					yield return CupheadTime.WaitForSeconds(this, p.WaveBulletDelay);
				}
				base.animator.SetBool("Spit", false);
				yield return CupheadTime.WaitForSeconds(this, p.WaveATKDelay);
				break;
			}
			case HorseType.Drop:
			{
				hesitate = p.DropHesitate;
				float spawnY = 0f;
				float nextSpawnY = 0f;
				int n = 0;
				dropBulletPositionPattern = p.DropBulletPositionString[dropMainIndex].Split(',');
				string[] droppattern = dropBulletPositionPattern[dropBulletIndex].Split('-');
				SpitSFX();
				base.animator.SetBool("Spit", true);
				float[] durationBeforeDrops = new float[droppattern.Length];
				List<int> indexPatterns = new List<int>(droppattern.Length);
				for (int i = 0; i < droppattern.Length; i++)
				{
					indexPatterns.Add(i);
				}
				float currentDuration = base.properties.CurrentState.horse.DropBulletDelay;
				bool dropTwo = true;
				while (indexPatterns.Count > 0)
				{
					if (indexPatterns.Count > 1 && dropTwo)
					{
						currentDuration += base.properties.CurrentState.horse.DropBulletTwoDelay.RandomFloat();
						int index = Random.Range(0, indexPatterns.Count);
						durationBeforeDrops[indexPatterns[index]] = currentDuration;
						indexPatterns.RemoveAt(index);
						index = Random.Range(0, indexPatterns.Count);
						durationBeforeDrops[indexPatterns[index]] = currentDuration;
						indexPatterns.RemoveAt(index);
						dropTwo = false;
					}
					else
					{
						currentDuration += base.properties.CurrentState.horse.DropBulletOneDelay.RandomFloat();
						int index2 = Random.Range(0, indexPatterns.Count);
						durationBeforeDrops[indexPatterns[index2]] = currentDuration;
						indexPatterns.RemoveAt(index2);
						dropTwo = true;
					}
				}
				for (int j = 0; j < droppattern.Length; j++)
				{
					n = ((j < droppattern.Length - 1) ? (j + 1) : 0);
					float.TryParse(droppattern[j], out spawnY);
					float.TryParse(droppattern[n], out nextSpawnY);
					float dist = nextSpawnY - spawnY;
					FireDropBullets(spawnY, durationBeforeDrops[j]);
					float halfSpeed = p.DropBulletInitalSpeed / 2f;
					yield return CupheadTime.WaitForSeconds(this, dist / halfSpeed / 2f);
				}
				base.animator.SetBool("Spit", false);
				if (dropBulletIndex < dropBulletPositionPattern.Length - 1)
				{
					dropBulletIndex++;
				}
				else
				{
					dropMainIndex = (dropMainIndex + 1) % p.DropBulletPositionString.Length;
					dropBulletIndex = 0;
				}
				yield return CupheadTime.WaitForSeconds(this, p.DropATKDelay);
				break;
			}
			}
			positionIndex %= positionPattern.Length;
		}
		yield return CupheadTime.WaitForSeconds(this, hesitate);
		while (base.transform.position.y != startPos.y)
		{
			this.pos = base.transform.position;
			this.pos.y = Mathf.MoveTowards(base.transform.position.y, startPos.y, p.HorseSpeed * (float)CupheadTime.Delta);
			base.transform.position = this.pos;
			yield return null;
		}
		StartCoroutine(select_horse_cr());
		yield return null;
	}

	private IEnumerator spit_fx_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.167f);
		do
		{
			Effect effectA = spitFxPrefabA.Create(spitFxRoot.position, base.transform.localScale);
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.125f, 0.21f));
			if (!base.animator.GetBool("Spit"))
			{
				break;
			}
			Effect effectB = spitFxPrefabB.Create(spitFxRoot.position, base.transform.localScale);
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.125f, 0.21f));
		}
		while (base.animator.GetBool("Spit"));
	}

	private void SelectStartPos()
	{
		startPos.y = 860f;
		if (Rand.Bool())
		{
			startPos.x = -640f + base.properties.CurrentState.horse.HorseXPosOffset;
			base.transform.position = startPos;
			base.transform.SetScale(-1f, 1f, 1f);
		}
		else
		{
			startPos.x = 640f - base.properties.CurrentState.horse.HorseXPosOffset;
			base.transform.position = startPos;
			base.transform.SetScale(1f, 1f, 1f);
		}
	}

	private void SpitSFX()
	{
		AudioManager.Play("clown_horse_head_spit");
		emitAudioFromObject.Add("clown_horse_head_spit");
	}

	private void FireWaveBullets(int index, bool isPink, float YSpeed, Vector3 pos)
	{
		LevelProperties.Clown.Horse horse = base.properties.CurrentState.horse;
		bool onRight = ((base.transform.position.x > 0f) ? true : false);
		if (isPink)
		{
			ClownLevelHorseshoe clownLevelHorseshoe = Object.Instantiate(pinkHorseshoe);
			clownLevelHorseshoe.Init(pos, horse.WaveBulletSpeed, YSpeed, onRight, 0f, horse, HorseType.Wave);
		}
		else
		{
			ClownLevelHorseshoe clownLevelHorseshoe2 = Object.Instantiate(regularHorseshoe);
			clownLevelHorseshoe2.Init(pos, horse.WaveBulletSpeed, YSpeed, onRight, 0f, horse, HorseType.Wave);
		}
	}

	private void FireDropBullets(float spawnY, float durationBeforeDrop)
	{
		LevelProperties.Clown.Horse horse = base.properties.CurrentState.horse;
		bool onRight = ((projectileRoot.transform.position.x > 0f) ? true : false);
		ClownLevelHorseshoe clownLevelHorseshoe = Object.Instantiate(regularHorseshoe);
		clownLevelHorseshoe.Init(projectileRoot.transform.position, horse.DropBulletInitalSpeed, spawnY, onRight, durationBeforeDrop, horse, HorseType.Drop);
	}

	public void StartDeath()
	{
		StopAllCoroutines();
		StartCoroutine(horse_death_cr());
	}

	private IEnumerator horse_death_cr()
	{
		float t = 0f;
		float time = 3f;
		Vector2 start = base.transform.position;
		startPos.x = base.transform.position.x;
		GetComponent<SpriteRenderer>().color = ColorUtils.HexToColor("FFFFFFFF");
		clownHorseHead.GetComponent<Collider2D>().enabled = false;
		clownHorseBody.GetComponent<Collider2D>().enabled = false;
		StartExplosions();
		base.animator.Play("Off");
		base.animator.SetTrigger("Dead");
		FallHorseScreamSFX();
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, startPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		t = 0f;
		pos.x = 0f;
		pos.y = base.transform.position.y;
		base.transform.position = pos;
		yield return CupheadTime.WaitForSeconds(this, 0.75f);
		while (t < time)
		{
			float val2 = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(pos, new Vector3(0f, 250f, 0f), val2);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		startPos.x = base.transform.position.x;
		EndExplosions();
		while (clownHorseHead.GetComponent<HitFlash>().flashing)
		{
			yield return null;
		}
		Object.Destroy(clownHorseHead.GetComponent<HitFlash>());
		yield return CupheadTime.WaitForSeconds(this, 1f);
		GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		GetComponent<SpriteRenderer>().sortingOrder = 200;
		base.animator.SetTrigger("Fall");
		moveObject = base.transform;
		yield return CupheadTime.WaitForSeconds(this, 0.3f);
		while (!droppedClown)
		{
			yield return null;
		}
		t = 0f;
		time = 3f;
		while (t < time)
		{
			if ((float)CupheadTime.Delta != 0f)
			{
				float t2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
				moveObject.transform.position = Vector2.Lerp(moveObject.transform.position, startPos, t2);
				t += (float)CupheadTime.Delta;
			}
			yield return null;
		}
		moveObject.transform.position = startPos;
		Object.Destroy(moveObject.gameObject);
		yield return null;
	}

	private void Separate()
	{
		clownHorseBody.transform.parent = null;
		moveObject = clownHorseBody.transform;
	}

	private IEnumerator clown_fall_cr()
	{
		float fallGravity = -100f;
		float fallAccumulatedGravity = 0f;
		Vector2 fallVelocity = Vector3.zero;
		FallHorseSFXOff();
		droppedClown = true;
		while (base.transform.position.y > -660f)
		{
			if ((float)CupheadTime.Delta != 0f)
			{
				base.transform.position += (Vector3)(fallVelocity + new Vector2(-300f, fallAccumulatedGravity)) * CupheadTime.FixedDelta;
				fallAccumulatedGravity += fallGravity;
			}
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, 0.1f);
		while (moveObject != null)
		{
			yield return null;
		}
		clownSwing.StartSwing();
		Object.Destroy(base.gameObject);
		yield return null;
	}

	private void StartExplosions()
	{
		clownHorseHead.GetComponent<LevelBossDeathExploder>().StartExplosion();
	}

	private void EndExplosions()
	{
		clownHorseHead.GetComponent<LevelBossDeathExploder>().StopExplosions();
	}

	private void Dead()
	{
		StopAllCoroutines();
		base.animator.Play("Off");
		base.animator.SetTrigger("Dead");
		Level.Current.OnLevelEndEvent -= Dead;
		StartCoroutine(move_to_death_spot_cr());
	}

	private IEnumerator move_to_death_spot_cr()
	{
		float t = 0f;
		float time = 1f;
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(base.transform.position, new Vector3(pos.x, 250f, 0f), val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
	}

	private void FallHorseSFX()
	{
		AudioManager.Play("clown_horse_death_slide");
	}

	private void FallHorseScreamSFX()
	{
		if (!ScreamSFXPlaying)
		{
			ScreamSFXPlaying = true;
			AudioManager.PlayLoop("clown_horse_death");
		}
	}

	private void FallHorseSFXOff()
	{
		AudioManager.FadeSFXVolume("clown_horse_death", 0f, 1f);
		AudioManager.Play("clown_horse_death_end");
	}
}
