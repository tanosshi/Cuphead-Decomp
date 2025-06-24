using System.Collections;
using UnityEngine;

public class TrainLevelGhostCannonGhost : HomingProjectile
{
	[SerializeField]
	private TrainLevelGhostCannonGhostSkull skullPrefab;

	private float delay;

	private float health;

	private float skullSpeed;

	private bool damageable;

	private DamageReceiver damageReceiver;

	protected override float DestroyLifetime
	{
		get
		{
			return 1000f;
		}
	}

	public TrainLevelGhostCannonGhost Create(Vector3 pos, float delay, float speed, float aimSpeed, float health, float skullSpeed)
	{
		TrainLevelGhostCannonGhost trainLevelGhostCannonGhost = Create(pos, -90f, speed, speed, aimSpeed, float.MaxValue, 2f, PlayerManager.GetNext()) as TrainLevelGhostCannonGhost;
		trainLevelGhostCannonGhost.HomingEnabled = false;
		trainLevelGhostCannonGhost.transform.position = pos;
		trainLevelGhostCannonGhost.delay = delay;
		trainLevelGhostCannonGhost.health = health;
		trainLevelGhostCannonGhost.skullSpeed = skullSpeed;
		trainLevelGhostCannonGhost.GetComponent<Collider2D>().enabled = false;
		return trainLevelGhostCannonGhost;
	}

	protected override void Start()
	{
		base.Start();
		damageable = false;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		StartCoroutine(start_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
		Die();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (damageable && !(health <= 0f))
		{
			health -= info.damage;
			if (health <= 0f)
			{
				Die();
			}
		}
	}

	protected override void Die()
	{
		AudioManager.Play("train_lollipop_cannon_ghost_death");
		emitAudioFromObject.Add("train_lollipop_cannon_ghost_death");
		StopAllCoroutines();
		health = -1f;
		damageable = false;
		animator.Play("Die");
	}

	private void DropSkull()
	{
		skullPrefab.Create(base.transform.position, skullSpeed);
	}

	private void OnDieAnimComplete()
	{
		Object.Destroy(base.gameObject);
	}

	private IEnumerator start_cr()
	{
		yield return StartCoroutine(up_cr());
		yield return CupheadTime.WaitForSeconds(this, delay);
		animator.Play("Attack");
		damageable = true;
		base.HomingEnabled = true;
		GetComponent<Collider2D>().enabled = true;
	}

	private IEnumerator up_cr()
	{
		yield return TweenPositionY(base.transform.position.y, 500f, 0.4f, EaseUtils.EaseType.linear);
	}
}
