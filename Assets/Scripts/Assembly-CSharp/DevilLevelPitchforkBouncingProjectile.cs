using System.Collections;
using UnityEngine;

public class DevilLevelPitchforkBouncingProjectile : AbstractProjectile
{
	public enum State
	{
		Idle = 0,
		Attacking = 1
	}

	private const string ProjectilesLayerName = "Projectiles";

	private const int VariationMax = 3;

	private const float BounceTimeThreshold = 1f / 12f;

	public State state;

	private float attackDelay;

	private Vector2 velocity;

	private Vector2 velocityOld;

	private DevilLevelSittingDevil parent;

	private float waitTime;

	private bool waitTimeUp;

	private float bounceTime;

	[SerializeField]
	private Effect blueSparkle;

	[SerializeField]
	private Effect pinkSparkle;

	[SerializeField]
	private Effect bounceEffect;

	[SerializeField]
	private Effect bounceEffectPink;

	protected override float DestroyLifetime
	{
		get
		{
			return -1f;
		}
	}

	public int BouncesRemaining { get; private set; }

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return true;
		}
	}

	public DevilLevelPitchforkBouncingProjectile Create(Vector2 pos, float attackDelay, float speed, float angle, int numBounces, DevilLevelSittingDevil parent, float waitTime)
	{
		DevilLevelPitchforkBouncingProjectile devilLevelPitchforkBouncingProjectile = InstantiatePrefab<DevilLevelPitchforkBouncingProjectile>();
		devilLevelPitchforkBouncingProjectile.transform.position = pos;
		devilLevelPitchforkBouncingProjectile.attackDelay = attackDelay;
		devilLevelPitchforkBouncingProjectile.velocity = speed * MathUtils.AngleToDirection(angle);
		devilLevelPitchforkBouncingProjectile.BouncesRemaining = numBounces;
		devilLevelPitchforkBouncingProjectile.parent = parent;
		devilLevelPitchforkBouncingProjectile.state = State.Idle;
		devilLevelPitchforkBouncingProjectile.waitTime = waitTime;
		devilLevelPitchforkBouncingProjectile.StartCoroutine(devilLevelPitchforkBouncingProjectile.main_cr());
		devilLevelPitchforkBouncingProjectile.animator.SetFloat("Variation", (float)Random.Range(0, 3) / 2f);
		return devilLevelPitchforkBouncingProjectile;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void Update()
	{
		base.Update();
		if (parent == null)
		{
			Die();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!waitTimeUp || base.dead || state == State.Idle)
		{
			return;
		}
		base.transform.AddPosition(velocity.x * CupheadTime.FixedDelta, velocity.y * CupheadTime.FixedDelta);
		if (velocity == Vector2.zero)
		{
			bounceTime += CupheadTime.FixedDelta;
			if (bounceTime > 1f / 12f)
			{
				velocity = velocityOld;
			}
		}
		float radius = GetComponent<CircleCollider2D>().radius;
		if (BouncesRemaining > 0)
		{
			if ((velocity.x < 0f && base.transform.position.x < (float)Level.Current.Left + radius) || (velocity.x > 0f && base.transform.position.x > (float)Level.Current.Right - radius))
			{
				if (bounceTime == 0f)
				{
					animator.Play("BounceWall");
					BounceSFX();
					velocityOld = velocity;
					velocity = Vector2.zero;
				}
				else if (bounceTime > 1f / 12f)
				{
					BouncesRemaining--;
					velocity.x *= -1f;
					bounceTime = 0f;
				}
			}
			if (velocity.y > 0f && base.transform.position.y > (float)Level.Current.Ceiling + radius)
			{
				if (bounceTime == 0f)
				{
					animator.Play("BounceGround");
					BounceSFX();
					velocityOld = velocity;
					velocity = Vector2.zero;
				}
				else if (bounceTime > 1f / 12f)
				{
					BouncesRemaining--;
					velocity.y *= -1f;
					bounceTime = 0f;
				}
			}
		}
		if (!(velocity.y < 0f) || !(base.transform.position.y < (float)Level.Current.Ground + radius))
		{
			return;
		}
		if (bounceTime == 0f)
		{
			animator.Play("BounceGround");
			BounceSFX();
			velocityOld = velocity;
			velocity = Vector2.zero;
			if (base.CanParry)
			{
				bounceEffectPink.Create(base.transform.position);
			}
			else
			{
				bounceEffect.Create(base.transform.position);
			}
		}
		else if (bounceTime > 1f / 12f)
		{
			BouncesRemaining--;
			velocity.y *= -1f;
			bounceTime = 0f;
		}
	}

	private IEnumerator main_cr()
	{
		GetComponent<Collider2D>().enabled = false;
		yield return CupheadTime.WaitForSeconds(this, waitTime);
		animator.SetTrigger("Continue");
		waitTimeUp = true;
		GetComponent<Collider2D>().enabled = true;
		yield return CupheadTime.WaitForSeconds(this, attackDelay);
		state = State.Attacking;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
			Effect selectedSparkle = ((!base.CanParry) ? blueSparkle : pinkSparkle);
			Effect inst = selectedSparkle.Create(base.transform.position);
			SpriteRenderer r = inst.GetComponent<SpriteRenderer>();
			r.sortingLayerName = "Projectiles";
			r.sortingOrder = -1;
			yield return null;
		}
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

	public override void OnParry(AbstractPlayerController player)
	{
		base.OnParry(player);
		BouncesRemaining = 0;
	}

	private void BounceSFX()
	{
		AudioManager.Play("devil_projectile_bounce");
		emitAudioFromObject.Add("devil_projectile_bounce");
	}
}
