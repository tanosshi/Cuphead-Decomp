using System.Collections.Generic;
using UnityEngine;

public class DragonLevelBackgroundFlash : MonoBehaviour
{
	[SerializeField]
	private Sprite normalSprite;

	[SerializeField]
	private Sprite flashSprite1;

	[SerializeField]
	private Sprite flashSprite2;

	[SerializeField]
	private ScrollingSprite scrollSprite;

	public void SetFlash1()
	{
		List<SpriteRenderer> copyRenderers = scrollSprite.copyRenderers;
		for (int i = 0; i < copyRenderers.Count; i++)
		{
			copyRenderers[i].sprite = flashSprite1;
		}
	}

	public void SetFlash2()
	{
		List<SpriteRenderer> copyRenderers = scrollSprite.copyRenderers;
		for (int i = 0; i < copyRenderers.Count; i++)
		{
			copyRenderers[i].sprite = flashSprite2;
		}
	}

	public void SetNormal()
	{
		List<SpriteRenderer> copyRenderers = scrollSprite.copyRenderers;
		for (int i = 0; i < copyRenderers.Count; i++)
		{
			copyRenderers[i].sprite = normalSprite;
		}
	}
}
