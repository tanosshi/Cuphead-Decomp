using System.Collections;
using UnityEngine;

public class DicePalaceFlyingHorseLevelLights : AbstractPausableComponent
{
	[SerializeField]
	private float size = 500f;

	[SerializeField]
	private float speed = 30f;

	protected override void Start()
	{
		base.Start();
		FrameDelayedCallback(GetSprites, 1);
	}

	private void GetSprites()
	{
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			if (base.transform.position.x > -640f - size)
			{
				base.transform.position += Vector3.left * speed * CupheadTime.Delta;
			}
			else
			{
				base.transform.position = new Vector3(640f + size, base.transform.position.y, 0f);
			}
			yield return null;
		}
	}
}
