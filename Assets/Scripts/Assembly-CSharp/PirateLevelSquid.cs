using System.Collections;
using UnityEngine;

public class PirateLevelSquid : LevelProperties.Pirate.Entity
{
	public enum State
	{
		Init = 0,
		Enter = 1,
		Attack = 2,
		Exit = 3,
		Die = 4
	}

	private static readonly Vector2 START_POS = new Vector2(-200f, -220f);

	private const float BOB_OFFSET = -20f;

	[SerializeField]
	private Transform inkOrigin;

	[SerializeField]
	private PirateLevelSquidProjectile inkBlob;

	[SerializeField]
	private Effect splashPrefab;

	private float hp;

	private float startY;

	private float endY;

	private float bobTime;

	private float attackTime;

	private LevelProperties.Pirate.Squid squid;

	private new Animator animator;

	private bool InkAttackSoundActive;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		base.transform.position = START_POS;
		animator = GetComponent<Animator>();
		GetComponent<DamageReceiver>().OnDamageTaken += onDamageTaken;
		GetComponent<Collider2D>().enabled = false;
	}

	protected override void Update()
	{
		base.Update();
		if (state == State.Attack)
		{
			if (attackTime > squid.maxTime)
			{
				Exit();
			}
			else
			{
				attackTime += CupheadTime.Delta;
			}
		}
		float t = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, Mathf.PingPong(bobTime, 1f));
		base.transform.SetPosition(null, Mathf.Lerp(startY, endY, t));
		bobTime += CupheadTime.Delta;
	}

	public override void LevelInit(LevelProperties.Pirate properties)
	{
		base.LevelInit(properties);
		squid = properties.CurrentState.squid;
		float value = squid.xPos.RandomFloat();
		base.transform.SetPosition(value);
		splashPrefab.Create(base.transform.position + new Vector3(0f, -40f, 0f));
		AudioManager.Play("level_pirate_squid_splash");
		hp = squid.hp;
		startY = base.transform.position.y;
		endY = startY + -20f;
		state = State.Enter;
		AudioManager.Play("level_pirate_squid_enter");
		properties.OnBossDeath += OnBossDeath;
	}

	private void PlayPopSFX()
	{
		AudioManager.Play("level_pirate_squid_attack_pop");
	}

	private void onDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp <= 0f)
		{
			Die();
		}
	}

	private void Exit()
	{
		GetComponent<Collider2D>().enabled = false;
		state = State.Exit;
		AudioManager.Play("level_pirate_squid_exit");
		animator.SetTrigger("OnExit");
		base.properties.OnBossDeath -= OnBossDeath;
	}

	public void Die()
	{
		GetComponent<Collider2D>().enabled = false;
		state = State.Die;
		AudioManager.Play("level_pirate_squid_death");
		animator.SetTrigger("OnDeath");
		base.properties.OnBossDeath -= OnBossDeath;
	}

	private void OnBossDeath()
	{
		Die();
	}

	private IEnumerator attack_cr()
	{
		if (!InkAttackSoundActive)
		{
			AudioManager.PlayLoop("level_pirate_squid_attack_loop");
			InkAttackSoundActive = true;
		}
		while (state == State.Attack)
		{
			Vector2 v = Vector2.zero;
			v.y = squid.blobVelY;
			v.x = squid.blobVelX;
			inkBlob.Create(inkOrigin.position, v, squid.blobGravity);
			yield return CupheadTime.WaitForSeconds(this, squid.blobDelay);
		}
		AudioManager.Stop("level_pirate_squid_attack_loop");
		InkAttackSoundActive = false;
	}

	private void OnEnterAnimationComplete()
	{
		GetComponent<Collider2D>().enabled = true;
		state = State.Attack;
		attackTime = 0f;
		StartCoroutine(attack_cr());
	}

	private void OnExitAnimationComplete()
	{
		Object.Destroy(base.gameObject);
	}
}
