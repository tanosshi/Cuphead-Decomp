using System;
using System.Collections;
using UnityEngine;

public class BatLevelGoblin : AbstractCollidableObject
{
	[SerializeField]
	private BasicProjectile projectile;

	private LevelProperties.Bat.Goblins properties;

	private bool onLeft;

	private bool isShooter;

	private float health;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f)
		{
			Die();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public void Init(LevelProperties.Bat.Goblins properties, Vector2 pos, bool onLeft, bool isShooter, float health)
	{
		base.transform.position = pos;
		this.onLeft = onLeft;
		this.isShooter = isShooter;
		this.properties = properties;
		this.health = health;
		StartCoroutine(move_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		float endpos = ((!onLeft) ? (-640) : 640);
		float t = 0f;
		while (base.transform.position.x != endpos)
		{
			Vector3 pos = base.transform.position;
			pos.x = Mathf.MoveTowards(base.transform.position.x, endpos, properties.runSpeed * (float)CupheadTime.Delta);
			base.transform.position = pos;
			if (isShooter)
			{
				t += (float)CupheadTime.Delta;
				if (t >= properties.timeBeforeShoot)
				{
					yield return CupheadTime.WaitForSeconds(this, properties.initalShotDelay);
					ShootBullet();
					yield return CupheadTime.WaitForSeconds(this, properties.shooterHold);
					isShooter = false;
				}
			}
			yield return null;
		}
		Die();
	}

	private void ShootBullet()
	{
		float num = 0f;
		float rotation = Mathf.Atan2(x: (!onLeft) ? (-16200f / (float)Math.PI) : (48600f / (float)Math.PI), y: base.transform.position.y) * 57.29578f;
		projectile.Create(base.transform.position, rotation, properties.bulletSpeed);
	}

	private void Die()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
