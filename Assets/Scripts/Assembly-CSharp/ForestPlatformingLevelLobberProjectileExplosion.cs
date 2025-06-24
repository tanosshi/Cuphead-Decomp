using UnityEngine;

public class ForestPlatformingLevelLobberProjectileExplosion : Effect
{
	private DamageDealer damageDealer;

	protected override void Awake()
	{
		base.Awake();
		AudioManager.Play("level_lobber_projectile_explosion");
		emitAudioFromObject.Add("level_lobber_projectile_explosion");
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Start()
	{
		base.Start();
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
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}
}
