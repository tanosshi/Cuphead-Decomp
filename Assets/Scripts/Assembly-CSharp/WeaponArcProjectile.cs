using System.Collections;
using UnityEngine;

public class WeaponArcProjectile : AbstractProjectile
{
	public enum State
	{
		InAir = 0,
		OnGround = 1
	}

	[SerializeField]
	private bool isEx;

	[SerializeField]
	private WeaponArcProjectileExplosion exExplosion;

	public float chargeTime;

	public float gravity;

	public Vector2 velocity;

	public WeaponArc weapon;

	private State _state;

	protected override float DestroyLifetime
	{
		get
		{
			return 1000f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.dead)
		{
			return;
		}
		switch (_state)
		{
		case State.InAir:
			UpdateInAir();
			break;
		case State.OnGround:
			UpdateOnGround();
			break;
		}
		if (!isEx)
		{
			UpdateDamageState();
			if (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(150f, 1000f)))
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void UpdateInAir()
	{
		velocity.y -= gravity * CupheadTime.FixedDelta;
		base.transform.position += (Vector3)(velocity * CupheadTime.FixedDelta);
	}

	private void UpdateOnGround()
	{
	}

	private void UpdateDamageState()
	{
		if (base.lifetime < WeaponProperties.LevelWeaponArc.Basic.timeStateTwo)
		{
			Damage = WeaponProperties.LevelWeaponArc.Basic.baseDamage;
			base.transform.SetScale(1f, 1f);
		}
		else if (base.lifetime < WeaponProperties.LevelWeaponArc.Basic.timeStateThree)
		{
			Damage = WeaponProperties.LevelWeaponArc.Basic.damageStateTwo;
			base.transform.SetScale(1.5f, 1.5f);
		}
		else
		{
			Damage = WeaponProperties.LevelWeaponArc.Basic.damageStateThree;
			base.transform.SetScale(2.5f, 2.5f);
		}
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionGround(hit, phase);
		LevelPlatform component = hit.GetComponent<LevelPlatform>();
		if (_state == State.InAir && (component == null || (!component.canFallThrough && velocity.y < 0f)))
		{
			HitGround(hit);
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionOther(hit, phase);
		LevelPlatform component = hit.GetComponent<LevelPlatform>();
		if (_state == State.InAir && component != null && !component.canFallThrough && velocity.y < 0f)
		{
			HitGround(hit);
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		if (!isEx)
		{
			base.damageDealer.SetDamage(Damage);
			base.damageDealer.DealDamage(hit);
		}
		base.OnCollisionEnemy(hit, phase);
	}

	private void HitGround(GameObject hit)
	{
		_state = State.OnGround;
		if (!isEx)
		{
			weapon.projectilesOnGround.Add(this);
			if (weapon.projectilesOnGround.Count > WeaponProperties.LevelWeaponArc.Basic.maxNumMines)
			{
				WeaponArcProjectile weaponArcProjectile = weapon.projectilesOnGround[0];
				weapon.projectilesOnGround.RemoveAt(0);
				weaponArcProjectile.Die();
			}
		}
		else
		{
			StartCoroutine(timedExplode_cr());
		}
		base.transform.SetParent(hit.transform);
	}

	private IEnumerator timedExplode_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, WeaponProperties.LevelWeaponArc.Ex.explodeDelay);
		Die();
	}

	protected override void Die()
	{
		base.Die();
		if (isEx)
		{
			exExplosion.Create(base.transform.position, Damage, base.DamageMultiplier);
			AudioManager.Play("player_weapon_arc_ex_explosion");
			emitAudioFromObject.Add("player_weapon_arc_ex_explosion");
			Object.Destroy(base.gameObject);
		}
	}

	protected override void OnDestroy()
	{
		if (weapon.projectilesOnGround.Contains(this))
		{
			weapon.projectilesOnGround.Remove(this);
		}
		base.OnDestroy();
	}
}
