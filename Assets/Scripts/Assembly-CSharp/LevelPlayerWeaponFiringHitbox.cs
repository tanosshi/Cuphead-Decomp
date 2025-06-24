using UnityEngine;

public class LevelPlayerWeaponFiringHitbox : CollisionChild
{
	public LevelPlayerWeaponFiringHitbox Create(Vector2 pos, float rotation)
	{
		LevelPlayerWeaponFiringHitbox levelPlayerWeaponFiringHitbox = InstantiatePrefab<LevelPlayerWeaponFiringHitbox>();
		levelPlayerWeaponFiringHitbox.transform.position = pos;
		levelPlayerWeaponFiringHitbox.transform.SetEulerAngles(0f, 0f, rotation);
		return levelPlayerWeaponFiringHitbox;
	}
}
