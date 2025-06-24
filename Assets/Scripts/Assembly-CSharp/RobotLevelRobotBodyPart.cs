using System.Collections;
using UnityEngine;

public class RobotLevelRobotBodyPart : AbstractCollidableObject
{
	protected enum state
	{
		primary = 0,
		secondary = 1,
		none = 2
	}

	protected state current;

	protected LevelProperties.Robot properties;

	protected RobotLevelRobot parent;

	protected float decreaseAttackDelayAmount;

	protected float[] currentHealth;

	protected float primaryAttackDelay;

	protected float secondaryAttackDelay;

	protected float attackDelayMinus;

	protected bool isAttacking;

	protected DamageReceiver damageReceiver;

	[SerializeField]
	protected Effect deathEffect;

	[SerializeField]
	protected GameObject primary;

	[SerializeField]
	protected GameObject secondary;

	public virtual void InitBodyPart(RobotLevelRobot parent, LevelProperties.Robot properties, int primaryHP = 0, int secondaryHP = 1, float attackDelayMinus = 0f)
	{
		this.parent = parent;
		this.parent.OnDeathEvent += Die;
		this.parent.OnPrimaryDeathEvent += OnPrimaryDeath;
		this.parent.OnSecondaryDeathEvent += OnSecondaryDeath;
		this.properties = properties;
		current = state.primary;
		currentHealth[0] = primaryHP;
		currentHealth[1] = secondaryHP;
		this.attackDelayMinus = attackDelayMinus;
		StartCoroutine(checkCurrentState_cr());
	}

	protected virtual void StartPrimary()
	{
		StartCoroutine(primaryAttack_cr());
	}

	protected virtual IEnumerator primaryAttack_cr()
	{
		while (current == state.primary)
		{
			yield return CupheadTime.WaitForSeconds(this, primaryAttackDelay);
			isAttacking = true;
			OnPrimaryAttack();
			while (isAttacking)
			{
				yield return null;
			}
		}
		yield return null;
	}

	protected virtual void StartSecondary()
	{
		StartCoroutine(secondaryAttack_cr());
	}

	protected virtual IEnumerator secondaryAttack_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		while (current == state.secondary)
		{
			OnSecondaryAttack();
			yield return null;
			if (secondaryAttackDelay > 0f)
			{
				yield return CupheadTime.WaitForSeconds(this, secondaryAttackDelay);
			}
			else
			{
				yield return null;
			}
		}
	}

	protected virtual void AttackDestroyed(bool isPrimary)
	{
		if (isPrimary)
		{
			AudioManager.Play("robot_vocals_angry");
			emitAudioFromObject.Add("robot_vocals_angry");
			parent.PrimaryDied();
			current = state.secondary;
		}
		else
		{
			current = state.none;
			GetComponent<BoxCollider2D>().enabled = false;
		}
	}

	protected override void Awake()
	{
		currentHealth = new float[2];
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.Awake();
	}

	protected virtual void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (current == state.primary)
		{
			currentHealth[(int)current] -= info.damage;
		}
	}

	protected IEnumerator checkCurrentState_cr()
	{
		while (current != state.none)
		{
			switch (current)
			{
			case state.primary:
				if (currentHealth[0] <= 0f)
				{
					AttackDestroyed(true);
				}
				break;
			case state.secondary:
				if (currentHealth[1] <= 0f)
				{
					AttackDestroyed(false);
				}
				break;
			}
			yield return null;
		}
	}

	protected virtual void OnPrimaryAttack()
	{
	}

	protected virtual void OnPrimaryDeath()
	{
		if (current == state.primary)
		{
			primaryAttackDelay -= attackDelayMinus;
		}
	}

	protected virtual void OnSecondaryAttack()
	{
	}

	protected virtual void OnSecondaryDeath()
	{
	}

	protected virtual void ExitCurrentAttacks()
	{
	}

	protected virtual void DeathEffect()
	{
		if (deathEffect != null)
		{
			StartCoroutine(death_effects_cr());
		}
	}

	private IEnumerator death_effects_cr()
	{
		while (true)
		{
			yield return null;
			deathEffect.Create(base.transform.position).GetComponent<Animator>().SetBool("IsA", Rand.Bool());
			yield return CupheadTime.WaitForSeconds(this, Random.Range(2f, 5f));
		}
	}

	protected virtual void Die()
	{
		StopAllCoroutines();
		ExitCurrentAttacks();
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (component != null)
		{
			component.enabled = false;
		}
		Object.Destroy(base.gameObject, 15f);
	}

	protected override void OnDestroy()
	{
		parent.OnDeathEvent -= Die;
		parent.OnPrimaryDeathEvent -= OnPrimaryDeath;
		parent.OnSecondaryDeathEvent -= OnSecondaryDeath;
		base.OnDestroy();
	}
}
