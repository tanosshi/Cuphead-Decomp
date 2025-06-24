using UnityEngine;

public class PirateLevelPirate : LevelProperties.Pirate.Entity
{
	private const int MIN_IDLE_LOOPS = 2;

	private const int MAX_IDLE_LOOPS = 4;

	[SerializeField]
	private Transform gunRoot;

	[SerializeField]
	private BasicProjectile gunProjectile;

	[SerializeField]
	private BasicProjectile gunProjectileRegular;

	[SerializeField]
	private Effect muzzleFlash;

	[SerializeField]
	private Transform whistleRoot;

	[SerializeField]
	private Effect whistleEffect;

	private LevelProperties.Pirate.Peashot gunProperties;

	private new Animator animator;

	private PirateLevel.Creature creature;

	private int whistles;

	private int patchChance = 25;

	private int bothChance = 15;

	private int loops;

	private int max = 2;

	private int shotIndex;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		PirateLevel pirateLevel = Level.Current as PirateLevel;
		pirateLevel.OnWhistleEvent += onWhistle;
	}

	public override void LevelInit(LevelProperties.Pirate properties)
	{
		base.LevelInit(properties);
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		Level.Current.OnIntroEvent += OnIntroLaugh;
		properties.OnBossDeath += OnBossDeath;
	}

	private void OnIntroLaugh()
	{
		animator.SetTrigger("OnLaugh");
	}

	private void onWhistle(PirateLevel.Creature creature)
	{
		whistles = 0;
		this.creature = creature;
		animator.SetTrigger("OnWhistle");
		loops = 1000;
	}

	private void OnIdleEnd()
	{
		if (loops >= max)
		{
			int num = Random.Range(0, 100);
			int value = 0;
			if (num <= bothChance)
			{
				value = 2;
			}
			else if (num <= patchChance + bothChance)
			{
				value = 1;
			}
			animator.SetInteger("Blink", value);
			animator.SetTrigger("OnBlink");
		}
		else
		{
			loops++;
		}
	}

	private void OnBlink()
	{
		max = Random.Range(2, 5);
		loops = 0;
	}

	private void OnBossDeath()
	{
		StopAllCoroutines();
		animator.SetTrigger("OnDeath");
		AudioManager.Play("level_pirate_fall_death");
	}

	public void FireGun(LevelProperties.Pirate.Peashot properties)
	{
		animator.Play("Gun_Shoot");
	}

	private void Whistle()
	{
		int num = 1;
		switch (creature)
		{
		case PirateLevel.Creature.DogFish:
			num = 2;
			break;
		case PirateLevel.Creature.Shark:
			num = 3;
			break;
		}
		if (whistles < num)
		{
			whistleEffect.Create(whistleRoot.position);
			whistles++;
		}
	}

	private void WhistleSFX()
	{
		AudioManager.Play("levels_pirate_whistle");
		emitAudioFromObject.Add("levels_pirate_whistle");
	}

	public void EndGun()
	{
		animator.SetTrigger("OnGunEnd");
	}

	private void PlayLaughSound()
	{
		AudioManager.Play("levels_pirate_laugh");
		emitAudioFromObject.Add("levels_pirate_laugh");
	}

	public void StartGun()
	{
		animator.SetTrigger("OnGunStart");
		gunProperties = base.properties.CurrentState.peashot;
		shotIndex = Random.Range(0, gunProperties.shotType.Split(',').Length);
	}

	private void Shoot()
	{
		if (PlayerManager.Count <= 0)
		{
			gunRoot.LookAt2D(new Vector2(0f, 0f));
			return;
		}
		gunRoot.LookAt2D(PlayerManager.GetNext().center);
		AudioManager.Play("level_pirate_gun_shoot");
		emitAudioFromObject.Add("level_pirate_gun_shoot");
		muzzleFlash.Create(gunRoot.position);
		BasicProjectile basicProjectile = null;
		if (gunProperties.shotType.Split(',')[shotIndex][0] == 'P')
		{
			basicProjectile = gunProjectile.Create(gunRoot.position, gunRoot.eulerAngles.z, new Vector3(-1f, -1f, 1f), gunProperties.speed);
			basicProjectile.SetParryable(true);
		}
		else if (gunProperties.shotType.Split(',')[shotIndex][0] == 'R')
		{
			basicProjectile = gunProjectileRegular.Create(gunRoot.position, gunRoot.eulerAngles.z, new Vector3(-1f, -1f, 1f), gunProperties.speed);
		}
		basicProjectile.CollisionDeath.OnlyBounds();
		basicProjectile.CollisionDeath.Player = true;
		shotIndex = (shotIndex + 1) % gunProperties.shotType.Split(',').Length;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (base.properties.CurrentState.stateName != LevelProperties.Pirate.States.Boat)
		{
			base.properties.DealDamage(info.damage);
		}
	}

	public void CleanUp()
	{
		base.properties.OnBossDeath -= OnBossDeath;
		Object.Destroy(base.gameObject);
	}

	private void SoundGunStart()
	{
		AudioManager.Play("level_pirate_gun_start");
		emitAudioFromObject.Add("level_pirate_gun_start");
	}

	private void SoundGunEnd()
	{
		AudioManager.Play("level_pirate_gun_end");
		emitAudioFromObject.Add("level_pirate_gun_end");
	}

	private void SoundPirateFoot()
	{
		AudioManager.Play("level_pirate_pirate_foot");
		emitAudioFromObject.Add("level_pirate_pirate_foot");
	}
}
