using UnityEngine;

public class DicePalaceCardLevelGridBlock : AbstractCollidableObject
{
	public float Xcoordinate;

	public float Ycoordinate;

	public bool hasBlock;

	public float size;

	public DicePalaceCardLevelBlock blockHeld;

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionOther(hit, phase);
	}
}
