using System.Collections;
using UnityEngine;

public class TreePlatformingLevelLadyBug : PlatformingLevelGroundMovementEnemy
{
	public enum Type
	{
		GroundFast = 0,
		GroundSlow = 1,
		BounceFast = 2,
		BounceSlow = 3,
		BouncePink = 4
	}

	public Type type;

	protected override void Awake()
	{
		base.Awake();
		manuallySetJumpX = true;
	}

	protected override void Start()
	{
		Setup();
		base.Start();
	}

	public TreePlatformingLevelLadyBug Spawn(Vector3 pos, Direction dir, bool destroy, Type type)
	{
		TreePlatformingLevelLadyBug treePlatformingLevelLadyBug = Spawn(pos, dir, destroy) as TreePlatformingLevelLadyBug;
		treePlatformingLevelLadyBug.type = type;
		return treePlatformingLevelLadyBug;
	}

	public void Setup()
	{
		switch (type)
		{
		case Type.GroundFast:
			GoToGround();
			base.animator.Play("Fast_Ground");
			AudioManager.PlayLoop("level_platform_ladybug_ground_fast_loop");
			emitAudioFromObject.Add("level_platform_ladybug_ground_fast_loop");
			SetMoveSpeed(base.Properties.fastMovement);
			noTurn = true;
			StartCoroutine(no_y_cr());
			break;
		case Type.GroundSlow:
			GoToGround();
			base.animator.Play("Slow_Ground");
			AudioManager.PlayLoop("level_platform_ladybug_ground_slow_loop");
			emitAudioFromObject.Add("level_platform_ladybug_ground_slow_loop");
			SetMoveSpeed(base.Properties.slowMovement);
			noTurn = true;
			StartCoroutine(no_y_cr());
			break;
		case Type.BounceFast:
			base.animator.Play("Fast_Bounce");
			AudioManager.PlayLoop("level_platform_ladybug_bounce_fast_loop");
			emitAudioFromObject.Add("level_platform_ladybug_bounce_fast_loop");
			SetMoveSpeed(base.Properties.fastMovement);
			StartCoroutine(y_cr());
			noTurn = true;
			break;
		case Type.BounceSlow:
			base.animator.Play("Slow_Bounce");
			AudioManager.PlayLoop("level_platform_ladybug_bounce_slow_loop");
			emitAudioFromObject.Add("level_platform_ladybug_bounce_slow_loop");
			SetMoveSpeed(base.Properties.slowMovement);
			StartCoroutine(y_cr());
			noTurn = true;
			break;
		case Type.BouncePink:
			_canParry = true;
			base.animator.Play("Pink_Slow_Ground");
			AudioManager.PlayLoop("level_platform_ladybug_ground_slow_loop");
			emitAudioFromObject.Add("level_platform_ladybug_ground_slow_loop");
			SetMoveSpeed(base.Properties.slowMovement);
			StartCoroutine(y_cr());
			noTurn = true;
			break;
		}
	}

	private IEnumerator y_cr()
	{
		floating = false;
		yield return null;
		while (true)
		{
			if (!base.Grounded)
			{
				yield return null;
				continue;
			}
			Jump();
			AudioManager.Play("level_platform_ladybug_bounce");
			emitAudioFromObject.Add("level_platform_ladybug_bounce");
			yield return null;
		}
	}

	private IEnumerator no_y_cr()
	{
		while (true)
		{
			if (!base.Grounded)
			{
				fallInPit = true;
			}
			yield return null;
		}
	}

	protected override void Die()
	{
		base.Die();
		AudioManager.Play("level_platform_ladybug_death");
		emitAudioFromObject.Add("level_platform_ladybug_death");
		switch (type)
		{
		case Type.GroundFast:
			AudioManager.Stop("level_platform_ladybug_ground_fast_loop");
			break;
		case Type.GroundSlow:
			AudioManager.Stop("level_platform_ladybug_ground_slow_loop");
			break;
		case Type.BounceFast:
			AudioManager.Stop("level_platform_ladybug_bounce_fast_loop");
			break;
		case Type.BounceSlow:
			AudioManager.Stop("level_platform_ladybug_bounce_slow_loop");
			break;
		case Type.BouncePink:
			AudioManager.Stop("level_platform_ladybug_ground_slow_loop");
			break;
		}
	}
}
