using System;
using UnityEngine;

public class PlatformingLevelIntroAnimation : AbstractLevelHUDComponent
{
	private Action callback;

	public static PlatformingLevelIntroAnimation Create(Action callback)
	{
		PlatformingLevelIntroAnimation platformingLevelIntroAnimation = UnityEngine.Object.Instantiate(Level.Current.LevelResources.platformingIntro);
		platformingLevelIntroAnimation.callback = callback;
		return platformingLevelIntroAnimation;
	}

	protected override void Awake()
	{
		base.Awake();
		_parentToHudCanvas = true;
		base.transform.parent = Camera.main.transform;
		base.transform.ResetLocalTransforms();
	}

	private void StartLevel()
	{
		if (callback != null)
		{
			callback();
		}
	}

	private void OnAnimComplete()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Play()
	{
		GetComponent<Animator>().Play("Intro");
	}
}
