using System.Collections;

public class MountainPlatformingLevelFan : AbstractPlatformingLevelEnemy
{
	private float speed;

	private float offset = 50f;

	public bool fanOn;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(check_to_start_cr());
	}

	public float GetSpeed()
	{
		if (base.transform.localScale.x == 1f)
		{
			speed = 0f - base.Properties.fanVelocity;
		}
		else
		{
			speed = base.Properties.fanVelocity;
		}
		return speed;
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void OnStart()
	{
		StartCoroutine(fan_cr());
	}

	private IEnumerator check_to_start_cr()
	{
		while (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset)
		{
			yield return null;
		}
		OnStart();
		yield return null;
	}

	private void FanOn()
	{
		PlayLionRoarSFX();
		fanOn = true;
	}

	private void FanOff()
	{
		fanOn = false;
	}

	private IEnumerator fan_cr()
	{
		while (true)
		{
			if (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset || base.transform.position.x < CupheadLevelCamera.Current.Bounds.xMin - offset)
			{
				yield return null;
				continue;
			}
			base.animator.SetBool("IsFan", true);
			yield return null;
			yield return base.animator.WaitForAnimationToEnd(this, "Intro");
			base.animator.SetBool("WindOn", true);
			yield return null;
			float t = 0f;
			float time = base.Properties.fanWaitTime.RandomFloat();
			while (t < time)
			{
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			base.animator.SetBool("IsFan", false);
			base.animator.SetBool("WindOn", false);
			yield return null;
			yield return base.animator.WaitForAnimationToEnd(this, "Roar_End");
			yield return null;
		}
	}

	private void PlayLionRoarSFX()
	{
		AudioManager.Play("castle_rock_lion_roar");
		emitAudioFromObject.Add("castle_rock_lion_roar");
	}

	protected override void Die()
	{
		AudioManager.Play("castle_rock_lion_death");
		emitAudioFromObject.Add("castle_rock_lion_death");
		base.animator.SetBool("WindOn", false);
		speed = 0f;
		fanOn = false;
		StopAllCoroutines();
		base.animator.SetTrigger("Death");
	}
}
