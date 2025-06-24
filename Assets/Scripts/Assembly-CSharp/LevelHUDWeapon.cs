using System;
using System.Collections;
using UnityEngine;

public class LevelHUDWeapon : AbstractMonoBehaviour
{
	public const float TIME_IN = 0.2f;

	public const float TIME_DELAY = 2f;

	public const float TIME_OUT = 0.2f;

	public const float TIME_OUT_FAST = 0.05f;

	private bool ending;

	private Coroutine inCoroutine;

	private float startY;

	private float endY;

	private static event Action OnAwakeEvent;

	public LevelHUDWeapon Create(Transform parent, Weapon weapon)
	{
		LevelHUDWeapon levelHUDWeapon = InstantiatePrefab<LevelHUDWeapon>();
		levelHUDWeapon.transform.SetParent(parent, false);
		levelHUDWeapon.SetIcon(weapon);
		return levelHUDWeapon;
	}

	protected override void Awake()
	{
		base.Awake();
		startY = -80f;
		endY = -10f;
		base.transform.ResetLocalTransforms();
		base.transform.SetLocalPosition(0f, startY, 0f);
		inCoroutine = StartCoroutine(go_cr());
		if (LevelHUDWeapon.OnAwakeEvent != null)
		{
			LevelHUDWeapon.OnAwakeEvent();
		}
		LevelHUDWeapon.OnAwakeEvent = null;
		OnAwakeEvent += Out;
	}

	private void Out()
	{
		if (!ending)
		{
			if (inCoroutine != null)
			{
				StopCoroutine(inCoroutine);
			}
			StartCoroutine(out_cr());
		}
	}

	private void SetIcon(Weapon weapon)
	{
		base.animator.Play(weapon.ToString());
	}

	private IEnumerator go_cr()
	{
		yield return TweenLocalPositionY(startY, endY, 0.2f, EaseUtils.EaseType.easeOutSine);
		yield return CupheadTime.WaitForSeconds(this, 2f);
		ending = true;
		yield return TweenLocalPositionY(endY, startY, 0.2f, EaseUtils.EaseType.easeInSine);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator out_cr()
	{
		ending = true;
		yield return TweenLocalPositionY(endY, startY, 0.05f, EaseUtils.EaseType.easeInSine);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
