using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelArrowProjectile : HomingProjectile
{
	[SerializeField]
	private Effect trailPrefab;

	private float speed;

	private float health;

	private float timeToDeath;

	private DamageReceiver damageReceiver;

	public FlyingBlimpLevelArrowProjectile Create(Vector2 pos, float startRotation, float startSpeed, float speed, float rotation, float timeBeforeDeath, float timeBeforeHoming, AbstractPlayerController player, float hp)
	{
		FlyingBlimpLevelArrowProjectile flyingBlimpLevelArrowProjectile = Create(pos, startRotation, startSpeed, speed, rotation, timeBeforeDeath, timeBeforeHoming, player) as FlyingBlimpLevelArrowProjectile;
		flyingBlimpLevelArrowProjectile.CollisionDeath.OnlyPlayer();
		flyingBlimpLevelArrowProjectile.DamagesType.OnlyPlayer();
		flyingBlimpLevelArrowProjectile.health = hp;
		flyingBlimpLevelArrowProjectile.timeToDeath = timeBeforeDeath;
		flyingBlimpLevelArrowProjectile.speed = speed;
		return flyingBlimpLevelArrowProjectile;
	}

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		StartCoroutine(trail_cr());
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(timer_cr());
	}

	private IEnumerator trail_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
			Effect trail = Object.Instantiate(trailPrefab);
			trail.transform.position = base.transform.position;
			trail.GetComponent<Animator>().SetInteger("PickAni", Random.Range(0, 3));
			yield return null;
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f)
		{
			Die();
		}
	}

	protected override void Die()
	{
		animator.SetTrigger("dead");
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		base.transform.SetEulerAngles(0f, 0f, -90f);
		base.Die();
	}

	private void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	private IEnumerator timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, timeToDeath);
		YieldInstruction wait = new WaitForFixedUpdate();
		base.HomingEnabled = false;
		while (true)
		{
			base.transform.position += base.transform.right * speed * CupheadTime.FixedDelta;
			yield return wait;
		}
	}
}
