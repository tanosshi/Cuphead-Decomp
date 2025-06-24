using UnityEngine;

public class FunhousePlatformingLevelLips : BasicProjectile
{
	protected override void Awake()
	{
		base.Awake();
		PlatformingLevelExit.OnWinStartEvent += OnWin;
	}

	private void Kiss()
	{
		AudioManager.Play("funhouse_honkbullet_kiss");
		emitAudioFromObject.Add("funhouse_honkbullet_kiss");
	}

	protected override void OnDestroy()
	{
		PlatformingLevelExit.OnWinStartEvent -= OnWin;
		base.OnDestroy();
	}

	private void OnWin()
	{
		Object.Destroy(base.gameObject);
	}
}
