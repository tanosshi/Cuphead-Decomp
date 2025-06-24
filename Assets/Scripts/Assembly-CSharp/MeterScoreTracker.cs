using System.Collections.Generic;

public class MeterScoreTracker
{
	public enum Type
	{
		Super = 0,
		Ex = 1
	}

	private Type type;

	private bool alreadyAddedScore;

	private List<AbstractProjectile> projectilesToAdd;

	public MeterScoreTracker(Type type)
	{
		this.type = type;
	}

	public void Add(DamageDealer damageDealer)
	{
		damageDealer.OnDealDamage += OnDealDamage;
	}

	public void Add(AbstractProjectile projectile)
	{
		projectile.AddToMeterScoreTracker(this);
	}

	private void OnDealDamage(float damage, DamageReceiver damageReceiver, DamageDealer damageDealer)
	{
		if (!alreadyAddedScore)
		{
			Level.ScoringData.superMeterUsed += ((type != Type.Super) ? 1 : 5);
			alreadyAddedScore = true;
		}
	}
}
