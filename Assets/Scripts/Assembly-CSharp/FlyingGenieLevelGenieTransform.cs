using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingGenieLevelGenieTransform : LevelProperties.FlyingGenie.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Marionette = 2,
		Giant = 3,
		Dead = 4
	}

	private const float FRAME_TIME = 1f / 24f;

	[SerializeField]
	private Effect deathPuffEffect;

	[SerializeField]
	private SpriteRenderer bottomLayer;

	[Space(10f)]
	[SerializeField]
	private FlyingGenieLevelSpawner spawnerPrefab;

	[SerializeField]
	private Transform marionetteShootRoot;

	[SerializeField]
	private BasicProjectile shotBullet;

	[SerializeField]
	private BasicProjectile pinkBullet;

	[SerializeField]
	private BasicProjectile shootBullet;

	[SerializeField]
	private FlyingGenieLevelBomb regularBomb;

	[SerializeField]
	private FlyingGenieLevelBomb diagonalBomb;

	[SerializeField]
	private FlyingGenieLevelBomb plusSizedBomb;

	[SerializeField]
	private BasicProjectile spreadProjectile;

	[SerializeField]
	private FlyingGenieLevelProjectile scanBullet;

	[SerializeField]
	private FlyingGenieLevelProjectile scanBulletPink;

	[SerializeField]
	private FlyingGenieLevelRing ringPrefab;

	[SerializeField]
	private FlyingGenieLevelRing pinkRingPrefab;

	[SerializeField]
	private FlyingGenieLevelPyramid pyramidPrefab;

	[Space(10f)]
	[SerializeField]
	private Transform pyramidPivotPoint;

	[SerializeField]
	private Transform gemStone;

	[SerializeField]
	private Transform pipe;

	[SerializeField]
	private Transform giantRoot;

	[SerializeField]
	private Transform handFront;

	[SerializeField]
	private Transform handBack;

	[SerializeField]
	private Transform deathPuffRoot;

	[SerializeField]
	private Transform morphRoot;

	[SerializeField]
	private Transform marionetteRoot;

	private FlyingGenieLevelMeditateFX meditateP1;

	private FlyingGenieLevelMeditateFX meditateP2;

	private FlyingGenieLevelSpawner spawner;

	private List<FlyingGenieLevelBomb> bombs;

	private List<FlyingGenieLevelPyramid> pyramids;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Vector3 startPos;

	private bool isClockWise;

	private bool isScanning;

	private bool isShooting;

	private int pinkIndex;

	private string[] pinkString;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
	}

	public override void LevelInit(LevelProperties.FlyingGenie properties)
	{
		base.LevelInit(properties);
		state = State.Intro;
		pyramids = new List<FlyingGenieLevelPyramid>();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (base.properties.CurrentHealth <= 0f && state != State.Dead)
		{
			state = State.Dead;
			if (Level.Current.mode == Level.Mode.Easy)
			{
				MarionetteDead();
			}
			else
			{
				StartDeath();
			}
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

	public void StartMarionette(Vector3 spawnPos, FlyingGenieLevelMeditateFX meditateP1, FlyingGenieLevelMeditateFX meditateP2)
	{
		base.transform.position = spawnPos;
		this.meditateP1 = meditateP1;
		this.meditateP2 = meditateP2;
		StartCoroutine(phase2_intro_cr());
	}

	private IEnumerator phase2_intro_cr()
	{
		AudioManager.Play("genie_return");
		emitAudioFromObject.Add("genie_return");
		base.animator.Play("Meditate_FX");
		LevelProperties.FlyingGenie.Scan p = base.properties.CurrentState.scan;
		float timer = 0f;
		isScanning = true;
		StartCoroutine(scan_bullets_cr());
		while (timer < p.scanDuration)
		{
			timer += (float)CupheadTime.Delta;
			yield return null;
		}
		base.animator.SetTrigger("Continue");
		isScanning = false;
		yield return null;
	}

	private void EndFX()
	{
		if (meditateP1 != null)
		{
			meditateP1.EndEffect();
		}
		if (meditateP2 != null)
		{
			meditateP2.EndEffect();
		}
	}

	private void SnapPosition()
	{
		HandSFX();
		base.transform.position = morphRoot.position;
		bottomLayer.transform.localPosition = new Vector3(-160f, bottomLayer.transform.localPosition.y);
		StartCoroutine(handle_carpet_fadeout_cr());
	}

	private void MoveUp()
	{
		StartCoroutine(move_up_cr());
	}

	private IEnumerator move_up_cr()
	{
		float speed = 1300f;
		while (base.transform.position.y < 360f)
		{
			base.transform.AddPosition(0f, speed * (float)CupheadTime.Delta);
			yield return null;
		}
		base.animator.Play("Marionette_Intro");
		Vector3 pos = base.transform.position;
		pos.y = marionetteRoot.position.y - 50f;
		base.transform.position = pos;
		startPos = base.transform.position;
		yield return null;
	}

	private IEnumerator handle_carpet_fadeout_cr()
	{
		bottomLayer.color = new Color(1f, 1f, 1f, 1f);
		float t = 0f;
		float time = 2f;
		while (t < time)
		{
			bottomLayer.color = new Color(1f, 1f, 1f, 1f - t / time);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.animator.Play("Hands_Off");
		bottomLayer.color = new Color(1f, 1f, 1f, 1f);
		bottomLayer.transform.localPosition = Vector3.zero;
		yield return null;
	}

	private IEnumerator scan_bullets_cr()
	{
		LevelProperties.FlyingGenie.Scan p = base.properties.CurrentState.scan;
		string[] positionPattern = p.appearCoordinates.GetRandom().Split(',');
		string[] appearDelay = p.bulletAppearDelay.GetRandom().Split(',');
		string[] pinkPattern = p.bulletPinkString.Split(',');
		Vector3 endPosition = Vector3.zero;
		float location = 0f;
		float delay = 0f;
		int pinkIndex = UnityEngine.Random.Range(0, pinkPattern.Length);
		int delayIndex = UnityEngine.Random.Range(0, appearDelay.Length);
		while (isScanning)
		{
			for (int i = 0; i < positionPattern.Length; i++)
			{
				string[] coordinates = positionPattern[i].Split('-');
				for (int j = 0; j < coordinates.Length; j++)
				{
					float.TryParse(coordinates[j], out location);
					if (i % 2 == 0)
					{
						endPosition.x = -640f + location;
					}
					else
					{
						endPosition.y = 360f - location;
					}
				}
				float.TryParse(appearDelay[delayIndex], out delay);
				AbstractPlayerController player = PlayerManager.GetNext();
				if (pinkPattern[pinkIndex][0] == 'R')
				{
					scanBullet.Create(endPosition, base.properties.CurrentState.scan, player);
				}
				else if (pinkPattern[pinkIndex][0] == 'P')
				{
					scanBulletPink.Create(endPosition, base.properties.CurrentState.scan, player);
				}
				yield return CupheadTime.WaitForSeconds(this, delay);
				delayIndex = (delayIndex + 1) % appearDelay.Length;
				pinkIndex = (pinkIndex + 1) % pinkPattern.Length;
			}
			yield return null;
		}
		yield return base.animator.WaitForAnimationToEnd(this, "Marionette_Intro");
		state = State.Marionette;
		StartCoroutine(move_cr());
		if (p.usingBulletATK)
		{
			StartCoroutine(shoot_cr());
		}
		else
		{
			StartCoroutine(bombs_cr());
		}
		yield return null;
	}

	private void SpawnTurban()
	{
		spawner = spawnerPrefab.Create(new Vector3(base.transform.position.x, (float)Level.Current.Height + 100f), PlayerManager.GetNext(), base.properties.CurrentState.bullets, this);
		spawner.isDead = false;
	}

	private IEnumerator bombs_cr()
	{
		LevelProperties.FlyingGenie.Bomb p = base.properties.CurrentState.bomb;
		int bombIndex = UnityEngine.Random.Range(0, p.bombPlacementString.Length);
		string[] bombPattern = p.bombPlacementString[bombIndex].Split(',');
		Vector3 endPosition = Vector3.zero;
		int index = UnityEngine.Random.Range(0, bombPattern.Length);
		int stringCounter = 0;
		float location = 0f;
		bool bombsFinished = false;
		bombs = new List<FlyingGenieLevelBomb>();
		FlyingGenieLevelBomb picked = null;
		while (true)
		{
			for (stringCounter = 0; stringCounter < bombPattern.Length; stringCounter++)
			{
				state = State.Marionette;
				string[] bombType = bombPattern[index].Split(':');
				for (int n = 0; n < bombType.Length; n++)
				{
					if (bombType[n][0] == 'R')
					{
						picked = regularBomb;
					}
					else if (bombType[n][0] == 'D')
					{
						picked = diagonalBomb;
					}
					else if (bombType[n][0] == 'P')
					{
						picked = plusSizedBomb;
					}
					string[] bombPos = bombType[n].Split('-');
					for (int i = 0; i < bombPos.Length; i++)
					{
						float.TryParse(bombPos[i], out location);
						if (i % 2 == 0)
						{
							endPosition.x = -640f + location;
						}
						else
						{
							endPosition.y = 360f - location;
						}
					}
					yield return null;
				}
				SpawnBomb(picked, endPosition);
				index = (index + 1) % bombPattern.Length;
				yield return null;
			}
			int bombNum = bombs.Count - 1;
			int bombCounter = 0;
			while (!bombsFinished)
			{
				foreach (FlyingGenieLevelBomb bomb in bombs)
				{
					if (bomb.readyToDetonate)
					{
						if (bombCounter >= bombNum)
						{
							bombsFinished = true;
							break;
						}
						bombCounter++;
					}
				}
				bombCounter = 0;
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, p.bombDelay);
			foreach (FlyingGenieLevelBomb bomb2 in bombs)
			{
				bomb2.Explode();
			}
			bombs.Clear();
			bombsFinished = false;
			state = State.Idle;
			yield return CupheadTime.WaitForSeconds(this, p.hesitate);
			bombIndex = (bombIndex + 1) % p.bombPlacementString.Length;
			bombPattern = p.bombPlacementString[bombIndex].Split(',');
		}
	}

	private void SpawnBomb(FlyingGenieLevelBomb bomb, Vector2 targetPos)
	{
		bombs.Add(bomb.Create(base.transform.position, targetPos, base.properties.CurrentState.bomb));
	}

	private IEnumerator shoot_cr()
	{
		LevelProperties.FlyingGenie.Bullets p = base.properties.CurrentState.bullets;
		int mainShotIndex = UnityEngine.Random.Range(0, p.shotCount.Length);
		string[] shotCount = p.shotCount[mainShotIndex].Split(',');
		int shotIndex = 0;
		string[] pinkCount = p.pinkString.Split(',');
		int pinkIndex = 0;
		while (state == State.Marionette)
		{
			isShooting = false;
			shotCount = p.shotCount[mainShotIndex].Split(',');
			yield return CupheadTime.WaitForSeconds(this, p.hesitateRange.RandomFloat());
			isShooting = true;
			base.animator.SetBool("IsAttacking", true);
			yield return base.animator.WaitForAnimationToEnd(this, "Marionette_Attack_Start");
			AudioManager.Play("genie_voice_laugh_reverb");
			AbstractPlayerController player = PlayerManager.GetNext();
			base.animator.Play("Marionette_Spark");
			for (int i = 0; i < shotCount.Length; i++)
			{
				for (int n = 0; n < int.Parse(shotCount[shotIndex]); n++)
				{
					if (player == null || player.IsDead)
					{
						player = PlayerManager.GetNext();
					}
					Vector3 dir = player.transform.position - marionetteShootRoot.transform.position;
					if (dir.x > 0f)
					{
						dir.x = 0f;
					}
					if (pinkCount[pinkIndex][0] == 'P')
					{
						pinkBullet.Create(marionetteShootRoot.transform.position, MathUtils.DirectionToAngle(dir), p.shotSpeed);
						AudioManager.Play("genie_puppet_shoot");
						emitAudioFromObject.Add("genie_puppet_shoot");
					}
					else if (pinkCount[pinkIndex][0] == 'R')
					{
						shotBullet.Create(marionetteShootRoot.transform.position, MathUtils.DirectionToAngle(dir), p.shotSpeed);
						AudioManager.Play("genie_puppet_shoot");
						emitAudioFromObject.Add("genie_puppet_shoot");
					}
					yield return WaitWhileShooting(p.shotDelay, p.shotSpeed);
					pinkIndex = (pinkIndex + 1) % pinkCount.Length;
				}
				if (player == null || player.IsDead)
				{
					player = PlayerManager.GetNext();
				}
				Vector3 targetPos = player.transform.position - marionetteShootRoot.transform.position;
				Vector3 spawnDir = targetPos - marionetteShootRoot.transform.position;
				yield return WaitWhileShooting(p.shotDelay, p.shotSpeed);
				if (shotIndex < shotCount.Length - 1)
				{
					shotIndex++;
				}
				else
				{
					mainShotIndex = (mainShotIndex + 1) % p.shotCount.Length;
					shotIndex = 0;
				}
				yield return null;
			}
			yield return null;
			base.animator.SetBool("IsAttacking", false);
		}
		yield return null;
	}

	private IEnumerator WaitWhileShooting(float time, float shootSpeed)
	{
		bool pointingUp = false;
		float timeEsalpsed = 0f;
		float timeSinceSubShot = 0f;
		while (timeEsalpsed <= time)
		{
			if (timeSinceSubShot >= 0.12f)
			{
				shootBullet.Create(marionetteShootRoot.transform.position, (!pointingUp) ? (-100) : 100, shootSpeed);
				pointingUp = !pointingUp;
				timeSinceSubShot = 0f;
			}
			timeEsalpsed += Time.deltaTime;
			timeSinceSubShot += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator move_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (state == State.Marionette)
		{
			if (!isShooting)
			{
				if (base.transform.position.x > 0f - startPos.x)
				{
					base.transform.AddPosition((0f - base.properties.CurrentState.bullets.marionetteMoveSpeed) * CupheadTime.FixedDelta);
				}
			}
			else if (base.transform.position.x < startPos.x)
			{
				base.transform.AddPosition(base.properties.CurrentState.bullets.marionetteReturnSpeed * CupheadTime.FixedDelta);
			}
			yield return wait;
		}
	}

	public void EndMarionette()
	{
		AudioManager.Play("genie_puppet_exit");
		emitAudioFromObject.Add("genie_puppet_exit");
		spawner.isDead = true;
		StopAllCoroutines();
		state = State.Giant;
		StartCoroutine(genie_intro_cr());
	}

	private void MarionetteDead()
	{
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("MarionetteDeath");
	}

	private IEnumerator genie_intro_cr()
	{
		float pullSpeed = 700f;
		float size = GetComponent<SpriteRenderer>().bounds.size.x;
		float angle = 120f;
		int number = 1;
		isClockWise = Rand.Bool();
		base.animator.SetTrigger("MarionetteDeath");
		GetComponent<LevelBossDeathExploder>().StartExplosion();
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (base.transform.position.y < 960f)
		{
			base.transform.AddPosition(0f, pullSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		GetComponent<LevelBossDeathExploder>().StopExplosions();
		yield return CupheadTime.WaitForSeconds(this, 0.7f);
		base.animator.Play("Giant_Intro");
		base.transform.position = new Vector3(640f + size / 3f, 0f);
		Vector3 startPos = base.transform.position;
		float t = 0f;
		float time = 1f;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(startPos, giantRoot.position, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = giantRoot.position;
		for (int i = 0; i < 3; i++)
		{
			SpawnPyramids(angle * ((float)Math.PI / 180f) * (float)i, number);
			number++;
		}
		StartCoroutine(attack_cr());
		yield return null;
	}

	private void IntroHands()
	{
		StartCoroutine(intro_hands_cr());
	}

	private IEnumerator intro_hands_cr()
	{
		Vector3 end = handFront.transform.position;
		Vector3 start = handFront.transform.position;
		start.y = handFront.transform.position.y - 500f;
		handFront.transform.position = start;
		handBack.transform.position = start;
		base.animator.Play("Giant_Hands");
		float t = 0f;
		float time = 1.25f;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			handFront.transform.position = Vector2.Lerp(start, end, val);
			handBack.transform.position = Vector2.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, 2.42f);
		t = 0f;
		while (t < time)
		{
			float val2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			handFront.transform.position = Vector2.Lerp(end, start, val2);
			handBack.transform.position = Vector2.Lerp(end, start, val2);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.animator.Play("Hands_Off");
		StartCoroutine(gem_stone_cr());
		yield return null;
	}

	private void SpawnPyramids(float startingAngle, int number)
	{
		LevelProperties.FlyingGenie.Pyramids pyramids = base.properties.CurrentState.pyramids;
		FlyingGenieLevelPyramid flyingGenieLevelPyramid = UnityEngine.Object.Instantiate(pyramidPrefab);
		flyingGenieLevelPyramid.Init(pyramids, base.transform.position, startingAngle, pyramids.speedRotation, pyramidPivotPoint, number, isClockWise);
		flyingGenieLevelPyramid.GetComponent<Collider2D>().enabled = false;
		this.pyramids.Add(flyingGenieLevelPyramid);
	}

	private IEnumerator attack_cr()
	{
		LevelProperties.FlyingGenie.Pyramids p = base.properties.CurrentState.pyramids;
		string[] delayString = p.attackDelayString.GetRandom().Split(',');
		string[] attackString = p.pyramidAttackString.GetRandom().Split(',');
		int delayIndex = UnityEngine.Random.Range(0, delayString.Length);
		int attackIndex = UnityEngine.Random.Range(0, attackString.Length);
		float delay = 0f;
		int numberReceived = 0;
		float t = 0f;
		float time = 2.5f;
		foreach (FlyingGenieLevelPyramid pyramid in pyramids)
		{
			pyramid.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
		}
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			foreach (FlyingGenieLevelPyramid pyramid2 in pyramids)
			{
				pyramid2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, t / time);
			}
			yield return null;
		}
		foreach (FlyingGenieLevelPyramid pyramid3 in pyramids)
		{
			pyramid3.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			pyramid3.GetComponent<Collider2D>().enabled = true;
		}
		while (true)
		{
			int n;
			for (n = attackIndex; n < attackString.Length; n++)
			{
				float.TryParse(delayString[delayIndex], out delay);
				yield return CupheadTime.WaitForSeconds(this, delay);
				string[] attackOrder = attackString[n].Split('-');
				string[] array = attackOrder;
				foreach (string s in array)
				{
					int.TryParse(s, out numberReceived);
					for (int j = 0; j < pyramids.Count; j++)
					{
						if (pyramids[j].number == numberReceived)
						{
							StartCoroutine(pyramids[j].beam_cr());
						}
					}
				}
				for (int k = 0; k < pyramids.Count; k++)
				{
					if (pyramids[k].number == numberReceived)
					{
						while (!pyramids[k].finishedATK)
						{
							yield return null;
						}
					}
				}
				attackIndex = 0;
				n %= attackString.Length;
				delayIndex = (delayIndex + 1) % delayString.Length;
			}
			yield return null;
		}
	}

	private IEnumerator gem_stone_cr()
	{
		LevelProperties.FlyingGenie.GemStone p = base.properties.CurrentState.gemStone;
		string[] attackDelayPattern = p.attackDelayString.GetRandom().Split(',');
		int delayIndex = UnityEngine.Random.Range(0, attackDelayPattern.Length);
		pinkString = p.pinkString.Split(',');
		pinkIndex = UnityEngine.Random.Range(0, pinkString.Length);
		float delay = 0f;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, p.warningDuration);
			float.TryParse(attackDelayPattern[delayIndex], out delay);
			base.animator.SetTrigger("OnGiantAttack");
			yield return base.animator.WaitForAnimationToEnd(this, "Giant_Attack");
			yield return CupheadTime.WaitForSeconds(this, delay);
			delayIndex = (delayIndex + 1) % attackDelayPattern.Length;
			yield return null;
		}
	}

	private void OnRing()
	{
		LevelProperties.FlyingGenie.GemStone gemStone = base.properties.CurrentState.gemStone;
		bool flag = false;
		FlyingGenieLevelRing flyingGenieLevelRing = null;
		this.gemStone.LookAt2D(PlayerManager.GetNext().center);
		if (pinkString[pinkIndex][0] == 'P')
		{
			flag = true;
			flyingGenieLevelRing = pinkRingPrefab.Create(this.gemStone.position, this.gemStone.eulerAngles.z, gemStone.bulletSpeed) as FlyingGenieLevelRing;
		}
		else
		{
			flag = false;
			flyingGenieLevelRing = ringPrefab.Create(this.gemStone.position, this.gemStone.eulerAngles.z, gemStone.bulletSpeed) as FlyingGenieLevelRing;
		}
		StartCoroutine(ring_cr(flyingGenieLevelRing, flag));
		pinkIndex = (pinkIndex + 1) % pinkString.Length;
	}

	private IEnumerator ring_cr(FlyingGenieLevelRing ring, bool isPink)
	{
		ring.isMain = true;
		int frameCount = 0;
		float frameTime = 0f;
		FlyingGenieLevelRing trailRing = ((!isPink) ? ringPrefab : pinkRingPrefab);
		FlyingGenieLevelRing lastRing = null;
		while (ring != null)
		{
			frameTime += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				if (frameCount < 3)
				{
					frameCount++;
				}
				else
				{
					frameCount = 0;
					if (lastRing != null)
					{
						lastRing.DisableCollision();
					}
					lastRing = trailRing.Create(ring.transform.position, gemStone.eulerAngles.z, 0.1f) as FlyingGenieLevelRing;
					lastRing.SetProperties(base.properties.CurrentState.gemStone);
				}
				frameTime -= 1f / 24f;
				yield return null;
			}
			yield return null;
		}
		yield return null;
	}

	private void StartDeath()
	{
		base.animator.SetTrigger("Death");
	}

	private void SpawnPuff()
	{
		deathPuffEffect.Create(deathPuffRoot.transform.position);
	}

	private void HandSFX()
	{
		AudioManager.Play("genie_puppet_hand_enter");
		emitAudioFromObject.Add("genie_puppet_hand_enter");
	}

	private void SoundGenieVoiceMorph()
	{
		AudioManager.Play("genie_voice_excited");
		emitAudioFromObject.Add("genie_voice_excited");
	}

	private void SoundPuppetRun()
	{
		AudioManager.Play("genie_puppet_run");
		emitAudioFromObject.Add("genie_puppet_run");
	}

	private void SoundGenieVoicePhase3Intro()
	{
		AudioManager.Play("genie_voice_phase3_intro");
		emitAudioFromObject.Add("genie_voice_phase3_intro");
	}

	private void SoundGenieMindShoot()
	{
		AudioManager.Play("genie_phase3_mind_shoot");
		emitAudioFromObject.Add("genie_phase3_mind_shoot");
	}
}
