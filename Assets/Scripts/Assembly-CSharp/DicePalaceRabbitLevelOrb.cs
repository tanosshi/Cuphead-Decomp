using UnityEngine;

public class DicePalaceRabbitLevelOrb : AbstractProjectile
{
	private new DamageDealer damageDealer;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		base.Awake();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	public void SetAsGold(bool isGold)
	{
		if (isGold)
		{
			animator.SetTrigger("Gold");
		}
		else
		{
			animator.SetTrigger("Blue");
		}
	}
}
