using UnityEngine;

public class FlyingMermaidLevelLaser : AbstractCollidableObject
{
	private const float RIGHT_EDGE = 500f;

	private float stoneTime = 5f;

	private bool on;

	private bool checkCollider;

	public void SetStoneTime(float stoneTime)
	{
		this.stoneTime = stoneTime;
	}

	public void StartLaser()
	{
		if ((bool)GetComponent<Collider2D>())
		{
			checkCollider = true;
		}
		else
		{
			on = true;
		}
	}

	public void StopLaser()
	{
		if ((bool)GetComponent<Collider2D>())
		{
			checkCollider = false;
		}
		on = false;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (checkCollider)
		{
			on = true;
		}
	}

	protected override void Update()
	{
		if (!on)
		{
			return;
		}
		PlanePlayerController[] array = new PlanePlayerController[2]
		{
			PlayerManager.GetPlayer(PlayerId.PlayerOne) as PlanePlayerController,
			PlayerManager.GetPlayer(PlayerId.PlayerTwo) as PlanePlayerController
		};
		float num = base.transform.position.x + 500f;
		PlanePlayerController[] array2 = array;
		foreach (PlanePlayerController planePlayerController in array2)
		{
			if (!(planePlayerController == null) && !planePlayerController.IsDead && planePlayerController.right < num)
			{
				planePlayerController.GetStoned(stoneTime);
			}
		}
		base.Update();
	}
}
