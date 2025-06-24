using System.Collections;
using UnityEngine;

public class PirateLevelDogFish : AbstractProjectile
{
	public enum State
	{
		Init = 0,
		Jump = 1,
		Slide = 2,
		Death = 3
	}

	private static readonly Vector2 START_POS = new Vector2(235f, -245f);

	private const float DEATH_Y = 450f;

	[SerializeField]
	private Collider2D secretHitBox;

	[SerializeField]
	private Collider2D normalHitBox;

	[SerializeField]
	private Effect splashEffect;

	[SerializeField]
	private Transform splashRoot;

	[SerializeField]
	private Effect deathEffect;

	private State state;

	private float hp;

	private float speedY;

	private float slideTime;

	private LevelProperties.Pirate properties;

	private LevelProperties.Pirate.DogFish dogfish;

	private bool bossDied;

	private bool isSecret;

	public static bool dogKilled = false;

	protected override void Awake()
	{
		base.Awake();
		base.transform.position = START_POS;
		animator = GetComponent<Animator>();
		normalHitBox.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
		normalHitBox.GetComponent<DamageReceiver>().OnDamageTaken += onDamageTaken;
		secretHitBox.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTakenFromBehind;
	}

	protected override void Update()
	{
		base.Update();
		if (state == State.Slide)
		{
			base.transform.AddPosition((0f - speedY) * (float)CupheadTime.Delta);
			float num = slideTime / dogfish.speedFalloffTime;
			if (num < 1f)
			{
				speedY = EaseUtils.EaseOutQuart(dogfish.startSpeed, dogfish.endSpeed, num);
				slideTime += CupheadTime.Delta;
			}
			else
			{
				speedY = dogfish.endSpeed;
			}
			if (base.transform.position.x < -1000f)
			{
				properties.OnBossDeath -= OnBossDeath;
				Object.Destroy(base.gameObject);
			}
			if (bossDied)
			{
				Die();
			}
			if (dogKilled && isSecret)
			{
				isSecret = false;
				OnEnableCollider();
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (state != State.Death)
		{
			base.OnCollisionPlayer(hit, phase);
			base.damageDealer.DealDamage(hit);
		}
	}

	public void Init(LevelProperties.Pirate properties, bool isSecret)
	{
		this.properties = properties;
		dogfish = properties.CurrentState.dogFish;
		this.isSecret = isSecret;
		hp = dogfish.hp;
		state = State.Jump;
		normalHitBox.GetComponent<DamageReceiver>().enabled = false;
		AudioManager.Play("level_pirate_dogfish_jump");
		emitAudioFromObject.Add("level_pirate_dogfish_jump");
		splashEffect.Create(splashRoot.position);
		properties.OnBossDeath += OnBossDeath;
	}

	private void onDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if ((hp < 0f) & (state != State.Death))
		{
			OnDying();
			secretHitBox.GetComponent<Collider2D>().enabled = false;
			Die();
		}
	}

	private void OnDamageTakenFromBehind(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp < 0f && state != State.Death)
		{
			OnDying();
			SetParryable(true);
			Die();
		}
	}

	private void OnDying()
	{
		AudioManager.Stop("level_pirate_dogfish_jump");
		AudioManager.Play("level_pirate_dogfish_death_poof");
		emitAudioFromObject.Add("level_pirate_dogfish_death_poof");
		dogKilled = true;
		normalHitBox.GetComponent<Collider2D>().enabled = false;
		secretHitBox.GetComponent<DamageReceiver>().enabled = false;
	}

	private void OnEnableCollider()
	{
		normalHitBox.GetComponent<DamageReceiver>().enabled = true;
		secretHitBox.GetComponent<DamageReceiver>().enabled = false;
		base.gameObject.layer = 0;
	}

	private void OnJumpAnimationComplete()
	{
		Debug.Log("JUMP COMPLETE");
		if (state != State.Death)
		{
			state = State.Slide;
			AudioManager.Play("level_pirate_dogfish_slide");
			emitAudioFromObject.Add("level_pirate_dogfish_slide");
			slideTime = 0f;
			speedY = dogfish.startSpeed;
		}
	}

	protected override void Die()
	{
		state = State.Death;
		properties.OnBossDeath -= OnBossDeath;
		animator.SetTrigger("OnDeath");
		deathEffect.Create(base.transform.position);
		StartCoroutine(deathFloat_cr());
	}

	private void OnBossDeath()
	{
		bossDied = true;
		if (state == State.Slide)
		{
			Die();
		}
	}

	private IEnumerator deathFloat_cr()
	{
		AudioManager.Play("level_pirate_dogfish_death_flap");
		emitAudioFromObject.Add("level_pirate_dogfish_death_flap");
		while (base.transform.position.y < 360f)
		{
			base.transform.AddPosition(0f, properties.CurrentState.dogFish.deathSpeed * (float)CupheadTime.Delta);
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
