using UnityEngine;

public class BasicProjectile : AbstractProjectile
{
	[Space(10f)]
	public float Speed;

	public float Gravity;

	[Space(10f)]
	public Sfx SfxOnDeath;

	protected bool move = true;

	protected float _accumulativeGravity;

	private Collider2D collider;

	protected override float DestroyLifetime
	{
		get
		{
			return 10f;
		}
	}

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return true;
		}
	}

	protected virtual string projectileMissImpactSFX
	{
		get
		{
			return "player_weapon_peashot_miss";
		}
	}

	public virtual BasicProjectile Create(Vector2 position, float rotation, float speed)
	{
		BasicProjectile basicProjectile = Create(position, rotation) as BasicProjectile;
		basicProjectile.Speed = speed;
		return basicProjectile;
	}

	public virtual BasicProjectile Create(Vector2 position, float rotation, Vector2 scale, float speed)
	{
		BasicProjectile basicProjectile = Create(position, rotation, scale) as BasicProjectile;
		basicProjectile.Speed = speed;
		return basicProjectile;
	}

	protected override void Awake()
	{
		base.Awake();
		if (base.tag == Tags.EnemyProjectile.ToString())
		{
			DamagesType.Player = true;
		}
		collider = GetComponent<Collider2D>();
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (!(hit.tag == "Parry"))
		{
			base.OnCollisionOther(hit, phase);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		DealDamage(hit);
		base.OnCollisionEnemy(hit, phase);
	}

	protected override void OnCollisionDie(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionDie(hit, phase);
		if (base.tag == Tags.PlayerProjectile.ToString() && phase == CollisionPhase.Enter)
		{
			if ((bool)hit.GetComponent<DamageReceiver>() && hit.GetComponent<DamageReceiver>().enabled)
			{
				AudioManager.Play("player_shoot_hit_cuphead");
			}
			else
			{
				AudioManager.Play(projectileMissImpactSFX);
			}
		}
	}

	private void DealDamage(GameObject hit)
	{
		base.damageDealer.DealDamage(hit);
	}

	protected override void Die()
	{
		move = false;
		EffectSpawner component = GetComponent<EffectSpawner>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		base.Die();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (move)
		{
			Move();
		}
	}

	protected virtual void Move()
	{
		if (Speed == 0f)
		{
			Debug.Log("[Basic Projectile] Speed is 0", base.gameObject);
		}
		base.transform.position += base.transform.right * Speed * CupheadTime.FixedDelta - new Vector3(0f, _accumulativeGravity * CupheadTime.FixedDelta, 0f);
		_accumulativeGravity += Gravity * CupheadTime.FixedDelta;
	}
}
