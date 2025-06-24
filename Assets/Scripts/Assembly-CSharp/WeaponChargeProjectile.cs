using UnityEngine;

public class WeaponChargeProjectile : BasicProjectile
{
	public bool fullyCharged;

	protected override void Die()
	{
		if (fullyCharged)
		{
			Vector2 vector = MathUtils.AngleToDirection(base.transform.eulerAngles.z) * 75f;
			base.transform.AddPosition(vector.x, vector.y);
		}
		base.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0, 360));
		base.Die();
		if (fullyCharged)
		{
			AudioManager.Play("player_weapon_charge_full_impact");
			emitAudioFromObject.Add("player_weapon_charge_full_impact");
		}
	}
}
