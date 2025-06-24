using UnityEngine;

public class DragonLevelScrollingSprite : ScrollingSprite
{
	private const float MIN_SPEED = 0.1f;

	protected override void Awake()
	{
		base.Awake();
		playbackSpeed = 0f;
	}

	protected override void Update()
	{
		playbackSpeed = Mathf.Lerp(0.1f, 1f, DragonLevel.SPEED);
		base.Update();
	}
}
