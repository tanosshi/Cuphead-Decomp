using System.Collections;
using UnityEngine;

public class DevilLevelPitchforkWheelProjectile : AbstractProjectile
{
	public enum State
	{
		Idle = 0,
		Attacking = 1,
		Returning = 2
	}

	public State state;

	private float attackDelay;

	private Vector2 velocity;

	private float speed;

	private DevilLevelSittingDevil parent;

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

	public DevilLevelPitchforkWheelProjectile Create(Vector2 pos, float attackDelay, float speed, DevilLevelSittingDevil parent)
	{
		DevilLevelPitchforkWheelProjectile devilLevelPitchforkWheelProjectile = InstantiatePrefab<DevilLevelPitchforkWheelProjectile>();
		devilLevelPitchforkWheelProjectile.transform.position = pos;
		devilLevelPitchforkWheelProjectile.attackDelay = attackDelay;
		devilLevelPitchforkWheelProjectile.speed = speed;
		devilLevelPitchforkWheelProjectile.state = State.Idle;
		devilLevelPitchforkWheelProjectile.StartCoroutine(devilLevelPitchforkWheelProjectile.main_cr());
		devilLevelPitchforkWheelProjectile.parent = parent;
		return devilLevelPitchforkWheelProjectile;
	}

	protected override void Update()
	{
		base.Update();
		if (parent == null)
		{
			Die();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.dead && state != State.Idle)
		{
			base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
		}
	}

	private IEnumerator main_cr()
	{
		float t = 0f;
		yield return CupheadTime.WaitForSeconds(this, attackDelay);
		state = State.Attacking;
		velocity = speed * ((Vector2)(PlayerManager.GetNext().center - base.transform.position)).normalized;
		while (state == State.Attacking)
		{
			float colliderRadius = GetComponent<CircleCollider2D>().radius;
			if (base.transform.position.x < (float)Level.Current.Left + colliderRadius || base.transform.position.x > (float)Level.Current.Right - colliderRadius || base.transform.position.y < (float)Level.Current.Ground + colliderRadius || base.transform.position.y > (float)Level.Current.Ceiling - colliderRadius)
			{
				velocity *= -1f;
				state = State.Returning;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
