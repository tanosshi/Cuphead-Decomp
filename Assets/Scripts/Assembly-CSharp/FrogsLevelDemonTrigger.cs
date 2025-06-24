using UnityEngine;

public class FrogsLevelDemonTrigger : AbstractCollidableObject
{
	private DamageReceiver damageReceiver;

	private bool isTriggered;

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		isTriggered = true;
	}

	public bool getTrigger()
	{
		return isTriggered;
	}
}
