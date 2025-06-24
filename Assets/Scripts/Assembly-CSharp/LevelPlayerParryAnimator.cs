using System.Collections;
using UnityEngine;

public class LevelPlayerParryAnimator : AbstractMonoBehaviour
{
	[SerializeField]
	private Sprite[] sprites;

	private SpriteRenderer r;

	protected override void Awake()
	{
		base.Awake();
		r = GetComponent<SpriteRenderer>();
		Sprite[] array = sprites;
		foreach (Sprite sprite in array)
		{
			sprite.name = sprite.name.Replace("_pink", string.Empty);
		}
	}

	private void Set()
	{
		Sprite[] array = sprites;
		foreach (Sprite sprite in array)
		{
			if (sprite.name.Contains(r.sprite.name))
			{
				r.sprite = sprite;
				break;
			}
		}
	}

	public void StartSet()
	{
		StartCoroutine(set_cr());
		StartCoroutine(setLate_cr());
	}

	public void StopSet()
	{
		StopAllCoroutines();
	}

	private IEnumerator set_cr()
	{
		while (true)
		{
			Set();
			yield return null;
		}
	}

	private IEnumerator setLate_cr()
	{
		while (true)
		{
			Set();
			yield return new WaitForEndOfFrame();
		}
	}
}
