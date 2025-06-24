using System.Collections;

public class TestLevelPlatform : LevelPlatform
{
	private const float X = 700f;

	private const float TIME = 4f;

	private const EaseUtils.EaseType EASE = EaseUtils.EaseType.easeInOutSine;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(loop_cr());
	}

	private IEnumerator loop_cr()
	{
		while (true)
		{
			yield return TweenLocalPositionX(-700f, 700f, 4f, EaseUtils.EaseType.easeInOutSine);
			yield return TweenLocalPositionX(700f, -700f, 4f, EaseUtils.EaseType.easeInOutSine);
		}
	}
}
