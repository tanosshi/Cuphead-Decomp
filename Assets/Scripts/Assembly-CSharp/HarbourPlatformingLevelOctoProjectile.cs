using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelOctoProjectile : AbstractProjectile
{
	[SerializeField]
	private float speedX;

	[SerializeField]
	private float speedY;

	[SerializeField]
	private float gravity;

	private Vector2 velocity;

	protected override void Start()
	{
		base.Start();
		velocity.y = speedY;
		velocity.x = speedX;
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			base.transform.AddPosition(velocity.x * (float)CupheadTime.Delta, velocity.y * (float)CupheadTime.Delta);
			velocity.y -= gravity * (float)CupheadTime.Delta;
			yield return null;
		}
	}
}
