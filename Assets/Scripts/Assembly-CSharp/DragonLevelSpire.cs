using System.Collections;
using UnityEngine;

public class DragonLevelSpire : AbstractPausableComponent
{
	private AnimationHelper helper;

	[SerializeField]
	private SpriteRenderer replacementSprite;

	private float fadeTime;

	protected override void Start()
	{
		base.Start();
		helper = GetComponent<AnimationHelper>();
		helper.Speed = 0f;
		fadeTime = 3f;
		replacementSprite.enabled = false;
	}

	protected override void Update()
	{
		base.Update();
		helper.Speed = DragonLevel.SPEED;
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
			replacementSprite.enabled = true;
			replacementSprite.color = new Color(1f, 1f, 1f, t / fadeTime);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		replacementSprite.color = new Color(1f, 1f, 1f, 1f);
		yield return null;
	}
}
