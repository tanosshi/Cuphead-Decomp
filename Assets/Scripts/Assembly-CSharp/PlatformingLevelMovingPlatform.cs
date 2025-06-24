using System.Collections;
using UnityEngine;

public class PlatformingLevelMovingPlatform : AbstractPausableComponent
{
	private const float ON_SCREEN_SOUND_PADDING = 100f;

	protected int pathIndex;

	public float loopRepeatDelay;

	public float speed = 100f;

	public VectorPath path;

	public bool goingUp;

	public SpriteRenderer sprite;

	private EaseUtils.EaseType _easeType = EaseUtils.EaseType.linear;

	private Vector3 _offset;

	protected float[] allValues { get; private set; }

	protected override void Start()
	{
		base.Start();
		_offset = base.transform.position;
		StartCoroutine(pingpong_cr());
		AudioManager.PlayLoop("level_platform_propellor_loop");
		emitAudioFromObject.Add("level_platform_propellor_loop");
		StartCoroutine(check_sound_cr());
	}

	private IEnumerator check_sound_cr()
	{
		bool inRange = false;
		while (true)
		{
			if (CupheadLevelCamera.Current.ContainsPoint(base.transform.position, new Vector2(100f, 1000f)))
			{
				if (!inRange)
				{
					AudioManager.PlayLoop("level_platform_propellor_loop");
					emitAudioFromObject.Add("level_platform_propellor_loop");
					inRange = true;
				}
			}
			else if (inRange)
			{
				AudioManager.Stop("level_platform_propellor_loop");
				inRange = false;
			}
			yield return null;
		}
	}

	private float CalculateTime()
	{
		return path.Distance / speed;
	}

	private float CalculateRemainingTime(float t)
	{
		float num = CalculateTime();
		return (!goingUp) ? (t * num) : ((1f - t) * num);
	}

	private void MoveCallback(float value)
	{
		base.transform.position = _offset + path.Lerp(value);
	}

	private IEnumerator pingpong_cr()
	{
		if (goingUp)
		{
			yield return TweenValue(0f, 1f, CalculateTime(), _easeType, MoveCallback);
			yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
			yield return Turn();
		}
		else
		{
			yield return TweenValue(1f, 0f, CalculateTime(), _easeType, MoveCallback);
			yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
			yield return Turn();
		}
		while (true)
		{
			if (goingUp)
			{
				yield return TweenValue(0f, 1f, CalculateTime(), _easeType, MoveCallback);
				yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
				yield return Turn();
			}
			else
			{
				yield return TweenValue(1f, 0f, CalculateTime(), _easeType, MoveCallback);
				yield return CupheadTime.WaitForSeconds(this, loopRepeatDelay);
				yield return Turn();
			}
			yield return null;
		}
	}

	private Coroutine Turn()
	{
		return StartCoroutine(turn_cr());
	}

	private IEnumerator turn_cr()
	{
		goingUp = !goingUp;
		yield return null;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		DrawGizmos(0.2f);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		DrawGizmos(1f);
	}

	private void DrawGizmos(float a)
	{
		if (Application.isPlaying)
		{
			path.DrawGizmos(a, _offset);
			return;
		}
		path.DrawGizmos(a, base.baseTransform.position);
		Gizmos.color = new Color(1f, 0f, 0f, a);
		Gizmos.DrawSphere(path.Lerp(0f) + base.baseTransform.position, 10f);
		Gizmos.DrawWireSphere(path.Lerp(0f) + base.baseTransform.position, 11f);
	}
}
