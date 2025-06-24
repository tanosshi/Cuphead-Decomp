using System.Collections;

public class ForestPlatformingLevelFlowerGrunt : PlatformingLevelGroundMovementEnemy
{
	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(idle_audio_delayer_cr("level_flowergrunt", 2f, 4f));
		emitAudioFromObject.Add("level_flowergrunt");
		emitAudioFromObject.Add("level_flowergrunt_float");
		emitAudioFromObject.Add("level_flowergrunt_death");
	}

	protected override void OnStart()
	{
		base.OnStart();
		if (floating)
		{
			AudioManager.Play("level_flowergrunt_float");
			StartCoroutine(handle_float_cr());
		}
	}

	private IEnumerator handle_float_cr()
	{
		while (floating)
		{
			yield return null;
		}
		AudioManager.Play("level_flowergrunt_land");
		emitAudioFromObject.Add("level_flowergrunt_land");
	}

	protected override void Die()
	{
		AudioManager.Play("level_flowergrunt_death");
		FrameDelayedCallback(Kill, 1);
	}

	private void Kill()
	{
		base.Die();
	}
}
