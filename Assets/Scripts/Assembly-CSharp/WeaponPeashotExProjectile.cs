using UnityEngine;

public class WeaponPeashotExProjectile : AbstractProjectile
{
	[SerializeField]
	private Effect hitFXPrefab;

	[SerializeField]
	private Transform hitFxRoot;

	private float timeUntilUnfreeze;

	public float moveSpeed;

	public float hitFreezeTime;

	private float totalDamage;

	private float currentSpeed;

	public float maxDamage;

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.dead)
		{
			if (timeUntilUnfreeze > 0f)
			{
				timeUntilUnfreeze -= CupheadTime.FixedDelta;
				currentSpeed = 0f;
			}
			else
			{
				currentSpeed = moveSpeed;
			}
			Vector2 vector = MathUtils.AngleToDirection(base.transform.eulerAngles.z) * currentSpeed;
			base.transform.AddPosition(vector.x * CupheadTime.FixedDelta, vector.y * CupheadTime.FixedDelta);
		}
	}

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionEnemy(hit, phase);
		float num = base.damageDealer.DealDamage(hit);
		totalDamage += num;
		if (totalDamage > maxDamage)
		{
			Die();
		}
		if (num > 0f)
		{
			hitFXPrefab.Create(hitFxRoot.position);
			AudioManager.Play("player_ex_impact_hit");
			emitAudioFromObject.Add("player_ex_impact_hit");
			timeUntilUnfreeze = hitFreezeTime;
		}
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (!(hit.tag == "Parry"))
		{
			base.OnCollisionOther(hit, phase);
		}
	}
}
