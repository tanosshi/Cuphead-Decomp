using System.Collections;

public class ForestPlatformingLevelBlobRunner : PlatformingLevelGroundMovementEnemy
{
	private bool melted;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(idle_audio_delayer_cr("level_blobrunner", 2f, 4f));
		emitAudioFromObject.Add("level_blobrunner");
	}

	protected override void FixedUpdate()
	{
		if (!melted)
		{
			base.FixedUpdate();
		}
	}

	protected override void Die()
	{
		IdleSounds = false;
		melted = true;
		collider.enabled = false;
		StartCoroutine(melt_cr());
	}

	private IEnumerator melt_cr()
	{
		AudioManager.Stop("level_blobrunner");
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, AbstractPlatformingLevelEnemy.CAMERA_DEATH_PADDING))
		{
			AudioManager.Play("level_frogs_tall_firefly_death");
		}
		base.animator.Play("Melt");
		yield return base.animator.WaitForAnimationToEnd(this, "Melt");
		yield return CupheadTime.WaitForSeconds(this, base.Properties.BlobRunnerMeltDelay.RandomFloat());
		base.animator.SetTrigger("Continue");
		AudioManager.Play("level_blobrunner_reform");
		emitAudioFromObject.Add("level_blobrunner_reform");
		yield return CupheadTime.WaitForSeconds(this, base.Properties.BlobRunnerUnmeltLoopTime);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Unmelt");
		melted = false;
		collider.enabled = true;
		base.Health = base.Properties.Health;
		turning = false;
		timeSinceTurn = 10000f;
		IdleSounds = true;
	}
}
