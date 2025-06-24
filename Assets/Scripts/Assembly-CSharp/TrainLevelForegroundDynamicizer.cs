using System.Collections;
using UnityEngine;

public class TrainLevelForegroundDynamicizer : AbstractPausableComponent
{
	public const float X_OUT = -1280f;

	public const float X_IN = 1280f;

	public const float DELAY_MIN = 1f;

	public const float DELAY_MAX = 4f;

	public const float TIME = 1.3f;

	[SerializeField]
	private SpriteRenderer[] sprites;

	protected override void Awake()
	{
		base.Awake();
		ResetPositions();
		StartCoroutine(loop_cr());
	}

	private void ResetPositions()
	{
		SpriteRenderer[] array = sprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			spriteRenderer.transform.SetLocalPosition(-1280f, 0f, 0f);
		}
	}

	private IEnumerator loop_cr()
	{
		while (true)
		{
			ResetPositions();
			float t = 0f;
			Transform trans = sprites[Random.Range(0, sprites.Length)].transform;
			trans.SetScale((Random.value > 0.5f) ? 1 : (-1), 1f, 1f);
			while (t < 1.3f)
			{
				float x = Mathf.Lerp(1280f, -1280f, t / 1.3f);
				trans.SetLocalPosition(x, 0f, 0f);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, Random.Range(4f, 4f));
		}
	}
}
