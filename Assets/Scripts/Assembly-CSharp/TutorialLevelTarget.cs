using System;
using UnityEngine;

public class TutorialLevelTarget : AbstractCollidableObject
{
	public event Action OnShotEvent;

	protected override void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayerProjectile(hit, phase);
		GetComponent<Collider2D>().enabled = false;
		if (this.OnShotEvent != null)
		{
			this.OnShotEvent();
		}
	}
}
