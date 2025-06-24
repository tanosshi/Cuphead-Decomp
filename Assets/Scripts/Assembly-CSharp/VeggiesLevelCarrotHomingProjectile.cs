using System.Collections;
using UnityEngine;

public class VeggiesLevelCarrotHomingProjectile : HomingProjectile
{
	public enum State
	{
		In = 0,
		InComplete = 1,
		Firing = 2,
		Dead = 3
	}

	public const float SCALE_RAND = 0.1f;

	[SerializeField]
	private Transform hitBox;

	private VeggiesLevelCarrot parent;

	private float health;

	protected override float DestroyLifetime
	{
		get
		{
			return 1000f;
		}
	}

	public State state { get; private set; }

	public VeggiesLevelCarrotHomingProjectile Create(AbstractPlayerController player, VeggiesLevelCarrot parent, Vector2 pos, float speed, float rotationSpeed, float health)
	{
		VeggiesLevelCarrotHomingProjectile veggiesLevelCarrotHomingProjectile = Create(pos, -90f, speed, speed, rotationSpeed, DestroyLifetime, 0f, player) as VeggiesLevelCarrotHomingProjectile;
		veggiesLevelCarrotHomingProjectile.CollisionDeath.OnlyPlayer();
		veggiesLevelCarrotHomingProjectile.DamagesType.OnlyPlayer();
		veggiesLevelCarrotHomingProjectile.Init(parent, health);
		return veggiesLevelCarrotHomingProjectile;
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		UpdateHitBox();
	}

	private void UpdateHitBox()
	{
		hitBox.position = base.transform.position;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		parent.OnDeathEvent -= OnDeath;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		Die();
	}

	protected override void Die()
	{
		base.Die();
		animator.SetTrigger("OnDeath");
		GetComponent<Collider2D>().enabled = false;
		hitBox.gameObject.SetActive(false);
		StopAllCoroutines();
		base.transform.SetEulerAngles(0f, 0f, -90f);
	}

	private void Init(VeggiesLevelCarrot parent, float health)
	{
		hitBox.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		this.parent = parent;
		this.health = health;
		parent.OnDeathEvent += OnDeath;
		base.transform.localScale = Vector3.one * (1f + Random.Range(0.1f, -0.1f));
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f && state != State.Dead)
		{
			state = State.Dead;
			AudioManager.Play("level_veggies_carrot_projectile_death");
			emitAudioFromObject.Add("level_veggies_carrot_projectile_death");
			StartCoroutine(dying_cr());
		}
	}

	private IEnumerator dying_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.1f);
		Die();
		yield return null;
	}

	private void OnDeath()
	{
		hitBox.gameObject.SetActive(false);
		GetComponent<Collider2D>().enabled = false;
		StartCoroutine(parentDied_cr());
	}

	private void End()
	{
		Die();
		StopAllCoroutines();
	}

	private IEnumerator parentDied_cr()
	{
		GetComponent<Collider2D>().enabled = false;
		yield return CupheadTime.WaitForSeconds(this, Random.Range(0f, 0.5f));
		End();
	}
}
