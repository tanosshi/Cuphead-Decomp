using System.Collections;
using UnityEngine;

public class FrogsLevelMorphed : LevelProperties.Frogs.Entity
{
	public static FrogsLevelMorphed Current;

	[SerializeField]
	private FrogsLevelMorphedCoin coin;

	[SerializeField]
	private Transform coinRoot;

	[Space(10f)]
	public Transform switchRoot;

	[Space(10f)]
	[SerializeField]
	private GameObject slotsParent;

	[SerializeField]
	private Slots slots;

	[Space(10f)]
	[SerializeField]
	private FrogsLevelSnakeBullet snakeBullet;

	[SerializeField]
	private FrogsLevelBisonBullet bisonBullet;

	[SerializeField]
	private FrogsLevelTigerBullet tigerBullet;

	[SerializeField]
	private FrogsLevelOniBullet oniBullet;

	[SerializeField]
	private Transform slotBulletRoot;

	[Space(10f)]
	[SerializeField]
	private Effect dustEffect;

	private new Animator animator;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	private FrogsLevelMorphedSwitch handle;

	private bool demonTriggered;

	private int mainIndex;

	private int index;

	private bool handleActivated;

	private IEnumerator shootingCoroutine;

	private float coinSpeed;

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		animator = GetComponent<Animator>();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageDealer = new DamageDealer(1f, 0.3f, DamageDealer.DamageSource.Enemy, true, false, false);
		slots.Init(this);
		handle = FrogsLevelMorphedSwitch.Create(this);
		handle.enabled = false;
		handle.OnActivate += OnHandleActivated;
		slotsParent.SetActive(false);
		base.gameObject.SetActive(false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Start()
	{
		base.Start();
		damageReceiver.enabled = false;
	}

	protected override void Update()
	{
		base.Update();
		damageDealer.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		AudioManager.Play("level_frogs_short_clap_shock");
		emitAudioFromObject.Add("level_frogs_short_clap_shock");
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public override void LevelInit(LevelProperties.Frogs properties)
	{
		base.LevelInit(properties);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public void Enable(bool demonTriggered)
	{
		this.demonTriggered = demonTriggered;
		base.gameObject.SetActive(true);
		dustEffect.gameObject.SetActive(true);
		base.properties.OnBossDeath += OnBossDeath;
		GetComponent<LevelBossDeathExploder>().enabled = true;
		StartCoroutine(loop_cr());
	}

	private void OnBossDeath()
	{
		AudioManager.PlayLoop("level_frogs_morphed_death_loop");
		emitAudioFromObject.Add("level_frogs_morphed_death_loop");
		StopAllCoroutines();
		animator.SetTrigger("OnDeath");
		slotsParent.SetActive(false);
	}

	private void ShootCoin()
	{
		AudioManager.Play("level_frogs_morphed_mouth");
		emitAudioFromObject.Add("level_frogs_morphed_mouth");
		coinRoot.LookAt2D(PlayerManager.GetNext().center);
		FrogsLevelMorphedCoin frogsLevelMorphedCoin = coin.CreateCoin(coinRoot.position, coinSpeed, coinRoot.eulerAngles.z);
		frogsLevelMorphedCoin.transform.SetPosition(null, null, -600f);
	}

	private void StartShooting()
	{
		EndShooting();
		shootingCoroutine = shootingLoop_cr();
		StartCoroutine(shootingCoroutine);
	}

	private void EndShooting()
	{
		if (shootingCoroutine != null)
		{
			StopCoroutine(shootingCoroutine);
		}
	}

	private void OnHandleActivated()
	{
		handleActivated = true;
	}

	private IEnumerator loop_cr()
	{
		if (demonTriggered)
		{
			mainIndex = Random.Range(0, base.properties.CurrentState.demon.demonString.Length);
			index = Random.Range(0, base.properties.CurrentState.demon.demonString[mainIndex].Split(',').Length);
		}
		AudioManager.Play("level_frogs_morphed_open");
		emitAudioFromObject.Add("level_frogs_morphed_open");
		slotsParent.SetActive(true);
		yield return CupheadTime.WaitForSeconds(this, 1f);
		animator.Play("Open");
		AudioManager.Play("level_frogs_morphed_open");
		emitAudioFromObject.Add("level_frogs_morphed_open");
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			LevelProperties.Frogs.Morph p = base.properties.CurrentState.morph;
			StartShooting();
			yield return CupheadTime.WaitForSeconds(this, p.armDownDelay);
			yield return StartCoroutine(waitForActivate_cr());
			EndShooting();
			animator.SetTrigger("OnActivated");
			yield return StartCoroutine(pattern_cr(p));
			slotsParent.SetActive(true);
			animator.SetTrigger("OnAttackEnd");
			yield return animator.WaitForAnimationToEnd(this, "Attack_End");
		}
	}

	private IEnumerator waitForActivate_cr()
	{
		handleActivated = false;
		handle.enabled = true;
		animator.SetTrigger("OnArmDown");
		AudioManager.Play("level_frogs_morphed_arm_down");
		emitAudioFromObject.Add("level_frogs_morphed_arm_down");
		while (!handleActivated)
		{
			yield return null;
		}
		handle.enabled = false;
	}

	private IEnumerator shootingLoop_cr()
	{
		LevelProperties.Frogs.Morph p = base.properties.CurrentState.morph;
		float time = p.coinMinMaxTime;
		float t = 0f;
		float val = 0f;
		float coinDelay = 0f;
		while (true)
		{
			float delay = p.coinDelay.GetFloatAt(val);
			coinSpeed = p.coinSpeed.GetFloatAt(val);
			if (coinDelay >= delay)
			{
				animator.SetTrigger("OnShoot");
				coinDelay = 0f;
			}
			if (val < 1f)
			{
				val = t / time;
				t += (float)CupheadTime.Delta;
			}
			else
			{
				val = 1f;
			}
			coinDelay += (float)CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator pattern_cr(LevelProperties.Frogs.Morph p)
	{
		Slots.Mode mode = Slots.Mode.Snake;
		if (!demonTriggered)
		{
			int num = Random.Range(0, 3);
			Debug.Log("Mode: " + num);
			mode = (Slots.Mode)num;
		}
		else
		{
			mode = Slots.Mode.Oni;
			yield return null;
		}
		slots.Spin();
		yield return CupheadTime.WaitForSeconds(this, 3f * p.slotSelectionDurationPercentage);
		slots.Stop(mode);
		yield return CupheadTime.WaitForSeconds(this, 1f * p.slotSelectionDurationPercentage);
		slots.StartFlash();
		yield return CupheadTime.WaitForSeconds(this, 0.8f * p.slotSelectionDurationPercentage);
		slots.StartFlash();
		yield return CupheadTime.WaitForSeconds(this, 0.8f * p.slotSelectionDurationPercentage);
		slots.StartFlash();
		yield return CupheadTime.WaitForSeconds(this, 0.8f * p.slotSelectionDurationPercentage);
		damageReceiver.enabled = true;
		animator.SetTrigger("OnAttack");
		AudioManager.Play("level_frogs_morphed_attack");
		emitAudioFromObject.Add("level_frogs_morphed_attack");
		yield return animator.WaitForAnimationToEnd(this, "Attack");
		slotsParent.SetActive(false);
		AudioManager.PlayLoop("level_frogs_platform_loop");
		emitAudioFromObject.Add("level_frogs_platform_loop");
		switch (mode)
		{
		case Slots.Mode.Snake:
			yield return StartCoroutine(snake_cr(p));
			break;
		case Slots.Mode.Bison:
			yield return StartCoroutine(bison_cr(p));
			break;
		case Slots.Mode.Tiger:
			yield return StartCoroutine(tiger_cr(p));
			break;
		case Slots.Mode.Oni:
			yield return StartCoroutine(oni_cr());
			break;
		}
		AudioManager.Stop("level_frogs_platform_loop");
		damageReceiver.enabled = false;
	}

	private void ShootSnake(float speed)
	{
		snakeBullet.Create(slotBulletRoot.position, speed);
	}

	private void ShootBison(float speed, FrogsLevelBisonBullet.Direction dir, float bigX, float smallX)
	{
		bisonBullet.Create(slotBulletRoot.position, speed, dir, bigX, smallX);
	}

	private void ShootTiger(float speed)
	{
		tigerBullet.Create(slotBulletRoot.position, speed);
	}

	private void ShootOni(float speed)
	{
		oniBullet.Create(slotBulletRoot.position, speed, base.properties.CurrentState.demon);
	}

	private IEnumerator snake_cr(LevelProperties.Frogs.Morph p)
	{
		float t = 0f;
		float time = p.snakeDuration;
		float val = 0f;
		float bulletDelay = 1000f;
		float bulletSpeed = 0f;
		float delay = 0f;
		while (t < time)
		{
			if (bulletDelay >= delay)
			{
				bulletSpeed = p.snakeSpeed.GetFloatAt(val);
				ShootSnake(bulletSpeed);
				bulletDelay = 0f;
			}
			delay = p.snakeDelay.GetFloatAt(val);
			bulletDelay += (float)CupheadTime.Delta;
			if (val < 1f)
			{
				val = t / time;
				t += (float)CupheadTime.Delta;
			}
			else
			{
				val = 1f;
			}
			yield return null;
		}
	}

	private IEnumerator bison_cr(LevelProperties.Frogs.Morph p)
	{
		float t = 0f;
		float time = p.bisonDuration;
		float val = 0f;
		float bulletDelay = 10000f;
		float bulletSpeed = 0f;
		float delay = 0f;
		int sameDirCount = 0;
		FrogsLevelBisonBullet.Direction lastDir = FrogsLevelBisonBullet.Direction.Down;
		FrogsLevelBisonBullet.Direction dir = FrogsLevelBisonBullet.Direction.Up;
		while (t < time)
		{
			if (bulletDelay >= delay)
			{
				bulletSpeed = p.bisonSpeed.GetFloatAt(val);
				ShootBison(bulletSpeed, dir, p.bisonBigX, p.bisonSmallX);
				bulletDelay = 0f;
				lastDir = dir;
				dir = (FrogsLevelBisonBullet.Direction)Random.Range(0, 2);
				sameDirCount = ((lastDir == dir) ? (sameDirCount + 1) : 0);
				if (sameDirCount >= 3)
				{
					dir = ((dir == FrogsLevelBisonBullet.Direction.Up) ? FrogsLevelBisonBullet.Direction.Down : FrogsLevelBisonBullet.Direction.Up);
					sameDirCount = 0;
				}
			}
			delay = p.bisonDelay.GetFloatAt(val);
			bulletDelay += (float)CupheadTime.Delta;
			if (val < 1f)
			{
				val = t / time;
				t += (float)CupheadTime.Delta;
			}
			else
			{
				val = 1f;
			}
			yield return null;
		}
	}

	private IEnumerator tiger_cr(LevelProperties.Frogs.Morph p)
	{
		float t = 0f;
		float time = p.tigerDuration;
		float val = 0f;
		float bulletDelay = 1000f;
		float bulletSpeed = 0f;
		float delay = 0f;
		while (t < time)
		{
			if (bulletDelay >= delay)
			{
				bulletSpeed = p.tigerSpeed;
				ShootTiger(bulletSpeed);
				bulletDelay = 0f;
			}
			delay = p.tigerDelay.GetFloatAt(val);
			bulletDelay += (float)CupheadTime.Delta;
			if (val < 1f)
			{
				val = t / time;
				t += (float)CupheadTime.Delta;
			}
			else
			{
				val = 1f;
			}
			yield return null;
		}
	}

	private IEnumerator oni_cr()
	{
		LevelProperties.Frogs.Demon p = base.properties.CurrentState.demon;
		float bulletSpeed = 0f;
		float bulletDelay = 1000f;
		float delay = 0f;
		float val = 0f;
		float time = p.demonMaxTime;
		float t = 0f;
		while (true)
		{
			FrogsLevelBisonBullet.Direction dir = (FrogsLevelBisonBullet.Direction)Random.Range(0, 2);
			string[] demonPattern = p.demonString[mainIndex].Split(',');
			if (bulletDelay >= delay)
			{
				demonPattern = p.demonString[mainIndex].Split(',');
				bulletSpeed = p.demonSpeed.GetFloatAt(val);
				switch (demonPattern[index][0])
				{
				case 'S':
					ShootSnake(bulletSpeed);
					break;
				case 'T':
					ShootTiger(bulletSpeed);
					break;
				case 'B':
					dir = (FrogsLevelBisonBullet.Direction)Random.Range(0, 2);
					ShootBison(bulletSpeed, dir, base.properties.CurrentState.morph.bisonBigX, base.properties.CurrentState.morph.bisonSmallX);
					if (dir == FrogsLevelBisonBullet.Direction.Up)
					{
						dir = FrogsLevelBisonBullet.Direction.Down;
					}
					else
					{
						dir = FrogsLevelBisonBullet.Direction.Up;
					}
					break;
				case 'O':
					ShootOni(bulletSpeed);
					break;
				}
				if (index < demonPattern.Length - 1)
				{
					index++;
				}
				else
				{
					mainIndex = (mainIndex + 1) % p.demonString.Length;
					index = 0;
				}
				bulletDelay = 0f;
			}
			delay = p.demonDelay.GetFloatAt(val);
			bulletDelay += (float)CupheadTime.Delta;
			if (val < 1f)
			{
				val = t / time;
				t += (float)CupheadTime.Delta;
			}
			else
			{
				val = 1f;
			}
			yield return null;
		}
	}
}
