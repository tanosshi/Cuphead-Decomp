using System.Collections;
using UnityEngine;

public class DicePalaceBoozeLevelCharacterFader : AbstractPausableComponent
{
	[SerializeField]
	private SpriteRenderer[] sprites;

	private SpriteRenderer mainSprite;

	private bool fadingOut;

	protected override void Awake()
	{
		base.Awake();
		mainSprite = GetComponent<SpriteRenderer>();
		StartCoroutine(main_cr());
	}

	private IEnumerator main_cr()
	{
		SpriteRenderer[] array = sprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
		}
		mainSprite.color = new Color(1f, 1f, 1f, 0f);
		float fadeTime = 0.5f;
		while (true)
		{
			float holdTime = Random.Range(3f, 6f);
			yield return CupheadTime.WaitForSeconds(this, holdTime);
			if (fadingOut)
			{
				float t = 0f;
				while (t < fadeTime)
				{
					mainSprite.color = new Color(1f, 1f, 1f, 1f - t / fadeTime);
					SpriteRenderer[] array2 = sprites;
					foreach (SpriteRenderer spriteRenderer2 in array2)
					{
						spriteRenderer2.color = new Color(1f, 1f, 1f, 1f - t / fadeTime);
					}
					t += (float)CupheadTime.Delta;
					yield return null;
				}
			}
			else
			{
				float t2 = 0f;
				while (t2 < fadeTime)
				{
					mainSprite.color = new Color(1f, 1f, 1f, t2 / fadeTime);
					SpriteRenderer[] array3 = sprites;
					foreach (SpriteRenderer spriteRenderer3 in array3)
					{
						spriteRenderer3.color = new Color(1f, 1f, 1f, t2 / fadeTime);
					}
					t2 += (float)CupheadTime.Delta;
					yield return null;
				}
			}
			fadingOut = !fadingOut;
		}
	}
}
