using UnityEngine;

public class CircusPlatformingLevelBallRunner : PlatformingLevelPathMovementEnemy
{
	private const float ON_SCREEN_SOUND_PADDING = 100f;

	[SerializeField]
	private CircusPlatformingLevelBallRunnerBall ball;

	[SerializeField]
	private Transform ballRoot;

	protected override void Die()
	{
		AudioManager.Play("circus_generic_death_fun");
		emitAudioFromObject.Add("circus_generic_death_fun");
		ball.transform.parent = null;
		ball.isMoving = true;
		ball.direction = new Vector3((float)_direction, 0f, 0f);
		base.Die();
	}

	private void IdleSFX()
	{
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)))
		{
			AudioManager.Play("circus_ball_runner_idle");
			emitAudioFromObject.Add("circus_ball_runner_idle");
		}
	}
}
