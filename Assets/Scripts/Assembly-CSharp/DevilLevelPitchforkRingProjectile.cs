using System.Collections;
using UnityEngine;

public class DevilLevelPitchforkRingProjectile : AbstractProjectile
{
	public enum State
	{
		Idle = 0,
		Attacking = 1,
		OnGround = 2
	}

	public State state;

	private Vector2 velocity;

	private float speed;

	private float groundDuration;

	private DevilLevelSittingDevil parent;

	private float waitTime;

	private bool waitTimeUp;

	private bool soundPlayed;

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return true;
		}
	}

	protected override float DestroyLifetime
	{
		get
		{
			return -1f;
		}
	}

	public DevilLevelPitchforkRingProjectile Create(Vector2 pos, float speed, float groundDuration, DevilLevelSittingDevil parent, float waitTime)
	{
		DevilLevelPitchforkRingProjectile devilLevelPitchforkRingProjectile = InstantiatePrefab<DevilLevelPitchforkRingProjectile>();
		devilLevelPitchforkRingProjectile.transform.position = pos;
		devilLevelPitchforkRingProjectile.speed = speed;
		devilLevelPitchforkRingProjectile.state = State.Idle;
		devilLevelPitchforkRingProjectile.groundDuration = groundDuration;
		devilLevelPitchforkRingProjectile.parent = parent;
		devilLevelPitchforkRingProjectile.waitTime = waitTime;
		devilLevelPitchforkRingProjectile.StartCoroutine(devilLevelPitchforkRingProjectile.wait_cr());
		return devilLevelPitchforkRingProjectile;
	}

	protected override void Update()
	{
		base.Update();
		if (parent == null)
		{
			Die();
		}
	}

	protected override void Start()
	{
		base.Start();
		GetComponent<Collider2D>().enabled = false;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (waitTimeUp && !base.dead && state == State.Attacking)
		{
			if (!soundPlayed)
			{
				AttackSFX();
				soundPlayed = true;
			}
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
			float radius = GetComponent<CircleCollider2D>().radius;
		}
	}

	private IEnumerator wait_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, waitTime);
		waitTimeUp = true;
		GetComponent<Collider2D>().enabled = true;
		animator.SetTrigger("Continue");
	}

	public void Attack()
	{
		if (!base.dead)
		{
			state = State.Attacking;
			velocity = speed * ((Vector2)(PlayerManager.GetNext().center - base.transform.position)).normalized;
			StartCoroutine(main_cr());
		}
	}

	private IEnumerator main_cr()
	{
		while (state == State.Attacking)
		{
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, groundDuration);
		Die();
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}

	public override void SetParryable(bool parryable)
	{
		base.SetParryable(parryable);
		animator.SetBool("IsPink", parryable);
	}

	private void AttackSFX()
	{
		AudioManager.Play("devil_ring_projectile");
		emitAudioFromObject.Add("devil_ring_projectile");
	}
}
