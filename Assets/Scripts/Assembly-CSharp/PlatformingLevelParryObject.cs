using UnityEngine;

public class PlatformingLevelParryObject : ParrySwitch
{
	public bool hurtsPlayer;

	private DamageDealer damageDealer;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (hurtsPlayer && damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}
}
