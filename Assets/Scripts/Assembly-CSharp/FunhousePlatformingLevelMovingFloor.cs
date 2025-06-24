using UnityEngine;

public class FunhousePlatformingLevelMovingFloor : AbstractCollidableObject
{
	[SerializeField]
	private float velocity;

	private float speed;

	private LevelPlayerMotor.VelocityManager.Force scrollForce;

	protected override void Start()
	{
		base.Start();
		if (base.transform.localScale.x == 1f)
		{
			speed = 0f - velocity;
		}
		else
		{
			speed = velocity;
		}
		scrollForce = new LevelPlayerMotor.VelocityManager.Force(LevelPlayerMotor.VelocityManager.Force.Type.Ground, speed);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (phase == CollisionPhase.Stay)
		{
			if ((bool)hit.GetComponent<LevelPlayerMotor>())
			{
				ScrollOn(hit.GetComponent<LevelPlayerMotor>());
			}
			else
			{
				ScrollOff(hit.GetComponent<LevelPlayerMotor>());
			}
		}
		else
		{
			ScrollOff(hit.GetComponent<LevelPlayerMotor>());
		}
	}

	private void ScrollOn(LevelPlayerMotor player)
	{
		player.AddForce(scrollForce);
	}

	private void ScrollOff(LevelPlayerMotor player)
	{
		player.RemoveForce(scrollForce);
	}
}
