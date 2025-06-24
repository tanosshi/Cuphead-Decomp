using System;
using UnityEngine;

public class LevelIntroAnimation : AbstractLevelHUDComponent
{
	private Action callback;

	public static LevelIntroAnimation Create(Action callback)
	{
		LevelIntroAnimation levelIntroAnimation = UnityEngine.Object.Instantiate(Level.Current.LevelResources.levelIntro);
		levelIntroAnimation.callback = callback;
		return levelIntroAnimation;
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
