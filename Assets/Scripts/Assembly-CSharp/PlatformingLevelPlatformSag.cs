using System.Collections;
using UnityEngine;

public class PlatformingLevelPlatformSag : LevelPlatform
{
	[SerializeField]
	private float sagAmount = 30f;

	private const EaseUtils.EaseType FALL_BOUNCE_EASE = EaseUtils.EaseType.easeOutBounce;

	public const float FALL_TIME = 0.4f;

	public const float RISE_TIME = 0.6f;

	private float localPosY;

	protected override void Start()
	{
		base.Start();
		localPosY = base.transform.localPosition.y;
	}

	public override void AddChild(Transform player)
	{
		base.AddChild(player);
		StartCoroutine(fall_cr());
	}

	public override void OnPlayerExit(Transform player)
	{
		base.OnPlayerExit(player);
		StartCoroutine(go_up_cr());
	}

	private IEnumerator goTo_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		float t = 0f;
		base.transform.SetLocalPosition(null, start);
		while (t < time)
		{
			TransformExtensions.SetLocalPosition(y: EaseUtils.Ease(ease, start, end, t / time), transform: base.transform);
			t += Time.deltaTime;
			yield return StartCoroutine(WaitForPause_CR());
		}
		base.transform.SetLocalPosition(null, end);
	}

	private IEnumerator fall_cr()
	{
		yield return StartCoroutine(goTo_cr(base.transform.localPosition.y, localPosY - sagAmount, 0.4f, EaseUtils.EaseType.easeOutBounce));
	}

	private IEnumerator go_up_cr()
	{
		yield return StartCoroutine(goTo_cr(base.transform.localPosition.y, localPosY, 0.6f, EaseUtils.EaseType.easeOutBounce));
	}
}
