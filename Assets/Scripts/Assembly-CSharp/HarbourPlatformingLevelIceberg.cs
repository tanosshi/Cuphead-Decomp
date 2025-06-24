using UnityEngine;

public class HarbourPlatformingLevelIceberg : AbstractCollidableObject
{
	[SerializeField]
	private Effect explosion;

	[SerializeField]
	private SpriteDeathParts[] deathParts;

	private DamageDealer damageDealer;

	protected override void Start()
	{
		base.Start();
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

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if ((bool)hit.GetComponent<HarbourPlatformingLevelOctoProjectile>())
		{
			SmashSFX();
			Object.Destroy(hit.gameObject);
			DeathParts();
			Object.Destroy(base.gameObject);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void DeathParts()
	{
		explosion.Create(base.transform.position);
		SpriteDeathParts[] array = deathParts;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(base.transform.position);
		}
	}

	private void SmashSFX()
	{
		AudioManager.Play("harbour_iceberg_smash");
		emitAudioFromObject.Add("harbour_iceberg_smash");
	}
}
