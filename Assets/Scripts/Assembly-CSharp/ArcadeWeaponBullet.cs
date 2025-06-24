using UnityEngine;

public class ArcadeWeaponBullet : BasicProjectile
{
	private static float POINTS_BONUS_ACCURACY;

	private static bool IN_COMBO;

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if ((bool)hit.GetComponent<RetroArcadeEnemy>())
		{
			IN_COMBO = true;
			POINTS_BONUS_ACCURACY += RetroArcadeLevel.ACCURACY_BONUS;
		}
		else if (IN_COMBO)
		{
			Debug.Log(POINTS_BONUS_ACCURACY + " ACCURACY BONUS");
			RetroArcadeLevel.TOTAL_POINTS += POINTS_BONUS_ACCURACY;
			POINTS_BONUS_ACCURACY = 0f;
			IN_COMBO = false;
		}
	}
}
