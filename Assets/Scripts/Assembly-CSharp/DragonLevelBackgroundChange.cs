using System.Collections;
using UnityEngine;

public class DragonLevelBackgroundChange : DragonLevelScrollingSprite
{
	[SerializeField]
	private Transform replacementSprite;

	private SpriteRenderer[] replacementClones;

	private SpriteRenderer current;

	private SpriteRenderer[] currentClones;

	private bool changeStart;

	private float fadeTime;

	protected override void Start()
	{
		base.Start();
		FrameDelayedCallback(DisableSprites, 1);
	}

	private void DisableSprites()
	{
		fadeTime = 6f;
		current = GetComponent<SpriteRenderer>();
		replacementClones = replacementSprite.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
		currentClones = current.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
		replacementSprite.transform.position = new Vector2(base.transform.position.x, replacementSprite.transform.position.y);
		replacementSprite.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		for (int i = 0; i < replacementClones.Length; i++)
		{
			replacementClones[i].enabled = false;
		}
	}

	public void StartChange()
	{
		StartCoroutine(change_cr());
	}

	private IEnumerator change_cr()
	{
		float t = 0f;
		while (t < fadeTime)
		{
			for (int i = 0; i < replacementClones.Length; i++)
			{
				if (replacementClones[i].transform != null)
				{
					replacementClones[i].enabled = true;
					replacementClones[i].color = new Color(1f, 1f, 1f, t / fadeTime);
				}
			}
			for (int j = 0; j < currentClones.Length; j++)
			{
				currentClones[j].color = new Color(1f, 1f, 1f, 1f - t / fadeTime);
			}
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		for (int k = 0; k < replacementClones.Length; k++)
		{
			replacementClones[k].color = new Color(1f, 1f, 1f, 1f);
		}
		for (int l = 0; l < currentClones.Length; l++)
		{
			currentClones[l].enabled = false;
		}
		yield return null;
	}
}
