using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonColliderUpdater : AbstractMonoBehaviour
{
	private SpriteRenderer renderer;

	private Sprite sprite;

	private Dictionary<string, PolygonCollider2D> colliders;

	protected override void Awake()
	{
		base.Awake();
		colliders = new Dictionary<string, PolygonCollider2D>();
		renderer = GetComponent<SpriteRenderer>();
		sprite = null;
		if (GetComponent<Collider2D>() != null)
		{
			Object.Destroy(GetComponent<Collider2D>());
		}
		StartCoroutine(collider_cr());
	}

	private IEnumerator collider_cr()
	{
		while (true)
		{
			if (renderer.sprite != sprite)
			{
				sprite = renderer.sprite;
				PolygonCollider2D[] components = GetComponents<PolygonCollider2D>();
				foreach (PolygonCollider2D polygonCollider2D in components)
				{
					polygonCollider2D.enabled = false;
				}
				if (colliders.ContainsKey(sprite.name))
				{
					colliders[sprite.name].enabled = true;
				}
				else
				{
					colliders[sprite.name] = base.gameObject.AddComponent<PolygonCollider2D>();
					colliders[sprite.name].isTrigger = true;
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
