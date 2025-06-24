using System.Collections;
using UnityEngine;

public class FlowerLevelPlatform : LevelPlatform
{
	public enum State
	{
		Up = 0,
		Down = 1,
		PlayerOn = 2
	}

	public float YPositionUp;

	public const float TIME = 3f;

	public const float FALL_TIME = 0.13f;

	public const float FALL_BOUNCE_TIME = 0.12f;

	public const float DELAY = 0f;

	public const EaseUtils.EaseType FLOAT_EASE = EaseUtils.EaseType.easeInOutSine;

	public const EaseUtils.EaseType FALL_EASE = EaseUtils.EaseType.easeOutSine;

	public const EaseUtils.EaseType FALL_BOUNCE_EASE = EaseUtils.EaseType.easeInOutSine;

	[SerializeField]
	private State state;

	[SerializeField]
	private Transform shadow;

	private Vector3 startPos;

	private Vector3 endPos;

	private float YPositionDown;

	private float YFall;

	protected override void Start()
	{
		base.Start();
		YPositionDown = YPositionUp - 30f;
		YFall = YPositionUp - 35f;
		if (shadow != null)
		{
			shadow.parent = null;
			Vector3 position = shadow.position;
			position.y = Level.Current.Ground;
			shadow.position = position;
		}
		startPos = base.transform.position;
		startPos.y = YPositionUp;
		endPos = base.transform.position;
		endPos.y = YPositionDown;
		if (state == State.Down)
		{
			base.transform.SetPosition(null, YPositionUp);
			StartDown();
		}
		else
		{
			base.transform.SetPosition(null, YPositionDown);
			StartUp();
		}
	}

	public void StartDown()
	{
		StopAllCoroutines();
		StartCoroutine(down_cr());
	}

	public void StartUp()
	{
		StopAllCoroutines();
		StartCoroutine(up_cr());
	}

	public override void AddChild(Transform player)
	{
		base.AddChild(player);
		StopAllCoroutines();
		StartCoroutine(fall_cr());
	}

	public override void OnPlayerExit(Transform player)
	{
		base.OnPlayerExit(player);
		StartUp();
	}

	private IEnumerator down_cr()
	{
		yield return new WaitForSeconds(0f);
		yield return StartCoroutine(goTo_cr(YPositionUp, YPositionDown, 3f, EaseUtils.EaseType.easeInOutSine));
		StartUp();
	}

	private IEnumerator up_cr()
	{
		yield return new WaitForSeconds(0f);
		yield return StartCoroutine(goTo_cr(YPositionDown, YPositionUp, 3f, EaseUtils.EaseType.easeInOutSine));
		StartDown();
	}

	private IEnumerator fall_cr()
	{
		yield return StartCoroutine(goTo_cr(time: (1f - base.transform.position.y / YPositionDown) * 0.13f, start: base.transform.position.y, end: YFall, ease: EaseUtils.EaseType.easeOutSine));
		yield return StartCoroutine(goTo_cr(YFall, YPositionDown, 0.12f, EaseUtils.EaseType.easeInOutSine));
	}

	private IEnumerator goTo_cr(float start, float end, float time, EaseUtils.EaseType ease)
	{
		float t = 0f;
		base.transform.SetPosition(null, start);
		while (t < time)
		{
			TransformExtensions.SetPosition(y: EaseUtils.Ease(ease, start, end, t / time), transform: base.transform);
			t += Time.deltaTime;
			yield return StartCoroutine(WaitForPause_CR());
		}
		base.transform.SetPosition(null, end);
	}
}
