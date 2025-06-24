using UnityEngine;

public class MausoleumLevelUrn : AbstractCollidableObject
{
	private DamageDealer damageDealer;

	private float distanceToDie = 30f;

	public static Vector3 URN_POS { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		URN_POS = base.transform.position;
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
}
