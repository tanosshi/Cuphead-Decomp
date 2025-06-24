using UnityEngine;

public class BatLevelMiniBat : BasicSineProjectile
{
	private LevelProperties.Bat.MiniBats properties;

	private DamageReceiver damageReceiver;

	private float health;

	public BatLevelMiniBat Create(Vector2 pos, float rotation, float velocity, float sinVelocity, float sinSize, float health, LevelProperties.Bat.MiniBats properties)
	{
		BatLevelMiniBat batLevelMiniBat = Create(pos, rotation, velocity, sinVelocity, sinSize) as BatLevelMiniBat;
		batLevelMiniBat.health = health;
		batLevelMiniBat.properties = properties;
		return batLevelMiniBat;
	}

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
