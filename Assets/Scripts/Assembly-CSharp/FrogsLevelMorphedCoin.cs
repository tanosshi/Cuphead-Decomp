using UnityEngine;

public class FrogsLevelMorphedCoin : BasicProjectile
{
	public FrogsLevelMorphedCoin CreateCoin(Vector2 pos, float speed, float rotation)
	{
		FrogsLevelMorphedCoin frogsLevelMorphedCoin = base.Create(pos, rotation, speed) as FrogsLevelMorphedCoin;
		frogsLevelMorphedCoin.CollisionDeath.None();
		frogsLevelMorphedCoin.DamagesType.OnlyPlayer();
		return frogsLevelMorphedCoin;
	}
}
