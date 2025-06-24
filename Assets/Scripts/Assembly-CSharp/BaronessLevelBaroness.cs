using UnityEngine;

public class BaronessLevelBaroness : AbstractCollidableObject
{
	[SerializeField]
	private Transform baronessTossPoint;

	[SerializeField]
	private Transform baronessProjectileShootPoint;

	[SerializeField]
	private BaronessLevelBaronessProjectileBunch baronessProjectileBunch;

	[SerializeField]
	private BaronessLevelFollowingProjectile baronessFollowProjectile;

	[SerializeField]
	public Transform shootPoint;

	private LevelProperties.Baroness properties;

	private BaronessLevelCastle parent;

	public bool isEasyFinal;

	public int shootCounter;

	public int popUpCounter;

	public int transformCounter;

	public bool shotEnough;

	private float health;

	private float maxHealth;

	private DamageReceiver damageReceiver;

	protected override void Awake()
	{
		base.Awake();
		isEasyFinal = false;
		damageReceiver = shootPoint.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (isEasyFinal)
		{
			properties.DealDamage(info.damage);
			if (properties.CurrentHealth <= 0f && isEasyFinal)
			{
				isEasyFinal = false;
			}
		}
		else if (health < 0f && !shotEnough)
		{
			shotEnough = true;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (shotEnough)
		{
			health = maxHealth;
		}
	}

	public void getProperties(LevelProperties.Baroness properties, float health, BaronessLevelCastle parent)
	{
		this.properties = properties;
		maxHealth = health;
		this.parent = parent;
		health = maxHealth;
	}

	public void ShootCounter()
	{
		FireProjectileBunch();
		shootCounter++;
	}

	public void PopUpCounter()
	{
		popUpCounter++;
	}

	public void TransformCounter()
	{
		transformCounter++;
	}

	private void FireProjectileBunch()
	{
		AudioManager.Play("level_baroness_gun_fire");
		AbstractPlayerController next = PlayerManager.GetNext();
		float x = next.transform.position.x - base.transform.position.x;
		float y = next.transform.position.y - base.transform.position.y;
		float pointAt = Mathf.Atan2(y, x) * 57.29578f;
		BaronessLevelBaronessProjectileBunch baronessLevelBaronessProjectileBunch = Object.Instantiate(baronessProjectileBunch);
		baronessLevelBaronessProjectileBunch.Init(baronessProjectileShootPoint.position, properties.CurrentState.baronessVonBonbon.projectileSpeed, pointAt, properties.CurrentState.baronessVonBonbon.projectileRotationSpeed, properties.CurrentState.baronessVonBonbon, parent);
	}

	public void FireFinalProjectile()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		BaronessLevelFollowingProjectile baronessLevelFollowingProjectile = Object.Instantiate(baronessFollowProjectile);
		baronessLevelFollowingProjectile.Init(baronessTossPoint.position, properties.CurrentState.baronessVonBonbon.finalProjectileSpeed, next.transform.position, properties.CurrentState.baronessVonBonbon, next, parent);
	}

	private void SoundVoiceAngry()
	{
		AudioManager.Play("level_baroness_voice_angry");
		emitAudioFromObject.Add("level_baroness_voice_angry");
	}

	private void SoundVoiceEffort()
	{
		AudioManager.Play("level_baroness_voice_effort");
		emitAudioFromObject.Add("level_baroness_voice_effort");
	}

	private void SoundVoiceCastleyank()
	{
		AudioManager.Play("level_baroness_voice_castleyank");
		emitAudioFromObject.Add("level_baroness_voice_castleyank");
	}

	private void SoundVoiceIntroA()
	{
		AudioManager.Play("level_baroness_voice_intro_a");
		emitAudioFromObject.Add("level_baroness_voice_intro_a");
	}

	private void SoundVoiceIntroB()
	{
		AudioManager.Play("level_baroness_voice_intro_b");
		emitAudioFromObject.Add("level_baroness_voice_intro_b");
	}
}
