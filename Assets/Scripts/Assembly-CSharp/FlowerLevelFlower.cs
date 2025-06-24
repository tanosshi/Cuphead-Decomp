using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerLevelFlower : LevelProperties.Flower.Entity
{
	private Action attackCallback;

	public GameObject attackPoint;

	private bool topLaserAttack;

	private bool projectileSpawned;

	private bool isDead;

	private float attackCharge;

	private int attackCount;

	private int attackCountTarget;

	private char attackType;

	private int currentLaserAttack;

	private int currentPodHandsAttack;

	private int podHandsAttackCountTarget;

	private int currentGattlingGunAttackString;

	private List<string> currentGattlingGunAttackPattern;

	private int currentVineHandsAttack;

	private int pollenAttackCount;

	private int currentPollenType;

	private FlowerLevelPollenProjectile currentPollenShot;

	[Header("Vines")]
	[SerializeField]
	private GameObject vineHandPrefab;

	[Space(10f)]
	[Header("Prefabs")]
	[SerializeField]
	private GameObject boomerangPrefab;

	[SerializeField]
	private GameObject bulletSeedPrefab;

	[SerializeField]
	private GameObject cloudBombPrefab;

	[SerializeField]
	private GameObject enemySeedPrefab;

	private bool miniFlowerSpawned;

	[SerializeField]
	private GameObject pollenProjectile;

	[Space(10f)]
	[SerializeField]
	private Transform topProjectileSpawnPoint;

	[SerializeField]
	private Transform bottomProjectileSpawnPoint;

	[SerializeField]
	private GameObject mainVine;

	[SerializeField]
	private GameObject gattlingFX;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private List<AbstractProjectile> bulletSpawns = new List<AbstractProjectile>();

	public event Action OnDeathEvent;

	public event Action OnStateChanged;

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public void AdditionDamageTaken(DamageDealer.DamageInfo info)
	{
		OnDamageTaken(info);
	}

	public void PhaseTwoTrigger()
	{
		base.animator.SetTrigger("PhaseTwoTransition");
	}

	private void Die()
	{
		isDead = true;
		StartCoroutine(die_cr());
	}

	private void SpawnMainVine()
	{
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged();
		}
		mainVine.SetActive(true);
		base.animator.SetTrigger("SpawnMainVine");
	}

	private void MainVineSpawned()
	{
		StartCoroutine(vineHands_cr());
		projectileSpawned = false;
		StartCoroutine(pollenAttack_cr());
		attackCount = 1;
	}

	private IEnumerator die_cr()
	{
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
		StopAllCoroutines();
		GetComponent<Collider2D>().enabled = false;
		Debug.Log(Level.Current.mode);
		if (Level.Current.mode == Level.Mode.Easy)
		{
			base.animator.Play("Phase One Death");
		}
		else
		{
			base.animator.Play("Phase Two Death");
		}
		yield return null;
		base.animator.enabled = false;
		base.animator.enabled = true;
		base.properties.WinInstantly();
	}

	public override void LevelInit(LevelProperties.Flower properties)
	{
		properties.OnBossDeath += Phase2DeathAudio;
		properties.OnBossDeath += Die;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.LevelInit(properties);
		attackCount = 0;
		miniFlowerSpawned = false;
		Level.Current.OnIntroEvent += OnIntro;
		int num = UnityEngine.Random.Range(0, properties.CurrentState.laser.attackType.Split(',').Length);
		currentLaserAttack = num;
		currentGattlingGunAttackPattern = new List<string>();
		currentGattlingGunAttackString = UnityEngine.Random.Range(0, properties.CurrentState.gattlingGun.seedSpawnString.Length);
		string[] array = properties.CurrentState.vineHands.handAttackString.Split(',');
		currentVineHandsAttack = UnityEngine.Random.Range(0, array.Length);
		pollenAttackCount = UnityEngine.Random.Range(0, properties.CurrentState.pollenSpit.pollenAttackCount.Split(',').Length);
		currentPollenType = UnityEngine.Random.Range(0, properties.CurrentState.pollenSpit.pollenType.Split(',').Length);
		StartCoroutine(find_s_cr());
	}

	private IEnumerator find_s_cr()
	{
		while (base.properties.CurrentState.podHands.attacktype.Split(',')[currentPodHandsAttack][0] != 'S')
		{
			currentPodHandsAttack = UnityEngine.Random.Range(0, base.properties.CurrentState.podHands.attacktype.Split(',').Length);
			yield return null;
		}
		podHandsAttackCountTarget = UnityEngine.Random.Range(0, base.properties.CurrentState.podHands.attackAmount.Split(',').Length);
		yield return null;
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void StartLaser(Action callback)
	{
		attackCallback = callback;
		attackType = base.properties.CurrentState.laser.attackType.Split(',')[currentLaserAttack][0];
		attackCharge = base.properties.CurrentState.laser.anticHold;
		OnLaserStarted();
	}

	public void OnLaserStarted()
	{
		if (attackType.Equals('T'))
		{
			topLaserAttack = true;
		}
		else
		{
			topLaserAttack = false;
		}
		if (topLaserAttack)
		{
			base.animator.SetBool("TopLaser", true);
		}
		else
		{
			base.animator.SetBool("BottomLaser", true);
		}
		StartCoroutine(laserCharge_cr());
	}

	private IEnumerator laserCharge_cr()
	{
		if (topLaserAttack)
		{
			yield return base.animator.WaitForAnimationToEnd(this, "TopLaserAttackStart", true);
		}
		else
		{
			yield return base.animator.WaitForAnimationToEnd(this, "BottomLaserAttackStart", true);
		}
		yield return CupheadTime.WaitForSeconds(this, attackCharge);
		base.animator.SetTrigger("OnAttackChargeComplete");
	}

	private void OnHoldComplete()
	{
		StartCoroutine(onLaser_cr());
	}

	private IEnumerator onLaser_cr()
	{
		attackCharge = base.properties.CurrentState.laser.attackHold;
		yield return CupheadTime.WaitForSeconds(this, attackCharge);
		if (topLaserAttack)
		{
			base.animator.SetBool("TopLaser", false);
		}
		else
		{
			base.animator.SetBool("BottomLaser", false);
		}
		if (topLaserAttack)
		{
			yield return base.animator.WaitForAnimationToEnd(this, "TopLaserAttackEnd", true);
		}
		else
		{
			yield return base.animator.WaitForAnimationToEnd(this, "BottomLaserAttackEnd", true);
		}
	}

	public void OnLaserComplete()
	{
		currentLaserAttack++;
		if (currentLaserAttack >= base.properties.CurrentState.laser.attackType.Split(',').Length)
		{
			currentLaserAttack = 0;
		}
		topLaserAttack = false;
		if (attackCallback != null)
		{
			attackCallback();
		}
		attackCallback = null;
	}

	public void StartPotHands(Action callback)
	{
		attackCount = 0;
		attackCountTarget = int.Parse(base.properties.CurrentState.podHands.attackAmount.Split(',')[podHandsAttackCountTarget].ToString());
		attackType = base.properties.CurrentState.podHands.attacktype.Split(',')[currentPodHandsAttack][0];
		attackCallback = callback;
		attackCharge = base.properties.CurrentState.podHands.attackHold;
		OnPotHandsStarted();
	}

	public void OnPotHandsStarted()
	{
		base.animator.SetBool("PotHandsAttack", true);
		StartCoroutine(potHandsHold_cr());
		attackCount++;
	}

	private IEnumerator potHandsHold_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, attackCharge);
		base.animator.SetTrigger("OnAttackChargeComplete");
		OpenPotHands();
	}

	private void OpenPotHands()
	{
		attackCharge = base.properties.CurrentState.podHands.attackDelay;
	}

	public void OnPotHandsComplete()
	{
		projectileSpawned = false;
		if (attackCount >= attackCountTarget)
		{
			attackCount = 0;
			podHandsAttackCountTarget++;
			if (podHandsAttackCountTarget >= base.properties.CurrentState.podHands.attackAmount.Split(',').Length)
			{
				podHandsAttackCountTarget = 0;
			}
			base.animator.SetTrigger("OnAttackComplete");
			if (attackCallback != null)
			{
				attackCallback();
				attackCallback = null;
			}
		}
		base.animator.SetBool("PotHandsAttack", false);
	}

	public void StartGattlingGun(Action callback)
	{
		attackCallback = callback;
		attackCharge = base.properties.CurrentState.gattlingGun.loopDuration;
		base.animator.SetBool("GattlingGunAttack", true);
		attackType = 'G';
		StartCoroutine(startGattlingFX_cr());
	}

	private IEnumerator startGattlingFX_cr()
	{
		string animAttrib = "GattlingGunAttack";
		yield return base.animator.WaitForAnimationToEnd(this, "GattlingGunStart", true);
		while (base.animator.GetBool(animAttrib))
		{
			GameObject fxObject = UnityEngine.Object.Instantiate(gattlingFX, topProjectileSpawnPoint.position, Quaternion.identity);
			Animator fx = fxObject.GetComponent<Animator>();
			yield return StartCoroutine(killGattlingFX_cr(fx));
		}
	}

	private IEnumerator killGattlingFX_cr(Animator fx)
	{
		yield return fx.WaitForAnimationToEnd(this, true);
		UnityEngine.Object.Destroy(fx.gameObject);
	}

	public void OnGattlingGunEnded()
	{
		base.animator.SetBool("GattlingGunAttack", false);
		OnGattlingGunComplete();
	}

	public void OnGattlingGunComplete()
	{
		if (attackCallback != null)
		{
			attackCallback();
		}
		attackCallback = null;
	}

	private void AddAttackTypes(string[] s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			currentGattlingGunAttackPattern.Add(s[i]);
		}
	}

	private void StartVineHandsAttack()
	{
		StartCoroutine(vineHands_cr());
	}

	private IEnumerator vineHands_cr()
	{
		while (!isDead)
		{
			string[] attackPositions = base.properties.CurrentState.vineHands.handAttackString.Split(',');
			string[] currentWave = attackPositions[currentVineHandsAttack].Split('-');
			if (attackPositions[currentVineHandsAttack][0] != 'D')
			{
				if (currentWave.Length > 1)
				{
					currentVineHandsAttack += 2;
					vineHandPrefab.GetComponent<FlowerLevelFlowerVineHand>().OnVineHandSpawn(base.properties.CurrentState.vineHands.firstPositionHold, base.properties.CurrentState.vineHands.secondPositionHold, int.Parse(currentWave[0]), int.Parse(currentWave[1]));
				}
				else
				{
					currentVineHandsAttack++;
					vineHandPrefab.GetComponent<FlowerLevelFlowerVineHand>().OnVineHandSpawn(base.properties.CurrentState.vineHands.firstPositionHold, base.properties.CurrentState.vineHands.secondPositionHold, int.Parse(currentWave[0]));
				}
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, float.Parse(attackPositions[currentVineHandsAttack].Substring(1)));
			}
			if (currentVineHandsAttack >= attackPositions.Length)
			{
				currentVineHandsAttack = 0;
			}
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.vineHands.attackDelay.RandomFloat());
		}
	}

	private IEnumerator pollenAttack_cr()
	{
		while (!isDead)
		{
			if (!projectileSpawned)
			{
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.pollenSpit.pollenCommaDelay);
				string delay = base.properties.CurrentState.pollenSpit.pollenAttackCount.Split(',')[pollenAttackCount].ToString();
				if (delay[0].Equals('D'))
				{
					yield return CupheadTime.WaitForSeconds(this, float.Parse(delay.Substring(1).ToString()));
				}
				else
				{
					attackCountTarget = int.Parse(delay);
				}
				pollenAttackCount++;
				if (pollenAttackCount >= base.properties.CurrentState.pollenSpit.pollenAttackCount.Split(',').Length)
				{
					pollenAttackCount = 0;
				}
				projectileSpawned = true;
				base.animator.SetBool("OnPollenAttack", true);
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.pollenSpit.consecutiveAttackHold);
				base.animator.SetTrigger("OnAttackChargeComplete");
			}
			yield return null;
		}
	}

	private void launchPollen()
	{
		int num = 0;
		string text = base.properties.CurrentState.pollenSpit.pollenType.Split(',')[currentPollenType];
		num = ((!text[0].Equals('R')) ? 1 : 0);
		currentPollenType++;
		if (currentPollenType >= base.properties.CurrentState.pollenSpit.pollenType.Split(',').Length)
		{
			currentPollenType = 0;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(pollenProjectile, topProjectileSpawnPoint.position, Quaternion.identity);
		currentPollenShot = gameObject.GetComponent<FlowerLevelPollenProjectile>();
		currentPollenShot.InitPollen(base.properties.CurrentState.pollenSpit.pollenSpeed, base.properties.CurrentState.pollenSpit.pollenUpDownStrength, num, topProjectileSpawnPoint);
		AudioManager.Play("flower_phase2_spit_projectile");
		attackCount++;
		if (attackCount > attackCountTarget)
		{
			base.animator.SetBool("OnPollenAttack", false);
			attackCount = 1;
			projectileSpawned = false;
		}
		else
		{
			projectileSpawned = true;
		}
	}

	private void PollenShotEnd()
	{
		currentPollenShot.StartMoving();
	}

	private void OnIntro()
	{
		base.animator.SetTrigger("OnIntroEnded");
	}

	private void SpawnProjectile()
	{
		switch (attackType)
		{
		case 'B':
			SpawnBoomerang();
			break;
		case 'S':
			SpawnBullets();
			break;
		case 'R':
			SpawnCloudShot();
			break;
		case 'G':
			StartCoroutine(spawnGattlingGunSeeds_cr());
			break;
		}
		currentPodHandsAttack++;
		if (currentPodHandsAttack >= base.properties.CurrentState.podHands.attacktype.Split(',').Length)
		{
			currentPodHandsAttack = 0;
		}
		attackType = base.properties.CurrentState.podHands.attacktype.Split(',')[currentPodHandsAttack][0];
	}

	private IEnumerator spawnGattlingGunSeeds_cr()
	{
		currentGattlingGunAttackPattern.Clear();
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.gattlingGun.initialSeedDelay);
		string[] projectileAttributes = base.properties.CurrentState.gattlingGun.seedSpawnString[currentGattlingGunAttackString].Split(',');
		float delayNextProjectileWave = base.properties.CurrentState.gattlingGun.fallingSeedDelay;
		for (int i = 0; i < projectileAttributes.Length; i++)
		{
			string[] array = projectileAttributes[i].Split('-');
			if (array.Length > 1)
			{
				AddAttackTypes(array);
			}
			else
			{
				currentGattlingGunAttackPattern.Add(projectileAttributes[i]);
			}
			currentGattlingGunAttackPattern.Add("D" + base.properties.CurrentState.gattlingGun.fallingSeedDelay);
		}
		for (int j = 0; j < currentGattlingGunAttackPattern.Count; j++)
		{
			char t = currentGattlingGunAttackPattern[j][0];
			if (t == 'D')
			{
				yield return CupheadTime.WaitForSeconds(this, float.Parse(currentGattlingGunAttackPattern[j].Substring(1)));
				continue;
			}
			if (miniFlowerSpawned)
			{
				if (t != 'C')
				{
					SpawnEnemySeed(int.Parse(currentGattlingGunAttackPattern[j].Substring(1)), t);
				}
				else
				{
					SpawnEnemySeed(int.Parse(currentGattlingGunAttackPattern[j].Substring(1)), t, false);
				}
			}
			else
			{
				SpawnEnemySeed(int.Parse(currentGattlingGunAttackPattern[j].Substring(1)), t);
			}
			if (t == 'C')
			{
				miniFlowerSpawned = true;
			}
		}
		yield return CupheadTime.WaitForSeconds(this, delayNextProjectileWave);
		currentGattlingGunAttackString++;
		if (currentGattlingGunAttackString >= base.properties.CurrentState.gattlingGun.seedSpawnString.Length)
		{
			currentGattlingGunAttackString = 0;
		}
		AudioManager.Stop("flower_gattling_gun_loop");
		OnGattlingGunEnded();
	}

	public void OnMiniFlowerDeath()
	{
		miniFlowerSpawned = false;
	}

	private void SpawnEnemySeed(int xPos, char t, bool a = true)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(enemySeedPrefab);
		gameObject.transform.position = new Vector3(-600 + xPos, Level.Current.Height, 0f);
		gameObject.GetComponent<FlowerLevelEnemySeed>().OnSeedSpawn(base.properties, this, t, a);
	}

	private void SpawnBoomerang()
	{
		BasicProjectile proj = boomerangPrefab.GetComponent<FlowerLevelBoomerang>().Create(bottomProjectileSpawnPoint.position + (topProjectileSpawnPoint.position - bottomProjectileSpawnPoint.position) / 2f, 0f, 0f);
		StartCoroutine(spawnBoomerang_cr(proj));
	}

	private IEnumerator spawnBoomerang_cr(BasicProjectile proj)
	{
		proj.GetComponent<FlowerLevelBoomerang>().OnBoomerangStart(base.properties.CurrentState.boomerang.offScreenDelay);
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.boomerang.initialMovementDelay);
		proj.GetComponent<BasicProjectile>().Speed = -base.properties.CurrentState.boomerang.speed;
		OnPotHandsComplete();
	}

	private void SpawnBullets()
	{
		bulletSpawns.Clear();
		for (int i = 0; i < base.properties.CurrentState.bullets.numberOfProjectiles; i++)
		{
			bulletSpawns.Add(bulletSeedPrefab.GetComponent<FlowerLevelSeedBullet>().Create(Vector2.zero));
			Vector3 position = bottomProjectileSpawnPoint.position + (topProjectileSpawnPoint.position - bottomProjectileSpawnPoint.position) / (base.properties.CurrentState.bullets.numberOfProjectiles - 1) * i;
			bulletSpawns[bulletSpawns.Count - 1].transform.position = position;
		}
		StartCoroutine(spawnBullets_cr());
	}

	private IEnumerator spawnBullets_cr()
	{
		List<AbstractProjectile> bullets = new List<AbstractProjectile>();
		List<AbstractProjectile> activeBullets = bulletSpawns;
		for (int i = 0; i < base.properties.CurrentState.bullets.numberOfProjectiles; i++)
		{
			float delay = (float)base.properties.CurrentState.bullets.holdDelay / (float)base.properties.CurrentState.bullets.numberOfProjectiles;
			int rand = UnityEngine.Random.Range(0, activeBullets.Count);
			bullets.Add(activeBullets[rand]);
			bullets[i].GetComponent<FlowerLevelSeedBullet>().OnBulletSeedStart(this, PlayerManager.GetNext(), base.properties.CurrentState.bullets.acceleration, base.properties.CurrentState.bullets.speedMinMax.min, base.properties.CurrentState.bullets.speedMinMax.max, base.properties.CurrentState.bullets.delayNextShot);
			activeBullets.RemoveAt(rand);
			yield return CupheadTime.WaitForSeconds(this, delay);
		}
		yield return null;
		for (int j = 0; j < bullets.Count; j++)
		{
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.bullets.delayNextShot);
			yield return null;
			bullets[j].GetComponent<FlowerLevelSeedBullet>().LaunchBullet();
		}
		OnPotHandsComplete();
		yield return null;
	}

	private void SpawnCloudShot()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(cloudBombPrefab);
		Vector3 position = bottomProjectileSpawnPoint.position + (topProjectileSpawnPoint.position - bottomProjectileSpawnPoint.position) / 2f;
		gameObject.transform.position = position;
		gameObject.GetComponent<FlowerLevelCloudBomb>().OnCloudBombStart(PlayerManager.GetNext().center, base.properties.CurrentState.puffUp.speed, base.properties.CurrentState.puffUp.delayExplosion);
		OnPotHandsComplete();
	}

	private void PodHandsFX()
	{
		base.animator.Play("Twinkle", 2);
	}

	private void GattlingEndAudio()
	{
		AudioManager.Play("flower_gattling_gun_end");
		emitAudioFromObject.Add("flower_gattling_gun_end");
	}

	private void GattlingLoopAudio()
	{
		StartCoroutine(gattlingLoopEnd_cr());
	}

	private IEnumerator gattlingLoopEnd_cr()
	{
		yield return new WaitForEndOfFrame();
		AudioManager.PlayLoop("flower_gattling_gun_loop");
		emitAudioFromObject.Add("flower_gattling_gun_loop");
	}

	private void StopGattlingLoopAudio()
	{
		AudioManager.Stop("flower_gattling_gun_loop");
	}

	private void GattlingStartAudio()
	{
		AudioManager.Play("flower_gattling_gun_start");
		emitAudioFromObject.Add("flower_gattling_gun_start");
	}

	private void Phase1IntroAudio()
	{
		AudioManager.Play("flower_intro_yell");
		emitAudioFromObject.Add("flower_intro_yell");
	}

	private void Phase1_2TransitionAudio()
	{
		AudioManager.Play("flower_phase1_2_transition");
	}

	private void Phase2DeathAudio()
	{
	}

	private void PodHandsStartAudio()
	{
		AudioManager.Play("flower_pod_hands_start");
	}

	private void PodHandsOpenAudio()
	{
		AudioManager.Play("flower_pod_hands_open");
	}

	private void PodHandsCloseAudio()
	{
		AudioManager.Play("flower_pod_hands_end");
	}

	private void SpitStartAudio()
	{
		AudioManager.Play("flower_spit_start");
	}

	private void TopLaserAttackStartAudio()
	{
		AudioManager.Play("flower_top_laser_attack_start");
	}

	private void TopLaserAttackHoldAudio()
	{
		AudioManager.PlayLoop("flower_top_laser_attack_hold");
	}

	private void TopLaserAttackEndAudio()
	{
		AudioManager.Play("flower_top_laser_attack_end");
		AudioManager.Stop("flower_top_laser_attack_hold");
	}
}
