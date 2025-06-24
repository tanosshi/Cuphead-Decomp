using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractLevelEntity : AbstractCollidableObject
{
	protected const float DIE_FLASH_TIME = 0.3f;

	protected const float DIE_FLASH_DELAY = 0.2f;

	protected const int DIE_FLASH_LOOPS = 4;

	[SerializeField]
	protected bool _canParry;

	public bool canParry
	{
		get
		{
			return _canParry;
		}
	}

	public virtual void OnParry(AbstractPlayerController player)
	{
	}

	protected IEnumerator flash_cr(Color start, Color end, float time, Action onComplete = null)
	{
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		renderer.color = start;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			renderer.color = Color.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		renderer.color = end;
		if (onComplete != null)
		{
			onComplete();
		}
	}

	protected IEnumerator dieFlash_cr()
	{
		for (int i = 0; i < 4; i++)
		{
			yield return StartCoroutine(flash_cr(Color.red, Color.black, 0.3f));
			yield return CupheadTime.WaitForSeconds(this, 0.2f);
		}
	}
}
