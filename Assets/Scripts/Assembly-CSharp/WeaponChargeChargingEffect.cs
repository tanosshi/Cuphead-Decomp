using UnityEngine;

public class WeaponChargeChargingEffect : AbstractMonoBehaviour
{
	public WeaponChargeChargingEffect Create(Vector2 pos)
	{
		WeaponChargeChargingEffect weaponChargeChargingEffect = InstantiatePrefab<WeaponChargeChargingEffect>();
		weaponChargeChargingEffect.transform.position = pos;
		return weaponChargeChargingEffect;
	}
}
