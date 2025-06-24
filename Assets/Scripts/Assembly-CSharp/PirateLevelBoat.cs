using System;
using System.Collections;
using UnityEngine;

public class PirateLevelBoat : LevelProperties.Pirate.Entity
{
	public class IdleManager
	{
		private const int MIN_LOOPS = 20;

		private const int MAX_LOOPS = 60;

		public int loops;

		public int max = 20;

		public void OnBlink()
		{
			max = UnityEngine.Random.Range(20, 61);
			loops = 0;
		}
	}

	[SerializeField]
	private DamageReceiver damageReceiver;

	[Space(10f)]
	[SerializeField]
	private Transform cannonRoot;

	[SerializeField]
	private BasicProjectile cannonProjectile;

	[SerializeField]
	private Effect cannonSmokePrefab;

	[Space(10f)]
	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private Transform beamRoot;

	[SerializeField]
	private PirateLevelBoatProjectile projectilePrefab;

	[SerializeField]
	private PirateLevelBoatBeam beamPrefab;

	[Space(10f)]
	[SerializeField]
	private SpriteRenderer ully;

	private IdleManager idle;

	private new Animator animator;

	private bool hasTransformed;

	private PirateLevelBoatBeam beam;

	private const float Y_TRANSFORMED = 70f;

	private LevelProperties.Pirate.Boat boatProperties;

	public event Action OnLaunchPirate;

	protected override void Awake()
	{
		base.Awake();
		idle = new IdleManager();
		animator = GetComponent<Animator>();
		ully.gameObject.SetActive(false);
		GetComponent<LevelBossDeathExploder>().enabled = false;
	}

	public override void LevelInit(LevelProperties.Pirate properties)
	{
		base.LevelInit(properties);
		properties.OnStateChange += OnStateChange;
		properties.OnBossDeath += OnBossDeath;
	}

	private void OnStateChange()
	{
		animator.Play("Idle");
		StopAllCoroutines();
		if (base.properties.CurrentState.cannon.firing)
		{
			StartCoroutine(cannon_cr(base.properties.CurrentState.cannon.delayRange.RandomFloat()));
		}
	}

	private void OnIdleEnd()
	{
		if (idle.loops >= idle.max)
		{
			animator.SetTrigger("OnBlink");
		}
		else
		{
			idle.loops++;
		}
	}

	private void OnBlink()
	{
		idle.OnBlink();
	}

	private void FireCannon()
	{
		AudioManager.Play("level_pirate_ship_cannon_fire");
		if (base.properties.CurrentState.cannon.firing)
		{
			cannonSmokePrefab.Create(new Vector2(cannonRoot.position.x + 50f, cannonRoot.position.y));
			BasicProjectile basicProjectile = cannonProjectile.Create(cannonRoot.position, 0f, 0f - base.properties.CurrentState.cannon.speed);
			basicProjectile.CollisionDeath.None();
			basicProjectile.DamagesType.OnlyPlayer();
		}
	}

	private IEnumerator cannon_cr(float delay)
	{
		if (delay < 1f)
		{
			delay = 1f;
		}
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, delay);
			animator.Play("Cannon");
		}
	}

	private void ChewSound()
	{
		AudioManager.Play("level_pirate_ship_cannon_chew");
		emitAudioFromObject.Add("level_pirate_ship_cannon_chew");
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public void StartTransformation()
	{
		damageReceiver.OnDamageTaken += OnDamageTaken;
		GetComponent<LevelBossDeathExploder>().enabled = true;
		hasTransformed = true;
		StopAllCoroutines();
		StartCoroutine(transform_cr());
	}

	private void LaunchPirate()
	{
		CupheadLevelCamera.Current.Shake(15f, 1f);
		if (this.OnLaunchPirate != null)
		{
			this.OnLaunchPirate();
		}
		SetNewHeight();
	}

	private void Shoot()
	{
		AudioManager.Play("level_pirate_ship_uvula_shoot");
		emitAudioFromObject.Add("level_pirate_ship_uvula_shoot");
		projectilePrefab.Create(projectileRoot.position, boatProperties.bulletSpeed, boatProperties.bulletRotationSpeed);
	}

	private void SetNewHeight()
	{
		UnityEngine.Object.FindObjectOfType<PirateLevelBoatContainer>().EndBobbing();
		base.transform.parent.SetLocalPosition(null, 70f);
	}

	private void OnBossDeath()
	{
		StopAllCoroutines();
		CupheadLevelCamera.Current.ResetShake();
		if (hasTransformed)
		{
			if (beam != null)
			{
				beam.EndBeam();
			}
			animator.SetTrigger("OnDeath");
		}
		else
		{
			animator.SetTrigger("OnEasyDeath");
		}
	}

	private IEnumerator transform_cr()
	{
		boatProperties = base.properties.CurrentState.boat;
		animator.Play("Idle");
		yield return CupheadTime.WaitForSeconds(this, 0.3f);
		animator.SetTrigger("OnTransform");
		AudioManager.Play("level_pirate_boat_transform");
		emitAudioFromObject.Add("level_pirate_boat_transform");
		yield return CupheadTime.WaitForSeconds(this, boatProperties.winceDuration);
		animator.SetTrigger("OnTransformContinue");
		yield return CupheadTime.WaitForSeconds(this, 1f);
		ully.gameObject.SetActive(true);
		while (true)
		{
			for (int count = 0; count < boatProperties.bulletCount; count++)
			{
				yield return CupheadTime.WaitForSeconds(this, boatProperties.attackDelay);
				animator.SetTrigger("OnShoot");
			}
			yield return CupheadTime.WaitForSeconds(this, boatProperties.bulletPostWait);
			animator.SetTrigger("OnBeamStart");
			yield return CupheadTime.WaitForSeconds(this, boatProperties.beamDelay + 1f);
			animator.SetTrigger("OnBeamContinue");
			CupheadLevelCamera.Current.StartShake(2f);
			beam = beamPrefab.Create(beamRoot);
			yield return CupheadTime.WaitForSeconds(this, boatProperties.beamDuration);
			animator.SetTrigger("OnBeamEnd");
			CupheadLevelCamera.Current.EndShake(0.4f);
			beam.EndBeam();
			beam = null;
			yield return CupheadTime.WaitForSeconds(this, boatProperties.beamPostWait);
			animator.Play("Transform_Idle");
		}
	}

	private IEnumerator delay_cr(int frameDelay)
	{
		for (int i = 0; i < frameDelay; i++)
		{
			yield return null;
		}
		StartCoroutine(transform_cr());
	}
}
