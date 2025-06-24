using System.Collections;
using UnityEngine;

public class MapUIVignetteDialogue : AbstractMonoBehaviour
{
	public static float fadeTime = 0.5f;

	[SerializeField]
	private CanvasGroup canvasGroup;

	public static MapUIVignetteDialogue Current { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		canvasGroup.alpha = 0f;
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		base.transform.position = CupheadMapCamera.Current.transform.position;
	}

	public void FadeIn()
	{
		Fade(1f);
	}

	public void FadeOut()
	{
		Fade(0f);
	}

	public void Fade(float target)
	{
		StartCoroutine(fade_cr(canvasGroup.alpha, target));
	}

	private IEnumerator fade_cr(float startOpacity, float endOpacity)
	{
		float t = 0f;
		while (t < fadeTime)
		{
			yield return null;
			t += (float)CupheadTime.Delta;
			canvasGroup.alpha = Mathf.Lerp(startOpacity, endOpacity, t / fadeTime);
		}
		canvasGroup.alpha = endOpacity;
	}
}
