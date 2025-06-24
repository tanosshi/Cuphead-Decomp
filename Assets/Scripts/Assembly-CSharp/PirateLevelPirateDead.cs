using System.Collections;
using UnityEngine;

public class PirateLevelPirateDead : AbstractMonoBehaviour
{
	public const float END_Y = -250f;

	public const float SPLASH_Y = -25f;

	[SerializeField]
	private Effect splashPrefab;

	protected override void Awake()
	{
		base.Awake();
		base.gameObject.SetActive(false);
	}

	public void Go(float delay, float speed)
	{
		base.gameObject.SetActive(true);
		StartCoroutine(go_cr(delay, speed));
	}

	private void End()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator go_cr(float delay, float time)
	{
		float startY = base.transform.position.y;
		bool splash = false;
		yield return CupheadTime.WaitForSeconds(this, delay);
		float t = 0f;
		while (t < time)
		{
			TransformExtensions.SetLocalPosition(y: EaseUtils.Ease(EaseUtils.EaseType.linear, startY, -250f, t / time), transform: base.transform);
			if (!splash && base.transform.position.y <= -25f)
			{
				Debug.Log("SPLASH");
				splash = true;
				splashPrefab.Create(base.transform.position + new Vector3(0f, 20f, 0f));
			}
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetLocalPosition(null, -250f);
		End();
	}
}
