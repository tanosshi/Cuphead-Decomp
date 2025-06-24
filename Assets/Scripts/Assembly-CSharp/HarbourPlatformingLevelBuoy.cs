using UnityEngine;

public class HarbourPlatformingLevelBuoy : AbstractPausableComponent
{
	[SerializeField]
	private ParrySwitch parrySwitch;

	private const float ON_SCREEN_SOUND_PADDING = 100f;

	protected override void Start()
	{
		base.Start();
		parrySwitch.OnActivate += ParrySoundSFX;
	}

	private void PlayIdle()
	{
		if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)) && !AudioManager.CheckIfPlaying("harbour_buoy_idle"))
		{
			AudioManager.Play("harbour_buoy_idle");
			emitAudioFromObject.Add("harbour_buoy_idle");
		}
	}

	private void ParrySoundSFX()
	{
		AudioManager.Play("harbour_buoy_parry");
		emitAudioFromObject.Add("harbour_buoy_parry");
	}
}
