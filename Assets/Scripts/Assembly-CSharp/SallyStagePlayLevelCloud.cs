using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelCloud : AbstractPausableComponent
{
	[SerializeField]
	private SpriteRenderer shadowSprite;

	private SpriteRenderer[] shadowClones;

	[SerializeField]
	private SpriteRenderer normalSprite;

	private SpriteRenderer[] normalClones;

	protected override void Start()
	{
		base.Start();
		FrameDelayedCallback(GetSprites, 1);
	}

	private void GetSprites()
	{
		normalClones = normalSprite.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
		shadowClones = shadowSprite.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
		StartCoroutine(move_all_cr());
		StartCoroutine(move_shadow_local_pos_cr());
	}

	private IEnumerator move_all_cr()
	{
		float size = 500f;
		float speed = 30f;
		while (true)
		{
			for (int i = 0; i < normalClones.Length; i++)
			{
				if (normalClones[i].transform.position.x > -640f - size)
				{
					normalClones[i].transform.position += Vector3.left * speed * CupheadTime.Delta;
				}
				else
				{
					normalClones[i].transform.position = new Vector3(640f + size, normalClones[i].transform.position.y, 0f);
				}
			}
			for (int j = 0; j < shadowClones.Length; j++)
			{
				if (shadowClones[j].transform.position.x > -640f - size)
				{
					shadowClones[j].transform.position += Vector3.left * speed * CupheadTime.Delta;
					continue;
				}
				Vector3 position = shadowClones[j].transform.position;
				position.x = normalClones[j].transform.position.x;
				shadowClones[j].transform.position = position;
			}
			yield return null;
		}
	}

	private IEnumerator move_shadow_local_pos_cr()
	{
		float speed = 1.3f;
		float shadowOffset = 5f;
		while (true)
		{
			for (int i = 0; i < shadowClones.Length; i++)
			{
				if (normalClones[i].transform.position.x > 0f && normalClones[i].transform.position.x < 440f)
				{
					if (shadowClones[i].transform.position.x < normalClones[i].transform.position.x + shadowOffset)
					{
						shadowClones[i].transform.position += Vector3.right * speed * CupheadTime.Delta;
					}
				}
				else if (normalClones[i].transform.position.x < 0f && normalClones[i].transform.position.x > -640f && shadowClones[i].transform.position.x > normalClones[i].transform.position.x - shadowOffset)
				{
					shadowClones[i].transform.position -= Vector3.right * speed * CupheadTime.Delta;
				}
			}
			yield return null;
		}
	}
}
