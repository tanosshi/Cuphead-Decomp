using System.Collections;
using UnityEngine;

public class TreePlatformingLevelDragonflyProjectile : BasicProjectile
{
	private const string ProjectilesLayerName = "Projectiles";

	[SerializeField]
	private Effect bulletFX;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(bullet_trail_cr());
	}

	private IEnumerator bullet_trail_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.16f, 0.2f));
			Effect e = bulletFX.Create(base.transform.position);
			SpriteRenderer r = e.GetComponent<SpriteRenderer>();
			r.sortingOrder = -1;
			r.sortingLayerName = "Projectiles";
			yield return null;
		}
	}
}
