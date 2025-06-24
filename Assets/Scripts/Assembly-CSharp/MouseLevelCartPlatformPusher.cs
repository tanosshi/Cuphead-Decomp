using UnityEngine;

public class MouseLevelCartPlatformPusher : AbstractCollidableObject
{
	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		AbstractPlayerController component = hit.GetComponent<AbstractPlayerController>();
		Collider2D component2 = GetComponent<Collider2D>();
		Collider2D component3 = component.GetComponent<Collider2D>();
		if (!(component.bottom < component2.bounds.max.y))
		{
			return;
		}
		if (component.center.x < component2.bounds.center.x)
		{
			float num = component3.bounds.max.x - component2.bounds.min.x;
			if (num > 0f)
			{
				component.transform.AddPosition(0f - num);
			}
		}
		else
		{
			float num2 = component2.bounds.max.x - component3.bounds.min.x;
			if (num2 > 0f)
			{
				component.transform.AddPosition(num2);
			}
		}
	}
}
