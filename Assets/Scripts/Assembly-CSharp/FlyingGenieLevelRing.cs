using System.Collections;
using UnityEngine;

public class FlyingGenieLevelRing : BasicProjectile
{
	private const float FRAME_TIME = 1f / 24f;

	public bool isMain;

	private LevelProperties.FlyingGenie.GemStone properties;

	protected override void Start()
	{
		base.Start();
		if (isMain)
		{
			GetComponent<Collider2D>().enabled = false;
			animator.Play("Off");
		}
		else
		{
			StartCoroutine(fade_cr());
		}
	}

	public void SetProperties(LevelProperties.FlyingGenie.GemStone p)
	{
		properties = p;
	}

	private IEnumerator fade_cr()
	{
		float frameTime = 0f;
		float color = 1f;
		while (color > 0f)
		{
			GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, color);
			frameTime += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				color -= 0.05f;
				frameTime -= 1f / 24f;
			}
			yield return null;
		}
		OnComplete();
		yield return null;
	}

	private void OnComplete()
	{
		Object.Destroy(base.gameObject);
	}

	public void DisableCollision()
	{
		GetComponent<Collider2D>().enabled = false;
	}
}
