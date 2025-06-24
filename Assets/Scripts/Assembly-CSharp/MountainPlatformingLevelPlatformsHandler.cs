using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelPlatformsHandler : AbstractPausableComponent
{
	[SerializeField]
	private Transform platformHolder;

	[SerializeField]
	private ParrySwitch parrySwitch;

	[SerializeField]
	private float platformMoveTime;

	[SerializeField]
	private float platformAppearDelay;

	private bool hasSwitched;

	private Transform[] platforms;

	private Vector3[] platformsStartPos;

	private float lowerAmount = 1000f;

	protected override void Start()
	{
		base.Start();
		platforms = new Transform[platformHolder.GetComponentsInChildren<Transform>().Length];
		platforms = platformHolder.GetComponentsInChildren<Transform>();
		platformsStartPos = new Vector3[platformHolder.GetComponentsInChildren<Transform>().Length];
		for (int i = 0; i < platforms.Length; i++)
		{
			platformsStartPos[i] = platforms[i].position;
		}
		for (int j = 0; j < platforms.Length; j++)
		{
			OffScreen(platforms[j]);
		}
		parrySwitch.OnActivate += MovePlatforms;
	}

	private void MovePlatforms()
	{
		if (!hasSwitched)
		{
			StartCoroutine(moving_cr());
			hasSwitched = true;
		}
	}

	private IEnumerator moving_cr()
	{
		for (int i = 0; i < platforms.Length; i++)
		{
			StartCoroutine(move_platform_cr(platforms[i], platformsStartPos[i]));
			yield return CupheadTime.WaitForSeconds(this, platformAppearDelay);
		}
		yield return null;
	}

	private void OffScreen(Transform platform)
	{
		platform.transform.position += Vector3.down * lowerAmount;
	}

	private IEnumerator move_platform_cr(Transform platform, Vector3 startPos)
	{
		float t = 0f;
		float time = platformMoveTime;
		Vector2 start = platform.transform.position;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			platform.transform.position = Vector2.Lerp(start, startPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		platform.transform.position = startPos;
		yield return null;
	}
}
