using System;
using System.Collections;
using UnityEngine;

public class ClownLevelEnemy : AbstractProjectile
{
	[SerializeField]
	private BasicProjectile bullet;

	[SerializeField]
	private Transform root;

	private LevelProperties.Clown.Swing properties;

	private ClownLevelClownSwing parent;

	private ClownLevelCoasterHandler handler;

	private DamageReceiver damageReceiver;

	private float targetPosition;

	private float HP;

	private bool isDead;

	protected override float DestroyLifetime
	{
		get
		{
			return 0f;
		}
	}

	public ClownLevelEnemy Create(Vector3 pos, float targetPosition, float HP, LevelProperties.Clown.Swing properties, ClownLevelClownSwing parent)
	{
		ClownLevelEnemy clownLevelEnemy = base.Create(pos) as ClownLevelEnemy;
		clownLevelEnemy.transform.position = pos;
		clownLevelEnemy.properties = properties;
		clownLevelEnemy.targetPosition = targetPosition;
		clownLevelEnemy.HP = HP;
		clownLevelEnemy.parent = parent;
		return clownLevelEnemy;
	}

	protected override void Start()
	{
		base.Start();
		ClownLevelClownSwing clownLevelClownSwing = parent;
		clownLevelClownSwing.OnDeath = (Action)Delegate.Combine(clownLevelClownSwing.OnDeath, new Action(Die));
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageReceiver.enabled = false;
		AudioManager.Play("clown_penguin_roll_start");
		emitAudioFromObject.Add("clown_penguin_roll_start");
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
		base.damageDealer.DealDamage(hit);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		HP -= info.damage;
		if (HP <= 0f && !isDead)
		{
			isDead = true;
			Die();
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionOther(hit, phase);
		if ((bool)hit.GetComponent<ClownLevelCoaster>())
		{
			Die();
		}
	}

	private void StartMoving()
	{
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		Vector3 pos = base.transform.position;
		float target = -640f + targetPosition;
		while (base.transform.position.x != target)
		{
			pos.x = Mathf.MoveTowards(base.transform.position.x, target, properties.movementSpeed * (float)CupheadTime.Delta);
			base.transform.position = pos;
			yield return null;
		}
		animator.SetTrigger("Continue");
		AudioManager.Play("clown_penguin_roll_end");
		emitAudioFromObject.Add("clown_penguin_roll_end");
		damageReceiver.enabled = true;
		StartCoroutine(shoot_cr());
		yield return null;
	}

	private IEnumerator shoot_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.initialAttackDelay);
		while (true)
		{
			animator.SetTrigger("OnAttack");
			yield return CupheadTime.WaitForSeconds(this, properties.attackDelayRange.RandomFloat());
			yield return null;
		}
	}

	private void ShootBullet()
	{
		AudioManager.Play("clown_penguin_clap");
		emitAudioFromObject.Add("clown_penguin_clap");
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector3 vector = next.transform.position - base.transform.position;
		bullet.Create(root.transform.position, MathUtils.DirectionToAngle(vector), properties.bulletSpeed);
	}

	protected override void Die()
	{
		AudioManager.Play("clown_penguin_death");
		emitAudioFromObject.Add("clown_penguin_death");
		animator.SetTrigger("OnDeath");
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		ClownLevelClownSwing clownLevelClownSwing = parent;
		clownLevelClownSwing.OnDeath = (Action)Delegate.Remove(clownLevelClownSwing.OnDeath, new Action(Die));
		base.Die();
	}

	private void SwitchLayer()
	{
		GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Projectiles.ToString();
	}
}
