using UnityEngine;

public class TestLevelStationaryJared : LevelProperties.Test.Entity
{
	[SerializeField]
	private Transform childSprite;

	private DamageReceiver damageReceiver;

	public override void LevelInit(LevelProperties.Test properties)
	{
		base.LevelInit(properties);
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public override void OnParry(AbstractPlayerController player)
	{
		base.OnParry(player);
		player.stats.OnParry();
	}
}
